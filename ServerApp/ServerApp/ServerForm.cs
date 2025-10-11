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
        private bool serverRunning = false;

        private int serverPort = 9000;
        private string serverIP = "127.0.0.1";

        public ServerForm()
        {
            InitializeComponent();
            btnStopServer.Enabled = false; // Stop Server baþlangýçta pasif
            lblIPPort.Text = $"Server IP: {serverIP} | Port: {serverPort}";
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            // Formdan IP ve Port al
            serverIP = txtIP.Text.Trim();
            if (!int.TryParse(txtPort.Text.Trim(), out serverPort))
            {
                MessageBox.Show("Hatalý port numarasý!");
                return;
            }

            try
            {
                listener = new TcpListener(IPAddress.Any, serverPort);
                listener.Start();
                serverRunning = true;

                btnStartServer.Enabled = false;
                btnStopServer.Enabled = true;
                listBoxLog.Items.Add($"Server çalýþýyor (Port {serverPort})");

                Thread t = new Thread(AcceptLoop) { IsBackground = true };
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server baþlatýlamadý: " + ex.Message);
            }
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            serverRunning = false;
            try
            {
                listener?.Stop();
                listBoxLog.Items.Add("Server durduruldu.");
            }
            catch { }

            btnStartServer.Enabled = true;
            btnStopServer.Enabled = false;
        }

        private void AcceptLoop()
        {
            try
            {
                while (serverRunning)
                {
                    if (listener.Pending())
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        ThreadPool.QueueUserWorkItem(HandleClient, client);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (SocketException)
            {
                // listener.Stop() çaðrýldýðýnda gelir
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => listBoxLog.Items.Add("Hata: " + ex.Message)));
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = obj as TcpClient;
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            try
            {
                // Client doðrulama
                string first = reader.ReadLine();
                if (string.IsNullOrEmpty(first))
                {
                    client.Close();
                    return;
                }

                string[] firstParts = first.Split('|');
                if (!(firstParts.Length == 4 && firstParts[0] == "IP" && firstParts[1] == serverIP &&
                      firstParts[2] == "PORT" && firstParts[3] == serverPort.ToString()))
                {
                    Invoke((Action)(() => listBoxLog.Items.Add("Client doðrulama baþarýsýz: " + first)));
                    writer.WriteLine("DENIED");
                    client.Close();
                    return;
                }

                writer.WriteLine("OK");
                Invoke((Action)(() => listBoxLog.Items.Add("Yeni client baðlandý.")));

                string line;
                while ((line = reader.ReadLine()) != null && serverRunning)
                {
                    if (line.StartsWith("TEXT|"))
                    {
                        string[] parts = line.Split('|', 4);
                        if (parts.Length >= 4)
                        {
                            string method = parts[1];
                            string key = parts[2];
                            string encrypted = parts[3];
                            string decrypted = DecryptMessage(encrypted, method, key);
                            Invoke((Action)(() =>
                            {
                                listBoxLog.Items.Add($"[{method}] Þifreli: {encrypted}");
                                listBoxLog.Items.Add($"Çözülmüþ: {decrypted}");
                            }));
                        }
                    }
                    else if (line.StartsWith("IMG|") || line.StartsWith("VID|") || line.StartsWith("AUD|") || line.StartsWith("FILE|"))
                    {
                        string[] parts = line.Split('|', 3);
                        if (parts.Length >= 3)
                        {
                            string type = parts[0];
                            string filename = parts[1];
                            if (!int.TryParse(parts[2], out int length))
                                length = 0;

                            byte[] buffer = new byte[length];
                            int offset = 0;
                            while (offset < length)
                            {
                                int read = stream.Read(buffer, offset, length - offset);
                                if (read == 0) break;
                                offset += read;
                            }

                            string savePath = Path.Combine(Application.StartupPath, "Received_" + filename);
                            File.WriteAllBytes(savePath, buffer);
                            Invoke((Action)(() => listBoxLog.Items.Add($"{type} alýndý: {savePath}")));
                        }
                    }
                    else
                    {
                        Invoke((Action)(() => listBoxLog.Items.Add("Gelen: " + line)));
                    }
                }
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => listBoxLog.Items.Add("Client baðlantýsý kapandý: " + ex.Message)));
            }
            finally
            {
                try { client.Close(); } catch { }
                Invoke((Action)(() => listBoxLog.Items.Add("Client ayrýldý.")));
            }
        }

        private string DecryptMessage(string text, string method, string key)
        {
            try
            {
                return method switch
                {
                    "Caesar Cipher" => CaesarDecrypt(text, int.Parse(key)),
                    "Vigenere Cipher" => VigenereDecrypt(text, key),
                    "Substitution Cipher" => SubstitutionDecrypt(text, key),
                    "Affine Cipher" => AffineDecrypt(text, key),
                    _ => "[Unknown method]"
                };
            }
            catch
            {
                return "[Decrypt hata]";
            }
        }

        // --- Þifre çözme metodlarý ---
        private string CaesarDecrypt(string input, int key)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char basec = char.IsUpper(c) ? 'A' : 'a';
                    sb.Append((char)(((c - basec - key + 26) % 26) + basec));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private string VigenereDecrypt(string cipher, string key)
        {
            StringBuilder sb = new StringBuilder();
            key = key.ToUpper();
            int j = 0;
            foreach (char c in cipher)
            {
                if (char.IsLetter(c))
                {
                    char basec = char.IsUpper(c) ? 'A' : 'a';
                    int shift = key[j % key.Length] - 'A';
                    sb.Append((char)(((c - basec - shift + 26) % 26) + basec));
                    j++;
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private string SubstitutionDecrypt(string cipher, string key)
        {
            string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            key = key.ToUpper();
            StringBuilder sb = new StringBuilder();
            foreach (char c in cipher.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    int idx = key.IndexOf(c);
                    if (idx >= 0) sb.Append(alpha[idx]);
                    else sb.Append(c);
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private string AffineDecrypt(string cipher, string key)
        {
            string[] parts = key.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return "[Affine key hata]";
            if (!int.TryParse(parts[0], out int a)) return "[Affine key hata]";
            if (!int.TryParse(parts[1], out int b)) return "[Affine key hata]";

            int a_inv = -1;
            for (int i = 1; i < 26; i++)
                if ((a * i) % 26 == 1) { a_inv = i; break; }
            if (a_inv == -1) return "[Affine a inverse yok]";

            StringBuilder sb = new StringBuilder();
            foreach (char c in cipher.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    int y = c - 'A';
                    int dec = (a_inv * ((y - b + 26) % 26)) % 26;
                    sb.Append((char)(dec + 'A'));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
