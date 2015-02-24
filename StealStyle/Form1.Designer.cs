namespace StealStyle
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox_bbsmenu = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_ref = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_start = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.label_state = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // textBox_bbsmenu
            // 
            this.textBox_bbsmenu.Location = new System.Drawing.Point(12, 28);
            this.textBox_bbsmenu.Name = "textBox_bbsmenu";
            this.textBox_bbsmenu.Size = new System.Drawing.Size(328, 19);
            this.textBox_bbsmenu.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "bbsmenu.datファイルのパス";
            // 
            // button_ref
            // 
            this.button_ref.Location = new System.Drawing.Point(347, 26);
            this.button_ref.Name = "button_ref";
            this.button_ref.Size = new System.Drawing.Size(54, 23);
            this.button_ref.TabIndex = 2;
            this.button_ref.Text = "参照";
            this.button_ref.UseVisualStyleBackColor = true;
            this.button_ref.Click += new System.EventHandler(this.button_ref_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(210, 63);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(173, 16);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "最小化時にタスクトレイに入れる";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(67, 63);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(100, 19);
            this.textBox_port.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "ポート番号";
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(12, 100);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(192, 23);
            this.button_start.TabIndex = 6;
            this.button_start.Text = "開始";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_stop
            // 
            this.button_stop.Enabled = false;
            this.button_stop.Location = new System.Drawing.Point(210, 100);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(180, 23);
            this.button_stop.TabIndex = 7;
            this.button_stop.Text = "停止";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // label_state
            // 
            this.label_state.AutoSize = true;
            this.label_state.Location = new System.Drawing.Point(12, 130);
            this.label_state.Name = "label_state";
            this.label_state.Size = new System.Drawing.Size(31, 12);
            this.label_state.TabIndex = 9;
            this.label_state.Text = "state";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "StealStyle";
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 150);
            this.Controls.Add(this.label_state);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_port);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button_ref);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_bbsmenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Steal Style";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ClientSizeChanged += new System.EventHandler(this.Form1_ClientSizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ref;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Label label_state;
        public System.Windows.Forms.TextBox textBox_bbsmenu;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}

