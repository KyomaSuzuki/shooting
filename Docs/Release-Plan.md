# 無料Windows版の公開計画

2026-11-05 時点。最初は「0.1.0・開発中の無料版」として公開し、短いプレイ感想を集めることを推奨します。

## 推奨する公開先

| 公開先 | 使い方 | 判断 |
|---|---|---|
| itch.io | ゲーム紹介、スクリーンショット、Windows用ZIP、コメント窓口をまとめる | 最初の公開先に推奨 |
| GitHub Releases | 同じZIPと更新履歴をバージョンごとに保存する | 開発リポジトリの補助配布先 |
| フリーゲーム夢現 | 日本語紹介を追加して国内の無料ゲーム利用者へ届ける | itch.io公開後の追加候補 |
| Steam | 有料作品や内容拡充後の展開 | 今回の無料の初公開では優先しない |

itch.ioは無料アカウントで作品ページを作れ、価格設定を選べます。無料Windows版を希望されているため、最初は支払いを受け付けない設定を選びます。GitHub ReleasesではビルドZIPを明示的に添付してください。自動生成される「Source code.zip」はゲーム実行用の配布物ではありません。

## 公開手順

1. 下記のZIPを別フォルダーに展開して起動し、READMEとクレジットが入っていることを確認する。
2. 可能なら別のWindows PCで1回、タイトル→Manual→ゲーム→ゲームオーバー→Restartを確認する。
3. itch.ioで新規作品を作り、ダウンロード作品・Windows・無料・開発中として設定する。
4. ZIP全体をアップロードし、Windows向けのファイルに設定する。exe単体をアップロードしない。
5. タイトル・ゲーム中・Manualのスクリーンショット、操作、1,000点クリアの条件、バージョン、AI画像の使用、OtoLogicクレジットを掲載する。
6. まずDraft/限定共有でダウンロードを確認し、問題がなければ公開へ切り替える。
7. GitHubでは修正コードをコミットした後、対応するタグとReleaseを作成し、同じZIP・更新履歴・SHA-256を添付する。
8. 更新時は0.1.1、0.1.2と上げ、何を直したか短く記録する。

公開アカウントの作成、アップロード、タグ、GitHub Release作成、公開への切り替えは今回実施していません。

## 作品紹介文案

『Space Shooter』は、左右移動と射撃で敵を倒すシンプルな2Dシューティングです。支援機や3方向ショットを使い、1,000点のクリアを目指してください。
Windows 64ビット向け無料・開発中バージョンです。操作は左右矢印キーとSpace。ゲーム内Manualにアイテムの説明があります。

## 今回確認したことと残る範囲

自動検証では実弾の敵衝突・得点、緑・赤・黄アイテム、支援機上限と補充、黄色の再取得・10秒終了、停止中射撃防止、クリア、クリア後の得点停止、クリアとゲームオーバーの競合防止、両結果画面からのRestart、タイトル復帰、Manual往復を確認しました。
アイテム取得は実際のPrefabを生成して取得コールバックを実行し、衝突による弾撃破とゲームオーバーは物理更新で検証しました。自然プレイだけで1,000点到達する難易度、別PC、全解像度、全GPUの検証を代替するものではありません。
画像制作元はユーザー申告に基づきOpenAI/Gemini生成と記録。元の生成履歴までは照合していません。OtoLogic音源のクレジットとライセンスURLは配布物へ同梱しました。

## 参照した公式ページ

- itch.ioの作品ページ作成: https://itch.io/docs/creators/getting-started
- itch.ioの価格設定: https://itch.io/docs/creators/pricing
- GitHub Releases: https://docs.github.com/en/repositories/releasing-projects-on-github/about-releases
- フリーゲーム夢現の投稿条件: https://freegame-mugen.jp/publish/index.html
- Steam Directの費用: https://partner.steamgames.com/doc/gettingstarted/appfee
- OtoLogic利用規約: https://otologic.jp/free/license.html
