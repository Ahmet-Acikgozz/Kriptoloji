namespace ServerApp
{
    partial class ServerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblIPPort = new System.Windows.Forms.Label();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.pictureBoxReceived = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReceived)).BeginInit();
            this.SuspendLayout();
            // 
            // lblIPPort
            // 
            this.lblIPPort.AutoSize = true;
            this.lblIPPort.Location = new System.Drawing.Point(20, 20);
            this.lblIPPort.Name = "lblIPPort";
            this.lblIPPort.Size = new System.Drawing.Size(120, 15);
            this.lblIPPort.TabIndex = 0;
            this.lblIPPort.Text = "Server IP: 123.45.67.89 | Port: 9000";
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(350, 15);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(120, 23);
            this.btnStartServer.TabIndex = 1;
            this.btnStartServer.Text = "Server Başlat";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.ItemHeight = 15;
            this.listBoxLog.Location = new System.Drawing.Point(20, 50);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(450, 199);
            this.listBoxLog.TabIndex = 2;
            // 
            // pictureBoxReceived
            // 
            this.pictureBoxReceived.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxReceived.Location = new System.Drawing.Point(20, 260);
            this.pictureBoxReceived.Name = "pictureBoxReceived";
            this.pictureBoxReceived.Size = new System.Drawing.Size(450, 200);
            this.pictureBoxReceived.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxReceived.TabIndex = 3;
            this.pictureBoxReceived.TabStop = false;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 480);
            this.Controls.Add(this.pictureBoxReceived);
            this.Controls.Add(this.listBoxLog);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.lblIPPort);
            this.Name = "ServerForm";
            this.Text = "Server Application";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReceived)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblIPPort;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.PictureBox pictureBoxReceived;
    }
}
