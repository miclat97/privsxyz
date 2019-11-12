using System.Threading.Tasks;

namespace PrivsXYZ.Services
{
    public interface IMessageService
    {
        Task<string> CreateAndEncryptMessage(string message, string ipv4, string ipv6, string hostname);
        string Encrypt(string plainTextString, string encryptionKey);
        string Decrypt(string encryptedText, string encryptionKey);
        Task<string> DeleteAndDecryptMessage(string messageKeyId, string messageKey, string ipv4, string ipv6, string hostname);
    }
}