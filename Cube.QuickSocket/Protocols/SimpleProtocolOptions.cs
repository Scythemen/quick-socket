namespace Cube.QuickSocket.Protocols
{
    /// <summary>
    /// Indicate how to encode/decode the message. <br/>
    /// Message: [Delimiter][Head...][Length-Field][Payload...][Tail...]
    /// </summary>
    public class SimpleProtocolOptions
    {
        /// <summary>
        /// limit the length of the message
        /// </summary>
        public int MaxLength { get; set; } = 2048;

        /// <summary>
        /// the delimiter/start of the message, default=empty no delimiter<br/>
        /// it's a hex-string of bytes, two chars represent one byte. <br/>
        /// e.g:<br/>
        /// if the delimiter is '$', then Delimiter=24. <br/>
        /// if the delimiter is 'Start', then Delimiter=5374617274
        /// </summary>
        public string Delimiter { get; set; } = string.Empty;

        /// <summary>
        /// total bytes of the head, it begins from the end of delimiter. if no head set to zerro.
        /// </summary>
        public int HeadBytes { get; set; } = 0;

        /// <summary>
        /// total bytes of the length-field, it begins from the end of head. if no length-field set to zerro.
        /// </summary>
        public int LengthFieldBytes { get; set; } = 0;

        /// <summary>
        /// default=true,
        /// the most significant byte in the left(smaller index of the array), 
        /// the least significant byte int the right(higher index of the array)
        /// </summary>
        public bool LengthFieldBigEndian { get; set; } = true;

        /// <summary>
        /// total bytes of the tail, it begins from the end of *payload*. if no tail set to zerro.
        /// </summary>
        public int TailBytes { get; set; } = 0;

    }
}
