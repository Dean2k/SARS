using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VRChatAPI_New
{
    public static class EasyHash
    {
        internal static byte[] GetSHA1(byte[] input)
        {
            using (var sha256 = SHA1.Create())
                return sha256.ComputeHash(input);
        }

        public static string GetSHA1String(byte[] input) =>
            BitConverter.ToString(GetSHA1(input)).Replace("-", string.Empty).ToLower();
    }
}
