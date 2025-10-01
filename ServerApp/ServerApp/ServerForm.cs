using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class ServerForm : Form
    {
        private TcpListener listener;
        private int serverPort = 9000;
        private string serverIP = "123.45.67.89"; // Random belirlediðin IP (client ile eþleþecek)

        public ServerForm()
        {
            InitializeComponent();
            lblIPPort.Text = $"Server IP: {serverIP} | Port: {serverPort}";
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            Thread serverThread = new Thread(StartServer);
            serverThread.IsBackground = true;
            serverThread.Start();
            listBoxLog.Items.Add("Server baþlatýldý...");
            btnStartServer.Enabled = false;
        }

        private void StartServer()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, serverPort);
                listener.Start();

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => listBoxLog.Items.Add("Hata: " + ex.Message)));
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = obj as TcpClient;
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            try
            {
                // Client doðrulama
                string header = reader.ReadLine(); // Örn: "IP|123.45.67.89|PORT|9000"
                string[] parts = header.Split('|');

                if (parts.Length == 4 && parts[1] == serverIP && parts[3] == serverPort.ToString())
                {
                    Invoke((Action)(() => listBoxLog.Items.Add("Client doðrulandý ve baðlandý.")));
                    writer.WriteLine("OK"); // Onay mesajý

                    // Artýk mesaj ve dosya gönderebilir
                    while (true)
                    {
                        string msgHeader = reader.ReadLine();
                        if (msgHeader.StartsWith("MSG|"))
                        {
                            string msg = msgHeader.Substring(4);
                            Invoke((Action)(() => listBoxLog.Items.Add("Client: " + msg)));
                        }
                        else if (msgHeader.StartsWith("IMG|") || msgHeader.StartsWith("VID|") || msgHeader.StartsWith("AUD|"))
                        {
                            string type = msgHeader.Split('|')[0];
                            string filename = msgHeader.Split('|')[1];

                            // Dosya boyutu ve verisi alýnýyor
                            int length = int.Parse(msgHeader.Split('|')[2]);
                            byte[] data = new byte[length];
                            int read = 0;
                            while (read < length)
                            {
                                read += stream.Read(data, read, length - read);
                            }

                            string savePath = Path.Combine(Application.StartupPath, filename);
                            File.WriteAllBytes(savePath, data);
                            Invoke((Action)(() => listBoxLog.Items.Add($"{type} kaydedildi: {savePath}")));
                        }
                    }
                }
                else
                {
                    Invoke((Action)(() => listBoxLog.Items.Add("Client doðrulama hatasý, baðlantý reddedildi.")));
                    client.Close();
                }
            }
            catch
            {
                client.Close();
                Invoke((Action)(() => listBoxLog.Items.Add("Client baðlantýsý kapandý.")));
            }
        }
    }
}
