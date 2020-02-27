using System.Threading.Tasks;

namespace PrivsXYZ.Services
{
    public interface IFileService
    {
        Task<string> CreateAndEncryptFile(byte[] byteArray, string ipv4, string ipv6, string hostname, string fileName);
        byte[] Encrypt(byte[] clearBytes, string encryptionKey);
        byte[] Decrypt(byte[] encryptedBytes, string encryptionKey);
        Task<byte[]> DeleteAndDecryptFile(string fileKeyId, string photoKey, string ipv4, string ipv6, string hostname);
        Task<string> GetFileName(string fileKeyId);
    }
}