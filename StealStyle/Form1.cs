using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace StealStyle
{
    public partial class Form1 : Form
    {
        //public String bbsmenu = null;
        public Thread server = null;
        public Socket socket = null;
        public Boolean serverstatus = true;
        public Dictionary<string, string> dict = new Dictionary<string, string>();
        public Dictionary<string, string> dict2 = new Dictionary<string, string>(); //サーバ名保持

        public Form1()
        {
            InitializeComponent();
        }

        private void button_ref_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.FileName = "";
            ofd.InitialDirectory = @"C:\";
            ofd.Filter =
                "datファイル(*.dat)|*dat|すべてのファイル(*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Title = "ファイルを選択";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき
                textBox_bbsmenu.Text = ofd.FileName;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label_state.Text = "Apprication has started";
            this.textBox_bbsmenu.Text = Properties.Settings.Default.bbsmenu;
            this.textBox_port.Text = Properties.Settings.Default.port;
            this.checkBox1.Checked = Properties.Settings.Default.tasktray;
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            // check
            if (string.IsNullOrEmpty(textBox_port.Text)) {
                MessageBox.Show("port number is null.",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            int portnum=0;
            try
            {
                portnum = int.Parse(textBox_port.Text);
            }
            catch (Exception)
            {

                MessageBox.Show("port number isn't correct.",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (portnum <= 0 || portnum > 65535) {
                MessageBox.Show("port number isn't correct.",
    "エラー",
    MessageBoxButtons.OK,
    MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(textBox_bbsmenu.Text))
            {
                MessageBox.Show("bbsmenu path is null.",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            this.button_start.Enabled = false;
            // mapping

            StreamReader sr = new StreamReader(
                textBox_bbsmenu.Text, Encoding.GetEncoding("Shift_JIS"));

            // 読み込みできる文字がなくなるまで繰り返す
            string category = string.Empty;
            var r = new System.Text.RegularExpressions.Regex(@"<B>(.+)</B>");
            var r2 = new System.Text.RegularExpressions.Regex(@"<A HREF=s?https?://([-_a-zA-Z0-9]+\.2ch\.net|\.bbspink\.com)/([^/]+)/>([^<]+)</A>");
            System.Text.RegularExpressions.Match m;
            while (sr.Peek() >= 0)
            {
                // ファイルを 1 行ずつ読み込む
                string stBuffer = sr.ReadLine();
                m = r.Match(stBuffer);
                if (m.Success)
                {
                    category = m.Groups[1].Value;
                    continue;
                }
                m = r2.Match(stBuffer);
                if (m.Success && !string.IsNullOrEmpty(category) &&
                    !category.Equals("おすすめ") && !category.Equals("特別企画") && !category.Equals("運営案内"))
                {
                    if (!dict.ContainsKey(m.Groups[2].Value))
                    {
                        dict.Add(m.Groups[2].Value, category + "\\" + m.Groups[3].Value);
                    }
                    if (!dict2.ContainsKey(m.Groups[2].Value))
                    {
                        dict2.Add(m.Groups[2].Value,m.Groups[1].Value);
                    }
                }

            }

            sr.Close();

            // サーバを立てる
            // httpserver listening
            this.server = new Thread(
                new ThreadStart(HttpThread)
            );
            this.server.Start();

            this.button_stop.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.bbsmenu = this.textBox_bbsmenu.Text;
            Properties.Settings.Default.port = this.textBox_port.Text;
            Properties.Settings.Default.tasktray = this.checkBox1.Checked;
            
            Properties.Settings.Default.Save();

            notifyIcon1.Visible = false; 

            base.OnClosing(e);
            Environment.Exit(0);
        }
        

        private void HttpThread()
        {
            // サーバーソケット初期化
            this.BeginInvoke(new Action<String>(delegate(String str) { this.logoutput("サーバを初期化します。"); }), new object[] { "" });
            //logoutputDelegate("サーバを初期化します。"); 
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ip, int.Parse(this.textBox_port.Text));

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            socket.Bind(ipEndPoint);
            socket.Listen(5);
            // 要求待ち（無限ループ）
            while (serverstatus)
            {
                Socket sock = null;
                this.BeginInvoke(new Action<String>(delegate(String str) { this.logoutput("サーバを開始しました。"); }), new object[] { "" });
                try
                {
                    sock = socket.Accept();
                }
                catch
                {
                    //this.BeginInvoke(new Action<String>(delegate(String str) { this.textBox_log.AppendText("ソケット接続エラー。\r\n"); }), new object[] { "" });
                    //throw ex;
                    break;
                }
                HttpServer response = new HttpServer(sock, this);
                response.Start();
            }

            // socket dispose
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
            this.BeginInvoke(new Action<String>(delegate(String str) { this.logoutput("サーバを終了します。"); }), new object[] { "" });

        }

        public delegate void LogoutputDelegate(String str);
        public void logoutput(String str)
        {
            this.label_state.Text = str;
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            if (this.server != null) // 初期化
            {
                this.button_stop.Enabled = false;
                this.socket.Close();
                this.socket = null;
                if (this.server.IsAlive)
                    this.server.Abort();
                this.server = null;
                logoutput("サーバが停止しました。");

                this.button_start.Enabled = true;
            }
        }

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked && this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
            {
                // フォームが最小化の状態であればフォームを非表示にする
                this.Hide();
                // トレイリストのアイコンを表示する
                notifyIcon1.Visible = true;
            } 
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            // フォームを表示する
            this.Visible = true;
            // 現在の状態が最小化の状態であれば通常の状態に戻す
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            // フォームをアクティブにする
            notifyIcon1.Visible = false;
            this.Activate(); 
        }

    }
}

// API対策についてメモ
/*
 * AppkeyとHBを入力して使うイメージ
 * コードはそのへんに落ちてるのを流用。外部化すればそこだけ入れ替え可能なのでdllにする
 * bbsmenuを取得しないと動かないので、サーバ起動時にinfo.2ch.netからとって来る
 * その後は同じ処理に乗せるだけ。
 * dat取得以外の処理はホストだけ書き換えて全部丸投げする→書き込みにも自動対応するはず
 * 
 * クライアント設定は専ブラ毎に違うので、都度書いていく
 * プロキシとしては動かない。擬似プロキシなので
 * 
*/