using System.Security.Cryptography;
using System.Text;

namespace PersonalFinances.DAL.Helpers
{
    public class Cryptography
    {

        private static readonly int Keysize = 256; // 256 bits for the key
        private static readonly int BlockSize = 128;
        private static readonly int DerivationIterations = 1000;
        private static readonly string PassPhrase = "bE.nD)*nQ$!qWhO3zZo~%J%$lesY'Oa4*D-hdm>mMnfY^n|j`t~azQKxt+x)),`bE.nD)*nQ$!qWhO3zZo~%J%$lesY'Oa4*D-hdm>mMnfY^n|j`t~azQKxt+x)),`";

        public static string Encrypt(string plainText)
        {
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(PassPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new AesManaged())
                {
                    symmetricKey.BlockSize = BlockSize;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                var cipherTextBytes = saltStringBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();

                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(32).ToArray(); // 32 bytes for the salt
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(32).Take(16).ToArray(); // 16 bytes for the IV
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(48).ToArray(); // Rest is cipher text

            using (var password = new Rfc2898DeriveBytes(PassPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new AesManaged())
                {
                    symmetricKey.BlockSize = BlockSize;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                using (var streamReader = new StreamReader(cryptoStream))
                                {
                                    // Read all the decrypted bytes as a string and trim any trailing null characters
                                    var decryptedText = streamReader.ReadToEnd().TrimEnd('\0');
                                    return decryptedText;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static void SaveEncryptedConfig(string filePath, string json)
        {
            var encryptedText = Encrypt(json);
            File.WriteAllText(filePath, encryptedText);
        }


    }
}

