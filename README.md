# StealStyle
２ちゃん専ブラのdatから仮想掲示板を作成するサーバツール  
どうしても2ch読み上げがしたいっ・・・そのためならどんな労苦も厭わぬっ！！  
という人向けツールです。  

## 使い方
1. Jane Style3.80をダウンロードして適当に巡回する  
2. StealStyleを起動して、Janeフォルダにあるbbsmenu.datのファイルパスを設定する  
   (Jane Style\Logs\2ch\bbsmenu.dat)  
3. サーバを起動する。
4. メモ帳を右クリックして管理者権限で開く
5. "C:\Windows\System32\drivers\etc\hosts"を開く  
6. 以下の行を追加  
127.0.0.1(タブ)stealstyle.2ch.net  

### クライアント設定(softalkweb)
1. サイトの追加だと上手くいかないので、直接site.iniを修正する  
site.ini差分の内容を追加  
2. softlkwebを起動する。StealStyleは開始させておく。  
  
### クライアント設定(livemate)
1. 板URLの～～.2ch.netをstealstyle.2ch.netに変えて開く  
(http://stealstyle.2ch.net/livevenus/ のようにする)
