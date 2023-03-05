using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace Atlas
{
    internal class EncryptionEngine
    {
        private RijndaelManaged aes = new RijndaelManaged();
        private String password = null;

        public EncryptionEngine(String pPassword)
        {
            password = pPassword;

            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CFB;
        }

        public static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    rng.GetBytes(data);
                }
            }
            return data;
        }

        public void Encrypt(string pInputFile)
        {
            Debug.WriteLine($"[*] ({this.GetType().Name}) Encrypting {pInputFile}");

            byte[] salt = GenerateRandomSalt();
            FileStream fsCrypt = new FileStream(pInputFile + ".backup", FileMode.Create);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            fsCrypt.Write(salt, 0, salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(pInputFile, FileMode.Open);

            byte[] buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cs.Write(buffer, 0, read);
                }

                fsIn.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[-] ({this.GetType().Name}) Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }
            File.Delete(pInputFile);
        }

        public void Decrypt(string pInputFile)
        {
            Debug.WriteLine($"[*] ({this.GetType().Name}) Decrypting {pInputFile}");

            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            FileStream fsCrypt = new FileStream(pInputFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read);

            String outputPath = Path.Combine(Path.GetDirectoryName(pInputFile), Path.ChangeExtension(pInputFile, "zip"));
            FileStream fsOut = new FileStream(
                outputPath,
                FileMode.Create
            );

            int read;
            byte[] buffer = new byte[1048576];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fsOut.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Debug.WriteLine($"[-] ({this.GetType().Name}) CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[-] ({this.GetType().Name}) Error: " + ex.Message);
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[-] ({this.GetType().Name}) Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }
        }
    }
}