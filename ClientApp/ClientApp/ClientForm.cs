using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class ClientForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        public ClientForm()
        {
            InitializeComponent();

            cmbEncryptionMethod.Items.Clear();
            cmbEncryptionMethod.Items.AddRange(new object[] { "Caesar Cipher", "Vigenere Cipher", "Substitution Cipher", "Affine Cipher" });
            cmbEncryptionMethod.SelectedIndex = 0;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = txtServerIP.Text.Trim();
            if (!int.TryParse(txtServerPort.Text.Trim(), out int port))
            {
                MessageBox.Show("Geçerli port girin.");
                return;
            }

            try
            {
                client = new TcpClient();
                client.Connect(ip, port);
                stream = client.GetStream();
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                reader = new StreamReader(stream, Encoding.UTF8);


                writer.WriteLine($"IP|{txtServerIP.Text.Trim()}|PORT|{port}");

                string resp = reader.ReadLine();
                if (resp == "OK")
                {
                    listBoxStatus.Items.Add($"Baðlandý ve doðrulandý: {ip}:{port}");
                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = true;
                }
                else
                {
                    listBoxStatus.Items.Add("Server doðrulama hatasý, baðlantý kapatýldý.");
                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add("Baðlantý hatasý: " + ex.Message);
                CloseConnection();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            CloseConnection();
            listBoxStatus.Items.Add("Baðlantý kapatýldý.");
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (!EnsureConnected()) return;

            string method = cmbEncryptionMethod.SelectedItem?.ToString() ?? "";
            string key = txtKey.Text.Trim();
            string plain = txtMessage.Text;

            if (string.IsNullOrEmpty(method))
            {
                MessageBox.Show("Þifreleme metodu seçin.");
                return;
            }
            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Key girin.");
                return;
            }

            string encrypted;
            try
            {
                encrypted = EncryptMessage(plain, method, key);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Þifreleme hatasý: " + ex.Message);
                return;
            }

            string header = $"TEXT|{method}|{key}|{encrypted}";
            try
            {
                writer.WriteLine(header);
                listBoxStatus.Items.Add($"Gönderildi ({method}): {encrypted}");
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add("Gönderme hatasý: " + ex.Message);
            }
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            if (!EnsureConnected()) return;

            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            string filePath = ofd.FileName;
            byte[] data = File.ReadAllBytes(filePath);
            string filename = Path.GetFileName(filePath);

            string ext = Path.GetExtension(filename).ToLower();
            string type = "FILE";
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif") type = "IMG";
            else if (ext == ".mp4" || ext == ".avi" || ext == ".mkv") type = "VID";
            else if (ext == ".wav" || ext == ".mp3" || ext == ".aac") type = "AUD";

            try
            {
                string header = $"{type}|{filename}|{data.Length}";
                writer.WriteLine(header);
                stream.Write(data, 0, data.Length);
                listBoxStatus.Items.Add($"{type} gönderildi: {filename} ({data.Length} bytes)");
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add("Dosya gönderme hatasý: " + ex.Message);
            }
        }

        private bool EnsureConnected()
        {
            if (client == null || !client.Connected)
            {
                MessageBox.Show("Önce baðlanýn.");
                return false;
            }
            return true;
        }

        private void CloseConnection()
        {
            try { writer?.Close(); } catch { }
            try { reader?.Close(); } catch { }
            try { stream?.Close(); } catch { }
            try { client?.Close(); } catch { }
            writer = null;
            reader = null;
            stream = null;
            client = null;
        }

        private string EncryptMessage(string text, string method, string key)
        {
            switch (method)
            {
                case "Caesar Cipher":
                    if (!int.TryParse(key, out int shift)) throw new Exception("Caesar key (number) olmalý.");
                    return Caesar.Encrypt(text, shift);
                case "Vigenere Cipher":
                    if (string.IsNullOrEmpty(key)) throw new Exception("Vigenere key boþ olamaz.");
                    return Vigenere.Encrypt(text, key);
                case "Substitution Cipher":
                    if (key.Length != 26) throw new Exception("Substitution key 26 harf olmalý.");
                    return Substitution.Encrypt(text, key);
                case "Affine Cipher":
                    string[] parts = key.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 2) throw new Exception("Affine key: a,b formatýnda olmalý.");
                    if (!int.TryParse(parts[0], out int a)) throw new Exception("Affine a tam sayý olmalý.");
                    if (!int.TryParse(parts[1], out int b)) throw new Exception("Affine b tam sayý olmalý.");
                    return Affine.Encrypt(text, a, b);
                default:
                    throw new Exception("Bilinmeyen metod.");
            }
        }

        private static class Caesar
        {
            public static string Encrypt(string input, int key)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in input)
                {
                    if (char.IsLetter(c))
                    {
                        char basec = char.IsUpper(c) ? 'A' : 'a';
                        sb.Append((char)(((c - basec + key) % 26 + 26) % 26 + basec));
                    }
                    else sb.Append(c);
                }
                return sb.ToString();
            }
        }

        private static class Vigenere
        {
            public static string Encrypt(string text, string key)
            {
                StringBuilder sb = new StringBuilder();
                key = key.ToUpper();
                int j = 0;
                foreach (char c in text)
                {
                    if (char.IsLetter(c))
                    {
                        char basec = char.IsUpper(c) ? 'A' : 'a';
                        int shift = key[j % key.Length] - 'A';
                        sb.Append((char)(((c - basec + shift) % 26 + 26) % 26 + basec));
                        j++;
                    }
                    else sb.Append(c);
                }
                return sb.ToString();
            }
        }

        private static class Substitution
        {
            public static string Encrypt(string text, string key)
            {
                string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                key = key.ToUpper();
                StringBuilder sb = new StringBuilder();
                foreach (char c in text.ToUpper())
                {
                    if (char.IsLetter(c))
                    {
                        int idx = alpha.IndexOf(c);
                        sb.Append(key[idx]);
                    }
                    else sb.Append(c);
                }
                return sb.ToString();
            }
        }

        private static class Affine
        {
            public static string Encrypt(string text, int a, int b)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in text.ToUpper())
                {
                    if (char.IsLetter(c))
                    {
                        int x = c - 'A';
                        int enc = (a * x + b) % 26;
                        sb.Append((char)(enc + 'A'));
                    }
                    else sb.Append(c);
                }
                return sb.ToString();
            }
        }
    }
}
