namespace ServerApp
{
    partial class ServerForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Label lblIPPort;
        private System.Windows.Forms.ListBox listBoxLog;

        private void InitializeComponent()
        {
            this.btnStartServer = new System.Windows.Forms.Button();
            this.lblIPPort = new System.Windows.Forms.Label();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(10, 10);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(120, 30);
            this.btnStartServer.Text = "Server Başlat";
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // lblIPPort
            // 
            this.lblIPPort.Location = new System.Drawing.Point(150, 15);
            this.lblIPPort.Name = "lblIPPort";
            this.lblIPPort.Size = new System.Drawing.Size(300, 20);
            this.lblIPPort.Text = "IP: 0.0.0.0 | Port: 9000";
            // 
            // listBoxLog
            // 
            this.listBoxLog.Location = new System.Drawing.Point(10, 50);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(460, 400);
            // 
            // ServerForm
            // 
            this.ClientSize = new System.Drawing.Size(480, 470);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.lblIPPort);
            this.Controls.Add(this.listBoxLog);
            this.Name = "ServerForm";
            this.Text = "Server App";
            this.ResumeLayout(false);
        }
    }
}
