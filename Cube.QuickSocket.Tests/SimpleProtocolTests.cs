using Cube.QuickSocket.Protocols;
using System.Buffers;
using System.Diagnostics;

namespace Cube.QuickSocket.Tests
{
    public class SimpleProtocolTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_delimiter()
        {
            // delimiter + payload
            var options = new SimpleProtocolOptions()
            {
                //  MaxLength= 6,
                Delimiter = "24", //   '$'
                HeadBytes = 0, // no head
                LengthFieldBytes = 0, // no length-field 
                TailBytes = 0 // no tail
            };

            var msg = new SimpleProtocolMessage(null, payload: "0102030405060708".HexToBytes(), null);

            Debug.WriteLine("before: " + msg.ToHex());

            var writer = new ArrayBufferWriter<byte>();
            var proto = new SimpleProtocol(options);
            proto.Encode(msg, writer);

            string hex = "EFEFEF    " + writer.WrittenMemory.ToArray().ToHex() + "      0E 0F 10 11 12 13 14 15 16 17 18 19 10 1a";

            Debug.WriteLine(hex);

            var input = new ReadOnlySequence<byte>(hex.Replace(" ", "").HexToBytes());
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            proto.Decode(input, ref consumed, ref examined, out SimpleProtocolMessage message);

            Debug.WriteLine("after: " + message.ToHex());

            Assert.IsTrue(msg.ToHex() == message.ToHex());
        }

        [Test]
        public void Test_head()
        {
            // delimiter + head
            var options = new SimpleProtocolOptions()
            {
                Delimiter = "242424", //   '$'
                HeadBytes = 3, //  
                LengthFieldBytes = 0, // no length-field 
                TailBytes = 0 // no tail
            };

            var msg = new SimpleProtocolMessage(head: "010203".HexToBytes(), null, null);

            Debug.WriteLine("before: " + msg.ToHex());

            var writer = new ArrayBufferWriter<byte>();
            var proto = new SimpleProtocol(options);
            proto.Encode(msg, writer);

            string hex = "EFEFEF    " + writer.WrittenMemory.ToArray().ToHex() + "      0E 0F 10 11 12 13 14 15 16 17 18 19 10 1a";

            Debug.WriteLine(hex);

            var input = new ReadOnlySequence<byte>(hex.Replace(" ", "").HexToBytes());
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            proto.Decode(input, ref consumed, ref examined, out SimpleProtocolMessage message);

            Debug.WriteLine("after: " + message.ToHex());

            Assert.IsTrue(msg.ToHex() == message.ToHex());
        }

        [Test]
        public void Test_head2()
        {
            // delimiter + head
            var options = new SimpleProtocolOptions()
            {
                HeadBytes = 3, //  
                LengthFieldBytes = 0, // no length-field 
                TailBytes = 0 // no tail
            };

            var msg = new SimpleProtocolMessage(head: "010203".HexToBytes(), null, null);

            Debug.WriteLine("before: " + msg.ToHex());

            var writer = new ArrayBufferWriter<byte>();
            var proto = new SimpleProtocol(options);
            proto.Encode(msg, writer);

            string hex = writer.WrittenMemory.ToArray().ToHex() + "      0E 0F 10 11 12 13 14 15 16 17 18 19 10 1a";

            Debug.WriteLine(hex);

            var input = new ReadOnlySequence<byte>(hex.Replace(" ", "").HexToBytes());
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            proto.Decode(input, ref consumed, ref examined, out SimpleProtocolMessage message);

            Debug.WriteLine("after: " + message.ToHex());

            Assert.IsTrue(msg.ToHex() == message.ToHex());
        }

        [Test]
        public void Test_length_based()
        {
            var options = new SimpleProtocolOptions()
            {
                Delimiter = "7e7e7e",
                LengthFieldBytes = 3,
                TailBytes = 0
            };

            var msg = new SimpleProtocolMessage(null, payload: "01020304050607".HexToBytes(), null);

            Debug.WriteLine("before: " + msg.ToHex());

            var writer = new ArrayBufferWriter<byte>();
            var proto = new SimpleProtocol(options);
            proto.Encode(msg, writer);

            string hex = "EFEFEF    " + writer.WrittenMemory.ToArray().ToHex() + "      0E 0F 10 11 12 13 14 15 16 17 18 19 10 1a";

            Debug.WriteLine(hex);

            var input = new ReadOnlySequence<byte>(hex.Replace(" ", "").HexToBytes());
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            proto.Decode(input, ref consumed, ref examined, out SimpleProtocolMessage message);

            Debug.WriteLine("after: " + message.ToHex());

            Assert.IsTrue(msg.ToHex() == message.ToHex());
        }

        [Test]
        public void Test_length_based2()
        {
            var options = new SimpleProtocolOptions()
            {
                LengthFieldBytes = 1,
                TailBytes = 0
            };

            var msg = new SimpleProtocolMessage(null, payload: "01020304050607".HexToBytes(), null);

            Debug.WriteLine("before: " + msg.ToHex());

            var writer = new ArrayBufferWriter<byte>();
            var proto = new SimpleProtocol(options);
            proto.Encode(msg, writer);

            string hex = writer.WrittenMemory.ToArray().ToHex() + "      0E 0F 10 11 12 13 14 15 16 17 18 19 10 1a";

            Debug.WriteLine(hex);

            var input = new ReadOnlySequence<byte>(hex.Replace(" ", "").HexToBytes());
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            proto.Decode(input, ref consumed, ref examined, out SimpleProtocolMessage message);

            Debug.WriteLine("after: " + message.ToHex());

            Assert.IsTrue(msg.ToHex() == message.ToHex());
        }

        [Test]
        public void Test_length_tail()
        {
            var options = new SimpleProtocolOptions()
            {
                TailBytes = 7
            };

            var msg = new SimpleProtocolMessage(null, null, tail: "01020304050607".HexToBytes());

            Debug.WriteLine("before: " + msg.ToHex());

            var writer = new ArrayBufferWriter<byte>();
            var proto = new SimpleProtocol(options);
            proto.Encode(msg, writer);

            string hex = writer.WrittenMemory.ToArray().ToHex() + "      0E 0F 10 11 12 13 14 15 16 17 18 19 10 1a";

            Debug.WriteLine(hex);

            var input = new ReadOnlySequence<byte>(hex.Replace(" ", "").HexToBytes());
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            proto.Decode(input, ref consumed, ref examined, out SimpleProtocolMessage message);

            Debug.WriteLine("after: " + message.ToHex());

            Assert.IsTrue(msg.ToHex() == message.ToHex());
        }

        [Test]
        public void Test_msg()
        {
            var options = new SimpleProtocolOptions()
            {
                Delimiter = "7e7eef",
                HeadBytes = 3,
                LengthFieldBytes = 3,
                LengthFieldBigEndian = false,
                TailBytes = 7
            };

            var msg = new SimpleProtocolMessage(
                head: "080909".HexToBytes(),
                payload: "0E0F10111213141516171819101a".HexToBytes(),
                tail: "01020304050607".HexToBytes());

            Debug.WriteLine("before: " + msg.ToHex());

            var writer = new ArrayBufferWriter<byte>();
            var proto = new SimpleProtocol(options);
            proto.Encode(msg, writer);

            string hex = writer.WrittenMemory.ToArray().ToHex() + "      0E 0F 10 11 12 13 14 15 16 17 18 19 10 1a";

            Debug.WriteLine(hex);

            var input = new ReadOnlySequence<byte>(hex.Replace(" ", "").HexToBytes());
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            proto.Decode(input, ref consumed, ref examined, out SimpleProtocolMessage message);

            Debug.WriteLine("after: " + message.ToHex());

            Assert.IsTrue(msg.ToHex() == message.ToHex());
        }

        [Test]
        public void Test_msg2()
        {
            var options = new SimpleProtocolOptions()
            {
                Delimiter = "7e7eef",
                HeadBytes = 3,
                LengthFieldBytes = 3,
                LengthFieldBigEndian = false,
                TailBytes = 7
            };

            var proto = new SimpleProtocol(options);

            var msg = new SimpleProtocolMessage(
                   head: "080909".HexToBytes(),
                   payload: "0E0F10111213141516171819101a".HexToBytes(),
                   tail: "01020304050607".HexToBytes());

            var writer = new ArrayBufferWriter<byte>();
            proto.Encode(msg, writer);

            string hex = "0001000100027E7EEF0809090E00000E0F10111213141516171819101A010203040506070E0F10111213141516171819101a";
            for (int i = 0; i < 5; i++)
            {
                hex += writer.WrittenMemory.ToArray().ToHex() + "ccbbaaeeff7e7e00";
            }

            Debug.WriteLine(hex.HexInsertSpace());

            var input = new ReadOnlySequence<byte>(hex.Replace(" ", "").HexToBytes());
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            SimpleProtocolMessage last = null;
            for (int i = 0; i < 6; i++)
            {
                proto.Decode(input, ref consumed, ref examined, out SimpleProtocolMessage message);
                Debug.WriteLine("after: " + message.ToHex());
                if (i > 0)
                {
                    Assert.IsTrue(message.ToHex() == last.ToHex());
                }
                last = message;
            }


        }

    }
}