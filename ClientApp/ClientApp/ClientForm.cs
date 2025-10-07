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
        private StreamWriter writer;
        private NetworkStream stream;

        public ClientForm()
        {
            InitializeComponent();
            cmbEncryptionMethod.Items.AddRange(new[] { "Caesar", "Vigenere", "Substitution", "Affine" });
            cmbEncryptionMethod.SelectedIndex = 0;
        }

        private void btnconnect_Click(object sender, EventArgs e)
        {
            string inputIP = txtServerIP.Text.Trim();
            if (!int.TryParse(txtServerPort.Text.Trim(), out int port))
            {
                listBoxStatus.Items.Add("Hatalý port numarasý!");
                return;
            }

            try
            {
                client = new TcpClient("127.0.0.1", port);
                stream = client.GetStream();
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                writer.WriteLine($"IP|{inputIP}|PORT|{port}");
                string response = reader.ReadLine();
                if (response == "OK")
                    listBoxStatus.Items.Add("Baðlantý baþarýlý ve doðrulandý!");
                else
                {
                    listBoxStatus.Items.Add("Server doðrulama hatasý, baðlantý reddedildi.");
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add("Baðlantý hatasý: " + ex.Message);
            }
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (writer == null)
            {
                listBoxStatus.Items.Add("Baðlý deðil.");
                return;
            }

            string msg = txtMessage.Text.Trim();
            string method = cmbEncryptionMethod.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(msg))
            {
                string encrypted = Encrypt(msg, method);
                writer.WriteLine($"MSG|{method}|{encrypted}");
                listBoxStatus.Items.Add($"Mesaj gönderildi ({method}): {encrypted}");
            }
        }

        // --- DOSYA GÖNDERME ---
        private void btnSendImage_Click(object sender, EventArgs e)
        {
            SendFile("IMG");
        }

        private void btnSendVideo_Click(object sender, EventArgs e)
        {
            SendFile("VID");
        }

        private void btnSendAudio_Click(object sender, EventArgs e)
        {
            SendFile("AUD");
        }

        private void SendFile(string type)
        {
            if (writer == null)
            {
                listBoxStatus.Items.Add("Baðlý deðil.");
                return;
            }

            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = type switch
            {
                "IMG" => "Resim Dosyalarý|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                "VID" => "Video Dosyalarý|*.mp4;*.avi;*.mkv",
                "AUD" => "Ses Dosyalarý|*.wav;*.mp3;*.aac",
                _ => "Tüm Dosyalar|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] data = File.ReadAllBytes(ofd.FileName);
                string header = $"{type}|{Path.GetFileName(ofd.FileName)}|{data.Length}";
                writer.WriteLine(header);
                stream.Write(data, 0, data.Length);
                listBoxStatus.Items.Add($"{type} gönderildi: {ofd.FileName}");
            }
        }

        // --- ÞÝFRELEME METODLARI ---
        private string Encrypt(string text, string method)
        {
            return method switch
            {
                "Caesar" => CaesarEncrypt(text, 3),
                "Vigenere" => VigenereEncrypt(text, "KEY"),
                "Substitution" => SubstitutionEncrypt(text),
                "Affine" => AffineEncrypt(text),
                _ => text
            };
        }

        private string CaesarEncrypt(string text, int shift)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    char a = char.IsUpper(c) ? 'A' : 'a';
                    sb.Append((char)(((c - a + shift) % 26) + a));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private string VigenereEncrypt(string text, string key)
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
                    sb.Append((char)(((c - a + shift) % 26) + a));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private string SubstitutionEncrypt(string text)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string key = "QWERTYUIOPASDFGHJKLZXCVBNM";
            StringBuilder sb = new StringBuilder();
            foreach (char c in text.ToUpper())
            {
                int index = alphabet.IndexOf(c);
                sb.Append(index >= 0 ? key[index] : c);
            }
            return sb.ToString();
        }

        private string AffineEncrypt(string text)
        {
            int a = 5, b = 8;
            StringBuilder sb = new StringBuilder();
            foreach (char c in text.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    int x = c - 'A';
                    sb.Append((char)(((a * x + b) % 26) + 'A'));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
