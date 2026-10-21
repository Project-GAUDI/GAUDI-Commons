using System;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class StateController_GetTransitable
    {
        [Fact(DisplayName = "No043_GetTransitable_Start_false_Start")]
        public void Case043() => AssertState(43, ApplicationState.Start, false, ApplicationState.Start, StateTransitable.Undefined);
        [Fact(DisplayName = "No044_GetTransitable_Start_false_Initialize")]
        public void Case044() => AssertState(44, ApplicationState.Start, false, ApplicationState.Initialize, StateTransitable.Transitable);
        [Fact(DisplayName = "No045_GetTransitable_Start_false_Ready")]
        public void Case045() => AssertState(45, ApplicationState.Start, false, ApplicationState.Ready, StateTransitable.Undefined);
        [Fact(DisplayName = "No046_GetTransitable_Start_false_Running")]
        public void Case046() => AssertState(46, ApplicationState.Start, false, ApplicationState.Running, StateTransitable.Untransitable);
        [Fact(DisplayName = "No047_GetTransitable_Start_false_Terminate")]
        public void Case047() => AssertState(47, ApplicationState.Start, false, ApplicationState.Terminate, StateTransitable.Untransitable);
        [Fact(DisplayName = "No048_GetTransitable_Start_false_End")]
        public void Case048() => AssertState(48, ApplicationState.Start, false, ApplicationState.End, StateTransitable.Untransitable);
        [Fact(DisplayName = "No049_GetTransitable_Start_true_Start")]
        public void Case049() => AssertState(49, ApplicationState.Start, true, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No050_GetTransitable_Start_true_Initialize")]
        public void Case050() => AssertState(50, ApplicationState.Start, true, ApplicationState.Initialize, StateTransitable.Untransitable);
        [Fact(DisplayName = "No051_GetTransitable_Start_true_Ready")]
        public void Case051() => AssertState(51, ApplicationState.Start, true, ApplicationState.Ready, StateTransitable.Untransitable);
        [Fact(DisplayName = "No052_GetTransitable_Start_true_Running")]
        public void Case052() => AssertState(52, ApplicationState.Start, true, ApplicationState.Running, StateTransitable.Untransitable);
        [Fact(DisplayName = "No053_GetTransitable_Start_true_Terminate")]
        public void Case053() => AssertState(53, ApplicationState.Start, true, ApplicationState.Terminate, StateTransitable.Untransitable);
        [Fact(DisplayName = "No054_GetTransitable_Start_true_End")]
        public void Case054() => AssertState(54, ApplicationState.Start, true, ApplicationState.End, StateTransitable.Untransitable);

        [Fact(DisplayName = "No055_GetTransitable_Initialize_false_Start")]
        public void Case055() => AssertState(55, ApplicationState.Initialize, false, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No056_GetTransitable_Initialize_false_Initialize")]
        public void Case056() => AssertState(56, ApplicationState.Initialize, false, ApplicationState.Initialize, StateTransitable.Undefined);
        [Fact(DisplayName = "No057_GetTransitable_Initialize_false_Ready")]
        public void Case057() => AssertState(57, ApplicationState.Initialize, false, ApplicationState.Ready, StateTransitable.Transitable);
        [Fact(DisplayName = "No058_GetTransitable_Initialize_false_Running")]
        public void Case058() => AssertState(58, ApplicationState.Initialize, false, ApplicationState.Running, StateTransitable.Untransitable);
        [Fact(DisplayName = "No059_GetTransitable_Initialize_false_Terminate")]
        public void Case059() => AssertState(59, ApplicationState.Initialize, false, ApplicationState.Terminate, StateTransitable.Transitable);
        [Fact(DisplayName = "No060_GetTransitable_Initialize_false_End")]
        public void Case060() => AssertState(60, ApplicationState.Initialize, false, ApplicationState.End, StateTransitable.Untransitable);
        [Fact(DisplayName = "No061_GetTransitable_Initialize_true_Start")]
        public void Case061() => AssertState(61, ApplicationState.Initialize, true, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No062_GetTransitable_Initialize_true_Initialize")]
        public void Case062() => AssertState(62, ApplicationState.Initialize, true, ApplicationState.Initialize, StateTransitable.Untransitable);
        [Fact(DisplayName = "No063_GetTransitable_Initialize_true_Ready")]
        public void Case063() => AssertState(63, ApplicationState.Initialize, true, ApplicationState.Ready, StateTransitable.Untransitable);
        [Fact(DisplayName = "No064_GetTransitable_Initialize_true_Running")]
        public void Case064() => AssertState(64, ApplicationState.Initialize, true, ApplicationState.Running, StateTransitable.Untransitable);
        [Fact(DisplayName = "No065_GetTransitable_Initialize_true_Terminate")]
        public void Case065() => AssertState(65, ApplicationState.Initialize, true, ApplicationState.Terminate, StateTransitable.Transitable);
        [Fact(DisplayName = "No066_GetTransitable_Initialize_true_End")]
        public void Case066() => AssertState(66, ApplicationState.Initialize, true, ApplicationState.End, StateTransitable.Untransitable);

        [Fact(DisplayName = "No067_GetTransitable_Ready_false_Start")]
        public void Case067() => AssertState(67, ApplicationState.Ready, false, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No068_GetTransitable_Ready_false_Initialize")]
        public void Case068() => AssertState(68, ApplicationState.Ready, false, ApplicationState.Initialize, StateTransitable.Untransitable);
        [Fact(DisplayName = "No069_GetTransitable_Ready_false_Ready")]
        public void Case069() => AssertState(69, ApplicationState.Ready, false, ApplicationState.Ready, StateTransitable.Undefined);
        [Fact(DisplayName = "No070_GetTransitable_Ready_false_Running")]
        public void Case070() => AssertState(70, ApplicationState.Ready, false, ApplicationState.Running, StateTransitable.Transitable);
        [Fact(DisplayName = "No071_GetTransitable_Ready_false_Terminate")]
        public void Case071() => AssertState(71, ApplicationState.Ready, false, ApplicationState.Terminate, StateTransitable.Queueable);
        [Fact(DisplayName = "No072_GetTransitable_Ready_false_End")]
        public void Case072() => AssertState(72, ApplicationState.Ready, false, ApplicationState.End, StateTransitable.Untransitable);
        [Fact(DisplayName = "No073_GetTransitable_Ready_true_Start")]
        public void Case073() => AssertState(73, ApplicationState.Ready, true, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No074_GetTransitable_Ready_true_Initialize")]
        public void Case074() => AssertState(74, ApplicationState.Ready, true, ApplicationState.Initialize, StateTransitable.Untransitable);
        [Fact(DisplayName = "No075_GetTransitable_Ready_true_Ready")]
        public void Case075() => AssertState(75, ApplicationState.Ready, true, ApplicationState.Ready, StateTransitable.Untransitable);
        [Fact(DisplayName = "No076_GetTransitable_Ready_true_Running")]
        public void Case076() => AssertState(76, ApplicationState.Ready, true, ApplicationState.Running, StateTransitable.Untransitable);
        [Fact(DisplayName = "No077_GetTransitable_Ready_true_Terminate")]
        public void Case077() => AssertState(77, ApplicationState.Ready, true, ApplicationState.Terminate, StateTransitable.Queueable);
        [Fact(DisplayName = "No078_GetTransitable_Ready_true_End")]
        public void Case078() => AssertState(78, ApplicationState.Ready, true, ApplicationState.End, StateTransitable.Untransitable);

        [Fact(DisplayName = "No079_GetTransitable_Running_false_Start")]
        public void Case079() => AssertState(79, ApplicationState.Running, false, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No080_GetTransitable_Running_false_Initialize")]
        public void Case080() => AssertState(80, ApplicationState.Running, false, ApplicationState.Initialize, StateTransitable.Untransitable);
        [Fact(DisplayName = "No081_GetTransitable_Running_false_Ready")]
        public void Case081() => AssertState(81, ApplicationState.Running, false, ApplicationState.Ready, StateTransitable.Transitable);
        [Fact(DisplayName = "No082_GetTransitable_Running_false_Running")]
        public void Case082() => AssertState(82, ApplicationState.Running, false, ApplicationState.Running, StateTransitable.Transitable);
        [Fact(DisplayName = "No083_GetTransitable_Running_false_Terminate")]
        public void Case083() => AssertState(83, ApplicationState.Running, false, ApplicationState.Terminate, StateTransitable.Queueable);
        [Fact(DisplayName = "No084_GetTransitable_Running_false_End")]
        public void Case084() => AssertState(84, ApplicationState.Running, false, ApplicationState.End, StateTransitable.Untransitable);
        [Fact(DisplayName = "No085_GetTransitable_Running_true_Start")]
        public void Case085() => AssertState(85, ApplicationState.Running, true, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No086_GetTransitable_Running_true_Initialize")]
        public void Case086() => AssertState(86, ApplicationState.Running, true, ApplicationState.Initialize, StateTransitable.Untransitable);
        [Fact(DisplayName = "No087_GetTransitable_Running_true_Ready")]
        public void Case087() => AssertState(87, ApplicationState.Running, true, ApplicationState.Ready, StateTransitable.Transitable);
        [Fact(DisplayName = "No088_GetTransitable_Running_true_Running")]
        public void Case088() => AssertState(88, ApplicationState.Running, true, ApplicationState.Running, StateTransitable.Untransitable);
        [Fact(DisplayName = "No089_GetTransitable_Running_true_Terminate")]
        public void Case089() => AssertState(89, ApplicationState.Running, true, ApplicationState.Terminate, StateTransitable.Untransitable);
        [Fact(DisplayName = "No090_GetTransitable_Running_true_End")]
        public void Case090() => AssertState(90, ApplicationState.Running, true, ApplicationState.End, StateTransitable.Untransitable);

        [Fact(DisplayName = "No091_GetTransitable_Terminate_false_Start")]
        public void Case091() => AssertState(91, ApplicationState.Terminate, false, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No092_GetTransitable_Terminate_false_Initialize")]
        public void Case092() => AssertState(92, ApplicationState.Terminate, false, ApplicationState.Initialize, StateTransitable.Transitable);
        [Fact(DisplayName = "No093_GetTransitable_Terminate_false_Ready")]
        public void Case093() => AssertState(93, ApplicationState.Terminate, false, ApplicationState.Ready, StateTransitable.Untransitable);
        [Fact(DisplayName = "No094_GetTransitable_Terminate_false_Running")]
        public void Case094() => AssertState(94, ApplicationState.Terminate, false, ApplicationState.Running, StateTransitable.Unqueueable);
        [Fact(DisplayName = "No095_GetTransitable_Terminate_false_Terminate")]
        public void Case095() => AssertState(95, ApplicationState.Terminate, false, ApplicationState.Terminate, StateTransitable.Undefined);
        [Fact(DisplayName = "No096_GetTransitable_Terminate_false_End")]
        public void Case096() => AssertState(96, ApplicationState.Terminate, false, ApplicationState.End, StateTransitable.Transitable);
        [Fact(DisplayName = "No097_GetTransitable_Terminate_true_Start")]
        public void Case097() => AssertState(97, ApplicationState.Terminate, true, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No098_GetTransitable_Terminate_true_Initialize")]
        public void Case098() => AssertState(98, ApplicationState.Terminate, true, ApplicationState.Initialize, StateTransitable.Transitable);
        [Fact(DisplayName = "No099_GetTransitable_Terminate_true_Ready")]
        public void Case099() => AssertState(99, ApplicationState.Terminate, true, ApplicationState.Ready, StateTransitable.Untransitable);
        [Fact(DisplayName = "No100_GetTransitable_Terminate_true_Running")]
        public void Case100() => AssertState(100, ApplicationState.Terminate, true, ApplicationState.Running, StateTransitable.Untransitable);
        [Fact(DisplayName = "No101_GetTransitable_Terminate_true_Terminate")]
        public void Case101() => AssertState(101, ApplicationState.Terminate, true, ApplicationState.Terminate, StateTransitable.Untransitable);
        [Fact(DisplayName = "No102_GetTransitable_Terminate_true_End")]
        public void Case102() => AssertState(102, ApplicationState.Terminate, true, ApplicationState.End, StateTransitable.Transitable);

        [Fact(DisplayName = "No103_GetTransitable_End_false_Start")]
        public void Case103() => AssertState(103, ApplicationState.End, false, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No104_GetTransitable_End_false_Initialize")]
        public void Case104() => AssertState(104, ApplicationState.End, false, ApplicationState.Initialize, StateTransitable.Untransitable);
        [Fact(DisplayName = "No105_GetTransitable_End_false_Ready")]
        public void Case105() => AssertState(105, ApplicationState.End, false, ApplicationState.Ready, StateTransitable.Untransitable);
        [Fact(DisplayName = "No106_GetTransitable_End_false_Running")]
        public void Case106() => AssertState(106, ApplicationState.End, false, ApplicationState.Running, StateTransitable.Untransitable);
        [Fact(DisplayName = "No107_GetTransitable_End_false_Terminate")]
        public void Case107() => AssertState(107, ApplicationState.End, false, ApplicationState.Terminate, StateTransitable.Untransitable);
        [Fact(DisplayName = "No108_GetTransitable_End_false_End")]
        public void Case108() => AssertState(108, ApplicationState.End, false, ApplicationState.End, StateTransitable.Undefined);
        [Fact(DisplayName = "No109_GetTransitable_End_true_Start")]
        public void Case109() => AssertState(109, ApplicationState.End, true, ApplicationState.Start, StateTransitable.Untransitable);
        [Fact(DisplayName = "No110_GetTransitable_End_true_Initialize")]
        public void Case110() => AssertState(110, ApplicationState.End, true, ApplicationState.Initialize, StateTransitable.Untransitable);
        [Fact(DisplayName = "No111_GetTransitable_End_true_Ready")]
        public void Case111() => AssertState(111, ApplicationState.End, true, ApplicationState.Ready, StateTransitable.Untransitable);
        [Fact(DisplayName = "No112_GetTransitable_End_true_Running")]
        public void Case112() => AssertState(112, ApplicationState.End, true, ApplicationState.Running, StateTransitable.Untransitable);
        [Fact(DisplayName = "No113_GetTransitable_End_true_Terminate")]
        public void Case113() => AssertState(113, ApplicationState.End, true, ApplicationState.Terminate, StateTransitable.Untransitable);
        [Fact(DisplayName = "No114_GetTransitable_End_true_End")]
        public void Case114() => AssertState(114, ApplicationState.End, true, ApplicationState.End, StateTransitable.Untransitable);

        [Fact(DisplayName = "No115_GetTransitable_未定義値-1")]
        public void Case115()
        {
            var controller = new TestStateController(ApplicationState.Start, false);
            Assert.Throws<NullReferenceException>(() => controller.GetState((ApplicationState)(-1)));
        }

        [Fact(DisplayName = "No116_GetTransitable_未定義値6")]
        public void Case116()
        {
            var controller = new TestStateController(ApplicationState.Start, false);
            Assert.Throws<NullReferenceException>(() => controller.GetState((ApplicationState)6));
        }

        private static void AssertState(int no, ApplicationState current, bool terminating, ApplicationState next, StateTransitable expected)
        {
            var controller = new TestStateController(current, terminating);
            Assert.Equal(expected, controller.GetState(next));
        }

        private sealed class TestStateController : StateController
        {
            public TestStateController(ApplicationState current, bool terminating)
            {
                currentState = current;
                isTerminating = terminating;
            }

            public StateTransitable GetState(ApplicationState nextState)
            {
                return GetTransitable(nextState);
            }
        }
    }
}