using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Cube.QuickSocket
{
    public static class Utils
    {
        public static byte CS(ArraySegment<byte> b, int startIndex = 0, int length = -1)
        {
            length = length < 0 ? b.Count : length;
            if (length == 0 || b == null || b.Count < 1)
            {
                return 0;
            }
            startIndex = startIndex < 0 ? 0 : startIndex;
            if (startIndex >= b.Count || (startIndex + length) > b.Count)
            {
                throw new IndexOutOfRangeException();
            }

            int sum = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                sum += b[i];
            }

            return (byte)(sum & 0xff);
        }

        public static byte CS(ReadOnlySequence<byte> b)
        {
            int cs = 0;
            foreach (var item in b)
            {
                foreach (var n in item.Span)
                {
                    cs += n;
                }
            }
            return (byte)cs;
        }

        public static byte CS(IEnumerable<byte> list, int startIndex = 0, int length = -1)
        {
            length = length < 0 ? list.Count() : length;
            if (length == 0 || list == null || list.Count() < 1)
            {
                return 0;
            }
            startIndex = startIndex < 0 ? 0 : startIndex;
            if (startIndex >= list.Count() || (startIndex + length) > list.Count())
            {
                throw new IndexOutOfRangeException();
            }

            var i = 0;
            var n = 0;
            int sum = 0;
            var em = list.GetEnumerator();
            while (em.MoveNext())
            {
                if (n >= length)
                {
                    break;
                }
                if (i >= startIndex)
                {
                    n++;
                    sum += em.Current;
                }
                i++;
            }
            return (byte)(sum & 0xff);
        }

        public static byte[] Slice(this byte[] arr, int index, int? count = null)
        {
            if (index < 0 || index > arr.Length || (count.HasValue && (index + count.Value) > arr.Length))
                throw new IndexOutOfRangeException();

            if (!count.HasValue) count = arr.Length - index;

            byte[] b = new byte[count.Value];
            for (int i = 0; i < count.Value; i++)
            {
                b[i] = arr[index + i];
            }
            return b;
        }

        public static (string s1, string s2) Split2(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str)) return (str, null);

            var s = str.Split(separator, opt);
            return (s[0], s.Length > 1 ? s[1] : null);
        }

        public static (string s1, string s2, string s3) Split3(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str)) return (str, null, null);

            var s = str.Split(separator, opt);
            return (s[0], s.Length > 1 ? s[1] : null, s.Length > 2 ? s[2] : null);
        }

        public static (string s1, string s2, string s3, string s4) Split4(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str)) return (str, null, null, null);

            var s = str.Split(separator, opt);
            return (s[0], s.Length > 1 ? s[1] : null, s.Length > 2 ? s[2] : null, s.Length > 3 ? s[3] : null);
        }

        public static (string s1, string s2, string s3, string s4, string s5) Split5(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str)) return (str, null, null, null, null);

            var s = str.Split(separator, opt);
            return (s[0], s.Length > 1 ? s[1] : null, s.Length > 2 ? s[2] : null, s.Length > 3 ? s[3] : null, s.Length > 4 ? s[4] : null);
        }

        public static (string s1, string s2, string s3, string s4, string s5, string s6) Split6(this string str, string separator, StringSplitOptions opt = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str)) return (str, null, null, null, null, null);

            var s = str.Split(separator, opt);
            return (s[0], s.Length > 1 ? s[1] : null, s.Length > 2 ? s[2] : null, s.Length > 3 ? s[3] : null, s.Length > 4 ? s[4] : null, s.Length > 5 ? s[5] : null);
        }

        public static string ReverseHex(this string hex)
        {
            if (hex.Length % 2 != 0) hex += "0";
            var b = new StringBuilder();
            for (int i = hex.Length - 1; i >= 0; i = i - 2)
            {
                b.Append($"{hex[i - 1]}{hex[i]}");
            }
            return b.ToString();
        }

        public static string HexInsertSpace(this string hex)
        {
            if (hex == null || hex.Length < 2) return hex;
            var s = new StringBuilder(hex.Length + hex.Length >> 2);
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

        public static byte[] HexToBytes(this string hex, bool trimSpace = false)
        {
            if (string.IsNullOrWhiteSpace(hex)) return Array.Empty<byte>();
            if (trimSpace)
            {
                hex = new string(hex.Where(x => x != ' ').ToArray());
            }
            if (hex.Length % 2 != 0) hex = "0" + hex;
            var b = new byte[hex.Length / 2];
            for (int i = 0, j = 0; i < hex.Length; i += 2, j++)
            {
                b[j] = Convert.ToByte($"{hex[i]}{hex[i + 1]}", 16);
            }
            return b;
        }

        private static readonly string[] hexArray = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
        public static string ToHex(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1) return string.Empty;
            var sb = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++) sb.Append(hexArray[(bytes[i] & 0xf0) >> 4] + hexArray[bytes[i] & 0x0f]);
            return sb.ToString();
        }

        public static string ToHex(this byte bytes)
        {
            return hexArray[(bytes & 0xf0) >> 4] + hexArray[bytes & 0x0f];
        }

        public static string ToHex(this ArraySegment<byte> bytes, int startIndex = 0, int length = -1)
        {
            if (bytes == null || bytes.Count < 1) return string.Empty;

            if (length < 0)
            {
                length = bytes.Count;
            }
            var sb = new StringBuilder(length * 2);
            for (int i = startIndex, n = 0; i < bytes.Count && n < length; i++, n++)
            {
                var b = bytes[i];
                sb.Append(hexArray[(b & 0xf0) >> 4] + hexArray[b & 0x0f]);
            }
            return sb.ToString();
        }

        public static string ToHex(this ReadOnlySequence<byte> bytes)
        {
            if (bytes.IsEmpty) return string.Empty;

            var sb = new StringBuilder((int)bytes.Length * 2);
            foreach (var memory in bytes)
            {
                foreach (var b in memory.Span)
                {
                    sb.Append(hexArray[(b & 0xf0) >> 4] + hexArray[b & 0x0f]);
                }
            }
            return sb.ToString();
        }

        public static int BCD2Int(this byte b)
        {
            return b & 0x0f + ((b & 0xf0) >> 4) * 10;
        }

        public static byte[] Add33(this byte[] b)
        {
            for (int i = 0; i < b.Length; i++) b[i] = (byte)(b[i] + 0x33);
            return b;
        }

        public static List<byte> Add33(this List<byte> b)
        {
            for (int i = 0; i < b.Count; i++) b[i] = (byte)(b[i] + 0x33);
            return b;
        }

        public static byte[] Sub33(this byte[] b)
        {
            for (int i = 0; i < b.Length; i++) b[i] = (byte)(b[i] - 0x33);
            return b;
        }

        public static string Reverse(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            StringBuilder b = new StringBuilder();
            for (int i = str.Length - 1; i >= 0; i--)
            {
                b.Append(str[i]);
            }
            return b.ToString();
        }

        public static byte[] Reverse(this byte[] b)
        {
            if (b == null || b.Length == 1) return b;
            byte[] c = new byte[b.Length];
            for (int i = 0, j = b.Length - 1; i < b.Length; i++, j--)
            {
                c[j] = b[i];
            }
            return c;
        }

        public static string BCD2Str(this byte[] b, int decimalPlace = 0)
        {
            if (decimalPlace <= 0) return b.ToHex();
            var v = b.ToHex();
            return v.Insert(v.Length - decimalPlace, ".");
        }

        public static byte[] IntToByte(this string str, int? byteLength = null)
        {
            if (string.IsNullOrWhiteSpace(str)) return new byte[0];

            long v = 0;
            if (long.TryParse(str, out v))
            {
            }
            else
            {
                v = (long)(Convert.ToDouble(str));// Convert.ToInt64(str);
            }
            var arr = BitConverter.GetBytes(v);

            if (byteLength.HasValue && byteLength > 0)
            {
                var b = new byte[byteLength.Value];
                for (int i = 0; i < b.Length; i++)
                {
                    if (i < arr.Length)
                    {
                        b[i] = arr[i];
                    }
                    else
                    {
                        b[i] = 0;
                    }
                }
                return b.Reverse();
            }
            return arr;
        }

        public static int ToInt(this byte[] bs)
        {
            if (bs == null || bs.Length < 1)
            {
                return 0;
            }
            var n = 0;
            for (int i = bs.Length - 1, j = 0; i >= 0; i--, j++)
            {
                n = n | (bs[i] << (j * 8));
            }
            return n;
        }

        /// <summary>
        /// transform decimal string to bcd-byte-array according to format
        /// </summary>
        /// <param name="str"></param>
        /// <param name="fmt">Indicate the format of the decimal string</param>
        /// <param name="byteLength">limit the length to byte-array, It will be ignored has no effect to the result if  fmt is indeicated</param>
        /// <returns></returns>
        public static byte[] DecimalToBCD(this string str, string fmt = null, int? byteLength = null)
        {
            // fmt=x.xxx, len=2, => 9.3542  -> 09354
            if (string.IsNullOrWhiteSpace(str)) return new byte[0];

            if (string.IsNullOrWhiteSpace(fmt))
            {
                str = str.Replace(".", "");
                if (!byteLength.HasValue || byteLength.Value < 1)
                {
                    return str.HexToBytes();
                }
                else
                {
                    str = str.PadLeft(byteLength.Value * 2, '0');
                    str = str.Substring(0, byteLength.Value * 2);
                    return str.HexToBytes();
                }
            }
            else
            {
                fmt = fmt.ToLowerInvariant();
                var index = fmt.IndexOf(".");
                if (index < 0)
                {
                    //指定返回格式无小数部分，只处理字符串的整数部分
                    if (str.IndexOf(".") > 0) str = str.Substring(0, str.IndexOf("."));

                    //开始补0填充
                    if (byteLength.HasValue && byteLength.Value > 0)
                    {
                        // 指定返回字节长度，则以它为准
                    }
                    else
                    {
                        // 不指定返回字节长度，则以fmt 为准 ，左边补0
                        if (fmt.Length % 2 != 0) byteLength = (fmt.Length + 1) / 2;
                        else byteLength = fmt.Length / 2;
                    }

                    str = str.PadLeft(byteLength.Value * 2, '0');
                    str = str.Substring(0, byteLength.Value * 2);// 截断

                    return str.HexToBytes();
                }
                else
                {
                    // 指定返回格式带小数部分，分别处理整数位及小数位 

                    // 首先填充返回格式为偶数长度
                    if ((fmt.Length - 1) % 2 != 0) fmt = "x" + fmt;
                    index = fmt.IndexOf('.');

                    // 截取返回值的整数位和小数位
                    if (str.EndsWith(".")) str = str + "0";
                    var str_i = str.Contains('.') ? str.Substring(0, str.IndexOf('.')) : str;
                    var str_d = str.Contains('.') ? str.Substring(str.IndexOf('.') + 1) : string.Empty;

                    // 指定返回格式，以格式长度为准，byteLength 无效
                    // 处理整数部分
                    str_i = str_i.PadLeft(index, '0').Substring(0, index);
                    //if (str_i.Length % 2 != 0) str_i = "0" + str_i;
                    //处理小数部分
                    int lengthOfDecimal = fmt.Length - index - 1;
                    str_d = str_d.PadRight(lengthOfDecimal, '0').Substring(0, lengthOfDecimal);

                    return (str_i + str_d).HexToBytes();
                }
            }

        }


    }
}
