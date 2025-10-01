namespace ClientApp
{
    partial class ClientForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblServerIP;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Label lblServerPort;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnSendImage;
        private System.Windows.Forms.Button btnSendVideo;
        private System.Windows.Forms.Button btnSendAudio;
        private System.Windows.Forms.ListBox listBoxStatus;

        private void InitializeComponent()
        {
            this.lblServerIP = new System.Windows.Forms.Label();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.lblServerPort = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btnSendImage = new System.Windows.Forms.Button();
            this.btnSendVideo = new System.Windows.Forms.Button();
            this.btnSendAudio = new System.Windows.Forms.Button();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lblServerIP
            // 
            this.lblServerIP.Location = new System.Drawing.Point(10, 10);
            this.lblServerIP.Size = new System.Drawing.Size(70, 20);
            this.lblServerIP.Text = "Server IP:";
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(90, 10);
            this.txtServerIP.Size = new System.Drawing.Size(120, 22);
            this.txtServerIP.Text = "123.45.67.89"; // Server IP ile eşleşecek
            // 
            // lblServerPort
            // 
            this.lblServerPort.Location = new System.Drawing.Point(220, 10);
            this.lblServerPort.Size = new System.Drawing.Size(40, 20);
            this.lblServerPort.Text = "Port:";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(260, 10);
            this.txtServerPort.Size = new System.Drawing.Size(60, 22);
            this.txtServerPort.Text = "9000"; // Server port ile eşleşecek
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(330, 8);
            this.btnConnect.Size = new System.Drawing.Size(80, 25);
            this.btnConnect.Text = "Bağlan";
            this.btnConnect.Click += new System.EventHandler(this.btnconnect_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(10, 50);
            this.txtMessage.Size = new System.Drawing.Size(360, 22);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(380, 48);
            this.btnSendMessage.Size = new System.Drawing.Size(100, 25);
            this.btnSendMessage.Text = "Mesaj Gönder";
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnSendImage
            // 
            this.btnSendImage.Location = new System.Drawing.Point(10, 80);
            this.btnSendImage.Size = new System.Drawing.Size(100, 25);
            this.btnSendImage.Text = "Resim Gönder";
            this.btnSendImage.Click += new System.EventHandler(this.btnSendImage_Click);
            // 
            // btnSendVideo
            // 
            this.btnSendVideo.Location = new System.Drawing.Point(120, 80);
            this.btnSendVideo.Size = new System.Drawing.Size(100, 25);
            this.btnSendVideo.Text = "Video Gönder";
            this.btnSendVideo.Click += new System.EventHandler(this.btnSendVideo_Click);
            // 
            // btnSendAudio
            // 
            this.btnSendAudio.Location = new System.Drawing.Point(230, 80);
            this.btnSendAudio.Size = new System.Drawing.Size(100, 25);
            this.btnSendAudio.Text = "Ses Gönder";
            this.btnSendAudio.Click += new System.EventHandler(this.btnSendAudio_Click);
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.Location = new System.Drawing.Point(10, 120);
            this.listBoxStatus.Size = new System.Drawing.Size(470, 300);
            // 
            // ClientForm
            // 
            this.ClientSize = new System.Drawing.Size(500, 440);
            this.Controls.Add(this.lblServerIP);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.lblServerPort);
            this.Controls.Add(this.txtServerPort);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.btnSendImage);
            this.Controls.Add(this.btnSendVideo);
            this.Controls.Add(this.btnSendAudio);
            this.Controls.Add(this.listBoxStatus);
            this.Text = "Client App";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
