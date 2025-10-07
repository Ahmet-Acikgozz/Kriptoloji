namespace ClientApp
{
    partial class ClientForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnSendImage;
        private System.Windows.Forms.Button btnSendVideo;
        private System.Windows.Forms.Button btnSendAudio;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.ComboBox cmbEncryptionMethod;

        private void InitializeComponent()
        {
            lblIP = new Label();
            txtServerIP = new TextBox();
            lblPort = new Label();
            txtServerPort = new TextBox();
            btnConnect = new Button();
            txtMessage = new TextBox();
            btnSendMessage = new Button();
            btnSendImage = new Button();
            btnSendVideo = new Button();
            btnSendAudio = new Button();
            listBoxStatus = new ListBox();
            cmbEncryptionMethod = new ComboBox();
            SuspendLayout();
            // 
            // lblIP
            // 
            lblIP.Location = new Point(10, 10);
            lblIP.Name = "lblIP";
            lblIP.Size = new Size(70, 20);
            lblIP.TabIndex = 0;
            lblIP.Text = "Server IP:";
            // 
            // txtServerIP
            // 
            txtServerIP.Location = new Point(90, 10);
            txtServerIP.Name = "txtServerIP";
            txtServerIP.Size = new Size(120, 27);
            txtServerIP.TabIndex = 1;
            txtServerIP.Text = "123.45.67.89";
            // 
            // lblPort
            // 
            lblPort.Location = new Point(220, 10);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(40, 20);
            lblPort.TabIndex = 2;
            lblPort.Text = "Port:";
            // 
            // txtServerPort
            // 
            txtServerPort.Location = new Point(260, 10);
            txtServerPort.Name = "txtServerPort";
            txtServerPort.Size = new Size(60, 27);
            txtServerPort.TabIndex = 3;
            txtServerPort.Text = "9000";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(330, 8);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(80, 25);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Bağlan";
            btnConnect.Click += btnconnect_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(10, 50);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(360, 27);
            txtMessage.TabIndex = 5;
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(380, 48);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(170, 25);
            btnSendMessage.TabIndex = 6;
            btnSendMessage.Text = "Mesaj Gönder";
            btnSendMessage.Click += btnSendMessage_Click;
            // 
            // btnSendImage
            // 
            btnSendImage.Location = new Point(10, 80);
            btnSendImage.Name = "btnSendImage";
            btnSendImage.Size = new Size(100, 25);
            btnSendImage.TabIndex = 7;
            btnSendImage.Text = "Resim Gönder";
            btnSendImage.Click += btnSendImage_Click;
            // 
            // btnSendVideo
            // 
            btnSendVideo.Location = new Point(120, 80);
            btnSendVideo.Name = "btnSendVideo";
            btnSendVideo.Size = new Size(100, 25);
            btnSendVideo.TabIndex = 8;
            btnSendVideo.Text = "Video Gönder";
            btnSendVideo.Click += btnSendVideo_Click;
            // 
            // btnSendAudio
            // 
            btnSendAudio.Location = new Point(230, 80);
            btnSendAudio.Name = "btnSendAudio";
            btnSendAudio.Size = new Size(100, 25);
            btnSendAudio.TabIndex = 9;
            btnSendAudio.Text = "Ses Gönder";
            btnSendAudio.Click += btnSendAudio_Click;
            // 
            // listBoxStatus
            // 
            listBoxStatus.Location = new Point(10, 120);
            listBoxStatus.Name = "listBoxStatus";
            listBoxStatus.Size = new Size(460, 284);
            listBoxStatus.TabIndex = 11;
            // 
            // cmbEncryptionMethod
            // 
            cmbEncryptionMethod.Location = new Point(405, 86);
            cmbEncryptionMethod.Name = "cmbEncryptionMethod";
            cmbEncryptionMethod.Size = new Size(121, 28);
            cmbEncryptionMethod.TabIndex = 10;
            // 
            // ClientForm
            // 
            ClientSize = new Size(649, 450);
            Controls.Add(lblIP);
            Controls.Add(txtServerIP);
            Controls.Add(lblPort);
            Controls.Add(txtServerPort);
            Controls.Add(btnConnect);
            Controls.Add(txtMessage);
            Controls.Add(btnSendMessage);
            Controls.Add(btnSendImage);
            Controls.Add(btnSendVideo);
            Controls.Add(btnSendAudio);
            Controls.Add(cmbEncryptionMethod);
            Controls.Add(listBoxStatus);
            Name = "ClientForm";
            Text = "File Transfer Client";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
