namespace Cube.QuickSocket.Protocols
{
    public class LengthBasedProtocolOptions
    {
        /// <summary>
        /// the head of the message, default =null, no head.
        /// it's a hex-string of bytes
        /// </summary>
        public string Head { get; set; } = string.Empty;

        /// <summary>
        /// total bytes of the head-field
        /// </summary>
        public int HeadBytes { get; set; } = 0;

        /// <summary>
        /// the offset of the length-field, it begins from the start of the message 
        /// </summary>
        public int LengthFieldOffset { get; set; } = 0;

        /// <summary>
        /// total bytes of the length-field
        /// </summary>
        public int LengthFieldBytes { get; set; } = 1;

        /// <summary>
        /// default=true,
        /// the most significant byte in the left(smaller index of the array), 
        /// the least significant byte int the right(higher index of the array)
        /// </summary>
        public bool LengthFieldIsBigEndian { get; set; } = true;

        /// <summary>
        /// total bytes of the tail, it begins from the end of payload
        /// </summary>
        public int TailBytes { get; set; } = 0;

    }
}
