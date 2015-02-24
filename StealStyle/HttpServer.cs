using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace StealStyle
{
    class HttpServer
    {
        private Socket mClient;
        private Form1 mForm;

        // コンストラクタ
        public HttpServer(Socket client, Form1 form)
        {
            mClient = client;
            mForm = form;
        }

        // 応答開始
        public void Start()
        {
            Thread thread = new Thread(Run);
            thread.Start();
        }

        // 応答実行
        public void Run()
        {
            try
            {
                // 要求受信
                byte[] buffer = new byte[4096];
                int recvLen = 0;
                System.Threading.Thread.Sleep(10); //タイミング対策
                try
                {
                    while (mClient.Available > 0)
                    {
                        recvLen += mClient.Receive(buffer);
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
                    throw e;
                }

                String message = Encoding.ASCII.GetString(buffer, 0, recvLen);
                Console.Write("httprequest:" + message);

                // httpmessageを展開する。
                String req_method;
                String req_path;
                String req_httpversion;
                var req_dict = new Dictionary<string, string>();

                // 改行で分解
                String[] ms = message.Split(new String[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var rm1 = new System.Text.RegularExpressions.Regex(@"^([A-Z]{3,4}) +(/.*) +HTTP/([0-9]+.[0-9]+)"); //リクエスト行
                var mm = rm1.Match(ms[0]);
                if (!mm.Success) { throw new Exception(); } //リクエスト不正

                // メソッド、ターゲット、HTTPバージョンなど取得
                req_method = mm.Groups[1].Value;
                req_path = mm.Groups[2].Value;
                req_httpversion = mm.Groups[3].Value;
                
                // 他の要素取得 - ハッシュでいいかー
                var rm2 = new System.Text.RegularExpressions.Regex(@"^([^:]+) *: +(.+)");
                for (int i = 1; i < ms.Length; i++)
                {
                    var mmm = rm2.Match(ms[i]);
                    if (mmm.Success)
                    {
                        req_dict.Add(mmm.Groups[1].Value, mmm.Groups[2].Value);
                    }
                }
                
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                String httpHeader = null;
                byte[] httpHeaderBuffer = new byte[4096];
                String body = null;
                byte[] bodyBuffer = new byte[4096];
                string basedir = System.IO.Path.GetDirectoryName(mForm.textBox_bbsmenu.Text);
                // reg
                var r = new System.Text.RegularExpressions.Regex(@"([^/]+)/subject\.txt");
                var r1 = new System.Text.RegularExpressions.Regex(@"([^/]+)/setting\.txt");
                var r2 = new System.Text.RegularExpressions.Regex(@"([^/]+)/dat/([0-9]+)\.dat");
                var r4 = new System.Text.RegularExpressions.Regex(@"([0-9]+\.dat)(<>.+\t )\([0-9]+\)");
                var r5 = new System.Text.RegularExpressions.Regex(@"^(/test/read.cgi)?/([^/]+)/");
                var md = r5.Match(req_path);

                try
                {
                    

                    // 要求されてから作り込む
                    if (req_path.Contains("bbsmenu.dat")) // menuは全部返す
                    {
                        StreamReader sr = new StreamReader(
                            mForm.textBox_bbsmenu.Text, Encoding.GetEncoding("Shift_JIS"));
                        body = convertLocalURI(sr.ReadToEnd(), req_dict["Host"]);
                        sr.Close();
                        bodyBuffer = sjisEnc.GetBytes(body);

                    }
                    //subjectは要求された板のフォルダ下存在datをまとめて返す（subjectに存在するもののみ）
                    //→subject.txtを上からさらって件数修正する。datの件数だけ読めればベター
                    //書き込み中の場合、待つ
                    else if (req_path.Contains("subject.txt"))
                    {
                        var m = r.Match(message);
                        if (m.Success)
                        {
                            var t = basedir + "\\" + mForm.dict[m.Groups[1].Value] + "\\subject.txt";
                            // ファイル一覧を取得
                            string[] files = System.IO.Directory.GetFiles(
                                basedir + "\\" + mForm.dict[m.Groups[1].Value], "*", SearchOption.TopDirectoryOnly);

                            StreamReader sr = new StreamReader(
                                basedir + "\\" + mForm.dict[m.Groups[1].Value] + "\\subject.txt", Encoding.GetEncoding("Shift_JIS"));
                            var filesArr = new List<string>(files);
                            var sb = new StringBuilder();
                            while (sr.Peek() >= 0)
                            {
                                // ファイルを 1 行ずつ読み込む
                                var stBuffer = sr.ReadLine();
                                var m2 = r4.Match(stBuffer);
                                if (m2.Success) //宣伝は対象外
                                {
                                    if (filesArr.Contains(basedir + "\\" + mForm.dict[m.Groups[1].Value] + "\\" + m2.Groups[1].Value))
                                    {
                                        // ファイルを読む
                                        var fsr = new StreamReader(
                                            basedir + "\\" + mForm.dict[m.Groups[1].Value] + "\\" + m2.Groups[1].Value, Encoding.GetEncoding("Shift_JIS"));
                                        // 件数取得 書き込み中かもしれないが、どうせ追加していくだけなので行数を見ればだいたいあってる
                                        int c = 0;
                                        while (fsr.Peek() >= 0)
                                        {
                                            fsr.ReadLine();
                                            c++;
                                        }
                                        fsr.Close();
                                        sb.Append(m2.Groups[1].Value + m2.Groups[2].Value + "(" + c + ")\n"); // 現時点での読み込んだdatを反映
                                    }
                                }

                            }
                            sr.Close();
                            bodyBuffer = sjisEnc.GetBytes(sb.ToString());
                        }
                    }
                    // settingは特に変わらないのでそのまま送る
                    else if (req_path.Contains("setting.txt"))
                    {
                        var m = r1.Match(message);
                        if (m.Success)
                        {
                            StreamReader sr = new StreamReader(
                                basedir + "\\" + mForm.dict[m.Groups[1].Value] + "\\setting.txt", Encoding.GetEncoding("Shift_JIS"));
                            body = sr.ReadToEnd();
                            sr.Close();
                            bodyBuffer = sjisEnc.GetBytes(body);
                        }
                    }
                    // datは直返しで問題ない。
                    else if (req_path.Contains("/dat/"))
                    {
                        var m = r2.Match(message);
                        if (m.Success)
                        {
                            StreamReader sr = new StreamReader(
                                basedir + "\\" + mForm.dict[m.Groups[1].Value] + "\\" + m.Groups[2].Value + ".dat", Encoding.GetEncoding("Shift_JIS"));
                            body = convertLocalURI(sr.ReadToEnd(), req_dict["Host"]);
                            sr.Close();
                            bodyBuffer = sjisEnc.GetBytes(body);
                        }
                    }
                        //* 作ったけど使われなさそうな部分 ここから *//
                    else if (req_path.Contains("/test/bbs.cgi"))
                    {
                        var sb = new StringBuilder();
                        sb.Append(@"<html> <head><title>■ 書き込み確認 ■</title><meta http-equiv=""Content-Type"" content=""text/html; charset=Shift_JIS"">");
                        sb.Append(@"<meta name=""viewport"" content=""width=device-width,initial-scale=1.0,minimum-scale=1.0,maximum-scale=1.6,user-scalable=yes""/>");
                        sb.Append(@"</head><body bgcolor=#EEEEEE><font size=+1 color=#FF0000><b>書きこみ＆クッキー確認</b></font><ul><br><br><b> </b><br>名前： ");
                        sb.Append(@"<br>E-mail： <br>内容：<br>a<br><br></ul><form method=POST action=""../test/bbs.cgi?guid=ON""><input type=hidden name=subject value="""">");
                        sb.Append(@"<input TYPE=hidden NAME=FROM value=""""><input TYPE=hidden NAME=mail value=""""><input type=hidden name=MESSAGE value="""">");
                        sb.Append(@"<input type=hidden name=bbs value=namazuplus><input type=hidden name=sid value=><input type=hidden name=time value=1424749403>");
                        sb.Append(@"<input type=hidden name=key value=1422604082><br><input type=submit value=""確認して書き込む"" name=""submit""><br></form>");
                        sb.Append(@"変更する場合は戻るボタンで戻って書き直して下さい。<br><br>現在、荒らし対策でクッキーを設定していないと書きこみできないようにしています。<br>");
                        sb.Append(@"<font size=-1>(cookieを設定するとこの画面はでなくなります。)</font><br></body></html>");
                        bodyBuffer = sjisEnc.GetBytes(sb.ToString());
                    }
                    // /livevenus/* 型 または /test/read.cgi/livevenus/*
                    else if (md.Success && !string.IsNullOrEmpty(md.Groups[2].Value) && mForm.dict2.ContainsKey(md.Groups[2].Value))
                    {
                        // 元データを取得して返す（リダイレクトしたいけどたぶんサポートしてないやろ）
                        string server = mForm.dict2[md.Groups[2].Value];
                        
                        //リクエストメッセージを作成する
                        StringBuilder sb = new StringBuilder();
                        sb.Append(req_method + " " + req_path + " HTTP/" + req_httpversion + "\r\n"); // リクエスト行
                        sb.Append("Host: " + server + "\r\n");
                        foreach (string s in req_dict.Keys)
                        {
                            if (!s.Equals("Host"))
                            {
                                sb.Append(s + ": " + req_dict[s] + "\r\n");
                            }
                        }
                        sb.Append("\r\n\r\n");
                        var reqMsg = sb.ToString();

                        //文字列をbyte配列に変換
                        byte[] reqBytes = Encoding.UTF8.GetBytes(reqMsg);

                        //ホスト名からIPアドレスを取得
                        System.Net.IPAddress hostadd =
                            System.Net.Dns.GetHostEntry(server).AddressList[0];
                        //IPEndPointを取得
                        System.Net.IPEndPoint ephost =
                            new System.Net.IPEndPoint(hostadd, 80);

                        //Socketの作成
                        System.Net.Sockets.Socket sock =
                            new System.Net.Sockets.Socket(
                            System.Net.Sockets.AddressFamily.InterNetwork,
                            System.Net.Sockets.SocketType.Stream,
                            System.Net.Sockets.ProtocolType.Tcp);

                        //接続
                        sock.Connect(ephost);

                        //リクエストメッセージを送信
                        sock.Send(reqBytes, reqBytes.Length,
                            System.Net.Sockets.SocketFlags.None);

                        //受信する
                        byte[] resBytes = new byte[4096];
                        System.IO.MemoryStream mem = new System.IO.MemoryStream();
                        while (true)
                        {
                            int resSize =
                                sock.Receive(resBytes, resBytes.Length,
                                System.Net.Sockets.SocketFlags.None);
                            if (resSize == 0)
                                break;
                            mem.Write(resBytes, 0, resSize);
                        }
                        //string resMsg = sjisEnc.GetString(mem.GetBuffer(), 0, (int)mem.Length);
                        mClient.Send(mem.GetBuffer(), (int)mem.Length, SocketFlags.None); //クライアントに返す
                        mem.Close();

                        //閉じる
                        sock.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                        sock.Close();

                        return;

                    }
                    //* ここまで *//
                    else
                    {
                        httpHeader = String.Format(
                                "HTTP/" + req_httpversion + " 404 Not Found\r\n" +
                                "Cache-Control: no-cache\r\n" +
                                "\r\n"
                            );
                        httpHeaderBuffer = Encoding.UTF8.GetBytes(httpHeader);

                        mForm.BeginInvoke(new Action<String>(delegate(String str) { mForm.logoutput("リクエストが不正です。"); }), new object[] { "" });
                        //mClient.Close();
                        return;
                    }

                    // HTTPヘッダー生成 gzipの対応はしない。
                    httpHeader = String.Format(
                                "HTTP/" + req_httpversion + " 200 OK\r\n" +
                                "Cache-Control: no-cache\r\n" +
                                "Content-Type: text/html\r\n" +
                                "Content-Length: " + bodyBuffer.Length + "\r\n" +
                                "\r\n"
                            );
                    httpHeaderBuffer = Encoding.UTF8.GetBytes(httpHeader);
                    mClient.Send(httpHeaderBuffer);
                    mClient.Send(bodyBuffer, bodyBuffer.Length, SocketFlags.None);
                }
                catch (System.IO.IOException) // ファイルかフォルダが見つからない場合⇒まだ作られていない
                {
                    // HTTPヘッダー生成 0byteを返す
                    httpHeader = String.Format(
                                "HTTP/" + req_httpversion + " 200 OK\r\n" +
                                "Cache-Control: no-cache\r\n" +
                                "Content-Type: text/plain\r\n" +
                                "Content-Length: 0\r\n" +
                                "\r\n"
                            );
                    httpHeaderBuffer = Encoding.UTF8.GetBytes(httpHeader);
                    mClient.Send(httpHeaderBuffer);
                }
                catch (Exception)
                {
                    httpHeader = String.Format(
                           "HTTP/" + req_httpversion + " 404 Not Found\r\n" +
                           "Cache-Control: no-cache\r\n" +
                           "\r\n"
                       );
                    httpHeaderBuffer = Encoding.UTF8.GetBytes(httpHeader);

                    mForm.BeginInvoke(new Action<String>(delegate(String str) { mForm.logoutput("例外が発生しました。"); }), new object[] { "" });
                }

            }
            catch (System.Net.Sockets.SocketException e)
            {
                Console.Write(e.Message);
            }
            catch (System.ObjectDisposedException e)
            {
                Console.Write(e.Message);
            }
            catch (Exception e) {
                Console.Write(e.Message);
            }
            finally
            {
                mClient.Close();
            }
        }

        public string convertLocalURI(string str, string host)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                str,
                @"s?https?://[-_a-zA-Z0-9]+(\.2ch\.net|\.bbspink\.com)/",
                "http://" + host + "/");
        }
    }
}
