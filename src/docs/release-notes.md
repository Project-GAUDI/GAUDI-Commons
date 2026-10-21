---
slug: "/iotedge-v2/Commons/7.0.0/releases"
---

# Commons Release Notes

## 7.0.0

2026-05-19

* GAUDI標準メッセージ形式の定義明確化
  * RecordInfo.RecordData の型を `List<string>` から `List<object>` に変更
    * string, 数値(long/double), bool, null を格納可能
    * デシリアライズ時、JSON の整数は long、小数は double として復元される
  * RecordInfo.RecordHeader を任意フィールドに変更（将来廃止予定）
    * null の場合、シリアライズ時に JSON から省略される
    * RecordHeader なしの JSON をデシリアライズした場合、null として復元される
  * JsonMessage.Validate メソッドを追加
    * RecordList および各 RecordInfo.RecordData が null でないことを検証する
    * allowEmptyBody=true の場合、message が null でも有効とする（EOF メッセージ用）
  * GAUDI 標準メッセージプロパティ仕様を追加
    * Properties.type: メッセージ種別（"eof" の場合 Body 空/省略を許容）
    * Properties.columns: カラム定義（JSON 文字列配列）

2026-02-03

* 機能改善
  * .NET8が2026-11-10にサポート終了するため、.NET10に更新
  * ライブラリを最新化
    * M2MqttDotnetCore(1.1.0)
  * テスト用ライブラリを最新化
    * Lib.Harmony(2.4.2)
    * Microsoft.NET.Test.Sdk(18.0.1)
    * xunit.v3(3.2.2)
    * xunit.runner.visualstudio(3.1.5)
    * coverlet.collector(6.0.4)
  * テスト用ライブラリを追加
    * Microsoft.Testing.Extensions.CodeCoverage(18.0.6)
  * xunit.v3更新によるテストコード修正
  * 大容量メッセージ対応の破棄のためにDeviceClientSDKをAzure公式のものに変更
* 仕様書改善
  * 仕様書から大容量メッセージ対応関連の記述を削除
    * MessageSizeLimitExpansion…モジュール間のメッセージサイズの上限拡張指定(true：16MB、false：256KB)
    * メッセージサイズの上限拡張指定における注意
* 不具合修正
  * xunitの更新により全てのテストが実行しきらない場合がある問題に対応

## 6.1.1

2025-09-12

* 必須のDesiredProperty・環境変数がnullまたは空白の場合、エラーとなるように修正

## 6.1.0

2025-08-21

* 同期メソッドから非同期メソッドを呼び出すことによるデッドロックリスク解消のための修正
  * Dispose()メソッドをDisposeAsync()メソッドに置き換え

2025-08-19

* spec.md 修正
* internal_spec.md 追加
* azure-pipelines.yml 修正（カバレッジ対応）

* 省略可能なDesiredProperty・環境変数で、Type・Conditions 違反の値が設定されていた場合、デフォルト値を採用せずエラーとなるように変更
  * DesiredProperty・環境変数を取得する新規メソッドを追加（Commons 側でログ出力を行う）
* InitializeAsync で、Info 以下のログが出力できるように修正
* DesiredProperty 取得エラー時、アプリが異常終了する問題を修正
* ModuleTwin 変更時、RunAsync にて NullException が発生する問題を修正
* アプリケーション終了時、ModuleClient 解放後にログをアップロードしようとしてエラーになる問題を修正
* StateController のメソッド SetCurrentState の戻り値が使用されていないため廃止

## 6.0.5

2025-05-09

* モジュール起動時接続不良問題修正
  * RunAsync メソッド内で await しているすべての非同期メソッド呼び出しに対して ConfigureAwait(false) を適用するように修正
* エラーが発生する事があるテストコードを修正

## 6.0.4

2024-08-30

* モジュール基本実装共通化
* 新大容量メッセージ対応
* ダイレクトメソッド追加
* .NET8.0対応
* ubuntu-22.04対応
* AzureSDKの最新化
  * Microsoft.Azure.Devices.Clientのバージョン1.42.0→1.42.3

## 5.0.0

2023-10-31

* AzureSDKの最新化
  * Microsoft.Azure.Devices.Clientのバージョン1.39.0→1.42.0

## 1.0.0

2022-mm-dd

* Artifact対応
* .NET6対応
* TDD対応
* ModuleClientのDEVICEID, MODULEID指定対応
