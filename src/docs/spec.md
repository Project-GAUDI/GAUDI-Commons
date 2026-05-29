---
slug: "/iotedge-v2/iotedge-v2-Commons/6.1.1/spec"
version: "6.1.1"
---

# Commons(6.1.1)

## 目次

* [Deployment 設定値](#deployment-設定値)
  * [環境変数](#環境変数)
* [利用可能クラス](#利用可能クラス)
* [Applications](#applications)
  * [Applications-デリゲート](#applications-デリゲート)
  * [Applications-列挙型](#applications-列挙型)
  * [Applications-IApplicationMain-インターフェース](#applications-iapplicationmain-インターフェース)
  * [Applications-IApplicationEngine-インターフェース](#applications-iapplicationengine-インターフェース)
  * [Applications-ApplicationEngineFactory-クラス](#applications-applicationenginefactory-クラス)
* [DirectMethod](#directmethod)
  * [DirectMethod-IDirectMethodRunner-インターフェース](#directmethod-idirectmethodrunner-インターフェース)
  * [DirectMethod-DirectMethodCaller-クラス](#directmethod-directmethodcaller-クラス)
  * [DirectMethod-DirectMethodRequest-クラス](#directmethod-directmethodrequest-クラス)
  * [DirectMethod-DirectMethodResponse-クラス](#directmethod-directmethodresponse-クラス)
* [JsonSerializer](#jsonserializer)
  * [JsonSerializer-列挙型](#jsonserializer-列挙型)
  * [JsonSerializer-IJsonSerializer-インターフェース](#jsonserializer-ijsonserializer-インターフェース)
  * [JsonSerializer-JsonSerializerFactory-クラス](#jsonserializer-jsonserializerfactory-クラス)
* [Logger](#logger)
  * [Logger-ILogger-インターフェース](#logger-ilogger-インターフェース)
  * [Logger-LoggerFactory-クラス](#logger-loggerfactory-クラス)
* [Message](#message)
  * [Message-IotMessage-クラス](#message-iotmessage-クラス)
  * [Message-JsonMessage-クラス](#message-jsonmessage-クラス)
* [ModuleClient](#moduleclient)
  * [ModuleClient-デリゲート](#moduleclient-デリゲート)
  * [ModuleClient-列挙型](#moduleclient-列挙型)
  * [ModuleClient-IModuleClient-インターフェース](#moduleclient-imoduleclient-インターフェース)
  * [ModuleClient-ModuleClientFactory-クラス](#moduleclient-moduleclientfactory-クラス)
* [Utilities](#utilities)
  * [Utilities-列挙型](#utilities-列挙型)
  * [Utilities-I2CActionExtentions-クラス](#utilities-i2cactionextentions-クラス)
  * [Utilities-I2CCommandsMessage-クラス](#utilities-i2ccommandsmessage-クラス)
  * [Utilities-I2CCommand-クラス](#utilities-i2ccommand-クラス)
  * [Utilities-Util-クラス](#utilities-util-クラス)

## Deployment 設定値

### 環境変数

#### 環境変数の値

| Key                                   | Type    | Required | Conditions                                    | Default        | Description                                                                                                                                                             |
| ------------------------------------- | ------- | -------- | --------------------------------------------- | -------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| TransportProtocol                     | string  |          | ["Amqp", "Mqtt"]                              | Amqp           | ModuleClient の接続プロトコル。                                                                                                                                         |
| LogLevel                              | string  |          | ["trace", "debug", "info", "warn", "error"]   | info           | 出力ログレベル。                                                                                                                                                        |
| MessageSizeLimitExpansion             | boolean |          | ["true", "false"]                             | false          | モジュール間のメッセージサイズの上限拡張指定。<br>true：メッセージサイズ上限が拡張され16MBになる(※1,2)<br>false：メッセージサイズ上限はデフォルトの256KBになる          |
| IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER | string  |          | ["NEWTONSOFTJSON", "SYSRUNTIMESERIALIZATION"] | NEWTONSOFTJSON | Json化クラスの使用Jsonライブラリのデフォルトを切り替える。<br>NEWTONSOFTJSON：Newtonsoft.Jsonを使用<br>SYSRUNTIMESERIALIZATION：System.Runtime.Serialization.Jsonを使用 |

※1：**IoTHubへ**のupstreamする際のメッセージサイズ上限は256KBのまま変わらない。  
送信してしまった場合、edgeHubからの送信がエラーとなり、その後のメッセージ送信も停滞してしまう。  
Edge→Fog間の拡張サイズメッセージの送信は可能。  
※2：edgeHubは、メッセージサイズ拡張対応が入っているGAUDIIotEdge-Hubを使用し、edgeHub・送信先モジュールにもMessageSizeLimitExpansion=trueが設定されている必要がある。  

## 利用可能クラス

## Applications

### Applications-デリゲート

| 名前                | 引数（型：説明）                                                                  | 戻り値（型：説明）                           | 説明                                     |
| ------------------- | --------------------------------------------------------------------------------- | -------------------------------------------- | ---------------------------------------- |
| MessageEventHandler | string：インプット名<br>IotMessage：イベントコールバック<br>object：拡張データ    | Task\<bool>：処理成功(true)、処理失敗(false) | メッセージ受信イベントハンドラー         |
| DirectMethodHandler | string：メソッド名<br>DirectMethodRequest：リクエストデータ<br>object：拡張データ | Task\<DirectMethodResponse>：実行結果        | ダイレクトメソッド受信イベントハンドラー |

### Applications-列挙型

| 名前                         | 説明                         |
| ---------------------------- | ---------------------------- |
| ApplicationState             | アプリケーション状態         |
| - Start                      | - 開始状態                   |
| - Initialize                 | - 初期化状態                 |
| - Ready                      | - 待機状態                   |
| - Running                    | - 実行中状態                 |
| - Terminate                  | - 終了処理状態               |
| - End                        | - 終了状態                   |
| ApplicationStateChangeResult | アプリケーション状態変更結果 |
| - Success                    | - 成功                       |
| - Ignored                    | - 無視                       |

### Applications-IApplicationMain-インターフェース

アプリケーションのライフサイクル管理、および、DesiredProperties 更新時のコールバック処理を定義しているインターフェース。  
IAsyncDisposable を継承している。  

#### Applications-IApplicationMain-メソッド

| 静的 | 名前                             | 引数（型：説明）            | 戻り値（型：説明）                           | 説明                                                                                                                                  |
| ---- | -------------------------------- | --------------------------- | -------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
|      | InitializeAsync                  | なし                        | Task\<bool>：処理成功(true)、処理失敗(false) | アプリケーション初期化処理（システム初期化前に呼び出される）                                                                          |
|      | StartAsync                       | なし                        | Task\<bool>：処理成功(true)、処理失敗(false) | アプリケーション起動処理（システム初期化完了後に呼び出される）                                                                        |
|      | TerminateAsync                   | なし                        | Task\<bool>：処理成功(true)、処理失敗(false) | アプリケーション解放処理（システム終了要求、または、再起動要求があった際に呼び出される）                                              |
|      | OnDesiredPropertiesReceivedAsync | JObject：Desired Properties | Task\<bool>：処理成功(true)、処理失敗(false) | DesiredProperties 更新コールバック処理（システム初期化完了後（StartAsync より前）と、DesiredProperties 更新通知受信時に呼び出される） |

### Applications-IApplicationEngine-インターフェース

アプリケーションの実行制御・状態管理・メッセージ/ダイレクトメソッドハンドラの登録・メッセージ送信など、IoT Edge アプリケーションのエンジン機能を定義しているインターフェース。  
IAsyncDisposable を継承している。  

#### Applications-IApplicationEngine-メソッド

| 静的 | 名前                         | 引数（型：説明）                                                                                            | 戻り値（型：説明）                                                | 説明                                                                                 |
| ---- | ---------------------------- | ----------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------- | ------------------------------------------------------------------------------------ |
|      | SetApplication               | IApplicationMain：インスタンス                                                                              | void                                                              | 引数で受け取った IApplicationMain インスタンスを内部に保持する。                     |
|      | RunAsync                     | なし                                                                                                        | Task：タスク                                                      | アプリケーションエンジンを起動する。<br>詳細は internal-spec.md を参照。             |
|      | SetApplicationRunningAsync   | なし                                                                                                        | Task\<ApplicationStateChangeResult>：アプリケーション状態変更結果 | アプリケーション実行権を獲得する。<br>詳細は internal-spec.md を参照。               |
|      | UnsetApplicationRunningAsync | なし                                                                                                        | Task\<ApplicationStateChangeResult>：アプリケーション状態変更結果 | アプリケーション実行権を解放する。<br>詳細は internal-spec.md を参照。               |
|      | SetApplicationRestartAsync   | なし                                                                                                        | Task\<ApplicationStateChangeResult>：アプリケーション状態変更結果 | アプリケーションを再起動する（致命的障害時用）。<br>詳細は internal-spec.md を参照。 |
|      | IsTerminating                | なし                                                                                                        | bool：終了中(true)、未起動・通常状態(false)                       | アプリケーションが終了中かどうか判定する。                                           |
|      | AddMessageInputHandlerAsync  | string：インプット名<br>MessageEventHandler：イベントコールバック<br>object：拡張データ（デフォルト：null） | Task：タスク                                                      | メッセージ受信時のイベントハンドラを追加する。                                       |
|      | SendMessageAsync             | string：アウトプット名<br>IotMessage：送信メッセージ                                                        | Task：タスク                                                      | 指定したアウトプット名でメッセージを送信する。<br>詳細は internal-spec.md を参照。   |
|      | AddDirectMethodHandlerAsync  | string：メソッド名<br>DirectMethodHandler：イベントコールバック<br>object：拡張データ（デフォルト：null）   | Task：タスク                                                      | ダイレクトメソッド受信時のイベントハンドラを追加する。                               |

### Applications-ApplicationEngineFactory-クラス

IApplicationEngine インスタンスを取得するためのファクトリークラス。  

#### Applications-ApplicationEngineFactory-メソッド

| 静的 | 名前      | 引数（型：説明） | 戻り値（型：説明）               | 説明                                                |
| ---- | --------- | ---------------- | -------------------------------- | --------------------------------------------------- |
| ○    | GetEngine | なし             | IApplicationEngine：インスタンス | IApplicationEngine クラスのインスタンスを取得する。 |

---

## DirectMethod

### DirectMethod-IDirectMethodRunner-インターフェース

ダイレクトメソッドの実行に関する処理を提供するインターフェース。  
IDisposable を継承している。  

#### DirectMethod-IDirectMethodRunner-メソッド

| 静的 | 名前         | 引数（型：説明）                      | 戻り値（型：説明）                           | 説明                                                      |
| ---- | ------------ | ------------------------------------- | -------------------------------------------- | --------------------------------------------------------- |
|      | ParseRequest | string：リクエスト文字列（JSON 形式） | Task\<bool>：処理成功(true)、処理失敗(false) | リクエスト文字列（JSON 形式）を解析して、処理結果を返す。 |
|      | Run          | なし                                  | Task\<bool>：処理成功(true)、処理失敗(false) | ダイレクトメソッドを実行する。                            |
|      | GetResult    | なし                                  | DirectMethodResponse：実行結果               | ダイレクトメソッドの実行結果を取得する。                  |

### DirectMethod-DirectMethodCaller-クラス

ダイレクトメソッド呼び出し処理を統括するユーティリティクラス。  

#### DirectMethod-DirectMethodCaller-メソッド

| 静的 | 名前 | 引数（型：説明）                    | 戻り値（型：説明）                    | 説明                                                                                                                               |
| ---- | ---- | ----------------------------------- | ------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| ○    | Run  | DirectMethodRequest：リクエスト情報 | Task\<DirectMethodResponse>：実行結果 | 指定したリクエスト情報をもとに、IDirectMethodRunner インターフェースの ParseRequest / Run / GetResult を呼び出し、実行結果を返す。 |

### DirectMethod-DirectMethodRequest-クラス

ダイレクトメソッド呼び出し時に利用するリクエスト情報を保持するデータコンテナ。  

#### DirectMethod-DirectMethodRequest-プロパティ

| 名前        | 型     | 説明                   |
| ----------- | ------ | ---------------------- |
| MethodName  | string | メソッド名             |
| RequestJson | string | リクエスト JSON 文字列 |

#### DirectMethod-DirectMethodRequest-コンストラクタ

 | 引数（型：説明）                                     | 説明                         |
 | ---------------------------------------------------- | ---------------------------- |
 | なし                                                 | デフォルトコンストラクタ     |
 | string：メソッド名<br>string：リクエスト JSON 文字列 | パラメータ指定コンストラクタ |

### DirectMethod-DirectMethodResponse-クラス

ダイレクトメソッドの実行結果を保持するためのデータコンテナ。  

#### DirectMethod-DirectMethodResponse-プロパティ

| 名前    | 型                         | 説明                   |
| ------- | -------------------------- | ---------------------- |
| Status  | int                        | ステータスコード       |
| Results | Dictionary<string, object> | 実行結果ディクショナリ |

#### DirectMethod-DirectMethodResponse-コンストラクタ

 | 引数（型：説明）                                                                                                     | 説明           |
 | -------------------------------------------------------------------------------------------------------------------- | -------------- |
 | int：ステータスコード（デフォルト：0）<br>string：結果キー（デフォルト：null）<br>object：結果値（デフォルト：null） | コンストラクタ |

---

## JsonSerializer

JSON のシリアライザとして、以下の2種類を実装している。

* System.Runtime を使用：/（スラッシュ）、”（ダブルクウォート）、\（バックスラッシュ）は、"\"エスケープされる
* Newtonsoft.Json を使用：”（ダブルクウォート）、\（バックスラッシュ）は、"\"エスケープされる

/（スラッシュ）がエスケープされる事でメッセージの upstream 時に問題が発生するため、Newtonsoft.Json の使用を推奨

### JsonSerializer-列挙型

| 名前                      | 説明                                           |
| ------------------------- | ---------------------------------------------- |
| SerializerType            | シリアライザタイプ                             |
| - Default                 | - デフォルトのシリアライザタイプ               |
| - SysRuntimeSerialization | - System.Runtime を使用するシリアライザタイプ  |
| - NewtonsoftJson          | - Newtonsoft.Json を使用するシリアライザタイプ |

### JsonSerializer-IJsonSerializer-インターフェース

JSON シリアライズ／デシリアライズの共通インターフェース。  

#### JsonSerializer-IJsonSerializer-メソッド

| 静的 | 名前           | 引数（型：説明）             | 戻り値（型：説明）                           | 説明                                                |
| ---- | -------------- | ---------------------------- | -------------------------------------------- | --------------------------------------------------- |
|      | Serialize      | TargetType：対象オブジェクト | string：シリアライズされた文字列             | オブジェクトを JSON 文字列にシリアライズする。      |
|      | SerializeBytes | TargetType：対象オブジェクト | byte[]：シリアライズされたバイト配列         | オブジェクトを JSON バイト配列にシリアライズする。  |
|      | Deserialize    | string：対象 JSON 文字列     | TargetType：デシリアライズされたオブジェクト | JSON 文字列をオブジェクトにデシリアライズする。     |
|      | Deserialize    | byte[]：対象 JSON バイト配列 | TargetType：デシリアライズされたオブジェクト | JSON バイト配列をオブジェクトにデシリアライズする。 |

### JsonSerializer-JsonSerializerFactory-クラス

IJsonSerializer のインスタンスを生成するファクトリクラス。  

#### JsonSerializer-JsonSerializerFactory-メソッド

| 静的 | 名前              | 引数（型：説明）                                          | 戻り値（型：説明）            | 説明                                                                                                                                                                                                                                                                                                            |
| ---- | ----------------- | --------------------------------------------------------- | ----------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ○    | GetJsonSerializer | SerializerType：シリアライザタイプ（デフォルト：Default） | IJsonSerializer：インスタンス | 指定したシリアライザタイプに対応した IJsonSerializer クラスのインスタンスを取得する。<br>シリアライザタイプが Default の場合、ENV:IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER が指定されていれば、それを使用する。<br>ENV:IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER が指定されていなければ、"NEWTONSOFTJSON" を使用する。 |

---

## Logger

### Logger-ILogger-インターフェース

ログ出力機能の共通インターフェース。  

#### Logger-ILogger-列挙型  

| 名前     | 値  | 説明                     |
| -------- | --- | ------------------------ |
| LogLevel |     | ログレベル               |
| - TRACE  | 0   | - トレース               |
| - DEBUG  | 1   | - デバッグ               |
| - INFO   | 2   | - インフォ（デフォルト） |
| - WARN   | 3   | - ワーニング             |
| - ERROR  | 4   | - エラー                 |

#### Logger-ILogger-プロパティ

| 名前           | 型            | 説明                   |
| -------------- | ------------- | ---------------------- |
| OutputLogLevel | LogLevel      | 出力ログレベル         |
| OutputName     | string        | ログ出力名             |
| MyClient       | IModuleClient | モジュールクライアント |
| OutTextWriter  | TextWriter    | ログ出力先ストリーム   |

#### Logger-ILogger-定数

| 名前                         | 型  | 値  | 説明                               |
| ---------------------------- | --- | --- | ---------------------------------- |
| SecondsDefinition_OUTOFRANGE | int | -2  | 強制レベル設定時の時間定義－未設定 |
| SecondsDefinition_UNLIMITED  | int | -1  | 強制レベル設定時の時間定義－無制限 |
| SecondsDefinition_CANCEL     | int | 0   | 強制レベル設定時の時間定義－解除   |
| SecondsDefinition_MINIMUM    | int | 1   | 強制レベル設定時の時間定義－最小値 |

#### Logger-ILogger-メソッド

| 静的 | 名前                 | 引数（型：説明）                                                                                    | 戻り値（型：説明）            | 説明                                                                                                                                                                                                                                                               |
| ---- | -------------------- | --------------------------------------------------------------------------------------------------- | ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
|      | SetOutputLogLevel    | string：出力ログレベル                                                                              | void                          | 出力ログレベルを文字列から設定する。<br>指定した出力ログレベルが不正な場合、ArgumentException をスローする。                                                                                                                                                       |
|      | SetModuleClient      | IModuleClient：モジュールクライアント                                                               | void                          | Loggerで使用するモジュールクライアントをセットする。                                                                                                                                                                                                               |
|      | SetOutputTextWriter  | TextWriter：出力ストリーム                                                                          | void                          | 出力ストリームを設定する。                                                                                                                                                                                                                                         |
|      | WriteLog             | LogLevel：出力ログレベル<br>string：ログメッセージ<br>bool：メッセージ送信有無（デフォルト：false） | void                          | 指定した出力ログレベルに応じて、ログメッセージを出力する。<br>メッセージ送信有無が true の場合、ログメッセージをアップロードする。<br>（アップロードには、SetModuleClient によるモジュールクライアントのセットが必要）<br>処理に失敗した場合、警告ログを出力する。 |
|      | SetMandatoryLogLevel | string：出力ログレベル<br>int：強制レベル設定時の時間定義                                           | void                          | 指定した時間定義に応じて、強制ログレベルを変更する。<br>時間定義が0の場合：解除<br>時間定義が1以上の場合：指定時間（秒）後まで適用<br>時間定義が-1の場合：無制限<br>時間定義が上記以外：ArgumentException をスローする                                             |
|      | IsLogLevelToOutput   | LogLevel：出力ログレベル                                                                            | bool：有効(true)／無効(false) | 指定した出力ログレベルが有効かチェックする。                                                                                                                                                                                                                       |

### Logger-LoggerFactory-クラス

ILogger インスタンスを取得するためのファクトリークラス。  

#### Logger-LoggerFactory-メソッド

| 静的 | 名前      | 引数（型：説明）               | 戻り値（型：説明）    | 説明                                                                                                                           |
| ---- | --------- | ------------------------------ | --------------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| ○    | GetLogger | Type：ログ出力時に name を使用 | ILogger：インスタンス | 指定した Type に対応した ILogger クラスのインスタンスを取得する。<br>環境変数:LogLevel を取得し、ログレベルを設定する。<br>環境変数:LogLevel が不正な場合、ポリシーとしてはエラーを返すべきだが、ログが出力できない状態としないため、デフォルトのログレベル（INFO）とする。|

---

## Message

### Message-IotMessage-クラス

Azure IoT Edge の Message オブジェクトをラップし、メッセージボディやプロパティへの安全かつ柔軟なアクセスを提供するクラス。  
IDisposable を継承している。  

#### Message-IotMessage-列挙型

| 名前            | 説明                       |
| --------------- | -------------------------- |
| PropertySetMode | プロパティ設定モード       |
| - Add           | - 追加のみ（上書きしない） |
| - Modify        | - 更新のみ（追加しない）   |
| - AddOrModify   | - 追加または更新           |

#### Message-IotMessage-コンストラクタ

 | 引数（型：説明）                    | 説明                                                                                       |
 | ----------------------------------- | ------------------------------------------------------------------------------------------ |
 | なし                                | 空のメッセージ Body で、IotMessage インスタンスを生成する。*1                              |
 | byte[]：メッセージ Body バイト配列  | 指定したメッセージ Body で、IotMessage インスタンスを生成する。*1                          |
 | string：メッセージ Body 文字列      | 指定したメッセージ Body で、IotMessage インスタンスを生成する。*1                          |
 | Stream：メッセージ Body ストリーム  | 指定したメッセージ Body で、IotMessage インスタンスを生成する。*1                          |
 | IotMessage：IotMessage インスタンス | 既存の IotMessage インスタンスをコピーして生成する（メッセージプロパティもコピーする）。*1 |

 *1 ContentType プロパティが設定されていない場合は "application/json" を設定、ContentEncoding プロパティが設定されていない場合は "utf-8" を設定する。  

#### Message-IotMessage-メソッド

| 静的 | 名前                  | 引数（型：説明）                                                                                                   | 戻り値（型：説明）                                            | 説明                                                                                    |
| ---- | --------------------- | ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------- | --------------------------------------------------------------------------------------- |
|      | Dispose               | なし                                                                                                               | void                                                          | リソースを解放する。                                                                    |
|      | GetBytes              | なし                                                                                                               | byte[]：メッセージ Body データ                                | メッセージ Body をバイト配列で取得して返す。                                            |
|      | GetBodyString         | なし                                                                                                               | string：メッセージ Body データ                                | メッセージ Body を文字列で取得して返す。                                                |
|      | GetBodyStream         | なし                                                                                                               | Stream：メッセージ Body データ                                | メッセージ Body をストリームで取得して返す。                                            |
|      | GetBodyLength         | なし                                                                                                               | long：メッセージ Body サイズ                                  | メッセージ Body のサイズを取得する。                                                    |
|      | SetProperty           | string：プロパティキー<br>string：プロパティ値<br>PropertySetMode：プロパティ設定モード（デフォルト：AddOrModify） | bool：設定した(true)／設定しなかった(false)                   | 指定したプロパティ設定モードに応じて、プロパティを追加・更新する。                      |
|      | GetProperty           | string：プロパティキー                                                                                             | string：プロパティ値                                          | 指定したプロパティキーのプロパティ値を取得する。<br>キーが存在しない場合、null を返す。 |
|      | SetProperties         | IDictionary<string, string>：プロパティ辞書<br>PropertySetMode：プロパティ設定モード（デフォルト：AddOrModify）    | bool：全て設定した(true)／一部または全て設定しなかった(false) | 指定したプロパティ設定モードに応じて、複数のプロパティを一括で追加・更新する。          |
|      | GetProperties         | なし                                                                                                               | IDictionary<string, string>：プロパティ辞書                   | 全プロパティを取得する。<br>プロパティが存在しない場合、null を返す。                   |
| ○    | BytesToString         | byte[]：変換するバイト配列                                                                                         | string：変換した文字列                                        | バイト配列を文字列に変換する。                                                          |
| ○    | StringToByates        | string：変換する文字列                                                                                             | byte[]：変換したバイト配列                                    | 文字列をバイト配列に変換する。                                                          |
|      | GetMessageId          | なし                                                                                                               | string：MessageId                                             | MessageId プロパティを取得する。                                                        |
|      | SetMessageId          | string：MessageId                                                                                                  | void                                                          | MessageId プロパティを設定する。                                                        |
|      | GetContentType        | なし                                                                                                               | string：ContentType                                           | ContentType プロパティを取得する。                                                      |
|      | SetContentType        | string：ContentType                                                                                                | void                                                          | ContentType プロパティを設定する。                                                      |
|      | GetContentEncoding    | なし                                                                                                               | string：ContentEncoding                                       | ContentEncoding プロパティを取得する。                                                  |
|      | SetContentEncoding    | string：ContentEncoding                                                                                            | void                                                          | ContentEncoding プロパティを設定する。                                                  |
|      | GetConnectionDeviceId | なし                                                                                                               | string：ConnectionDeviceId                                    | ConnectionDeviceId プロパティを取得する。                                               |
|      | GetConnectionModuleId | なし                                                                                                               | string：ConnectionModuleId                                    | ConnectionModuleId プロパティを取得する。                                               |

### Message-JsonMessage-クラス

レコード情報のリストを JSON 形式で管理・シリアライズ／デシリアライズするためのデータコンテナ。  

#### Message-JsonMessage-フィールド

| 名前       | 型                | 説明                      |
| ---------- | ----------------- | ------------------------- |
| RecordList | List\<RecordInfo> | RecordInfo クラスのリスト |

#### Message-JsonMessage-メソッド

| 静的 | 名前                     | 引数（型：説明）             | 戻り値（型：説明）           | 説明                                                                                                     |
| ---- | ------------------------ | ---------------------------- | ---------------------------- | -------------------------------------------------------------------------------------------------------- |
| ○    | DeserializeJsonMessage   | string：JSON 文字列          | JsonMessage：JSON メッセージ | JSON 文字列から JSON メッセージへデシリアライズする。<br>デシリアライズに失敗した場合、null を返す。     |
| ○    | DeserializeJsonMessage   | byte[]：JSON バイト配列      | JsonMessage：JSON メッセージ | JSON バイト配列から JSON メッセージへデシリアライズする。<br>デシリアライズに失敗した場合、null を返す。 |
| ○    | SerializeJsonMessage     | JsonMessage：JSON メッセージ | string：JSON 文字列          | JSON メッセージから JSON 文字列へシリアライズする。                                                      |
| ○    | SerializeJsonMessageByte | JsonMessage：JSON メッセージ | byte[]：JSON バイト配列      | JSON メッセージから JSON バイト配列へシリアライズする。<br>シリアライズに失敗した場合、null を返す。     |

#### Message-JsonMessage-RecordInfo-クラス（内部クラス）

#### Message-JsonMessage-RecordInfo-フィールド

| 名前         | 型            | 説明                   |
| ------------ | ------------- | ---------------------- |
| RecordHeader | List\<string> | レコードヘッダーリスト |
| RecordData   | List\<string> | レコードデータリスト   |

#### Message-JsonMessage-RecordInfo-メソッド

| 静的 | 名前                    | 引数（型：説明）              | 戻り値（型：説明）            | 説明                                                                                                       |
| ---- | ----------------------- | ----------------------------- | ----------------------------- | ---------------------------------------------------------------------------------------------------------- |
| ○    | DeserializeRecordInfo   | string：JSON 文字列           | RecordInfo：RecordInfo クラス | JSON 文字列から RecordInfo クラスへデシリアライズする。<br>デシリアライズに失敗した場合、null を返す。     |
| ○    | DeserializeRecordInfo   | byte[]：JSON バイト配列       | RecordInfo：RecordInfo クラス | JSON バイト配列から RecordInfo クラスへデシリアライズする。<br>デシリアライズに失敗した場合、null を返す。 |
| ○    | SerializeRecordInfo     | RecordInfo：RecordInfo クラス | string：JSON 文字列           | RecordInfo クラスから JSON 文字列へシリアライズする。                                                      |
| ○    | SerializeRecordInfoByte | RecordInfo：RecordInfo クラス | byte[]：JSON バイト配列       | RecordInfo クラスから JSON バイト配列へシリアライズする。<br>シリアライズに失敗した場合、null を返す。     |

---

## ModuleClient

### ModuleClient-デリゲート

| 名前                             | 引数（型：説明）                                                           | 戻り値（型：説明）                           | 説明                           |
| -------------------------------- | -------------------------------------------------------------------------- | -------------------------------------------- | ------------------------------ |
| IotMessageHandler                | IotMessage：IoT メッセージ<br>object：ユーザーコンテキスト                 | Task\<MessageResponse>：メッセージレスポンス | Iot メッセージハンドラの定義   |
| IotConnectionStatusChangeHandler | IotConnectionStatus：接続状態<br>IotConnectionStatusChangeReason：変更理由 | Task：タスク                                 | Iot 接続状態変更ハンドラの定義 |

### ModuleClient-列挙型

| 名前                            | 値  | 説明                       |
| ------------------------------- | --- | -------------------------- |
| TransportTopic                  |     | トランスポートトピック種別 |
| - Iothub                        | 0   | - IoThub トピック          |
| IotConnectionStatus             |     | IoT 接続状態               |
| - Disconnected                  | 0   | - 切断                     |
| - Connected                     | 1   | - 接続                     |
| - Disconnected_Retrying         | 2   | - 再接続を試みている       |
| - Disabled                      | 3   | - 接続が閉じられた         |
| IotConnectionStatusChangeReason |     | IoT 接続状態変更理由       |
| - Connection_Ok                 | 0   | - 正常                     |
| - Expired_SAS_Token             | 1   | - SAS トークン期限切れ     |
| - Device_Disabled               | 2   | - デバイス無効             |
| - Bad_Credential                | 3   | - 認証情報不正             |
| - Retry_Expired                 | 4   | - リトライ期限切れ         |
| - No_Network                    | 5   | - ネットワークなし         |
| - Communication_Error           | 6   | - 通信エラー               |
| - Client_Close                  | 7   | - クライアントクローズ     |

### ModuleClient-IModuleClient-インターフェース

Azure IoT Edge モジュールクライアントの基本的な操作（接続、切断、メッセージ送受信、Twin 管理、ハンドラ設定など）を抽象化したインターフェース。  
IDisposable を継承している。  

#### ModuleClient-IModuleClient-プロパティ

| 名前             | 型                  | 説明     |
| ---------------- | ------------------- | -------- |
| ConnectionStatus | IotConnectionStatus | 接続状態 |

#### ModuleClient-IModuleClient-メソッド

| 静的 | 名前                                   | 引数（型：説明）                                                                                  | 戻り値（型：説明）     | 説明                                                                                                                                                        |
| ---- | -------------------------------------- | ------------------------------------------------------------------------------------------------- | ---------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
|      | OpenAsync                              | なし                                                                                              | Task：タスク           | 接続開始処理                                                                                                                                                |
|      | CloseAsync                             | なし                                                                                              | Task：タスク           | 切断処理                                                                                                                                                    |
|      | GetTwinAsync                           | なし                                                                                              | Task\<Twin>：Twin 情報 | Twin 情報取得処理                                                                                                                                           |
|      | SetDesiredPropertyUpdateCallbackAsync  | DesiredPropertyUpdateCallback：Desired プロパティ更新コールバック<br>object：ユーザーコンテキスト | Task：タスク           | Desired プロパティ更新時のコールバックを設定する。                                                                                                          |
|      | SetInputMessageHandlerAsync            | string：インプット名<br>IotMessageHandler：IoT メッセージハンドラ<br>object：ユーザーコンテキスト | Task：タスク           | 入力メッセージを処理するためのコールバックを設定する。                                                                                                      |
|      | SetMethodHandlerAsync                  | string：メソッド名<br>MethodCallback：メソッドコールバック<br>object：ユーザーコンテキスト        | Task：タスク           | ダイレクトメソッドを処理するためのコールバックを設定する。                                                                                                  |
|      | SendEventAsync                         | string：アウトプット名<br>IotMessage：IoT メッセージ                                              | Task：タスク           | 指定したアプトプット名と IoT メッセージを使って、IoT ハブにイベントを送信する。<br>IoT メッセージに メッセージID が設定されていない場合、生成して設定する。 |
|      | UpdateReportedPropertiesAsync          | TwinCollection：Reported プロパティ                                                               | Task：タスク           | Reported プロパティを更新する。                                                                                                                             |
|      | SetConnectionStatusChangedHandlerAsync | IotConnectionStatusChangeHandler：Iot 接続状態変更ハンドラ                                        | Task：タスク           | 接続状態の変更通知を処理するためのコールバックを設定する。<br>受信した接続状態は、プロパティ：ConnectionStatus に保存する。                                 |

### ModuleClient-ModuleClientFactory-クラス

IModuleClient インスタンスを取得するためのファクトリークラス。  

#### ModuleClient-ModuleClientFactory-メソッド

| 静的 | 名前        | 引数（型：説明） | 戻り値（型：説明）                                       | 説明                                                                                                                                                                                       |
| ---- | ----------- | ---------------- | -------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| ○    | CreateAsync | なし             | Task\<IModuleClient>：IModuleClient クラスのインスタンス | ENV:TransportProtocol の値に基づいてトランスポート設定を選定し、IModuleClient クラスのインスタンスを生成する。<br>ENV:TransportProtocol の値が不正な場合、ArgumentException をスローする。 |

---

## Utilities

### Utilities-列挙型

| 名前              | 説明                         |
| ----------------- | ---------------------------- |
| I2CAction         | I2C アクション種別           |
| - Read            | - 読取                       |
| - Write           | - 書込                       |
| - Wait            | - 待機                       |
| TransportProtocol | トランスポートプロトコル種別 |
| - Amqp            | AMQP プロトコル              |
| - Mqtt            | MQTT プロトコル              |

### Utilities-I2CActionExtentions-クラス

I2C 通信におけるアクション種別の変換処理を提供する拡張メソッドクラス。  

#### Utilities-I2CActionExtentions-メソッド

| 静的 | 名前        | 引数（型：説明）                   | 戻り値（型：説明）            | 説明                                                                                                                         |
| ---- | ----------- | ---------------------------------- | ----------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| ○    | ToString    | this I2CAction：I2C アクション種別 | string：文字列（小文字）      | I2C アクション種別を文字列（小文字）へ変換する。<br>想定外のアクション種別が指定された場合、ArgumentException をスローする。 |
| ○    | ToI2CAction | this string：変換対象文字列        | I2CAction：I2C アクション種別 | 文字列をI2C アクション種別へ変換する。<br>想定外の変換対象文字列が指定された場合、ArgumentException をスローする。           |

### Utilities-I2CCommandsMessage-クラス

I2C 通信で複数のコマンドをまとめて管理・シリアライズ／デシリアライズするためのクラス。  

#### Utilities-I2CCommandsMessage-フィールド

| 名前        | 型                | 説明           |
| ----------- | ----------------- | -------------- |
| CommandList | List\<I2CCommand> | コマンドリスト |

#### Utilities-I2CCommandsMessage-メソッド

| 静的 | 名前               | 引数（型：説明）                                    | 戻り値（型：説明）                                  | 説明                                                                                                         |
| ---- | ------------------ | --------------------------------------------------- | --------------------------------------------------- | ------------------------------------------------------------------------------------------------------------ |
| ○    | DeserializeJson    | string：JSON 文字列                                 | I2CCommandsMessage：I2CCommandsMessage インスタンス | JSON 文字列から I2CCommandsMessage へデシリアライズする。<br>デシリアライズに失敗した場合、null を返す。     |
| ○    | DeserializeJson    | byte[]：JSON バイト配列                             | I2CCommandsMessage：I2CCommandsMessage インスタンス | JSON バイト配列から I2CCommandsMessage へデシリアライズする。<br>デシリアライズに失敗した場合、null を返す。 |
| ○    | SerializeJson      | I2CCommandsMessage：I2CCommandsMessage インスタンス | string：JSON 文字列                                 | I2CCommandsMessage を JSON 文字列へシリアライズする。<br>シリアライズに失敗した場合、null を返す。           |
| ○    | SerializeJsonBytes | I2CCommandsMessage：I2CCommandsMessage インスタンス | byte[]：JSON バイト配列                             | I2CCommandsMessage を JSON バイト配列へシリアライズする。<br>シリアライズに失敗した場合、null を返す。       |

### Utilities-I2CCommand-クラス

I2C 通信で複数のコマンドをまとめて管理・シリアライズ／デシリアライズするためのデータコンテナ。  

#### Utilities-I2CCommand-プロパティ

| 名前     | 型     | 説明         |
| -------- | ------ | ------------ |
| Action   | string | アクション   |
| Address  | string | アドレス     |
| Command  | string | コマンド     |
| Data     | string | データ       |
| Filter   | string | フィルタ     |
| Length   | int    | データ長     |
| Interval | int    | インターバル |

### Utilities-Util-クラス

汎用的に利用されるユーティリティ機能を提供するクラス。  

#### Utilities-Util-デリゲート

| 名前          | 引数（型：説明） | 戻り値（型：説明）                                | 説明                                                       |
| ------------- | ---------------- | ------------------------------------------------- | ---------------------------------------------------------- |
| Func<T, bool> | T：対象の値の型  | bool：条件を満たす(true)／条件を満たさない(false) | 制約条件を満たしているかどうか検証するためのデリゲート関数 |

#### Utilities-Util-メソッド

| 静的 | 名前                   | 引数（型：説明）                                                                                                                                                         | 戻り値（型：説明）                           | 説明                                                                                                                                                                                                                                                    |
| ---- | ---------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | -------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ○    | GetMessageId           | なし                                                                                                                                                                     | string：生成したメッセージ ID                | 現在日時("yyyyMMddHHmmssfff")とGUIDを組み合わせて、ユニークなメッセージ ID を生成する。                                                                                                                                                                 |
| ○    | GetTransportSettings   | this TransportProtocol：トランスポートプロトコル種別                                                                                                                     | ITransportSettings[]：トランスポート設定配列 | 指定したトランスポートプロトコル種別で生成したトランスポート設定を取得する。<br>想定外のトランスポートプロトコル種別が指定された場合、null を返す。                                                                                                     |
| ○    | GetRequiredValue       | T：戻り値の型<br>JObject：JObject インスタンス<br>string：キー名                                                                                                         | T：取得した値                                | JObject より必須値を取得し、指定した型で取得する。<br>指定したキー名が存在しない場合、Exception をスローする。<br>【v6.1.0～非推奨】GetDesiredProperty、または、GetEnvironmentVariable を使用してください。<!--（使用している場合、ビルド時に警告が出ます）--> |
| ○    | HexStringToByte        | string：16進数文字列（2文字）                                                                                                                                            | byte：バイト値                               | 16進数文字列からバイト値へ変換する。<br>指定した16進数文字列が2文字でない場合、Exception をスローする。                                                                                                                                                 |
| ○    | HexStringToBytes       | string：16進数文字列（"-"区切り）                                                                                                                                        | byte[]：バイト配列                           | 16進数文字列からバイト配列へ変換する。<br>エラー処理は特にしていない。                                                                                                                                                                                  |
| ○    | GetDesiredProperty     | T：戻り値の型<br>JObject：DesiredProperty<br>string：キー名<br>bool：Required<br>T：デフォルト値（デフォルト：Default）<br>Func<T, bool>：制約条件（デフォルト：null）※1 | T：取得した値                                | DesiredProperty より、指定した型でキーの値を取得する。<br>必須項目でキーが存在しない場合、Exception をスローする。<br>必須項目で値が null または空白の場合、Exception をスローする。<br>必須でない項目でキーが存在しない場合、デフォルト値を返す。<br>型・制約条件違反の場合、Exception をスローする。                    |
| ○    | GetEnvironmentVariable | T：戻り値の型<br>string：環境変数名<br>bool：Required<br>T：デフォルト値（デフォルト：Default）<br>Func<T, bool>：制約条件（デフォルト：null）※1                         | T：取得した値                                | 環境変数より、指定した型で値を取得する。<br>必須項目でキーが存在しない場合、Exception をスローする。<br>必須項目で値が null または空白の場合、Exception をスローする。<br>必須でない項目でキーが存在しない場合、デフォルト値を返す。<br>型・制約条件違反の場合、Exception をスローする。                                  |

※1：戻り値の型が int で、1以上100以下のみ指定可の場合、以下のように設定する。  
     value => value >= 1 && value <= 100

---
