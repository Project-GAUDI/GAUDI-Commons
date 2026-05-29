# Commons(6.1.1)

## 目次

* [IApplicationEngine-RunAsync](#iapplicationengine-runasync)
* [IApplicationEngine-SetApplicationRunningAsync](#iapplicationengine-setapplicationrunningasync)
* [IApplicationEngine-UnsetApplicationRunningAsync](#iapplicationengine-unsetapplicationrunningasync)
* [IApplicationEngine-SetApplicationRestartAsync](#iapplicationengine-setapplicationrestartasync)
* [IApplicationEngine-SendMessageAsync](#iapplicationengine-sendmessageasync)

## IApplicationEngine-RunAsync

:::mermaid
flowchart TD
Start(["開始"])
Start-->S1
S1{"アプリケーションインスタンスを確認"}
S1-->|null|Exception
S1-->|null でない|A1
subgraph "エンジンの初期化"
A1["アプリケーション状態を <br> 初期化状態に変更 *1"]
A1-->A2
A2["アプリケーション初期化処理 *1 <br> IApplicationMain.InitializeAsync"]
A2-->A3
A3["通信の初期化 *1 <br> ModuleClientFactory.CreateAsync"]
A3-->A4
A4["EdgeHub へ接続 *2 <br> IModuleClient.OpenAsync"]
A4-->A5
A5["Logger の設定 *1 <br> ILogger.SetModuleClient"]
A5-->A7
A7["DesiredProperties の取得 <br> IModuleClient.GetTwinAsync"]
A7-->A8
A8["DesiredProperties 更新コールバック処理 *1 <br> IApplicationMain.OnDesiredPropertiesReceivedAsync"]
A8-->A9
A9["DesiredProperties 更新時コールバックを登録 <br> IModuleClient.SetDesiredPropertyUpdateCallbackAsync"]
A9-->A10
A10["ダイレクトメソッドコールバックの設定 <br> IModuleClient.SetMethodHandlerAsync"]
A10-->A11
A11["通信切断時コールバックの設定 <br> IModuleClient.SetConnectionStatusChangedHandlerAsync"]
A11-->A12
A12["アプリケーション起動処理 *1 <br> IApplicationMain.StartAsync"]
end
A12-->B1
B1["アプリケーション状態を <br> 待機状態に変更 *1"]
B1-->LoopStart
LoopStart[/"アプリケーションループ <br> キャンセル要求がされるまで待機"\]
LoopStart-->LoopEnd
LoopEnd[\"アプリケーションループ"/]
LoopEnd-->C1
subgraph "エンジンの解放"
C1["アプリケーション状態を <br> 終了処理状態に変更"]
C1-->C2
C2["アプリケーション解放処理 <br> IApplicationMain.TerminateAsync"]
C2-->C3
C3["ModuleClient を解放"]
end
subgraph "エンジンの停止"
C3-->D1
D1["アプリケーション状態を <br> 終了状態に変更"]
D1-->D2
D2["アプリケーションのキャンセル"]
end
D2-->Finish
Finish(["終了"])
Exception(["Exception を返す"])
:::

*1 処理中にエラーが発生した場合、その後の処理は行わず、エンジンの解放処理へ遷移する  
*2 処理中にエラーが発生した場合、1秒間待機し、リトライし続ける  

## IApplicationEngine-SetApplicationRunningAsync

:::mermaid
flowchart TD
Start(["開始"])
Start-->A1
A1["状態遷移の排他制御を設定"]
A1-->A2
A2{"現在のアプリケーション状態を確認"}
A2-->|"Running"|B1
A2-->|"Ready"|C1
A2-->|"Start、Initialize、Terminate、End"|D1
B1["タスク数をインクリメント"]
B1-->B2
B2["状態変更結果＝ Success に設定"]
B2-->E1
C1["タスク数をデクリメント"]
C1-->C2
C2{"タスク数＝0"}
C2-->|"true"|C3
C2-->|"false"|B2
C3["アプリケーション状態 ＝ Running に設定"]
C3-->B1
D1["状態変更結果＝ Ignored に設定"]
D1-->E1
E1["状態遷移の排他制御を解除"]
E1-->E2
E2["状態変更結果を返す"]
E2-->Finish
Finish(["終了"])
:::

## IApplicationEngine-UnsetApplicationRunningAsync

:::mermaid
flowchart TD
Start(["開始"])
Start-->A1
A1["状態遷移の排他制御を設定"]
A1-->A2
A2{"現在のアプリケーション状態を確認"}
A2-->|"Ready"|B1
A2-->|"Initialize、Running"|C1
A2-->|"Start、Terminate、End"|D1
B1["タスク数をインクリメント"]
B1-->B2
B2["状態変更結果＝ Success に設定"]
B2-->E1
C1["タスク数をデクリメント"]
C1-->C2
C2{"タスク数＝0"}
C2-->|"true"|C3
C2-->|"false"|B2
C3["アプリケーション状態 ＝ Ready に設定"]
C3-->B1
D1["状態変更結果＝ Ignored に設定"]
D1-->E1
E1["状態遷移の排他制御を解除"]
E1-->E2
E2["状態変更結果を返す"]
E2-->Finish
Finish(["終了"])
:::

## IApplicationEngine-SetApplicationRestartAsync

:::mermaid
flowchart TD
Start(["開始"])
Start-->S1
S1{"アプリケーションインスタンスを確認"}
S1-->|null|Exception
S1-->|null でない|C1
subgraph "エンジンの解放"
C1["アプリケーション状態を <br> 終了処理状態に変更"]
C1-->C2
C2["アプリケーション解放処理 <br> IApplicationMain.TerminateAsync"]
C2-->C3
C3["ModuleClient を解放"]
end
C3-->A1
subgraph "エンジンの初期化"
A1["アプリケーション状態を <br> 初期化状態に変更 *1"]
A1-->A2
A2["アプリケーション初期化処理 *1 <br> IApplicationMain.InitializeAsync"]
A2-->A3
A3["通信の初期化 *1 <br> ModuleClientFactory.CreateAsync"]
A3-->A4
A4["EdgeHub へ接続 *2 <br> IModuleClient.OpenAsync"]
A4-->A5
A5["Logger の設定 *1 <br> ILogger.SetModuleClient"]
A5-->A7
A7["DesiredProperties の取得 <br> IModuleClient.GetTwinAsync"]
A7-->A8
A8["DesiredProperties 更新コールバック処理 *1 <br> IApplicationMain.OnDesiredPropertiesReceivedAsync"]
A8-->A9
A9["DesiredProperties 更新時コールバックを登録 <br> IModuleClient.SetDesiredPropertyUpdateCallbackAsync"]
A9-->A10
A10["ダイレクトメソッドコールバックの設定 <br> IModuleClient.SetMethodHandlerAsync"]
A10-->A11
A11["通信切断時コールバックの設定 <br> IModuleClient.SetConnectionStatusChangedHandlerAsync"]
A11-->A12
A12["アプリケーション起動処理 *1 <br> IApplicationMain.StartAsync"]
end
A12-->B1
B1["アプリケーション状態を <br> 待機状態に変更 *1"]
B1-->Finish
Finish(["終了"])
Exception(["Exception を返す"])
:::

*1 処理中にエラーが発生した場合、その後の処理は行わず、エンジンの解放処理へ遷移する  
*2 処理中にエラーが発生した場合、1秒間待機し、リトライし続ける  

## IApplicationEngine-SendMessageAsync

:::mermaid
flowchart TD
Start(["開始"])
Start-->A1
A1{"現在の IoT 接続状態を確認"}
A1-->|"Connected"|B1
A1-->|"Disabled、Disconnected"|C1
A1-->|"Disconnected_Retrying"|D1
B1["メッセージ送信 <br> IModuleClient.SendEventAsync"]
B1-->Finish
C1["Exception を返す"]
C1-->Finish
D1["1秒間隔で IoT 接続状態が <br> Connected になるまで待つ"]
D1-->B1
Finish(["終了"])
:::
