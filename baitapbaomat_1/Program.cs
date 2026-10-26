using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("================ BÀI 1: MÃ HÓA AES ================");
            Console.WriteLine("1. Sinh khóa & IV, lưu ra file");
            Console.WriteLine("2. Mã hóa & Giải mã đoạn văn bản");
            Console.WriteLine("3. Mã hóa & Giải mã file lớn (Đo thời gian & So sánh byte)");
            Console.WriteLine("0. Thoát");
            Console.Write("Chọn chức năng (0-3): ");

            string choice = Console.ReadLine();
            if (choice == "0") break;

            switch (choice)
            {
                case "1": GenerateAndSaveKeyAes(); break;
                case "2": EncryptDecryptTextAes(); break;
                case "3": EncryptDecryptLargeFileAes(); break;
                default: Console.WriteLine("Lựa chọn không hợp lệ!"); break;
            }
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
    }

    static void GenerateAndSaveKeyAes()
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();
            File.WriteAllBytes("aes_key.bin", aes.Key);
            File.WriteAllBytes("aes_iv.bin", aes.IV);
        }
        Console.WriteLine("[Thành công] Đã sinh khóa AES-256 và lưu file 'aes_key.bin', 'aes_iv.bin'!");
    }

    static void EncryptDecryptTextAes()
    {
        Console.Write("Nhập đoạn văn bản cần mã hóa: ");
        string originalText = Console.ReadLine();
        if (!File.Exists("aes_key.bin")) GenerateAndSaveKeyAes();

        byte[] key = File.ReadAllBytes("aes_key.bin");
        byte[] iv = File.ReadAllBytes("aes_iv.bin");
        byte[] encryptedData;

        using (Aes aes = Aes.Create())
        {
            aes.Key = key; aes.IV = iv;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(originalText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                }
                encryptedData = ms.ToArray();
            }
        }
        Console.WriteLine($"-> Chuỗi mã hóa (Base64): {Convert.ToBase64String(encryptedData)}");
    }

    static void EncryptDecryptLargeFileAes()
    {
        string inputPath = "source_file.dat";
        string encryptedPath = "encrypted_file.enc";
        string decryptedPath = "decrypted_file.dat";

        Console.WriteLine("Đang tạo file mẫu 20MB để thử nghiệm...");
        using (var fs = new FileStream(inputPath, FileMode.Create))
        {
            byte[] buffer = new byte[1024 * 1024];
            new Random().NextBytes(buffer);
            for (int i = 0; i < 20; i++) fs.Write(buffer, 0, buffer.Length);
        }

        if (!File.Exists("aes_key.bin")) GenerateAndSaveKeyAes();
        byte[] key = File.ReadAllBytes("aes_key.bin");
        byte[] iv = File.ReadAllBytes("aes_iv.bin");

        // Đo thời gian mã hóa
        Stopwatch sw = Stopwatch.StartNew();
        using (Aes aes = Aes.Create())
        {
            aes.Key = key; aes.IV = iv;
            using (FileStream fsIn = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (FileStream fsEnc = new FileStream(encryptedPath, FileMode.Create, FileAccess.Write))
            using (CryptoStream cs = new CryptoStream(fsEnc, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                fsIn.CopyTo(cs);
            }
        }
        sw.Stop();
        Console.WriteLine($"[Đo lường] Thời gian mã hóa 20MB: {sw.ElapsedMilliseconds} ms");

        // Đo thời gian giải mã
        sw.Restart();
        using (Aes aes = Aes.Create())
        {
            aes.Key = key; aes.IV = iv;
            using (FileStream fsEnc = new FileStream(encryptedPath, FileMode.Open, FileAccess.Read))
            using (FileStream fsDec = new FileStream(decryptedPath, FileMode.Create, FileAccess.Write))
            using (CryptoStream cs = new CryptoStream(fsEnc, aes.CreateDecryptor(), CryptoStreamMode.Read))
            {
                cs.CopyTo(fsDec);
            }
        }
        sw.Stop();
        Console.WriteLine($"[Đo lường] Thời gian giải mã 20MB: {sw.ElapsedMilliseconds} ms");

        // So sánh bytes
        bool isIdentical = CompareFiles(inputPath, decryptedPath);
        Console.WriteLine($"[Kết quả] So sánh bytes nguồn và giải mã: {(isIdentical ? "Khớp hoàn toàn 100%!" : "Có sai lệch!")}");
    }

    static bool CompareFiles(string f1, string f2)
    {
        FileInfo fi1 = new FileInfo(f1);
        FileInfo fi2 = new FileInfo(f2);
        if (fi1.Length != fi2.Length) return false;
        using (FileStream s1 = new FileStream(f1, FileMode.Open), s2 = new FileStream(f2, FileMode.Open))
        {
            int b1, b2;
            while ((b1 = s1.ReadByte()) != -1)
            {
                b2 = s2.ReadByte();
                if (b1 != b2) return false;
            }
        }
        return true;
    }
}