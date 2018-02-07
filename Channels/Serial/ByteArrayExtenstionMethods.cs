using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Channels.Serial
{
    static public class ByteArrayExtensionMethods
    {
        /// <summary>
        /// Append two byte array and return an new one
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static byte[] Append(this byte[] a1, byte[] a2)
        {
            var result = new byte[a1.Length + a2.Length];
            Buffer.BlockCopy(a1, 0, result, 0, a1.Length);
            Buffer.BlockCopy(a2, 0, result, a1.Length, a2.Length);
            return result;
        }

        /// <summary>
        /// Slices a array , returns an new array.
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public static byte[] Slice(this byte[] a1, int start, int length)
        {
            var result = new byte[length];
            Buffer.BlockCopy(a1, start, result, 0, length);
            return result;
        }
    }
}
