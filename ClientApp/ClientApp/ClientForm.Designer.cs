namespace ClientApp
{
    partial class ClientForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

       

        private void InitializeComponent()
        {
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btnSendFile = new System.Windows.Forms.Button();
            this.cmbEncryptionMethod = new System.Windows.Forms.ComboBox();
            this.cmbKeyExchange = new System.Windows.Forms.ComboBox();
            this.lblKeyExchange = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkManualMode = new System.Windows.Forms.CheckBox();
            this.btnEncryptFile = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Server IP:";
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(70, 19);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(100, 23);
            this.txtServerIP.TabIndex = 0;
            this.txtServerIP.Text = "127.0.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(180, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Port:";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(218, 19);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(60, 23);
            this.txtServerPort.TabIndex = 1;
            this.txtServerPort.Text = "9000";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(290, 18);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(80, 25);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Baglan";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(376, 18);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(80, 25);
            this.btnDisconnect.TabIndex = 3;
            this.btnDisconnect.Text = "Kopar";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtServerIP);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtServerPort);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.btnDisconnect);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 55);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Baglanti";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "Metod:";
            // 
            // cmbEncryptionMethod
            // 
            this.cmbEncryptionMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncryptionMethod.FormattingEnabled = true;
            this.cmbEncryptionMethod.Location = new System.Drawing.Point(60, 22);
            this.cmbEncryptionMethod.Name = "cmbEncryptionMethod";
            this.cmbEncryptionMethod.Size = new System.Drawing.Size(145, 23);
            this.cmbEncryptionMethod.TabIndex = 8;
            // 
            // lblKeyExchange
            // 
            this.lblKeyExchange.AutoSize = true;
            this.lblKeyExchange.Location = new System.Drawing.Point(10, 52);
            this.lblKeyExchange.Name = "lblKeyExchange";
            this.lblKeyExchange.Size = new System.Drawing.Size(80, 15);
            this.lblKeyExchange.TabIndex = 18;
            this.lblKeyExchange.Text = "Key Exchange:";
            // 
            // cmbKeyExchange
            // 
            this.cmbKeyExchange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKeyExchange.FormattingEnabled = true;
            this.cmbKeyExchange.Location = new System.Drawing.Point(95, 49);
            this.cmbKeyExchange.Name = "cmbKeyExchange";
            this.cmbKeyExchange.Size = new System.Drawing.Size(110, 23);
            this.cmbKeyExchange.TabIndex = 19;
            this.cmbKeyExchange.Enabled = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(215, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Anahtar:";
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(275, 22);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(181, 23);
            this.txtKey.TabIndex = 9;
            // 
            // chkManualMode
            // 
            this.chkManualMode.AutoSize = true;
            this.chkManualMode.Location = new System.Drawing.Point(215, 51);
            this.chkManualMode.Name = "chkManualMode";
            this.chkManualMode.Size = new System.Drawing.Size(150, 19);
            this.chkManualMode.TabIndex = 15;
            this.chkManualMode.Text = "Kutuphanesiz (Manuel)";
            this.chkManualMode.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cmbEncryptionMethod);
            this.groupBox2.Controls.Add(this.lblKeyExchange);
            this.groupBox2.Controls.Add(this.cmbKeyExchange);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtKey);
            this.groupBox2.Controls.Add(this.chkManualMode);
            this.groupBox2.Location = new System.Drawing.Point(12, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(470, 80);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sifreleme Ayarlari";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 15);
            this.label5.TabIndex = 16;
            this.label5.Text = "Mesaj:";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(10, 40);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(340, 50);
            this.txtMessage.TabIndex = 5;
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(360, 22);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(96, 30);
            this.btnSendMessage.TabIndex = 6;
            this.btnSendMessage.Text = "Mesaj Gonder";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnSendFile
            // 
            this.btnSendFile.Location = new System.Drawing.Point(360, 58);
            this.btnSendFile.Name = "btnSendFile";
            this.btnSendFile.Size = new System.Drawing.Size(96, 30);
            this.btnSendFile.TabIndex = 7;
            this.btnSendFile.Text = "Dosya Gonder";
            this.btnSendFile.UseVisualStyleBackColor = true;
            this.btnSendFile.Click += new System.EventHandler(this.btnSendFile_Click);
            // 
            // btnEncryptFile
            // 
            this.btnEncryptFile.Location = new System.Drawing.Point(360, 94);
            this.btnEncryptFile.Name = "btnEncryptFile";
            this.btnEncryptFile.Size = new System.Drawing.Size(96, 30);
            this.btnEncryptFile.TabIndex = 17;
            this.btnEncryptFile.Text = "Dosya Sifrele";
            this.btnEncryptFile.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtMessage);
            this.groupBox3.Controls.Add(this.btnSendMessage);
            this.groupBox3.Controls.Add(this.btnSendFile);
            this.groupBox3.Controls.Add(this.btnEncryptFile);
            this.groupBox3.Location = new System.Drawing.Point(12, 159);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(470, 135);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Mesaj ve Dosya";
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.ItemHeight = 15;
            this.listBoxStatus.Location = new System.Drawing.Point(12, 300);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(470, 199);
            this.listBoxStatus.TabIndex = 4;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 506);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.listBoxStatus);
            this.Name = "ClientForm";
            this.Text = "Kripto Client - AES/DES/RSA";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        

        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnSendFile;
        private System.Windows.Forms.ComboBox cmbEncryptionMethod;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkManualMode;
        private System.Windows.Forms.Button btnEncryptFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbKeyExchange;
        private System.Windows.Forms.Label lblKeyExchange;
    }
}
