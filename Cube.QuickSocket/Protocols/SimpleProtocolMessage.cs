using System.Buffers;

namespace Cube.QuickSocket.Protocols
{
    public class SimpleProtocolMessage : ICloneable
    {
        public SimpleProtocolMessage(byte[] head, byte[] payload, byte[] tail)
            : this(head == null ? ReadOnlySequence<byte>.Empty : new ReadOnlySequence<byte>(head),
                  payload == null ? ReadOnlySequence<byte>.Empty : new ReadOnlySequence<byte>(payload),
                  tail == null ? ReadOnlySequence<byte>.Empty : new ReadOnlySequence<byte>(tail))
        {
        }

        public SimpleProtocolMessage(
            ReadOnlySequence<byte> head,
            ReadOnlySequence<byte> payload,
            ReadOnlySequence<byte> tail)
        {
            Payload = payload;
            Head = head;
            Tail = tail;
        }

        public ReadOnlySequence<byte> Head { get; }
        public ReadOnlySequence<byte> Payload { get; }
        public ReadOnlySequence<byte> Tail { get; }

        public object Clone()
        {
            return new SimpleProtocolMessage(Head, Payload, Tail);
        }

        public string ToHex()
        {
            return string.Format("[{6}: {0}={1}, {2}={3}, {4}={5}]",
                nameof(Head), Head.ToHex(),
                nameof(Payload), Payload.ToHex(),
                nameof(Tail), Tail.ToHex(),
                nameof(SimpleProtocolMessage));
        }

    }
}
