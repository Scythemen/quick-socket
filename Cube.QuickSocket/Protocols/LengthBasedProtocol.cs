using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime;

namespace Cube.QuickSocket.Protocols
{
    public class LengthBasedProtocol
    {
        // message: [......] [head: 0x00,0x01,0x02...] [length-field: 0x11,0x12,0x13...] [payload: 0x22,0x23,0x24....] [tail: 0x33,0x34,0x35....]


        private readonly LengthBasedProtocolOptions options;
        private readonly int miniLength = 0;
        private readonly byte[] headBytes;

        public LengthBasedProtocol(LengthBasedProtocolOptions options)
        {
            ValidateOptions(options);
            this.options = options;
            this.miniLength = (options.HeadBytes < 0 ? 0 : options.HeadBytes) + 1 + (options.TailBytes < 0 ? 0 : options.TailBytes);
            this.headBytes = options.Head.HexToBytes();
        }

        private void ValidateOptions(LengthBasedProtocolOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(LengthBasedProtocolOptions));
            }
            if (options.Head?.Length > 0 && (options.HeadBytes < 1 || options.Head?.Length / 2 > options.HeadBytes))
            {
                throw new ArgumentOutOfRangeException(nameof(LengthBasedProtocolOptions.HeadBytes),
                    "the total bytes of the Head should be greater than ZERO, when the Head is NOT empty");
            }
            if (options.LengthFieldBytes < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(LengthBasedProtocolOptions.LengthFieldBytes),
                    "the total bytes of the LengthField should be greater than ZERO");
            }
            if (options.Head?.Length > 0 && options.LengthFieldOffset < options.HeadBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(LengthBasedProtocolOptions.LengthFieldOffset),
                    "the LengthFieldOffset should be equals/greater than HeadBytes");
            }
        }

        public bool TryParseMessage(
            in ReadOnlySequence<byte> input,
            ref SequencePosition consumed,
            ref SequencePosition examined,
            out LengthBasedProtocolMessage message)
        {
            var reader = new SequenceReader<byte>(input);

            if (reader.Remaining < miniLength)
            {
                consumed = reader.Position;
                examined = reader.Position;
                message = default;
                return false;
            }

            var start = reader.Position;
            consumed = start;
            examined = start;

            // look for the Head
            var head = ReadOnlySequence<byte>.Empty;
            if (this.headBytes.Length > 0)
            {
                bool matchHead = true;
                while (reader.Remaining >= options.HeadBytes)
                {
                    if (reader.TryAdvanceTo(this.headBytes[0], false))
                    {
                        matchHead = true;
                        start = reader.Position;
                        for (int i = 0; i < this.headBytes.Length; i++)
                        {
                            if (!reader.TryRead(out byte h) || h != this.headBytes[i])
                            {
                                matchHead = false;
                                break;
                            }
                        }
                        if (!matchHead)
                        {
                            reader.Advance(1);
                            continue; // next round
                        }
                        else
                        {
                            head = input.Slice(start, options.HeadBytes);
                            break;
                        }
                    }
                }

                if (!matchHead)
                {
                    consumed = reader.Position;
                    examined = reader.Position;
                    message = default;
                    return false;
                }
            }

            // caculate the length of payload
            var lengthPosition = input.GetPosition(options.LengthFieldOffset, start);
            var lengthSequence = input.Slice(lengthPosition, options.LengthFieldBytes);
            int length = 0;
            int ix = 0;
            foreach (var memory in lengthSequence)
            {
                foreach (var b in memory.Span)
                {
                    if (options.LengthFieldIsBigEndian)
                    {
                        var mv = ix * 8;
                        length = length + (b << (int)mv);
                    }
                    else
                    {
                        var mv = (lengthSequence.Length - ix) * 8;
                        length = length + (b << (int)mv);
                    }

                    ix++;
                }
            }

            reader.Advance(options.LengthFieldBytes);

            // no enough bytes for payload
            if (reader.Remaining < length)
            {
                message = default;
                return false;
            }

            var payloadPosition = input.GetPosition(options.LengthFieldOffset + options.LengthFieldBytes, start);
            var payload = input.Slice(payloadPosition, length);

            reader.Advance(length);

            // look for the Tail
            var tail = ReadOnlySequence<byte>.Empty;
            if (options.TailBytes > 0)
            {
                tail = input.Slice(reader.Position, options.TailBytes);
                reader.Advance(options.TailBytes);
            }

            message = new LengthBasedProtocolMessage(payload, head, tail);

            consumed = reader.Position;
            examined = reader.Position;
            return true;
        }



        public void WriteMessage(LengthBasedProtocolMessage message, IBufferWriter<byte> output)
        {
            // write the head
            foreach (var memory in message.Head)
            {
                output.Write(memory.Span);
            }

            // write the length of payload
            var lengthBuffer = output.GetSpan(options.LengthFieldBytes);
            if (options.LengthFieldIsBigEndian)
            {
                for (int i = 0; i < options.LengthFieldBytes; i++)
                {
                    var mv = (options.LengthFieldBytes - i - 1) * 8;
                    lengthBuffer[i] = (byte)(message.Payload.Length >> mv);
                }
            }
            else
            {
                for (int i = 0; i < options.LengthFieldBytes; i++)
                {
                    var mv = i * 8;
                    lengthBuffer[i] = (byte)((message.Payload.Length >> mv) & 0xff);
                }
            }
            output.Advance(options.LengthFieldBytes);

            // write the payload
            foreach (var memory in message.Payload)
            {
                output.Write(memory.Span);
            }

            // write the tail
            foreach (var memory in message.Tail)
            {
                output.Write(memory.Span);
            }

        }
    }


}
