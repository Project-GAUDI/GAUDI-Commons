using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// enumare of StateTransitable
    /// </summary>
    internal enum StateTransitable
    {
        Undefined = 0,
        Untransitable,
        Unqueueable,
        Transitable,
        Queueable
    }

    /// <summary>
    /// Table info class of state transitable
    /// </summary>
    internal class TransitableInfo {
        public ApplicationState currentState;
        public ApplicationState nextState;
        public StateTransitable transitable;
        public bool transitableWhenTerminating;

        public TransitableInfo(ApplicationState current,ApplicationState next,StateTransitable able,bool whenTerm = false) {
            currentState = current;
            nextState = next;
            transitable = able;
            transitableWhenTerminating = whenTerm;
        }
    }


    /// <summary>
    /// State Translation Table internal class
    /// </summary>
    internal class TranstableTable : Dictionary<ApplicationState, Dictionary<ApplicationState, TransitableInfo>>
    {
        public void Add(TransitableInfo info)
        {
            if (false == this.ContainsKey(info.currentState))
            {
                this.Add(info.currentState, new Dictionary<ApplicationState, TransitableInfo>());
            }
            this[info.currentState].Add(info.nextState, info);
        }

        public TransitableInfo GetTranstableInfo(ApplicationState currentState, ApplicationState nextState)
        {
            TransitableInfo retTransitableInfo = null;

            if (this.ContainsKey(currentState))
            {
                if (this[currentState].ContainsKey(nextState))
                {
                    retTransitableInfo = this[currentState][nextState];
                }
            }

            return retTransitableInfo;
        }
    }

    /// <summary>
    /// Application State Controll class
    /// </summary>
    internal class StateController
    {
        protected static readonly TranstableTable transiTable = CreateTranstableTable();
        protected ApplicationState currentState = ApplicationState.Start;
        internal ApplicationState CurrentState { get { return currentState; } }
        protected int tasksInCurrentState = 0;
        protected bool isTerminating = false;
        public bool IsTerminating{
            get{ return isTerminating;}
            set{ isTerminating=value;}
        }
        protected Dictionary<ApplicationState, StateTransitable> currentTranstables = null;

        private SemaphoreSlim entrySemaphor { get; } = new SemaphoreSlim(1, 1);
        private ConcurrentQueue<string> stateQueue = new ConcurrentQueue<string>();
        private SemaphoreSlim actionSemaphor { get; } = new SemaphoreSlim(1, 1);


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StateController()
        {
            SetCurrentState(ApplicationState.Start);
        }

        /// <summary>
        /// 次ステート設定。
        /// 遷移不可能な場合は失敗する。
        /// キューイング可能時はキューイングし、遷移可能となったら遷移して返る。
        /// </summary>
        /// <param name="nextState">遷移先ステート</param>
        /// <returns></returns>
        public ApplicationStateChangeResult ChangeState(ApplicationState nextState)
        {
            ApplicationStateChangeResult result = ApplicationStateChangeResult.Ignored;

            // 遷移エントリ用の排他を設定
            entrySemaphor.Wait();

            StateTransitable transitable = GetTransitable(nextState);

            int intTransitable = (int)transitable;

            if (intTransitable <= (int)StateTransitable.Untransitable)
            {
                // 遷移不可
                result = ApplicationStateChangeResult.Ignored;
                entrySemaphor.Release();
            }
            else if (intTransitable == (int)StateTransitable.Transitable)
            {
                // 遷移可能
                // カレントの切り替え
                var status = SetCurrentState(nextState);
                result = ApplicationStateChangeResult.Success;

                entrySemaphor.Release();
            }
            else if (intTransitable == (int)StateTransitable.Queueable)
            {
                // キューイング可能
                var taskID = Guid.NewGuid().ToString("N");
                stateQueue.Enqueue(taskID);

                // キューイング中の個別処理を実施
                switch (nextState)
                {
                    case ApplicationState.Terminate:
                        isTerminating = true;
                        break;
                    default:
                        break;
                }
                                
                // キューイングしたので使用権を開放
                entrySemaphor.Release();

                // 待機状態とキューの先頭に到達するまで待機
                while (0 < stateQueue.Count)
                {
                    if ( ApplicationState.Ready == CurrentState ) {
                        if (stateQueue.TryPeek(out var queuePeek))
                        {
                            if (queuePeek == taskID)
                            {
                                result = ApplicationStateChangeResult.Success;
                                break;
                            }
                        }
                    }
                    Task.Delay(10);
                }

                if (ApplicationStateChangeResult.Success == result)
                {
                    // 再度遷移エントリ用の排他を設定
                    entrySemaphor.Wait();

                    // カレントの切り替え
                    var status = SetCurrentState(nextState);

                    // キューイング中の個別処理を実施
                    switch (nextState)
                    {
                        case ApplicationState.Terminate:
                            isTerminating = true;
                            break;
                        default:
                            break;
                    }
                                    
                    // キューイングしたので使用権を開放
                    entrySemaphor.Release();
                }

            }
            else
            {
                entrySemaphor.Release();
            }

            return result;
        }

        protected bool SetCurrentState(ApplicationState newCurrentState)
        {
            bool result = false;

            if ( currentState == newCurrentState ) {
                // カレントおよび遷移先が同一の場合、タスク数をインクリメントする
                tasksInCurrentState ++;
            } else {
                // カレントおよび遷移先が不一致の場合、タスク数をデクリメントする
                tasksInCurrentState --;

                // 現行Taskが０になったらステート遷移
                if ( 0 == tasksInCurrentState ) {
                    currentState = newCurrentState;
                    tasksInCurrentState ++;
                }
            }

            // 切り替え後の後処理を実施
            switch (newCurrentState)
            {
                case ApplicationState.Start:
                    break;
                case ApplicationState.Initialize:
                    isTerminating = false;
                    stateQueue.Clear();
                    break;
                case ApplicationState.Ready:
                    break;
                case ApplicationState.Running:
                    break;
                case ApplicationState.Terminate:
                    stateQueue.Clear();
                    break;
                case ApplicationState.End:
                    break;
                default:
                    break;
            }

            return result;
        }

        protected StateTransitable GetTransitable(ApplicationState nextState)
        {
            StateTransitable retTransitable = StateTransitable.Undefined;

            var info = transiTable.GetTranstableInfo(currentState, nextState);
            if ( true == isTerminating && false == info.transitableWhenTerminating ) {
                // 終了中は特例遷移以外は遷移不可
                retTransitable = StateTransitable.Untransitable;
            } else {
                // 通常時はテーブルに従う
                retTransitable = info.transitable;
            }

            return retTransitable;
        }

        private static TranstableTable CreateTranstableTable()
        {
            TranstableTable retTransiTable = new TranstableTable();

            // ApplicationState.Start
            retTransiTable.Add(new TransitableInfo( ApplicationState.Start, ApplicationState.Start, StateTransitable.Undefined) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Start, ApplicationState.Initialize, StateTransitable.Transitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Start, ApplicationState.Ready, StateTransitable.Undefined) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Start, ApplicationState.Running, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Start, ApplicationState.Terminate, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Start, ApplicationState.End, StateTransitable.Untransitable) );

            // ApplicationState.Initialize
            retTransiTable.Add(new TransitableInfo( ApplicationState.Initialize, ApplicationState.Start, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Initialize, ApplicationState.Initialize, StateTransitable.Undefined) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Initialize, ApplicationState.Ready, StateTransitable.Transitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Initialize, ApplicationState.Running, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Initialize, ApplicationState.Terminate, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Initialize, ApplicationState.End, StateTransitable.Untransitable) );

            // ApplicationState.Ready
            retTransiTable.Add(new TransitableInfo( ApplicationState.Ready, ApplicationState.Start, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Ready, ApplicationState.Initialize, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Ready, ApplicationState.Ready, StateTransitable.Undefined) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Ready, ApplicationState.Running, StateTransitable.Transitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Ready, ApplicationState.Terminate, StateTransitable.Queueable, true) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Ready, ApplicationState.End, StateTransitable.Untransitable) );

            // ApplicationState.Running
            retTransiTable.Add(new TransitableInfo( ApplicationState.Running, ApplicationState.Start, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Running, ApplicationState.Initialize, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Running, ApplicationState.Ready, StateTransitable.Transitable, true) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Running, ApplicationState.Running, StateTransitable.Transitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Running, ApplicationState.Terminate, StateTransitable.Queueable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Running, ApplicationState.End, StateTransitable.Untransitable) );

            // ApplicationState.Terminate
            retTransiTable.Add(new TransitableInfo( ApplicationState.Terminate, ApplicationState.Start, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Terminate, ApplicationState.Initialize, StateTransitable.Transitable, true) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Terminate, ApplicationState.Ready, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Terminate, ApplicationState.Running, StateTransitable.Unqueueable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Terminate, ApplicationState.Terminate, StateTransitable.Undefined) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.Terminate, ApplicationState.End, StateTransitable.Transitable, true) );

            // ApplicationState.End
            retTransiTable.Add(new TransitableInfo( ApplicationState.End, ApplicationState.Start, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.End, ApplicationState.Initialize, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.End, ApplicationState.Ready, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.End, ApplicationState.Running, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.End, ApplicationState.Terminate, StateTransitable.Untransitable) );
            retTransiTable.Add(new TransitableInfo( ApplicationState.End, ApplicationState.End, StateTransitable.Undefined) );

            return retTransiTable;
        }
    }
}
