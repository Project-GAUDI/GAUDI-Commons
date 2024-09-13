---
slug: "/iotedge-v2/iotedge-v2-Commons/6.0.4/spec"
version: "6.0.4"
---

# Commons(6.0.4)

## 目次

* [Deployment 設定値](#deployment-設定値)
  * [環境変数](#環境変数)
* [利用可能クラス](#利用可能クラス)
  * [class1](#class1)


## Deployment 設定値

### 環境変数

#### 環境変数の値

| Key                                   | Required | Default        | Recommend | Description                                                     |
| ------------------------------------- | -------- | -------------- | --------- | ---------------------------------------------------------------- |
| TransportProtocol                     |          | Amqp           |           | ModuleClient の接続プロトコル。<br>["Amqp", "Mqtt"] |
| LogLevel                              |          | info           |           | 出力ログレベル。<br>["trace", "debug", "info", "warn", "error"] |
| MessageSizeLimitExpansion             |          | false          |           | モジュール間のメッセージサイズの上限拡張指定。<br>["true", "false"]<br>true：メッセージサイズ上限が拡張され16MBになる(※1,2)。<br>false：メッセージサイズ上限はデフォルトの256KBになる。 |
| IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER |          | NEWTONSOFTJSON |           | Json化クラスの使用Jsonライブラリのデフォルトを切り替える。<br>["NEWTONSOFTJSON", "SYSRUNTIMESERIALIZATION"]<br>NEWTONSOFTJSON：Newtonsoft.Jsonを使用する。<br>SYSRUNTIMESERIALIZATION：System.Runtime.Serialization.Jsonを使用する。 |

※1：**IoTHubへ**のupstreamする際のメッセージサイズ上限は256KBのまま変わらない。<br>
送信してしまった場合、edgeHubからの送信がエラーとなり、その後のメッセージ送信も停滞してしまう。<br>
Edge→Fog間の拡張サイズメッセージの送信は可能。<br>
※2：edgeHubは、メッセージサイズ拡張対応が入っているGAUDIIotEdge-Hubを使用し、edgeHub・送信先モジュールにもMessageSizeLimitExpansion=trueが設定されている必要がある。<br>

## 利用可能クラス

順次追加予定

### class1

* 機能概要
  xxxx
