using System;
using System.IO;

namespace Imagegram.Api.Tests
{
    public static class StreamHelper
    {
        public static bool IsEqualTo(this Stream a, Stream b)
        {
            if (a == b)
            {
                return true;
            }

            if (a == null || b == null)
            {
                throw new ArgumentNullException(a == null ? "a" : "b");
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            a.Position = 0;
            b.Position = 0;
            for (int i = 0; i < a.Length; i++)
            {
                int aByte = a.ReadByte();
                int bByte = b.ReadByte();
                if (aByte.CompareTo(bByte) != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
