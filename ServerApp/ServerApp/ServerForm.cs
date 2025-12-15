using System;
using System.IO;
using System.Linq;
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
            btnStopServer.Enabled = false;
            lblIPPort.Text = $"Server IP: {serverIP} | Port: {serverPort}";
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            serverIP = txtIP.Text.Trim();
            if (!int.TryParse(txtPort.Text.Trim(), out serverPort))
            {
                MessageBox.Show("Hatalý port!");
                return;
            }
            try
            {
                listener = new TcpListener(IPAddress.Any, serverPort);
                listener.Start();
                serverRunning = true;
                btnStartServer.Enabled = false;
                btnStopServer.Enabled = true;
                listBoxLog.Items.Add($"Server baþlatýldý: {serverPort}");
                new Thread(AcceptLoop) { IsBackground = true }.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server hatasý: " + ex.Message);
            }
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            serverRunning = false;
            try { listener?.Stop(); } catch { }
            btnStartServer.Enabled = true;
            btnStopServer.Enabled = false;
            listBoxLog.Items.Add("Server durduruldu.");
        }

        private void AcceptLoop()
        {
            try
            {
                while (serverRunning)
                {
                    if (listener.Pending())
                        ThreadPool.QueueUserWorkItem(HandleClient, listener.AcceptTcpClient());
                    else
                        Thread.Sleep(100);
                }
            }
            catch (SocketException) { }
            catch (Exception ex)
            {
                Invoke((Action)(() => listBoxLog.Items.Add("Loop Hatasý: " + ex.Message)));
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
                string first = reader.ReadLine();
                if (string.IsNullOrEmpty(first)) { client.Close(); return; }

                string[] fParts = first.Split('|');
                if (!(fParts.Length == 4 && fParts[1] == serverIP && fParts[3] == serverPort.ToString()))
                {
                    writer.WriteLine("DENIED"); client.Close(); return;
                }
                writer.WriteLine("OK");
                Invoke((Action)(() => listBoxLog.Items.Add("Yeni client.")));

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
                            Invoke((Action)(() => {
                                listBoxLog.Items.Add($"[{method}] Þifreli: {encrypted}");
                                listBoxLog.Items.Add($"Çözülmüþ: {decrypted}");
                            }));
                        }
                    }
                    else if (line.Contains("|")) // Dosya/Resim
                    {
                        string[] parts = line.Split('|', 3);
                        if (parts.Length >= 3)
                        {
                            string type = parts[0];
                            string fn = parts[1];
                            if (int.TryParse(parts[2], out int len))
                            {
                                byte[] buff = new byte[len];
                                int totalRead = 0;
                                while (totalRead < len)
                                {
                                    int read = stream.Read(buff, totalRead, len - totalRead);
                                    if (read == 0) break;
                                    totalRead += read;
                                }
                                string path = Path.Combine(Application.StartupPath, "Rec_" + fn);
                                File.WriteAllBytes(path, buff);
                                Invoke((Action)(() => listBoxLog.Items.Add($"{type} alýndý: {fn}")));
                            }
                        }
                    }
                }
            }
            catch { }
            finally { try { client.Close(); } catch { } }
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
                    "Rail Fence" => RailFenceDecrypt(text, int.Parse(key)),

                    // --- YENÝ EKLENENLER ---
                    "Route Cipher" => RouteCipher.Decrypt(text, int.Parse(key)),
                    "Columnar Transposition" => ColumnarLogic.Decrypt(text, key),
                    "Hill Cipher" => Hill.Decrypt(text, key),
                    // -----------------------

                    _ => "[Bilinmeyen Metod]"
                };
            }
            catch { return "[Decrypt Hatasý]"; }
        }

        // --- DECRYPT LOGIC ---

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
            string[] parts = key.Split(',');
            int a = int.Parse(parts[0]), b = int.Parse(parts[1]);
            int a_inv = -1;
            for (int i = 1; i < 26; i++) if ((a * i) % 26 == 1) { a_inv = i; break; }
            if (a_inv == -1) return "Hata";

            StringBuilder sb = new StringBuilder();
            foreach (char c in cipher.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    int dec = (a_inv * ((c - 'A' - b + 26) % 26)) % 26;
                    sb.Append((char)(dec + 'A'));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private string RailFenceDecrypt(string cipher, int rails)
        {
            if (rails < 2) return cipher;
            bool[][] mark = new bool[rails][];
            for (int i = 0; i < rails; i++) mark[i] = new bool[cipher.Length];
            int row = 0; bool down = false;
            for (int i = 0; i < cipher.Length; i++)
            {
                mark[row][i] = true;
                if (row == 0 || row == rails - 1) down = !down;
                row += down ? 1 : -1;
            }
            char[][] filled = new char[rails][];
            for (int i = 0; i < rails; i++) filled[i] = new char[cipher.Length];
            int idx = 0;
            for (int r = 0; r < rails; r++)
                for (int c = 0; c < cipher.Length; c++)
                    if (mark[r][c] && idx < cipher.Length) filled[r][c] = cipher[idx++];

            StringBuilder sb = new StringBuilder();
            row = 0; down = false;
            for (int i = 0; i < cipher.Length; i++)
            {
                sb.Append(filled[row][i]);
                if (row == 0 || row == rails - 1) down = !down;
                row += down ? 1 : -1;
            }
            return sb.ToString();
        }

        // --- YENÝ ROUTE CIPHER (Görseldeki Spiral - Decrypt) ---
        private static class RouteCipher
        {
            public static string Decrypt(string cipher, int cols)
            {
                int len = cipher.Length;
                int rows = len / cols;
                char[,] grid = new char[rows, cols];

                // Decrypt Mantýðý: Þifreli metni Spiral yola göre YERLEÞTÝR
                int idx = 0;
                int top = 0, bottom = rows - 1;
                int left = 0, right = cols - 1;

                while (top <= bottom && left <= right && idx < len)
                {
                    // Aþaðý
                    for (int i = top; i <= bottom; i++) grid[i, right] = cipher[idx++];
                    right--; if (left > right) break;
                    // Sola
                    for (int i = right; i >= left; i--) grid[bottom, i] = cipher[idx++];
                    bottom--; if (top > bottom) break;
                    // Yukarý
                    for (int i = bottom; i >= top; i--) grid[i, left] = cipher[idx++];
                    left++; if (left > right) break;
                    // Saða
                    for (int i = left; i <= right; i++) grid[top, i] = cipher[idx++];
                    top++;
                }

                // Normal Oku
                StringBuilder sb = new StringBuilder();
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        sb.Append(grid[r, c]);

                return sb.ToString().TrimEnd('X');
            }
        }

        private static class ColumnarLogic
        {
            public static string Decrypt(string cipher, string key)
            {
                int cols = key.Length;
                int rows = cipher.Length / cols;
                char[,] grid = new char[rows, cols];
                var sortedIndices = key.Select((c, ix) => new { c, ix }).OrderBy(x => x.c).Select(x => x.ix).ToArray();

                int idx = 0;
                for (int i = 0; i < cols; i++)
                {
                    int k = sortedIndices[i];
                    for (int r = 0; r < rows; r++)
                        if (idx < cipher.Length) grid[r, k] = cipher[idx++];
                }
                StringBuilder sb = new StringBuilder();
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++) sb.Append(grid[r, c]);
                return sb.ToString().TrimEnd('X');
            }
        }

        // --- YENÝ HILL CIPHER (Decrypt - Matris Destekli) ---
        private static class Hill
        {
            public static string Decrypt(string cipher, string keyString)
            {
                int[,] matrix = ParseKey(keyString);

                // Determinant ve Tersini bul
                int det = (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) % 26;
                if (det < 0) det += 26;

                int detInv = -1;
                for (int i = 0; i < 26; i++) if ((det * i) % 26 == 1) { detInv = i; break; }
                if (detInv == -1) return "[Hata: Matrisin tersi yok!]";

                // Ters Matris (Inverse)
                int[,] invMatrix = new int[2, 2];
                invMatrix[0, 0] = (matrix[1, 1] * detInv) % 26;
                invMatrix[0, 1] = (-matrix[0, 1] * detInv) % 26;
                invMatrix[1, 0] = (-matrix[1, 0] * detInv) % 26;
                invMatrix[1, 1] = (matrix[0, 0] * detInv) % 26;

                for (int r = 0; r < 2; r++)
                    for (int c = 0; c < 2; c++) if (invMatrix[r, c] < 0) invMatrix[r, c] += 26;

                // Çözme Ýþlemi: P = K^-1 * C
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < cipher.Length; i += 2)
                {
                    int c1 = cipher[i] - 'A';
                    int c2 = cipher[i + 1] - 'A';
                    int p1 = (invMatrix[0, 0] * c1 + invMatrix[0, 1] * c2) % 26;
                    int p2 = (invMatrix[1, 0] * c1 + invMatrix[1, 1] * c2) % 26;
                    sb.Append((char)(p1 + 'A'));
                    sb.Append((char)(p2 + 'A'));
                }
                return sb.ToString().TrimEnd('X');
            }

            private static int[,] ParseKey(string input)
            {
                int[,] m = new int[2, 2];
                input = input.Trim();
                if (char.IsDigit(input[0]))
                {
                    string[] parts = input.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    m[0, 0] = int.Parse(parts[0]); m[0, 1] = int.Parse(parts[1]);
                    m[1, 0] = int.Parse(parts[2]); m[1, 1] = int.Parse(parts[3]);
                }
                else
                {
                    input = input.ToUpper();
                    m[0, 0] = input[0] - 'A'; m[0, 1] = input[1] - 'A';
                    m[1, 0] = input[2] - 'A'; m[1, 1] = input[3] - 'A';
                }
                return m;
            }
        }
    }
}