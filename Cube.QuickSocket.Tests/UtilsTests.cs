using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cube.QuickSocket.Tests
{
    internal class UtilsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(1, 0, new byte[] { 0x89 })]
        [TestCase(1, 1, new byte[] { 0x97 })]
        [TestCase(2, 1, new byte[] { 0x38, 0x97 })]
        [TestCase(1, 2, new byte[] { 0x74 })]
        [TestCase(3, 0, new byte[] { 0x01, 0x23, 0x89 })]
        [TestCase(6, 7, new byte[] { 0x12, 0x38, 0x97, 0x46, 0x58, 0x00 })] // 12389.7465800
        [TestCase(5, 3, new byte[] { 0x00, 0x12, 0x38, 0x97, 0x46 })] //  0012389.746
        [TestCase(7, 7, new byte[] { 0x00, 0x12, 0x38, 0x97, 0x46, 0x58, 0x00 })]  // 0012389.7465800
        public void Test_bcd2bytes(int bLength, int dPlace, byte[] expected)
        {
            var val = "12389.74658";
            var res = Utils.BCD2bytes(val, bLength, dPlace);

            Debug.WriteLine("result: " + res.ToHex());
            Debug.WriteLine("expected: " + expected.ToHex() + "\r\n");

            Assert.AreEqual(expected, res);

        }

        [TestCase(2, 7, new byte[] { 0x00, 0x12, 0x38, 0x97, 0x46, 0x58, 0x00 })]  // 0012389.7465800
        public void Test_bcd2bytes_index(int bLength, int dPlace, byte[] expected)
        {
            var val = "12389.74658";
            var res = Utils.BCD2bytes(val, bLength, dPlace);

            Debug.WriteLine("result: " + res.ToHex());
            Debug.WriteLine("expected: " + expected.ToHex() + "\r\n");

            Assert.AreEqual(expected, res);

        }

    }
}
