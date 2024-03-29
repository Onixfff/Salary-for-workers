using System;
using System.Security.Cryptography;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;
using System.Threading.Tasks;
using System.Linq;

namespace Salary_for_workers
{
    public class Ciphertar
    {
        public string EncryptsData(string password)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] key =
                    {
                    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                    0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                };

                    aes.Key = key;

                    // Генерируем случайный IV
                    aes.GenerateIV();
                    byte[] iv = aes.IV;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // Создаем поток для шифрования данных
                        using (CryptoStream cryptoStream = new CryptoStream(
                            memoryStream,
                            aes.CreateEncryptor(aes.Key, aes.IV), // Используем IV и ключ для шифрования
                            CryptoStreamMode.Write))
                        {
                            // Записываем зашифрованный пароль в поток
                            using (StreamWriter encryptWriter = new StreamWriter(cryptoStream))
                            {
                                encryptWriter.Write(password);
                            }
                        }

                        // Преобразуем IV и зашифрованные данные в строку в формате Base64
                        string encryptedMessage = Convert.ToBase64String(iv.Concat(memoryStream.ToArray()).ToArray());
                        return encryptedMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при шифровании пароля. {ex}");
                return null;
            }
        }

        public async Task<string> DecryptsDataAsync(string encryptedPassword)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] key =
                    {
                    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                    0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                };

                    // Читаем IV из начала зашифрованной строки
                    byte[] iv = Convert.FromBase64String(encryptedPassword).Take(aes.BlockSize / 8).ToArray();

                    // Получаем зашифрованные данные без IV
                    byte[] encryptedData = Convert.FromBase64String(encryptedPassword).Skip(aes.BlockSize / 8).ToArray();
                    string decryptedMessage;
                    using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(
                           memoryStream,
                           aes.CreateDecryptor(key, iv),
                           CryptoStreamMode.Read))
                        {
                            using (StreamReader decryptReader = new StreamReader(cryptoStream))
                            {
                                // Читаем расшифрованные данные
                                decryptedMessage = await decryptReader.ReadToEndAsync();
                            }
                        }
                    }

                    return decryptedMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при расшифровке данных. {ex}");
                return null;
            }
        }
    }
}
