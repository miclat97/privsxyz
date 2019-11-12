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
    public class PhotoService : IPhotoService
    {
        private readonly PrivsDbContext _context;

        public PhotoService(PrivsDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateAndEncryptPhoto(byte[] byteArray, string ipv4, string ipv6, string hostname)
        {
            string keyIntoDb = RandomString(10);
            string keyToDecrypt = RandomString(20);

            var photoWithGeneratedKey =
                await _context.Photo.FirstOrDefaultAsync(f => f.PhotoIdentityString.Equals(keyIntoDb));

            while (photoWithGeneratedKey != null)
            {
                keyIntoDb = RandomString(10);
                photoWithGeneratedKey =
                    await _context.Photo.FirstOrDefaultAsync(f => f.PhotoIdentityString.Equals(keyIntoDb));
            }



            PhotoEntity newPhoto = new PhotoEntity()
            {
                PhotoIdentityString = keyIntoDb,
                CreatedDateTime = DateTime.Now,
                IPv4Address = ipv4,
                IPv6Address = ipv6,
                Hostname = hostname,
                Image = Encrypt(byteArray, keyToDecrypt)
            };



            await _context.Photo.AddAsync(newPhoto);
            await _context.SaveChangesAsync();
            return $"{keyIntoDb}@{keyToDecrypt}";
        }

        public byte[] Encrypt(byte[] clearBytes, string encryptionKey)
        {
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] {
                    0x57, 0x22, 0x61, 0x8e, 0xd0, 0x4f, 0xda, 0x3b, 0x7a, 0x55, 0x12, 0xff, 0xde
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
                    0x57, 0x22, 0x61, 0x8e, 0xd0, 0x4f, 0xda, 0x3b, 0x7a, 0x55, 0x12, 0xff, 0xde
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

        public async Task<byte[]> DeleteAndDecryptPhoto(string photoKeyId, string photoKey, string ipv4, string ipv6, string hostname)
        {
            var photoEntityInDb =
                await _context.Photo.FirstOrDefaultAsync(f => f.PhotoIdentityString.Equals(photoKeyId));
            if (photoEntityInDb == null)
            {
                return null;
            }

            byte[] decryptedPhoto;

            try
            {
                decryptedPhoto = Decrypt(photoEntityInDb.Image, photoKey);
            }
            catch (Exception e)
            {
                return null;
            }

            try
            {
                photoEntityInDb.Image = new byte[0];
                photoEntityInDb.ViewerIPv4 = ipv4;
                photoEntityInDb.ViewerIPv6 = ipv6;
                photoEntityInDb.ViewerHostname = hostname;
                photoEntityInDb.OpenedDate = DateTime.Now;
                _context.Update(photoEntityInDb);
                await _context.SaveChangesAsync();
                return decryptedPhoto;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
