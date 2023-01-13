using Cube.QuickSocket.Protocols;
using System.Buffers;
using System.Diagnostics;

namespace Cube.QuickSocket.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var options = new LengthBasedProtocolOptions()
            {
                Head = "020304",
                HeadBytes = 3,
                LengthFieldOffset = 3,
                LengthFieldBytes = 1,
                LengthFieldIsBigEndian = true,
                TailBytes = 4
            };

            var proto = new LengthBasedProtocol(options);

            var msg = new LengthBasedProtocolMessage(payloadHex: "060708090A", headHex: "020304", tailHex: "0D0E0F10");
            var writer = new ArrayBufferWriter<byte>();
            proto.WriteMessage(msg, writer);

            string hex = "  0  0 09 0 8 01" + writer.WrittenMemory.ToArray().ToHex() + "11 1 2 13 1 4 15 1     6 17             18 19 10 1a ";

            Debug.WriteLine(hex.HexInsertSpace());

            var input = new ReadOnlySequence<byte>(hex.HexToBytes(trimSpace: true));
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            proto.TryParseMessage(input, ref consumed, ref examined, out LengthBasedProtocolMessage message);

            Debug.WriteLine(message);

            Assert.IsTrue(msg.ToHex() == message.ToHex());
        }
    }
}