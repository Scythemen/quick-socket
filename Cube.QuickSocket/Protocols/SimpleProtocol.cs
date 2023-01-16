using System.Buffers;

namespace Cube.QuickSocket.Protocols
{
    /// <summary>
    /// Message: [Delimiter][Head...][Length-Field][Payload...][Tail...]
    /// </summary>
    public class SimpleProtocol
    {
        private readonly SimpleProtocolOptions options;
        private readonly byte[] delimiterBytes;

        public SimpleProtocol(SimpleProtocolOptions options)
        {
            ValidateOptions(options);
            this.options = options;
            this.delimiterBytes = options.Delimiter.HexToBytes();
        }

        private void ValidateOptions(SimpleProtocolOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(SimpleProtocolOptions));
            }
            if ((options.HeadBytes + options.LengthFieldBytes + options.TailBytes) > options.MaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(SimpleProtocolOptions.MaxLength),
                    $"Invalid message length, MaxLength={options.MaxLength} ");
            }
            if (string.IsNullOrWhiteSpace(options.Delimiter) && options.HeadBytes < 1 && options.LengthFieldBytes < 1 && options.TailBytes < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(SimpleProtocolOptions), "Invalid options, no field has been set");
            }
        }

        public bool Decode(
            in ReadOnlySequence<byte> input,
            ref SequencePosition consumed,
            ref SequencePosition examined,
            out SimpleProtocolMessage message)
        {
            var reader = new SequenceReader<byte>(input);

            consumed = reader.Position;
            examined = reader.Position;
            message = default;

            if (reader.Remaining < 1)
            {
                return false;
            }

            // look for the delimiter
            if (delimiterBytes.Length > 0)
            {
                bool matchHead = Find(ref reader, delimiterBytes);
                if (!matchHead)
                {
                    return false;
                }
                else
                {
                    reader.Advance(delimiterBytes.Length);
                }
            }

            // parse head
            var head = ReadOnlySequence<byte>.Empty;
            if (options.HeadBytes > 0 && options.HeadBytes <= reader.Remaining)
            {
                head = input.Slice(reader.Position, options.HeadBytes);
                reader.Advance(options.HeadBytes);
            }

            // parse payload
            var payload = ReadOnlySequence<byte>.Empty;
            if (options.LengthFieldBytes > 0)
            {
                // caculate the length 
                var lengthSequence = input.Slice(reader.Position, options.LengthFieldBytes);
                int length = 0;
                int ix = 0;
                foreach (var memory in lengthSequence)
                {
                    foreach (var b in memory.Span)
                    {
                        if (options.LengthFieldBigEndian)
                        {
                            var mv = (lengthSequence.Length - ix - 1) * 8;
                            length = length | (b << (int)mv);
                        }
                        else
                        {
                            var mv = ix * 8;
                            length = length + (b << (int)mv);
                        }

                        ix++;
                    }
                }

                reader.Advance(options.LengthFieldBytes);

                if (length > options.MaxLength)
                {
                    throw new ArgumentOutOfRangeException(nameof(SimpleProtocolOptions.MaxLength), $"Message too long, {length} bytes, MaxLength={options.MaxLength}");
                }

                // no enough bytes for payload
                if (reader.Remaining < length)
                {
                    return false;
                }

                payload = input.Slice(reader.Position, length);
                reader.Advance(length);
            }

            // look for the Tail
            var tail = ReadOnlySequence<byte>.Empty;
            if (options.TailBytes > 0)
            {
                tail = input.Slice(reader.Position, options.TailBytes);
                reader.Advance(options.TailBytes);
            }

            // only delimiter & payload
            if (delimiterBytes.Length > 0 && options.HeadBytes < 1 && options.LengthFieldBytes < 1 && options.TailBytes < 1)
            {
                var from = reader.Position;
                if (Find(ref reader, delimiterBytes))
                {
                    payload = input.Slice(from, reader.Position);
                }
            }

            if (options.MaxLength < (head.Length + payload.Length + tail.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(SimpleProtocolOptions.MaxLength), $"Message too long, MaxLength={options.MaxLength}");
            }

            message = new SimpleProtocolMessage(head, payload, tail);

            consumed = reader.Position;
            examined = reader.Position;
            return true;
        }


        private bool Find(ref SequenceReader<byte> reader, byte[] delimiter)
        {
            bool matched = true;

            while (reader.Remaining >= delimiter.Length)
            {
                if (reader.TryAdvanceTo(delimiter[0], false))
                {
                    matched = true;
                    for (int i = 0; i < delimiterBytes.Length; i++)
                    {
                        if (!reader.TryPeek(i, out byte h) || h != delimiterBytes[i])
                        {
                            matched = false;
                            break;
                        }
                    }
                    if (!matched)
                    {
                        reader.Advance(1);
                        continue; // next round
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    matched = false;
                    break;
                }
            }

            return matched;
        }

        public void Encode(SimpleProtocolMessage message, IBufferWriter<byte> output)
        {
            if ((message.Head.Length + message.Payload.Length + message.Tail.Length) > options.MaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(SimpleProtocolOptions.MaxLength),
                    $"Message too long, {message.Head.Length + message.Payload.Length + message.Tail.Length} bytes, MaxLength={options.MaxLength}.");
            }

            // write delimiter
            if (delimiterBytes.Length > 0)
            {
                output.Write(delimiterBytes);
            }

            // write head
            if (options.HeadBytes > 0)
            {
                if (options.HeadBytes != message.Head.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(SimpleProtocolMessage.Head),
                        $"The length of {nameof(SimpleProtocolMessage.Head)} not match, {message.Head.Length} bytes, {nameof(SimpleProtocolOptions.HeadBytes)}={options.HeadBytes}");
                }

                foreach (var memory in message.Head)
                {
                    output.Write(memory.Span);
                }
            }

            // write length-field
            if (options.LengthFieldBytes > 0)
            {
                var lengthBuffer = output.GetSpan(options.LengthFieldBytes);
                if (options.LengthFieldBigEndian)
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
            }

            // write payload
            foreach (var memory in message.Payload)
            {
                output.Write(memory.Span);
            }

            // write tail
            if (options.TailBytes > 0)
            {
                if (options.TailBytes != message.Tail.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(SimpleProtocolMessage.Tail),
                        $"The length of {nameof(SimpleProtocolMessage.Tail)} not match, {message.Tail.Length} bytes, {nameof(SimpleProtocolOptions.TailBytes)}={options.TailBytes}");
                }
                foreach (var memory in message.Tail)
                {
                    output.Write(memory.Span);
                }
            }

            // only delimiter & payload
            if (delimiterBytes.Length > 0 && options.HeadBytes < 1 && options.LengthFieldBytes < 1 && options.TailBytes < 1)
            {
                // add the delimiter to the end 
                output.Write(delimiterBytes);
            }
        }

    }
}
