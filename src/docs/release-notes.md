---
slug: "/iotedge-v2/iotedge-v2-Commons/6.1.1/spec"
---

# Commons Release Notes

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
