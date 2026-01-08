# mochifitter-link-manager

VRChatの非対応衣装を自動変換するツールである[もちふぃった～](https://booth.pm/ja/items/7657840)は通常、各アバターフォルダに`BlenderTools`をダウンロードして使用します。

この仕様のため各プロジェクトフォルダ内に同じファイルが作成され、ストレージ容量を圧迫します。

VRChat プロジェクト内の複数アバターディレクトリに散在する `BlenderTools` フォルダを「VRChat ルート（VRCRoot）」へ集約し、各アバターディレクトリにはディレクトリ・シンボリックリンクを作成して参照を統一する Windows 用ツールです。

このツールにより、`BlenderTools` の重複配置・バージョン不一致・更新漏れを防ぎ、管理をシンプルにします。

## シンボリックリンクとは

シンボリックリンクは、実体のフォルダを別の場所から「そこにあるように見せる」ための仕組みです。見た目は普通のフォルダですが、実際の中身は1箇所の本体（本ツールでは VRCRoot 直下の `BlenderTools`）を参照します。

## 効果

もちふぃった～がダウンロードする`BlenderTools`は1フォルダでおよそ1.4GBを占有します。

シンボリックリンクで参照を統一することで、複数のプロジェクトでもちふぃった～を導入しても合計で1.4GB分しかストレージ容量を占有しないようになります。

## 機能

- フォルダ選択ダイアログで `BlenderTools` フォルダを指定（厳密にフォルダ名が「BlenderTools」である必要があります）
- `BlenderTools` がアバターディレクトリ配下にある場合は VRCRoot 直下へ移動
- VRCRoot 配下の他アバターディレクトリに存在する重複 `BlenderTools` フォルダ（リンクでない通常フォルダ）を削除
- 各アバターディレクトリに VRCRoot 直下の `BlenderTools` へのディレクトリ・シンボリックリンクを作成
- 進捗ダイアログの表示（移動／削除／リンク作成の進捗と、完了時サマリの件数表示）

## 前提と判定ロジック

- 対象 OS: Windows 10/11
- .NET: .NET 8.0（Windows Forms）
- 依存パッケージ: `WindowsAPICodePack-Shell`（フォルダ選択ダイアログに使用）
- アバターディレクトリの判定は、ディレクトリ直下に `VRC.SDK3A.csproj` が存在するかどうかで行います
	- `BlenderTools` の親に `VRC.SDK3A.csproj` があれば、その親のさらに親を VRCRoot とみなします
	- そうでない場合は、`BlenderTools` の親ディレクトリを VRCRoot とみなします

## セットアップ / ビルド / 実行

1. .NET 8 SDK をインストール（Windows）
2. プロジェクトの復元とビルド

```powershell
dotnet restore
dotnet build "mochifitter-link-manager\mochifitter-link-manager.csproj"
```

3. 実行（開発時）

```powershell
dotnet run --project "mochifitter-link-manager\mochifitter-link-manager.csproj"
```

4. ビルド成果物の実行ファイル

- `mochifitter-link-manager\bin\Debug\net8.0-windows\mochifitter-link-manager.exe`

## 使い方

1. アプリを起動します
2. 「BlenderToolsフォルダパス」横の「参照」ボタンで、対象の `BlenderTools` フォルダを選択します
	 - フォルダ名が厳密に `BlenderTools` である必要があります
	 - シンボリックリンクの `BlenderTools` は選択不可（通常フォルダのみ許可）
3. 「BlenderToolsをおまとめ」ボタンをクリックします
4. 進捗ダイアログの表示後、完了サマリ（削除件数／失敗件数／作成リンク数）を確認します

### 典型的なフォルダ構成（例）

```
VRCRoot/
	BlenderTools/            ← 集約先（実体）
	AvatarA/
		VRC.SDK3A.csproj
		BlenderTools → (link to ../BlenderTools)
	AvatarB/
		VRC.SDK3A.csproj
		BlenderTools → (link to ../BlenderTools)
```

## 注意事項 / トラブルシューティング

- シンボリックリンクの作成には、環境によって管理者権限や Windows の「開発者モード」有効化が必要になる場合があります
	- 作成に失敗した場合は、管理者として実行する／開発者モードの有効化を検討してください
- VRCRoot 直下にすでに `BlenderTools` が存在する場合、移動処理はエラーになります（事前に整理してください）
- `BlenderTools` フォルダ名が異なる場合（例: `blender_tools` 等）は選択できません
- アバター判定は `VRC.SDK3A.csproj` の有無に依存します。異なるファイル名や構成の場合は認識されません
- 削除対象は「通常フォルダの `BlenderTools`」のみです。既存のシンボリックリンクは削除しません

## ライセンス

本プロジェクトは MIT License の下で提供されています。詳細はリポジトリ同梱のライセンス本文を参照してください。

- ライセンス本文: [LICENSE](LICENSE)

## 開発情報

- ターゲットフレームワーク: `net8.0-windows`
- UI フレームワーク: Windows Forms
- 依存パッケージ: `WindowsAPICodePack-Shell`（1.1.1）
