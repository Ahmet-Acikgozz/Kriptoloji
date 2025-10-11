namespace ServerApp
{
    partial class ServerForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnStopServer;
        private System.Windows.Forms.Label lblIPPort;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Label lblPort;

        private void InitializeComponent()
        {
            btnStartServer = new Button();
            btnStopServer = new Button();
            lblIPPort = new Label();
            listBoxLog = new ListBox();
            txtIP = new TextBox();
            txtPort = new TextBox();
            lblIP = new Label();
            lblPort = new Label();
            SuspendLayout();


            
            btnStartServer.Location = new Point(12, 12);
            btnStartServer.Name = "btnStartServer";
            btnStartServer.Size = new Size(120, 30);
            btnStartServer.TabIndex = 0;
            btnStartServer.Text = "Start Server";
            btnStartServer.Click += btnStartServer_Click;


            
            btnStopServer.Location = new Point(140, 12);
            btnStopServer.Name = "btnStopServer";
            btnStopServer.Size = new Size(120, 30);
            btnStopServer.TabIndex = 1;
            btnStopServer.Text = "Stop Server";
            btnStopServer.Click += btnStopServer_Click;


            lblIPPort.Location = new Point(12, 50);
            lblIPPort.Name = "lblIPPort";
            lblIPPort.Size = new Size(400, 20);
            lblIPPort.TabIndex = 2;
            lblIPPort.Text = "Server IP: 127.0.0.1 | Port: 9000";


            
            listBoxLog.Location = new Point(12, 80);
            listBoxLog.Name = "listBoxLog";
            listBoxLog.Size = new Size(460, 344);
            listBoxLog.TabIndex = 3;


            
            txtIP.Location = new Point(301, 15);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(100, 27);
            txtIP.TabIndex = 4;
            txtIP.Text = "127.0.0.1";


             
            txtPort.Location = new Point(450, 15);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(60, 27);
            txtPort.TabIndex = 5;
            txtPort.Text = "9000";


             
            lblIP.Location = new Point(265, 18);
            lblIP.Name = "lblIP";
            lblIP.Size = new Size(30, 20);
            lblIP.TabIndex = 6;
            lblIP.Text = "IP:";


             
            lblPort.Location = new Point(407, 18);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(53, 20);
            lblPort.TabIndex = 7;
            lblPort.Text = "Port:";


            
            ClientSize = new Size(641, 450);
            Controls.Add(btnStartServer);
            Controls.Add(btnStopServer);
            Controls.Add(lblIPPort);
            Controls.Add(listBoxLog);
            Controls.Add(txtIP);
            Controls.Add(txtPort);
            Controls.Add(lblIP);
            Controls.Add(lblPort);
            Name = "ServerForm";
            Text = "ServerForm";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
