using DoDoHack.Services.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace DoDoHack.Services.Implementations
{
    public class ShaHashService : IHashService
    {
        private HMACSHA256 _sha;

        public ShaHashService(string privateKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(privateKey);
            _sha = new HMACSHA256(keyBytes);
        }

        public string GetStringHash(string str)
        {
            var strBytes = Encoding.UTF8.GetBytes(str);
            var hashBytes = _sha.ComputeHash(strBytes);
            var strHash = Encoding.UTF8.GetString(hashBytes);

            return strHash;
        }
    }
}
