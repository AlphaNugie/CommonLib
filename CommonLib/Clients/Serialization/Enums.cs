using System;

namespace CommonLib.Clients.Serialization
{
    public class Enums
    {
        public enum PrefixStyle
        {
            None,
            Base128,
            Fixed32,
            Fixed32BigEndian
        }

        public enum WireType
        {
            None = -1,
            Variant,
            Fixed64,
            String,
            StartGroup,
            EndGroup,
            Fixed32,
            SignedVariant = 8
        }
        internal enum ProtoTypeCode
        {
            Empty,
            Unknown,
            Boolean = 3,
            Char,
            SByte,
            Byte,
            Int16,
            UInt16,
            Int32,
            UInt32,
            Int64,
            UInt64,
            Single,
            Double,
            Decimal,
            DateTime,
            String = 18,
            TimeSpan = 100,
            ByteArray,
            Guid,
            Uri,
            Type
        }

        public enum DataFormat
        {
            Default,
            ZigZag,
            TwosComplement,
            FixedSize,
            Group
        }
    }
}
