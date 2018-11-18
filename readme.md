## Unityアセットバンドルの基礎
- https://unity3d.com/jp/learn/tutorials/topics/best-practices/assetbundle-fundamentals#AssetBundle.LoadFromFile

## アセットバンドルのビルド
- https://docs.unity3d.com/jp/540/ScriptReference/BuildPipeline.BuildAssetBundles.html

## Json形式のアセットバンドルテスト手順
1. 「Assets」→「Build Text Asset」
1. Assets/AssetBundles に info が出力されるので、webサーバーにコピー
1. Scenes/Text/TextAsset をUnityエディター上で選択
1. 実行
1. ログを確認
  1. バージョンはサーバーにアップしたものか？
  1. 項目の名前が正しいか？
1. PrepareAssets.cs に定義されている loadFromWeb() の version を変更することでwebサーバーからダウンロードされることを確認

