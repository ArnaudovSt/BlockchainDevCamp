using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bitcoin_Address_Generator
{
    class Startup
    {
        public static void Main()
        {
            string hexString = @"0450863AD64A87AE8A2FE83C1AF1A8403CB53F53E486D8511DAD8A04887E5B23522CD470243453A299FA9E77237716103ABC11A1DF38855ED6F2EE187E9C582BA6";

            byte[] publicKey = HexToBytes(hexString);

            Console.WriteLine($"Public Key: {BitConverter.ToString(publicKey.ToArray())}");

            Console.WriteLine(new string('=', 25));

            byte[] publicKeySha = Sha256(publicKey);

            Console.WriteLine($"Sha Public Key: {BitConverter.ToString(publicKeySha.ToArray())}");

            Console.WriteLine(new string('=', 25));

            byte[] publicKeyShaRipemd = Ripmd160(publicKeySha);

            Console.WriteLine($"Ripe Sha Public Key: {BitConverter.ToString(publicKeyShaRipemd.ToArray())}");

            Console.WriteLine(new string('=', 25));

            byte network = 0;

            List<byte> publicKeyWithAddedNetwork = publicKeyShaRipemd.ToList();
            publicKeyWithAddedNetwork.Insert(0, network);

            byte[] publicHash = Sha256(Sha256(publicKeyWithAddedNetwork.ToArray()));

            Console.WriteLine($"Public Hash: {BitConverter.ToString(publicHash.ToArray())}");

            Console.WriteLine(new string('=', 25));

            publicKeyWithAddedNetwork.AddRange(publicHash.Take(4));

            Console.WriteLine($"Address: {BitConverter.ToString(publicKeyWithAddedNetwork.ToArray())}");

            Console.WriteLine(new string('=', 25));

            Console.WriteLine($"Human Address: {Base58Encode(publicKeyWithAddedNetwork.ToArray())}");
        }

        public static string Base58Encode(byte[] array)
        {
            const string alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            BigInteger encodeSize = alphabet.Length;
            BigInteger arrayToInt = 0;

            for (int i = 0; i < array.Length; ++i)
            {
                arrayToInt = arrayToInt * 256 + array[i];
            }

            while (arrayToInt > 0)
            {
                int reminder = (int)(arrayToInt % encodeSize);
                arrayToInt /= encodeSize;
                sb.Insert(0, alphabet[reminder]);
            }

            for (int i = 0; i < array.Length && array[i] == 0; ++i)
            {
                sb.Insert(0, alphabet[0]);
            }

            return sb.ToString();
        }

        private static byte[] Ripmd160(byte[] publicKeySha)
        {
            RIPEMD160Managed ripemd = new RIPEMD160Managed();
            return ripemd.ComputeHash(publicKeySha);
        }

        private static byte[] Sha256(byte[] publicKey)
        {
            SHA256Managed sha = new SHA256Managed();
            return sha.ComputeHash(publicKey);
        }

        private static byte[] HexToBytes(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid hex string!");
            }

            byte[] result = new byte[hexString.Length / 2];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = byte.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
            }

            return result;
        }
    }
}
