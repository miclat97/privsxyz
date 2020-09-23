using Microsoft.EntityFrameworkCore;
using PrivsXYZ.Data;
using PrivsXYZ.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PrivsXYZ.Services
{
    public class FileService : IFileService
    {
        private readonly PrivsDbContext _context;

        public FileService(PrivsDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateAndEncryptFile(byte[] byteArray, string ipv4, string ipv6, string hostname, string fileName)
        {
            string keyIntoDb = RandomString(20);
            string keyToDecrypt = RandomString(25);

            var fileWithGeneratedKey =
                await _context.File.FirstOrDefaultAsync(f => f.FileIdentityString.Equals(keyIntoDb));

            while (fileWithGeneratedKey != null)
            {
                keyIntoDb = RandomString(20);
                fileWithGeneratedKey =
                    await _context.File.FirstOrDefaultAsync(f => f.FileIdentityString.Equals(keyIntoDb));
            }



            FileEntity newFile = new FileEntity()
            {
                FileIdentityString = keyIntoDb,
                CreatedDateTime = DateTime.Now,
                IPv4Address = ipv4,
                IPv6Address = ipv6,
                Hostname = hostname,
                File = Encrypt(byteArray, keyToDecrypt),
                FileName = fileName
            };



            await _context.File.AddAsync(newFile);
            await _context.SaveChangesAsync();
            return $"{keyIntoDb}@{keyToDecrypt}";
        }

        public byte[] Encrypt(byte[] clearBytes, string encryptionKey)
        {
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
                    return ms.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] encryptedBytes, string encryptionKey)
        {
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
                        cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                        cs.Close();
                    }
                    return ms.ToArray();
                }
            }
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

        public async Task<byte[]> DeleteAndDecryptFile(string fileKeyId, string fileKey, string ipv4, string ipv6, string hostname)
        {
            var fileEntityInDb =
                await _context.File.FirstOrDefaultAsync(f => f.FileIdentityString.Equals(fileKeyId));
            if (fileEntityInDb == null)
            {
                return null;
            }

            byte[] decryptedPhoto;

            try
            {
                decryptedPhoto = Decrypt(fileEntityInDb.File, fileKey);
            }
            catch (Exception)
            {
                return null;
            }

            try
            {
                fileEntityInDb.File = new byte[0];
                fileEntityInDb.ViewerIPv4 = ipv4;
                fileEntityInDb.ViewerIPv6 = ipv6;
                fileEntityInDb.ViewerHostname = hostname;
                fileEntityInDb.OpenedDate = DateTime.Now;
                _context.Update(fileEntityInDb);
                await _context.SaveChangesAsync();
                return decryptedPhoto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GetFileName(string fileKeyId)
        {
            var file = await _context.File.FirstOrDefaultAsync(f => f.FileIdentityString.Equals(fileKeyId));
            return file.FileName;
        }
    }
}
