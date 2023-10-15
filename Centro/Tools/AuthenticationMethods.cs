using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Centro
{
    class Authentication
    {

        private const string usr = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";

        private const string ans = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";

        public static bool LocalLogin(string user, string password)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] byteArrayUser = Encoding.ASCII.GetBytes(user);
                MemoryStream streamUser = new MemoryStream(byteArrayUser);

                byte[] hashValueUser = mySHA256.ComputeHash(streamUser);

                string hashTextUser = Encryption.printByteArray(hashValueUser);

                byte[] byteArrayPasswd = Encoding.ASCII.GetBytes(password);
                MemoryStream stream = new MemoryStream(byteArrayPasswd);

                byte[] hashValuePasswd = mySHA256.ComputeHash(stream);

                string hashTextPasswd = Encryption.printByteArray(hashValuePasswd);

                if (hashTextUser.Trim().ToUpper() == usr.ToUpper())
                {
                    if (hashTextPasswd.Trim().ToUpper() == ans.ToUpper())
                    {
                        return true;
                    }
                }

            }
            return false;
        }
    }

    class Encryption
    {

        private const string Key = "33rthhd35ahrhb1a6dca4e41";

        public static string EncryptString(string text)
        {
            byte[] iv = new byte[16];
            byte[] result;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(text);
                        }

                        result = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(result);
        }

        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
