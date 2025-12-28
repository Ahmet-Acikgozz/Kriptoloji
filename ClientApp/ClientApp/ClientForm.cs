using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using CryptoLib.AES;
using CryptoLib.DES;
using CryptoLib.RsaCrypto;
using CryptoLib.HillCipher;
using CryptoLib.ECC;

namespace ClientApp
{
    public partial class ClientForm : Form
    {
        private TcpClient? client;
        private NetworkStream? stream;
        private StreamWriter? writer;
        private StreamReader? reader;

        private string? serverPublicKeyXml;

        private byte[]? currentAesKey;
        private byte[]? currentDesKey;

        private bool useManualCrypto = false;

        private EccLib? clientEcc;
        private string? serverEccPublicKey;
        private string? serverEccManualPublicKey;

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
                "Route Cipher",
                "Columnar Transposition",
                "Hill Cipher 2x2",
                "Hill Cipher 3x3",
                "Hill Cipher 4x4",
                "AES-128",
                "DES"
            });
            cmbEncryptionMethod.SelectedIndex = 0;
            cmbEncryptionMethod.SelectedIndexChanged += CmbEncryptionMethod_SelectedIndexChanged;

            cmbKeyExchange.Items.Clear();
            cmbKeyExchange.Items.AddRange(new object[] {
                "RSA",
                "ECC"
            });
            cmbKeyExchange.SelectedIndex = 0;

            chkManualMode.CheckedChanged += (s, e) => useManualCrypto = chkManualMode.Checked;
            btnEncryptFile.Click += BtnEncryptFile_Click;
        }

        private void AddManualModeCheckbox()
        {
            if (this.Controls.Find("chkManualMode", false).Length == 0)
            {
                var chk = new CheckBox
                {
                    Name = "chkManualMode",
                    Text = "Kutuphanesiz (Manuel)",
                    Location = new System.Drawing.Point(12, 410),
                    Size = new System.Drawing.Size(150, 23),
                    Checked = false
                };
                chk.CheckedChanged += (s, e) => useManualCrypto = chk.Checked;
                this.Controls.Add(chk);
            }
        }

        private void AddFileEncryptButton()
        {
            if (this.Controls.Find("btnEncryptFile", false).Length == 0)
            {
                var btn = new Button
                {
                    Name = "btnEncryptFile",
                    Text = "Dosya Sifrele",
                    Location = new System.Drawing.Point(226, 146),
                    Size = new System.Drawing.Size(95, 23)
                };
                btn.Click += BtnEncryptFile_Click;
                this.Controls.Add(btn);
            }
        }

        private void CmbEncryptionMethod_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string method = cmbEncryptionMethod.SelectedItem?.ToString() ?? "";

            bool isSymmetric = method == "AES-128" || method == "DES";
            cmbKeyExchange.Enabled = isSymmetric;
            lblKeyExchange.Enabled = isSymmetric;

            if (method == "AES-128")
            {
                if (currentAesKey == null)
                    currentAesKey = AesLib.GenerateKey();
                txtKey.Text = Convert.ToBase64String(currentAesKey);
            }
            else if (method == "DES")
            {
                if (currentDesKey == null)
                    currentDesKey = DesLib.GenerateKey();
                txtKey.Text = Convert.ToBase64String(currentDesKey);
            }
            else if (method.Contains("Hill Cipher"))
            {
                if (method.Contains("2x2"))
                    txtKey.Text = "3 5 6 7";
                else if (method.Contains("3x3"))
                    txtKey.Text = "17 17 5 21 18 21 2 2 19";
                else if (method.Contains("4x4"))
                    txtKey.Text = "7 11 4 5 3 2 9 13 1 8 6 12 10 15 14 16";
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = txtServerIP.Text.Trim();
            if (!int.TryParse(txtServerPort.Text.Trim(), out int port))
            {
                MessageBox.Show("Gecerli port girin.");
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

                string? resp = reader.ReadLine();
                if (resp == "OK")
                {
                    listBoxStatus.Items.Add($"Baglandi ve dogrulandi: {ip}:{port}");

                    string? rsaKey = reader.ReadLine();
                    if (rsaKey != null && rsaKey.StartsWith("RSAPUBKEY|"))
                    {
                        serverPublicKeyXml = rsaKey.Substring("RSAPUBKEY|".Length);
                        listBoxStatus.Items.Add("RSA Public Key alindi.");

                        currentAesKey = AesLib.GenerateKey();
                        currentDesKey = DesLib.GenerateKey();

                        using var rsa = RsaLib.CreateFromPublicKeyXml(serverPublicKeyXml);
                        string encAes = rsa.EncryptSymmetricKey(currentAesKey);
                        string encDes = rsa.EncryptSymmetricKey(currentDesKey);

                        writer.WriteLine($"AESKEY|{encAes}");
                        writer.WriteLine($"DESKEY|{encDes}");
                        listBoxStatus.Items.Add("AES ve DES anahtarlari RSA ile sifrelenerek gonderildi.");
                    }

                    string? eccKey = reader.ReadLine();
                    if (eccKey != null && eccKey.StartsWith("ECCPUBKEY|"))
                    {
                        serverEccPublicKey = eccKey.Substring("ECCPUBKEY|".Length);
                        clientEcc = new EccLib();
                        writer.WriteLine($"ECCPUBKEY|{clientEcc.PublicKey}");
                        listBoxStatus.Items.Add("ECC Public Key alindi ve gonderildi.");
                    }
                    
                    string? eccManualKey = reader.ReadLine();
                    if (eccManualKey != null && eccManualKey.StartsWith("ECCMANUALPUBKEY|"))
                    {
                        serverEccManualPublicKey = eccManualKey.Substring("ECCMANUALPUBKEY|".Length);
                        listBoxStatus.Items.Add("Manuel ECC Public Key alindi.");
                    }

                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = true;
                }
                else
                {
                    listBoxStatus.Items.Add("Server dogrulama hatasi.");
                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add("Baglanti hatasi: " + ex.Message);
                CloseConnection();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            CloseConnection();
            listBoxStatus.Items.Add("Baglanti kapatildi.");
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (!EnsureConnected()) return;

            string method = cmbEncryptionMethod.SelectedItem?.ToString() ?? "";
            string keyExchange = cmbKeyExchange.SelectedItem?.ToString() ?? "Yok";
            string key = txtKey.Text.Trim();
            string plain = txtMessage.Text;

            if (string.IsNullOrEmpty(method))
            {
                MessageBox.Show("Sifreleme yontemi secin.");
                return;
            }

            bool isSymmetric = method == "AES-128" || method == "DES";
            if (!isSymmetric && string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Key alanini doldurun.");
                return;
            }

            string encrypted;
            try
            {
                encrypted = EncryptMessage(plain, method, key);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sifreleme hatasi: " + ex.Message);
                return;
            }

            string modeTag = useManualCrypto ? "MANUAL" : "LIB";
            string keyExchangeTag = isSymmetric ? keyExchange : "Yok";
            string header = $"TEXT|{method}|{keyExchangeTag}|{modeTag}|{key}|{encrypted}";
            try
            {
                writer!.WriteLine(header);
                string displayInfo = isSymmetric ? $"{method}+{keyExchange}/{modeTag}" : $"{method}/{modeTag}";
                listBoxStatus.Items.Add($"Gonderildi ({displayInfo}): {encrypted.Substring(0, Math.Min(50, encrypted.Length))}...");
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add("Gonderme hatasi: " + ex.Message);
            }
        }

        private void BtnEncryptFile_Click(object? sender, EventArgs e)
        {
            if (!EnsureConnected()) return;

            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files|*.txt|All Files|*.*";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            string filePath = ofd.FileName;
            string fileContent;
            try
            {
                fileContent = File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dosya okuma hatasi: " + ex.Message);
                return;
            }

            string method = cmbEncryptionMethod.SelectedItem?.ToString() ?? "";
            string key = txtKey.Text.Trim();

            if (string.IsNullOrEmpty(method))
            {
                MessageBox.Show("Sifreleme yontemi secin.");
                return;
            }

            string encrypted;
            try
            {
                encrypted = EncryptMessage(fileContent, method, key);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sifreleme hatasi: " + ex.Message);
                return;
            }

            string encryptedFilePath = filePath + ".encrypted";
            try
            {
                File.WriteAllText(encryptedFilePath, encrypted, Encoding.UTF8);
                listBoxStatus.Items.Add($"Sifreli dosya kaydedildi: {Path.GetFileName(encryptedFilePath)}");
                
                MessageBox.Show(
                    $"Sifreleme Dogrulama:\n\n" +
                    $"Orijinal dosya: {Path.GetFileName(filePath)}\n" +
                    $"Orijinal boyut: {fileContent.Length} karakter\n\n" +
                    $"Sifreli dosya: {Path.GetFileName(encryptedFilePath)}\n" +
                    $"Sifreli boyut: {encrypted.Length} karakter\n\n" +
                    $"Yontem: {method}\n" +
                    $"Mod: {(useManualCrypto ? "Manuel" : "Kutuphane")}\n\n" +
                    $"Sifreli dosya konumu:\n{encryptedFilePath}",
                    "Sifreleme Basarili",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add($"Sifreli dosya kaydetilemedi: {ex.Message}");
            }

            string filename = Path.GetFileName(filePath);
            string modeTag = useManualCrypto ? "MANUAL" : "LIB";
            string keyExchange = cmbKeyExchange.SelectedItem?.ToString() ?? "Yok";
            bool isSymmetric = method == "AES-128" || method == "DES";
            string keyExchangeTag = isSymmetric ? keyExchange : "Yok";

            string header = $"ENCFILE|{method}|{keyExchangeTag}|{modeTag}|{key}|{filename}|{encrypted}";
            try
            {
                writer!.WriteLine(header);
                string displayInfo = isSymmetric ? $"{method}+{keyExchange}/{modeTag}" : $"{method}/{modeTag}";
                listBoxStatus.Items.Add($"Dosya sifrelenerek gonderildi: {filename} ({displayInfo})");
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add("Dosya gonderme hatasi: " + ex.Message);
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
                writer!.WriteLine($"{type}|{filename}|{data.Length}");
                stream!.Write(data, 0, data.Length);
                listBoxStatus.Items.Add($"{type} gonderildi: {filename}");
            }
            catch (Exception ex)
            {
                listBoxStatus.Items.Add("Dosya hatasi: " + ex.Message);
            }
        }

        private bool EnsureConnected()
        {
            if (client == null || !client.Connected)
            {
                MessageBox.Show("Once baglanin.");
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
            string keyExchange = cmbKeyExchange.SelectedItem?.ToString() ?? "RSA";

            switch (method)
            {
                case "Caesar Cipher":
                    if (!int.TryParse(key, out int shift)) throw new Exception("Caesar key sayi olmali.");
                    return Caesar.Encrypt(text, shift);

                case "Vigenere Cipher":
                    return Vigenere.Encrypt(text, key);

                case "Substitution Cipher":
                    if (key.Length != 26) throw new Exception("Substitution key 26 harf olmali.");
                    return Substitution.Encrypt(text, key);

                case "Affine Cipher":
                    string[] parts = key.Split(',');
                    if (parts.Length != 2) throw new Exception("Affine key: a,b olmali.");
                    return Affine.Encrypt(text, int.Parse(parts[0]), int.Parse(parts[1]));

                case "Rail Fence":
                    if (!int.TryParse(key, out int rails)) throw new Exception("Rail Fence key sayi olmali.");
                    return RailFence.Encrypt(text, rails);

                case "Route Cipher":
                    if (!int.TryParse(key, out int cols)) throw new Exception("Route Cipher key sutun sayisi olmali.");
                    return RouteCipher.Encrypt(text, cols);

                case "Columnar Transposition":
                    return Columnar.Encrypt(text, key);

                case "Hill Cipher 2x2":
                    return HillNxN.Encrypt2x2(text, key);

                case "Hill Cipher 3x3":
                    return HillNxN.Encrypt3x3(text, key);

                case "Hill Cipher 4x4":
                    return HillNxN.Encrypt4x4(text, key);

                case "AES-128":
                    if (currentAesKey == null)
                    {
                        currentAesKey = Convert.FromBase64String(key);
                    }
                    if (keyExchange == "ECC")
                    {
                        if (serverEccPublicKey == null)
                            throw new Exception("Servera baglanin, ECC key alin.");
                        if (useManualCrypto)
                        {
                            if (serverEccManualPublicKey == null)
                                throw new Exception("Server manuel ECC public key bulunamadi.");
                            return EccManual.Encrypt(text, serverEccManualPublicKey);
                        }
                        else
                        {
                            var (encMsg, senderPubKey) = EccHybridCrypto.EncryptWithKeyExchange(text, serverEccPublicKey);
                            return $"{senderPubKey}|{encMsg}";
                        }
                    }
                    else
                    {
                        if (useManualCrypto)
                            return AesManual.Encrypt(text, currentAesKey);
                        else
                            return AesLib.Encrypt(text, currentAesKey);
                    }

                case "DES":
                    if (currentDesKey == null)
                    {
                        currentDesKey = Convert.FromBase64String(key);
                    }
                    if (keyExchange == "ECC")
                    {
                        if (serverEccPublicKey == null)
                            throw new Exception("Servera baglanin, ECC key alin.");
                        if (useManualCrypto)
                        {
                            if (serverEccManualPublicKey == null)
                                throw new Exception("Server manuel ECC public key bulunamadi.");
                            return EccManual.Encrypt(text, serverEccManualPublicKey);
                        }
                        else
                        {
                            var (encMsg, senderPubKey) = EccHybridCrypto.EncryptWithKeyExchange(text, serverEccPublicKey);
                            return $"{senderPubKey}|{encMsg}";
                        }
                    }
                    else
                    {
                        if (useManualCrypto)
                            return DesManual.Encrypt(text, currentDesKey);
                        else
                            return DesLib.Encrypt(text, currentDesKey);
                    }

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

        private static class RouteCipher
        {
            public static string Encrypt(string text, int cols)
            {
                text = text.Replace(" ", "").ToUpper();
                int len = text.Length;
                int rows = (int)Math.Ceiling((double)len / cols);
                int totalCells = rows * cols;
                text = text.PadRight(totalCells, 'X');

                char[,] grid = new char[rows, cols];
                int idx = 0;
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        grid[r, c] = text[idx++];

                StringBuilder sb = new StringBuilder();
                int top = 0, bottom = rows - 1;
                int left = 0, right = cols - 1;

                while (top <= bottom && left <= right)
                {
                    for (int i = top; i <= bottom; i++) sb.Append(grid[i, right]);
                    right--;
                    if (left > right) break;

                    for (int i = right; i >= left; i--) sb.Append(grid[bottom, i]);
                    bottom--;
                    if (top > bottom) break;

                    for (int i = bottom; i >= top; i--) sb.Append(grid[i, left]);
                    left++;
                    if (left > right) break;

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
                    var sortedIndices = key.Select((c, ix) => new { c, ix }).OrderBy(x => x.c).Select(x => x.ix).ToArray();
                    int targetCol = sortedIndices[i];

                    for (int r = 0; r < rows; r++) sb.Append(grid[r, targetCol]);
                }
                return sb.ToString();
            }
        }

        
    }
}
