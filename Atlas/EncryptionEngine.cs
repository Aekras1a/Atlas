using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace Atlas
{
    internal class EncryptionEngine
    {
        private String Password = null;

        RijndaelManaged AES = new RijndaelManaged();

        public EncryptionEngine(String pPassword)
        {
            Password = pPassword;

            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;
        }
        public static byte[] GenerateRandomSalt()
        {
            byte[] Data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    rng.GetBytes(Data);
                }
            }
            return Data;
        }

        public void Encrypt(string pInputFile)
        {
            byte[] Salt = GenerateRandomSalt();
            FileStream fsCrypt = new FileStream(pInputFile + ".backup", FileMode.Create);
            byte[] PasswordBytes = System.Text.Encoding.UTF8.GetBytes(Password);

            var Key = new Rfc2898DeriveBytes(PasswordBytes, Salt, 50000);
            AES.Key = Key.GetBytes(AES.KeySize / 8);
            AES.IV = Key.GetBytes(AES.BlockSize / 8);

            fsCrypt.Write(Salt, 0, Salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

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
                Debug.WriteLine("\n\n[-] Error: " + ex.Message);
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
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(Password);
            byte[] salt = new byte[32];

            FileStream fsCrypt = new FileStream(pInputFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            String Output_Path = Path.Combine(Path.GetDirectoryName(pInputFile), Path.ChangeExtension(pInputFile, "zip"));
            FileStream fsOut = new FileStream(
                Output_Path,
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
                Debug.WriteLine("\n\n[-] CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\n\n[-] Error: " + ex.Message);
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[-] Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }
        }
    }
}
