using System;
using System.IO;
using System.Linq;
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
            cmbEncryptionMethod.Items.AddRange(new object[] {
                "Caesar Cipher",
                "Vigenere Cipher",
                "Substitution Cipher",
                "Affine Cipher",
                "Rail Fence",
                "Route Cipher",           // YENÝ (Spiral)
                "Columnar Transposition",
                "Hill Cipher"             // GÜNCELLENDÝ (Matris destekli)
            });
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
                    listBoxStatus.Items.Add("Server doðrulama hatasý.");
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

            if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Metod ve Key alanlarýný doldurun.");
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
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png") type = "IMG";
            else if (ext == ".mp4" || ext == ".avi") type = "VID";
            else if (ext == ".wav" || ext == ".mp3") type = "AUD";

            try
            {
                writer.WriteLine($"{type}|{filename}|{data.Length}");
                stream.Write(data, 0, data.Length);
                listBoxStatus.Items.Add($"{type} gönderildi: {filename}");
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add("Dosya hatasý: " + ex.Message);
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
        }

        private string EncryptMessage(string text, string method, string key)
        {
            switch (method)
            {
                case "Caesar Cipher":
                    if (!int.TryParse(key, out int shift)) throw new Exception("Caesar key sayý olmalý.");
                    return Caesar.Encrypt(text, shift);
                case "Vigenere Cipher":
                    return Vigenere.Encrypt(text, key);
                case "Substitution Cipher":
                    if (key.Length != 26) throw new Exception("Substitution key 26 harf olmalý.");
                    return Substitution.Encrypt(text, key);
                case "Affine Cipher":
                    string[] parts = key.Split(',');
                    if (parts.Length != 2) throw new Exception("Affine key: a,b olmalý.");
                    return Affine.Encrypt(text, int.Parse(parts[0]), int.Parse(parts[1]));
                case "Rail Fence":
                    if (!int.TryParse(key, out int rails)) throw new Exception("Rail Fence key sayý olmalý.");
                    return RailFence.Encrypt(text, rails);

                // --- YENÝ EKLENENLER ---
                case "Route Cipher":
                    if (!int.TryParse(key, out int cols)) throw new Exception("Route Cipher key sütun sayýsý olmalý.");
                    return RouteCipher.Encrypt(text, cols);

                case "Columnar Transposition":
                    return Columnar.Encrypt(text, key);

                case "Hill Cipher":
                    // Key artýk hem "GYBN" hem "6 24 1 13" olabilir
                    return Hill.Encrypt(text, key);
                // -----------------------

                default: throw new Exception("Bilinmeyen metod.");
            }
        }

        // --- ÞÝFRELEME SINIFLARI (Sadece Encrypt) ---

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
                    if (char.IsLetter(c)) sb.Append(key[alpha.IndexOf(c)]);
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
                        int enc = (a * (c - 'A') + b) % 26;
                        sb.Append((char)(enc + 'A'));
                    }
                    else sb.Append(c);
                }
                return sb.ToString();
            }
        }

        private static class RailFence
        {
            public static string Encrypt(string text, int rails)
            {
                if (rails < 2) return text;
                string[] railRows = new string[rails];
                int row = 0;
                bool down = false;
                foreach (char c in text)
                {
                    railRows[row] += c;
                    if (row == 0 || row == rails - 1) down = !down;
                    row += down ? 1 : -1;
                }
                return string.Concat(railRows);
            }
        }

        // --- YENÝ ROUTE CIPHER (Görseldeki Spiral - Encrypt) ---
        private static class RouteCipher
        {
            public static string Encrypt(string text, int cols)
            {
                text = text.Replace(" ", "").ToUpper();
                int len = text.Length;
                int rows = (int)Math.Ceiling((double)len / cols);
                int totalCells = rows * cols;
                text = text.PadRight(totalCells, 'X');

                // 1. Grid'i normal doldur (Satýr satýr)
                char[,] grid = new char[rows, cols];
                int idx = 0;
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        grid[r, c] = text[idx++];

                // 2. Spiral Oku (Sað-Üstten baþla, Saat yönü)
                StringBuilder sb = new StringBuilder();
                int top = 0, bottom = rows - 1;
                int left = 0, right = cols - 1;

                while (top <= bottom && left <= right)
                {
                    // Aþaðý (Sað sütun)
                    for (int i = top; i <= bottom; i++) sb.Append(grid[i, right]);
                    right--;
                    if (left > right) break;

                    // Sola (Alt satýr)
                    for (int i = right; i >= left; i--) sb.Append(grid[bottom, i]);
                    bottom--;
                    if (top > bottom) break;

                    // Yukarý (Sol sütun)
                    for (int i = bottom; i >= top; i--) sb.Append(grid[i, left]);
                    left++;
                    if (left > right) break;

                    // Saða (Üst satýr)
                    for (int i = left; i <= right; i++) sb.Append(grid[top, i]);
                    top++;
                }
                return sb.ToString();
            }
        }

        private static class Columnar
        {
            public static string Encrypt(string text, string key)
            {
                int[] keyOrder = key.Select((c, i) => new { Char = c, Index = i })
                                    .OrderBy(x => x.Char).Select(x => x.Index).ToArray();
                int cols = key.Length;
                int rows = (int)Math.Ceiling((double)text.Length / cols);
                char[,] grid = new char[rows, cols];
                int idx = 0;

                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        grid[r, c] = (idx < text.Length) ? text[idx++] : 'X';

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < cols; i++)
                {
                    int k = Array.IndexOf(keyOrder, i);

                    // Doðru mantýk: Anahtarýn alfabetik sýrasýna göre o harfin olduðu sütunu bulup okumalýyýz
                    var sortedIndices = key.Select((c, ix) => new { c, ix }).OrderBy(x => x.c).Select(x => x.ix).ToArray();
                    int targetCol = sortedIndices[i];

                    for (int r = 0; r < rows; r++) sb.Append(grid[r, targetCol]);
                }
                return sb.ToString();
            }
        }

        // --- YENÝ HILL CIPHER (Encrypt - Matris Destekli) ---
        private static class Hill
        {
            public static string Encrypt(string text, string keyString)
            {
                int[,] matrix = ParseKey(keyString);
                CheckDeterminant(matrix);

                text = text.Replace(" ", "").ToUpper();
                if (text.Length % 2 != 0) text += "X";

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < text.Length; i += 2)
                {
                    int p1 = text[i] - 'A';
                    int p2 = text[i + 1] - 'A';
                    int c1 = (matrix[0, 0] * p1 + matrix[0, 1] * p2) % 26;
                    int c2 = (matrix[1, 0] * p1 + matrix[1, 1] * p2) % 26;
                    sb.Append((char)(c1 + 'A'));
                    sb.Append((char)(c2 + 'A'));
                }
                return sb.ToString();
            }

            private static int[,] ParseKey(string input)
            {
                int[,] m = new int[2, 2];
                input = input.Trim();
                // Sayýsal giriþ: "6 24 1 13"
                if (char.IsDigit(input[0]))
                {
                    string[] parts = input.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 4) throw new Exception("Hill Matrisi için 4 sayý gerekli.");
                    m[0, 0] = int.Parse(parts[0]); m[0, 1] = int.Parse(parts[1]);
                    m[1, 0] = int.Parse(parts[2]); m[1, 1] = int.Parse(parts[3]);
                }
                // Harf giriþ: "GYBN"
                else
                {
                    if (input.Length != 4) throw new Exception("Hill Key harf ise 4 karakter olmalý.");
                    input = input.ToUpper();
                    m[0, 0] = input[0] - 'A'; m[0, 1] = input[1] - 'A';
                    m[1, 0] = input[2] - 'A'; m[1, 1] = input[3] - 'A';
                }
                return m;
            }

            private static void CheckDeterminant(int[,] m)
            {
                int det = (m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0]) % 26;
                if (det < 0) det += 26;
                // EBOB(det, 26) == 1 olmalý. (26 = 2*13)
                if (det == 0 || det % 2 == 0 || det % 13 == 0)
                    throw new Exception($"Matris Determinantý ({det}) geçersiz! 26 ile aralarýnda asal olmalý.");
            }
        }
    }
}