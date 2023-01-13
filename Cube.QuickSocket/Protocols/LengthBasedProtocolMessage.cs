using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cube.QuickSocket.Protocols
{
    public class LengthBasedProtocolMessage
    {
        public LengthBasedProtocolMessage(string payloadHex, string headHex = null, string tailHex = null)
            : this(new ReadOnlySequence<byte>(payloadHex.HexToBytes()),
                  new ReadOnlySequence<byte>(headHex.HexToBytes()),
                  new ReadOnlySequence<byte>(tailHex.HexToBytes()))
        {
        }

        public LengthBasedProtocolMessage(byte[] payload, byte[] head = null, byte[] tail = null)
            : this(new ReadOnlySequence<byte>(payload),
                  new ReadOnlySequence<byte>(head),
                  new ReadOnlySequence<byte>(tail))
        {
        }

        public LengthBasedProtocolMessage(ReadOnlySequence<byte> payload, ReadOnlySequence<byte> head, ReadOnlySequence<byte> tail)
        {
            Payload = payload;
            Head = head;
            Tail = tail;
        }

        public ReadOnlySequence<byte> Head { get; }
        public ReadOnlySequence<byte> Payload { get; }
        public ReadOnlySequence<byte> Tail { get; }

        public override string ToString()
        {
            return string.Format("[{0}: {1}={2}, {3}={4}, {5}={6}]", nameof(LengthBasedProtocolMessage),
                nameof(Head), Head.ToHex(), nameof(Payload), Payload.ToHex(), nameof(Tail), Tail.ToHex());
        }

        public string ToHex()
        {
            return string.Format("{0}{1}{2}", Head.ToHex(), Payload.ToHex(), Tail.ToHex());
        }

    }
}
