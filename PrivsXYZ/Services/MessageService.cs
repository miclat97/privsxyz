using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrivsXYZ.Data;
using PrivsXYZ.Entities;

namespace PrivsXYZ.Services
{
    public class MessageService : IMessageService
    {
        private readonly PrivsDbContext _context;

        public MessageService(PrivsDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateAndEncryptMessage(string message, string ipv4, string ipv6, string hostname)
        {
            string keyIntoDb = RandomString(20);
            string keyToDecrypt = RandomString(25);

            var messageWithGeneratedKey =
                await _context.Message.FirstOrDefaultAsync(f => f.MessageIdentityString.Equals(keyIntoDb));

            while (messageWithGeneratedKey != null)
            {
                keyIntoDb = RandomString(20);
                messageWithGeneratedKey =
                    await _context.Message.FirstOrDefaultAsync(f => f.MessageIdentityString.Equals(keyIntoDb));
            }

            MessageEntity newMessage = new MessageEntity()
            {
                MessageIdentityString = keyIntoDb,
                CreatedDateTime = DateTime.Now,
                IPv4Address = ipv4,
                IPv6Address = ipv6,
                Hostname = hostname,
                EncryptedMessage = Encrypt(message, keyToDecrypt)
            };

            await _context.Message.AddAsync(newMessage);
            await _context.SaveChangesAsync();
            return $"{keyIntoDb}@{keyToDecrypt}";
        }

        public async Task<string> DeleteAndDecryptMessage(string messageKeyId, string messageKey, string ipv4, string ipv6, string hostname)
        {
            var messageEntityInDb =
                await _context.Message.FirstOrDefaultAsync(f => f.MessageIdentityString.Equals(messageKeyId));
            if (messageEntityInDb == null)
            {
                return "No message with this ID, or bad key!!!";
            }

            string decryptedMessage;

            try
            {
                decryptedMessage = Decrypt(messageEntityInDb.EncryptedMessage, messageKey);
            }
            catch (Exception e)
            {
                return "No message with this ID, or bad key!!!";
            }

            try
            {
                messageEntityInDb.EncryptedMessage = "";
                messageEntityInDb.ViewerIPv4 = ipv4;
                messageEntityInDb.ViewerIPv6 = ipv6;
                messageEntityInDb.ViewerHostname = hostname;
                messageEntityInDb.OpenedDate = DateTime.Now;
                _context.Message.Update(messageEntityInDb);
                await _context.SaveChangesAsync();
                return decryptedMessage;
            }
            catch (Exception e)
            {
                return
                    "Error when trying to delete message from database, please try again or contact with administrator.";
            }
        }


        public string Encrypt(string plainTextString, string encryptionKey)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(plainTextString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] {
                    0x57, 0x22, 0x61, 0x8e, 0xd0, 0x4f, 0xda, 0x3b, 0x7a, 0x55, 0x12, 0xff, 0xde, 0x54, 0x55, 0xdf, 0x33, 0x52, 0xaa, 0x2e
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    plainTextString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return plainTextString;
        }

        public string Decrypt(string encryptedText, string encryptionKey)
        {
            encryptedText = encryptedText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] {
                    0x57, 0x22, 0x61, 0x8e, 0xd0, 0x4f, 0xda, 0x3b, 0x7a, 0x55, 0x12, 0xff, 0xde, 0x54, 0x55, 0xdf, 0x33, 0x52, 0xaa, 0x2e
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encryptedText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return encryptedText;
        }

        public static string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }


    }
}
