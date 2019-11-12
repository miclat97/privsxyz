using System.Threading.Tasks;

namespace PrivsXYZ.Services
{
    public interface IPhotoService
    {
        Task<string> CreateAndEncryptPhoto(byte[] byteArray, string ipv4, string ipv6, string hostname);
        byte[] Encrypt(byte[] clearBytes, string encryptionKey);
        byte[] Decrypt(byte[] encryptedBytes, string encryptionKey);
        Task<byte[]> DeleteAndDecryptPhoto(string photoKeyId, string photoKey, string ipv4, string ipv6, string hostname);
    }
}