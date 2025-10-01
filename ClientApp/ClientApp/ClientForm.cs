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
                client = new TcpClient("127.0.0.1", port); // IP = local host yeterli
                stream = client.GetStream();
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                // Servera IP ve port doðrulama gönder
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
            if (writer == null) { listBoxStatus.Items.Add("Baðlý deðil."); return; }
            string msg = txtMessage.Text.Trim();
            if (!string.IsNullOrEmpty(msg))
            {
                writer.WriteLine("MSG|" + msg);
                listBoxStatus.Items.Add("Mesaj gönderildi.");
            }
        }

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
            if (writer == null) { listBoxStatus.Items.Add("Baðlý deðil."); return; }
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
    }
}
