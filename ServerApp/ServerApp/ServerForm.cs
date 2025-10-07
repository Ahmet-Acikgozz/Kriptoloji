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
        private string serverIP = "123.45.67.89";

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
                string header = reader.ReadLine();
                string[] parts = header.Split('|');

                if (parts.Length == 4 && parts[1] == serverIP && parts[3] == serverPort.ToString())
                {
                    Invoke((Action)(() => listBoxLog.Items.Add("Client doðrulandý ve baðlandý.")));
                    writer.WriteLine("OK");

                    while (true)
                    {
                        string msgHeader = reader.ReadLine();
                        if (msgHeader == null) break;

                        if (msgHeader.StartsWith("MSG|"))
                        {
                            string[] msgParts = msgHeader.Split('|');
                            if (msgParts.Length == 3)
                            {
                                string method = msgParts[1];
                                string encrypted = msgParts[2];
                                string decrypted = Decrypt(encrypted, method);
                                Invoke((Action)(() => listBoxLog.Items.Add($"Client ({method}): {decrypted}")));
                            }
                        }
                        else if (msgHeader.StartsWith("IMG|") || msgHeader.StartsWith("VID|") || msgHeader.StartsWith("AUD|"))
                        {
                            string[] parts2 = msgHeader.Split('|');
                            string type = parts2[0];
                            string filename = parts2[1];
                            int length = int.Parse(parts2[2]);

                            byte[] data = new byte[length];
                            int read = 0;
                            while (read < length)
                            {
                                int bytesRead = stream.Read(data, read, length - read);
                                if (bytesRead == 0) break;
                                read += bytesRead;
                            }

                            string savePath = Path.Combine(Application.StartupPath, "Received_" + filename);
                            File.WriteAllBytes(savePath, data);
                            Invoke((Action)(() => listBoxLog.Items.Add($"{type} kaydedildi: {savePath}")));
                        }
                    }
                }
                else
                {
                    Invoke((Action)(() => listBoxLog.Items.Add("Client doðrulama hatasý.")));
                    client.Close();
                }
            }
            catch
            {
                client.Close();
                Invoke((Action)(() => listBoxLog.Items.Add("Client baðlantýsý kapandý.")));
            }
        }

        // --- DECRYPT METODLARI ---
        private string Decrypt(string text, string method)
        {
            return method switch
            {
                "Caesar" => CaesarDecrypt(text, 3),
                "Vigenere" => VigenereDecrypt(text, "KEY"),
                "Substitution" => SubstitutionDecrypt(text),
                "Affine" => AffineDecrypt(text),
                _ => text
            };
        }

        private string CaesarDecrypt(string text, int shift)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    char a = char.IsUpper(c) ? 'A' : 'a';
                    sb.Append((char)(((c - a - shift + 26) % 26) + a));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private string VigenereDecrypt(string text, string key)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsLetter(c))
                {
                    char k = key[i % key.Length];
                    int shift = (char.ToUpper(k) - 'A');
                    char a = char.IsUpper(c) ? 'A' : 'a';
                    sb.Append((char)(((c - a - shift + 26) % 26) + a));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private string SubstitutionDecrypt(string text)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string key = "QWERTYUIOPASDFGHJKLZXCVBNM";
            StringBuilder sb = new StringBuilder();
            foreach (char c in text.ToUpper())
            {
                int index = key.IndexOf(c);
                sb.Append(index >= 0 ? alphabet[index] : c);
            }
            return sb.ToString();
        }

        private string AffineDecrypt(string text)
        {
            int a = 5, b = 8;
            int a_inv = 21; // 5'in mod 26 tersidir
            StringBuilder sb = new StringBuilder();
            foreach (char c in text.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    int y = c - 'A';
                    sb.Append((char)(((a_inv * (y - b + 26)) % 26) + 'A'));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
