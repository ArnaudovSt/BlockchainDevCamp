using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Calculate_a_SHA256_Hash
{
    class Startup
    {
        static void Main()
        {
            Console.WriteLine(GetHashSha256("HelloWorld!"));
        }

        private static string GetHashSha256(string input)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(input);

            SHA256Managed sha = new SHA256Managed();

            byte[] computedHash = sha.ComputeHash(bytes);

            return string.Join("", computedHash.Select(b => $"{b:x2}"));
        }
    }
}
