using System;
using System.Buffers;
using System.Text;

namespace Cube
{
    public static partial class Utils
    {
        public static byte CS(IEnumerable<byte> list, int startIndex = 0, int? length = -1)
        {
            if (list == null) return 0;

            startIndex = startIndex < 0 ? 0 : startIndex;

            // if length<=0, it means loop all items in the list.
            length = (length == null || length < 0) ? 0 : length;

            int count = 0, n = 0, sum = 0;
            var em = list.GetEnumerator();
            while (em.MoveNext())
            {
                if (length != 0 && n >= length)
                {
                    break;
                }
                if (count >= startIndex)
                {
                    n++;
                    sum += em.Current;
                }
                count++;
            }

            // the IEnumerable<T>.Count() is a O(n) operation from Linq extension methods,
            // so we count it ourself to prevent a second loop.

            if (count > 0 && (startIndex >= count || (startIndex + length) > count))
            {
                throw new IndexOutOfRangeException();
            }

            return (byte)sum;
        }

        public static byte[] Slice(this byte[] arr, int index = 0, int count = -1)
        {
            if (arr == null || arr.Length < 1)
            {
                return arr;
            }

            index = index < 0 ? 0 : index;
            count = count < 0 ? 0 : count;
            if (index >= arr.Length || (index + count) > arr.Length)
            {
                throw new IndexOutOfRangeException();
            }

            byte[] b = new byte[count];
            for (int i = 0; i < count; i++)
            {
                b[i] = arr[index + i];
            }
            return b;
        }

        public static (string s1, string s2) Split2(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return (str, string.Empty);
            }

            var s = str.Split(separator, opt);
            return (s[0], s.Length > 1 ? s[1] : string.Empty);
        }

        public static (string s1, string s2, string s3) Split3(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return (str, string.Empty, string.Empty);
            }

            var s = str.Split(separator, opt);
            return (s[0],
                s.Length > 1 ? s[1] : string.Empty,
                s.Length > 2 ? s[2] : string.Empty);
        }

        public static (string s1, string s2, string s3, string s4) Split4(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return (str, string.Empty, string.Empty, string.Empty);
            }

            var s = str.Split(separator, opt);
            return (s[0],
                s.Length > 1 ? s[1] : string.Empty,
                s.Length > 2 ? s[2] : string.Empty,
                s.Length > 3 ? s[3] : string.Empty);
        }

        public static (string s1, string s2, string s3, string s4, string s5) Split5(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return (str, string.Empty, string.Empty, string.Empty, string.Empty);
            }

            var s = str.Split(separator, opt);
            return (s[0],
                s.Length > 1 ? s[1] : string.Empty,
                s.Length > 2 ? s[2] : string.Empty,
                s.Length > 3 ? s[3] : string.Empty,
                s.Length > 4 ? s[4] : string.Empty);
        }

        public static (string s1, string s2, string s3, string s4, string s5, string s6) Split6(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return (str, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            }

            var s = str.Split(separator, opt);
            return (s[0],
                s.Length > 1 ? s[1] : string.Empty,
                s.Length > 2 ? s[2] : string.Empty,
                s.Length > 3 ? s[3] : string.Empty,
                s.Length > 4 ? s[4] : string.Empty,
                s.Length > 5 ? s[5] : string.Empty);
        }

        public static string ReverseHex(this string hex)
        {
            if ((hex.Length & 1) == 1)
            {
                throw new ArgumentOutOfRangeException(nameof(hex), "the length of hex-string should be a even number.");
            }

            var b = new StringBuilder(hex.Length);
            for (int i = hex.Length - 1; i >= 0; i -= 2)
            {
                b.Append(hex[i - 1]);
                b.Append(hex[i]);
            }
            return b.ToString();
        }

        public static string HexInsertSpace(this string hex)
        {
            if (hex == null || hex.Length < 3)
            {
                return hex;
            }

            var s = new StringBuilder(hex.Length + hex.Length >> 1);
            for (int i = 0, j = 0; i < hex.Length; i++)
            {
                if (hex[i] == ' ')
                {
                    continue;
                }
                s.Append(hex[i]);
                if (j > 0 && (j & 1) == 1)
                {
                    s.Append(' ');
                }
                j++;
            }
            if (s[s.Length - 1] == ' ')
            {
                s.Remove(s.Length - 1, 1);
            }
            return s.ToString();
        }

        public static byte[] HexToBytes(this string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                return Array.Empty<byte>();
            }

            if ((hex.Length & 1) == 1)
            {
                throw new ArgumentOutOfRangeException(nameof(hex), "the length of hex-string should be a even number.");
            }

            var b = new byte[hex.Length >> 1];
            for (int i = 0, j = 0; i < hex.Length; i += 2, j++)
            {
                b[j] = Convert.ToByte($"{hex[i]}{hex[i + 1]}", 16);
            }
            return b;
        }

        private static readonly string[] hexArray = Enumerable.Range(0, 256).Select(x => x.ToString("X02")).ToArray();

        public static string ToHex(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(bytes.Length << 1);
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(hexArray[bytes[i]]);
            }
            return sb.ToString();
        }

        public static string ToHex(this byte bytes)
        {
            return hexArray[bytes];
        }

        public static string ToHex(this ArraySegment<byte> bytes, int startIndex = 0, int length = -1)
        {
            if (bytes == null || bytes.Count < 1)
            {
                return string.Empty;
            }

            length = length < 1 ? bytes.Count : length;

            var sb = new StringBuilder(length << 1);
            for (int i = startIndex, n = 0; i < bytes.Count && n < length; i++, n++)
            {
                sb.Append(hexArray[bytes[i]]);
            }
            return sb.ToString();
        }

        public static string ToHex(this ReadOnlySequence<byte> bytes)
        {
            if (bytes.IsEmpty) return string.Empty;

            var sb = new StringBuilder((int)bytes.Length << 1);
            foreach (var memory in bytes)
            {
                foreach (var b in memory.Span)
                {
                    sb.Append(hexArray[b]);
                }
            }
            return sb.ToString();
        }

        public static int BCD2Int(this byte b)
        {
            return b & 0x0f + (b >> 4) * 10;
        }

        public static byte[] Add33(this byte[] b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)(b[i] + 0x33);
            }
            return b;
        }

        public static List<byte> Add33(this List<byte> b)
        {
            for (int i = 0; i < b.Count; i++)
            {
                b[i] = (byte)(b[i] + 0x33);
            }
            return b;
        }

        public static byte[] Sub33(this byte[] b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)(b[i] - 0x33);
            }
            return b;
        }

        public static string BCD2Str(this byte[] b, int decimalPlace = 0)
        {
            if (decimalPlace <= 0)
            {
                return b.ToHex();
            }

            var v = b.ToHex();
            return v.Insert(v.Length - decimalPlace, ".");
        }

        public static int ToInt(this byte[] bs)
        {
            if (bs == null || bs.Length < 1)
            {
                throw new ArgumentNullException();
            }
            if (bs.Length > 4)
            {
                throw new ArgumentOutOfRangeException();
            }

            var n = 0;
            for (int i = bs.Length - 1, j = 0; i >= 0; i--, j++)
            {
                n = n | (bs[i] << (j * 8));
            }
            return n;
        }

        public static long ToLong(this byte[] bs)
        {
            if (bs == null || bs.Length < 1)
            {
                throw new ArgumentNullException();
            }
            if (bs.Length > 8)
            {
                throw new ArgumentOutOfRangeException();
            }

            long n = 0;
            for (int i = bs.Length - 1, j = 0; i >= 0; i--, j++)
            {
                n = n | (bs[i] << (j * 8));
            }
            return n;
        }

        public static byte[] BCD2bytes(this string bcd, int bytesLength = 1, int decimalPlace = 0)
        {
            if (string.IsNullOrWhiteSpace(bcd))
            {
                throw new ArgumentNullException();
            }
            if (bytesLength < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            decimalPlace = decimalPlace < 0 ? 0 : decimalPlace;
            if (decimalPlace > bytesLength << 1)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesLength), "decimalPlace CAN NOT greater than bytesLength.");
            }

            (var istr, var dstr) = bcd.Split2(".", StringSplitOptions.RemoveEmptyEntries);
            if (decimalPlace > 0)
            {
                dstr = dstr.PadRight(decimalPlace, '0').Substring(0, decimalPlace);
            }
            else
            {
                dstr = string.Empty;
            }

            var len = bytesLength << 1;
            var v = (istr + dstr).PadLeft(len, '0');
            var res = new char[len];
            for (int i = len - 1, j = v.Length - 1; i >= 0; i--, j--)
            {
                res[i] = v[j];
            }

            return new string(res).HexToBytes();
        }

    }
}
