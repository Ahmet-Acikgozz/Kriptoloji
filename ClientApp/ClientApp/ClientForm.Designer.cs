namespace ClientApp
{
    partial class ClientForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnSendFile;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.ComboBox cmbEncryptionMethod;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.TextBox txtKey;

        private void InitializeComponent()
        {
            labelIP = new Label();
            txtServerIP = new TextBox();
            labelPort = new Label();
            txtServerPort = new TextBox();
            btnConnect = new Button();
            btnDisconnect = new Button();
            txtMessage = new TextBox();
            btnSendMessage = new Button();
            btnSendFile = new Button();
            listBoxStatus = new ListBox();
            cmbEncryptionMethod = new ComboBox();
            lblKey = new Label();
            txtKey = new TextBox();
            SuspendLayout();



            labelIP.Location = new Point(12, 12);
            labelIP.Name = "labelIP";
            labelIP.Size = new Size(60, 20);
            labelIP.TabIndex = 0;
            labelIP.Text = "Server IP:";



            txtServerIP.Location = new Point(78, 12);
            txtServerIP.Name = "txtServerIP";
            txtServerIP.Size = new Size(140, 27);
            txtServerIP.TabIndex = 1;
            txtServerIP.Text = "127.0.0.1";



            labelPort.Location = new Point(230, 12);
            labelPort.Name = "labelPort";
            labelPort.Size = new Size(35, 20);
            labelPort.TabIndex = 2;
            labelPort.Text = "Port:";



            txtServerPort.Location = new Point(265, 12);
            txtServerPort.Name = "txtServerPort";
            txtServerPort.Size = new Size(70, 27);
            txtServerPort.TabIndex = 3;
            txtServerPort.Text = "9000";



            btnConnect.Location = new Point(345, 10);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(99, 26);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Connect";
            btnConnect.Click += btnConnect_Click;



            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(345, 42);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(99, 26);
            btnDisconnect.TabIndex = 5;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.Click += btnDisconnect_Click;



            txtMessage.Location = new Point(12, 100);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.PlaceholderText = "Mesajınızı buraya giriniz";
            txtMessage.Size = new Size(320, 60);
            txtMessage.TabIndex = 9;



            btnSendMessage.Location = new Point(340, 100);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(104, 30);
            btnSendMessage.TabIndex = 10;
            btnSendMessage.Text = "Send Msg";
            btnSendMessage.Click += btnSendMessage_Click;



            btnSendFile.Location = new Point(340, 140);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(104, 30);
            btnSendFile.TabIndex = 11;
            btnSendFile.Text = "Send File";
            btnSendFile.Click += btnSendFile_Click;



            listBoxStatus.Location = new Point(12, 170);
            listBoxStatus.Name = "listBoxStatus";
            listBoxStatus.Size = new Size(413, 184);
            listBoxStatus.TabIndex = 12;



            cmbEncryptionMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEncryptionMethod.Location = new Point(12, 46);
            cmbEncryptionMethod.Name = "cmbEncryptionMethod";
            cmbEncryptionMethod.Size = new Size(200, 28);
            cmbEncryptionMethod.TabIndex = 6;



            lblKey.Location = new Point(220, 46);
            lblKey.Name = "lblKey";
            lblKey.Size = new Size(40, 23);
            lblKey.TabIndex = 7;
            lblKey.Text = "Key:";



            txtKey.Location = new Point(260, 46);
            txtKey.Name = "txtKey";
            txtKey.Size = new Size(80, 27);
            txtKey.TabIndex = 8;



            ClientSize = new Size(466, 390);
            Controls.Add(labelIP);
            Controls.Add(txtServerIP);
            Controls.Add(labelPort);
            Controls.Add(txtServerPort);
            Controls.Add(btnConnect);
            Controls.Add(btnDisconnect);
            Controls.Add(cmbEncryptionMethod);
            Controls.Add(lblKey);
            Controls.Add(txtKey);
            Controls.Add(txtMessage);
            Controls.Add(btnSendMessage);
            Controls.Add(btnSendFile);
            Controls.Add(listBoxStatus);
            Name = "ClientForm";
            Text = "ClientForm";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
