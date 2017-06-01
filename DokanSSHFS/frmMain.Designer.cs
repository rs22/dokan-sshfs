namespace SshFileSystem.WinForms
{
    partial class frmMain
    {
        /// <summary>
        /// 必要なデザイナ変数です。
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

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnMount = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSavePreset = new System.Windows.Forms.Button();
            this.btnRemovePreset = new System.Windows.Forms.Button();
            this.cmbSelectedPreset = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.txtPrivateKey = new System.Windows.Forms.TextBox();
            this.usePrivateKey = new System.Windows.Forms.RadioButton();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.rbtPassword = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.btnOpenPrivateKey = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.passphrase = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbDriveLetter = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnUnmount = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnMount
            // 
            this.btnMount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMount.Location = new System.Drawing.Point(120, 249);
            this.btnMount.Name = "btnMount";
            this.btnMount.Size = new System.Drawing.Size(75, 25);
            this.btnMount.TabIndex = 0;
            this.btnMount.Text = "Mount";
            this.btnMount.UseVisualStyleBackColor = true;
            this.btnMount.Click += new System.EventHandler(this.Connect_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(282, 249);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 25);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSavePreset
            // 
            this.btnSavePreset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSavePreset.Location = new System.Drawing.Point(218, 3);
            this.btnSavePreset.Name = "btnSavePreset";
            this.btnSavePreset.Size = new System.Drawing.Size(54, 19);
            this.btnSavePreset.TabIndex = 6;
            this.btnSavePreset.Text = "Save";
            this.btnSavePreset.UseVisualStyleBackColor = true;
            this.btnSavePreset.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRemovePreset
            // 
            this.btnRemovePreset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemovePreset.Location = new System.Drawing.Point(278, 3);
            this.btnRemovePreset.Name = "btnRemovePreset";
            this.btnRemovePreset.Size = new System.Drawing.Size(64, 19);
            this.btnRemovePreset.TabIndex = 7;
            this.btnRemovePreset.Text = "Remove";
            this.btnRemovePreset.UseVisualStyleBackColor = true;
            this.btnRemovePreset.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // cmbSelectedPreset
            // 
            this.cmbSelectedPreset.FormattingEnabled = true;
            this.cmbSelectedPreset.Items.AddRange(new object[] {
            "New Setting"});
            this.cmbSelectedPreset.Location = new System.Drawing.Point(63, 3);
            this.cmbSelectedPreset.Name = "cmbSelectedPreset";
            this.cmbSelectedPreset.Size = new System.Drawing.Size(149, 21);
            this.cmbSelectedPreset.TabIndex = 0;
            this.cmbSelectedPreset.SelectedIndexChanged += new System.EventHandler(this.cmbSelectedPreset_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 5);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 20);
            this.label7.TabIndex = 24;
            this.label7.Text = "Preset";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 186);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 20);
            this.label4.TabIndex = 11;
            this.label4.Text = "Path";
            // 
            // txtPath
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtPath, 3);
            this.txtPath.Location = new System.Drawing.Point(63, 184);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(279, 20);
            this.txtPath.TabIndex = 4;
            this.txtPath.Text = "/";
            // 
            // txtPrivateKey
            // 
            this.txtPrivateKey.Enabled = false;
            this.txtPrivateKey.Location = new System.Drawing.Point(91, 44);
            this.txtPrivateKey.Name = "txtPrivateKey";
            this.txtPrivateKey.Size = new System.Drawing.Size(190, 20);
            this.txtPrivateKey.TabIndex = 1;
            // 
            // usePrivateKey
            // 
            this.usePrivateKey.AutoSize = true;
            this.usePrivateKey.Location = new System.Drawing.Point(6, 45);
            this.usePrivateKey.Name = "usePrivateKey";
            this.usePrivateKey.Size = new System.Drawing.Size(79, 17);
            this.usePrivateKey.TabIndex = 9;
            this.usePrivateKey.Text = "Private Key";
            this.usePrivateKey.UseVisualStyleBackColor = true;
            this.usePrivateKey.CheckedChanged += new System.EventHandler(this.UsePrivateKey_CheckedChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(91, 19);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(242, 20);
            this.txtPassword.TabIndex = 0;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // rbtPassword
            // 
            this.rbtPassword.AutoSize = true;
            this.rbtPassword.Checked = true;
            this.rbtPassword.Location = new System.Drawing.Point(6, 19);
            this.rbtPassword.Name = "rbtPassword";
            this.rbtPassword.Size = new System.Drawing.Size(71, 17);
            this.rbtPassword.TabIndex = 7;
            this.rbtPassword.TabStop = true;
            this.rbtPassword.Text = "Password";
            this.rbtPassword.UseVisualStyleBackColor = true;
            this.rbtPassword.CheckedChanged += new System.EventHandler(this.UsePassword_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 211);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 20);
            this.label5.TabIndex = 16;
            this.label5.Text = "Drive";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(63, 53);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(149, 20);
            this.txtUser.TabIndex = 3;
            // 
            // btnOpenPrivateKey
            // 
            this.btnOpenPrivateKey.Enabled = false;
            this.btnOpenPrivateKey.Location = new System.Drawing.Point(287, 41);
            this.btnOpenPrivateKey.Name = "btnOpenPrivateKey";
            this.btnOpenPrivateKey.Size = new System.Drawing.Size(46, 25);
            this.btnOpenPrivateKey.TabIndex = 2;
            this.btnOpenPrivateKey.Text = "Load";
            this.btnOpenPrivateKey.UseVisualStyleBackColor = true;
            this.btnOpenPrivateKey.Click += new System.EventHandler(this.Open_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 55);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "User";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(222, 3);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(54, 20);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "22";
            // 
            // passphrase
            // 
            this.passphrase.Location = new System.Drawing.Point(91, 70);
            this.passphrase.Name = "passphrase";
            this.passphrase.Size = new System.Drawing.Size(242, 20);
            this.passphrase.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(190, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Passphrase";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(3, 3);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(181, 20);
            this.txtHost.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host";
            // 
            // cmbDriveLetter
            // 
            this.cmbDriveLetter.FormattingEnabled = true;
            this.cmbDriveLetter.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z"});
            this.cmbDriveLetter.Location = new System.Drawing.Point(63, 209);
            this.cmbDriveLetter.Name = "cmbDriveLetter";
            this.cmbDriveLetter.Size = new System.Drawing.Size(53, 21);
            this.cmbDriveLetter.TabIndex = 5;
            this.cmbDriveLetter.Text = "N";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.Controls.Add(this.cmbDriveLetter, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRemovePreset, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.cmbSelectedPreset, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSavePreset, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtPath, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtUser, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(345, 231);
            this.tableLayoutPanel1.TabIndex = 25;
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 4);
            this.groupBox1.Controls.Add(this.rbtPassword);
            this.groupBox1.Controls.Add(this.passphrase);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtPrivateKey);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.usePrivateKey);
            this.groupBox1.Controls.Add(this.btnOpenPrivateKey);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(339, 100);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Identity";
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 3);
            this.flowLayoutPanel1.Controls.Add(this.txtHost);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.txtPort);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(60, 25);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(285, 25);
            this.flowLayoutPanel1.TabIndex = 25;
            // 
            // btnUnmount
            // 
            this.btnUnmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnmount.Enabled = false;
            this.btnUnmount.Location = new System.Drawing.Point(201, 249);
            this.btnUnmount.Name = "btnUnmount";
            this.btnUnmount.Size = new System.Drawing.Size(75, 25);
            this.btnUnmount.TabIndex = 26;
            this.btnUnmount.Text = "Unmount";
            this.btnUnmount.UseVisualStyleBackColor = true;
            this.btnUnmount.Click += new System.EventHandler(this.btnUnmount_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 277);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(369, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 27;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Ready";
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnMount;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(369, 299);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnUnmount);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnMount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SSHFS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnMount;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSavePreset;
        private System.Windows.Forms.Button btnRemovePreset;
        private System.Windows.Forms.ComboBox cmbSelectedPreset;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbDriveLetter;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtPassword;
        private System.Windows.Forms.TextBox passphrase;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtPrivateKey;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton usePrivateKey;
        private System.Windows.Forms.Button btnOpenPrivateKey;
        private System.Windows.Forms.Button btnUnmount;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}