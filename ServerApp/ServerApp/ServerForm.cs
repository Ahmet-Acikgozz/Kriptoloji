using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CryptoLib.AES;
using CryptoLib.DES;
using CryptoLib.RsaCrypto;
using CryptoLib.HillCipher;
using CryptoLib.ECC;

namespace ServerApp
{
    public partial class ServerForm : Form
    {
        private TcpListener? listener;
        private bool serverRunning = false;
        private int serverPort = 9000;
        private string serverIP = "127.0.0.1";

        private RsaLib? rsaServer;

        private byte[]? clientAesKey;
        private byte[]? clientDesKey;

        private EccLib? eccServer;
        private EccManual? eccManualServer;

        public ServerForm()
        {
            InitializeComponent();
            btnStopServer.Enabled = false;
            lblIPPort.Text = $"Server IP: {serverIP} | Port: {serverPort}";

            rsaServer = new RsaLib(2048);

            eccServer = new EccLib();
            eccManualServer = new EccManual();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            serverIP = txtIP.Text.Trim();
            if (!int.TryParse(txtPort.Text.Trim(), out serverPort))
            {
                MessageBox.Show("Hatali port!");
                return;
            }
            try
            {
                listener = new TcpListener(IPAddress.Any, serverPort);
                listener.Start();
                serverRunning = true;
                btnStartServer.Enabled = false;
                btnStopServer.Enabled = true;
                listBoxLog.Items.Add($"Server baslatildi: {serverPort}");
                listBoxLog.Items.Add("RSA Key Pair olusturuldu.");
                new Thread(AcceptLoop) { IsBackground = true }.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server hatasi: " + ex.Message);
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
                    if (listener!.Pending())
                        ThreadPool.QueueUserWorkItem(HandleClient, listener.AcceptTcpClient());
                    else
                        Thread.Sleep(100);
                }
            }
            catch (SocketException) { }
            catch (Exception ex)
            {
                Invoke((Action)(() => listBoxLog.Items.Add("Loop Hatasi: " + ex.Message)));
            }
        }

        private void HandleClient(object? obj)
        {
            TcpClient client = obj as TcpClient ?? throw new ArgumentNullException();
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            byte[]? thisClientAesKey = null;
            byte[]? thisClientDesKey = null;
            string? thisClientEccPublicKey = null;

            try
            {
                string? first = reader.ReadLine();
                if (string.IsNullOrEmpty(first)) { client.Close(); return; }

                string[] fParts = first.Split('|');
                if (!(fParts.Length == 4 && fParts[1] == serverIP && fParts[3] == serverPort.ToString()))
                {
                    writer.WriteLine("DENIED"); client.Close(); return;
                }
                writer.WriteLine("OK");
                Invoke((Action)(() => listBoxLog.Items.Add("Yeni client baglandi.")));

                writer.WriteLine($"RSAPUBKEY|{rsaServer!.GetPublicKeyXml()}");
                Invoke((Action)(() => listBoxLog.Items.Add("RSA Public Key gonderildi.")));

                writer.WriteLine($"ECCPUBKEY|{eccServer!.PublicKey}");
                writer.WriteLine($"ECCMANUALPUBKEY|{eccManualServer!.GetPublicKeyBase64()}");
                Invoke((Action)(() => listBoxLog.Items.Add("ECC Public Key gonderildi (Lib + Manual).")));

                string? line;
                while ((line = reader.ReadLine()) != null && serverRunning)
                {
                    if (line.StartsWith("AESKEY|"))
                    {
                        string encKey = line.Substring("AESKEY|".Length);
                        thisClientAesKey = rsaServer.DecryptSymmetricKey(encKey);
                        clientAesKey = thisClientAesKey;
                        Invoke((Action)(() => listBoxLog.Items.Add("AES anahtari RSA ile cozuldu ve alindi.")));
                        continue;
                    }

                    if (line.StartsWith("DESKEY|"))
                    {
                        string encKey = line.Substring("DESKEY|".Length);
                        thisClientDesKey = rsaServer.DecryptSymmetricKey(encKey);
                        clientDesKey = thisClientDesKey;
                        Invoke((Action)(() => listBoxLog.Items.Add("DES anahtari RSA ile cozuldu ve alindi.")));
                        continue;
                    }

                    if (line.StartsWith("ECCPUBKEY|"))
                    {
                        thisClientEccPublicKey = line.Substring("ECCPUBKEY|".Length);
                        Invoke((Action)(() => listBoxLog.Items.Add("Client ECC Public Key alindi.")));
                        continue;
                    }

                    if (line.StartsWith("TEXT|"))
                    {
                        string[] parts = line.Split('|', 5);
                        if (parts.Length >= 5)
                        {
                            string method = parts[1];
                            string modeTag = parts[2];
                            string key = parts[3];
                            string encrypted = parts[4];
                            bool useManual = modeTag == "MANUAL";
                            string decrypted = DecryptMessage(encrypted, method, key, useManual, thisClientAesKey, thisClientDesKey, thisClientEccPublicKey);
                            Invoke((Action)(() => {
                                listBoxLog.Items.Add($"[{method}/{modeTag}] Sifreli: {encrypted.Substring(0, Math.Min(40, encrypted.Length))}...");
                                listBoxLog.Items.Add($"Cozulmus: {decrypted}");
                            }));
                        }
                    }
                    else if (line.StartsWith("ENCFILE|"))
                    {
                        string[] parts = line.Split('|', 6);
                        if (parts.Length >= 6)
                        {
                            string method = parts[1];
                            string modeTag = parts[2];
                            string key = parts[3];
                            string filename = parts[4];
                            string encrypted = parts[5];
                            bool useManual = modeTag == "MANUAL";
                            string decrypted = DecryptMessage(encrypted, method, key, useManual, thisClientAesKey, thisClientDesKey, thisClientEccPublicKey);

                            string encryptedSavePath = Path.Combine(Application.StartupPath, filename + ".encrypted");
                            File.WriteAllText(encryptedSavePath, encrypted, Encoding.UTF8);

                            string decryptedSavePath = Path.Combine(Application.StartupPath, "Decrypted_" + filename);
                            File.WriteAllText(decryptedSavePath, decrypted, Encoding.UTF8);

                            Invoke((Action)(() => {
                                listBoxLog.Items.Add($"=== Dosya Alindi: {filename} ({method}/{modeTag}) ===");
                                listBoxLog.Items.Add($"  Sifreli   -> {Path.GetFileName(encryptedSavePath)} ({encrypted.Length} karakter)");
                                listBoxLog.Items.Add($"  Cozulmus  -> {Path.GetFileName(decryptedSavePath)} ({decrypted.Length} karakter)");
                                listBoxLog.Items.Add($"  Konum: {Application.StartupPath}");
                            }));
                        }
                    }
                    else if (line.Contains("|"))
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
                                Invoke((Action)(() => listBoxLog.Items.Add($"{type} alindi: {fn}")));
                            }
                        }
                    }
                }
            }
            catch { }
            finally { try { client.Close(); } catch { } }
        }

        private string DecryptMessage(string text, string method, string key, bool useManual, byte[]? aesKey, byte[]? desKey, string? eccClientPublicKey)
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
                    "Route Cipher" => RouteCipherDecrypt(text, int.Parse(key)),
                    "Columnar Transposition" => ColumnarDecrypt(text, key),
                    "Hill Cipher 2x2" => HillNxN.Decrypt2x2(text, key),
                    "Hill Cipher 3x3" => HillNxN.Decrypt3x3(text, key),
                    "Hill Cipher 4x4" => HillNxN.Decrypt4x4(text, key),
                    "AES-128" => DecryptAes(text, aesKey, useManual),
                    "DES" => DecryptDes(text, desKey, useManual),
                    "RSA (Key Exchange)" => rsaServer!.DecryptString(text),
                    "ECC (Key Exchange)" => DecryptEcc(text, eccClientPublicKey, useManual),
                    _ => "[Bilinmeyen Metod]"
                };
            }
            catch (Exception ex) { return $"[Decrypt Hatasi: {ex.Message}]"; }
        }

        private string DecryptAes(string cipher, byte[]? aesKey, bool useManual)
        {
            if (aesKey == null) return "[AES Key yok!]";
            if (useManual)
                return AesManual.Decrypt(cipher, aesKey);
            else
                return AesLib.Decrypt(cipher, aesKey);
        }

        private string DecryptDes(string cipher, byte[]? desKey, bool useManual)
        {
            if (desKey == null) return "[DES Key yok!]";
            if (useManual)
                return DesManual.Decrypt(cipher, desKey);
            else
                return DesLib.Decrypt(cipher, desKey);
        }

        private string DecryptEcc(string cipher, string? clientEccPublicKey, bool useManual)
        {
            if (eccServer == null) return "[ECC Key yok!]";
            if (string.IsNullOrEmpty(clientEccPublicKey)) return "[Client ECC Key yok!]";
            
            if (useManual)
            {
                if (eccManualServer == null) return "[Manuel ECC Key yok!]";
                
                try
                {
                    return eccManualServer.Decrypt(cipher);
                }
                catch (Exception ex)
                {
                    return $"[Manuel ECC Decrypt Hatasi: {ex.Message}]";
                }
            }
            else
            {
                string[] parts = cipher.Split('|', 2);
                if (parts.Length != 2) return "[Gecersiz ECC format!]";
                string senderPubKey = parts[0];
                string encryptedMessage = parts[1];
                return EccHybridCrypto.DecryptWithKeyExchange(encryptedMessage, senderPubKey, eccServer);
            }
        }


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

        private string RouteCipherDecrypt(string cipher, int cols)
        {
            int len = cipher.Length;
            int rows = len / cols;
            char[,] grid = new char[rows, cols];

            int idx = 0;
            int top = 0, bottom = rows - 1;
            int left = 0, right = cols - 1;

            while (top <= bottom && left <= right && idx < len)
            {
                for (int i = top; i <= bottom && idx < len; i++) grid[i, right] = cipher[idx++];
                right--; if (left > right) break;
                for (int i = right; i >= left && idx < len; i--) grid[bottom, i] = cipher[idx++];
                bottom--; if (top > bottom) break;
                for (int i = bottom; i >= top && idx < len; i--) grid[i, left] = cipher[idx++];
                left++; if (left > right) break;
                for (int i = left; i <= right && idx < len; i++) grid[top, i] = cipher[idx++];
                top++;
            }

            StringBuilder sb = new StringBuilder();
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    sb.Append(grid[r, c]);

            return sb.ToString().TrimEnd('X');
        }

        private string ColumnarDecrypt(string cipher, string key)
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
}
