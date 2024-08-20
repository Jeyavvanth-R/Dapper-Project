using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TestSPInsertFinal.Repository
{
    public class PasswordRepository : IPasswordRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<PasswordRepository> logger;

        public PasswordRepository(IConfiguration configuration, ILogger<PasswordRepository> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public string EncryptString(string text)
        {
            try
            {
                string keyString = configuration.GetSection("AppSettings:EncryptionKey").Value;

                var key = Encoding.UTF8.GetBytes(keyString);

                using (var aesAlg = Aes.Create())
                {
                    using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                    {
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(text);
                            }

                            var iv = aesAlg.IV;

                            var decryptedContent = msEncrypt.ToArray();

                            var result = new byte[iv.Length + decryptedContent.Length];

                            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                            return Convert.ToBase64String(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at PasswordRepository.EncryptString: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public string DecryptString(string cipherText)
        {
            try
            {
                string keyString = configuration.GetSection("AppSettings:EncryptionKey").Value;
                var fullCipher = Convert.FromBase64String(cipherText);

                var iv = new byte[16];
                var cipher = new byte[16];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
                var key = Encoding.UTF8.GetBytes(keyString);

                using (var aesAlg = Aes.Create())
                {
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        string result;
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at PasswordRepository.DecryptString: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
