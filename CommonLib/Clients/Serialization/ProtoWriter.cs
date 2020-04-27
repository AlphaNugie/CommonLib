using CommonLib.Clients.Serialization.Meta;
using System;
using System.IO;
using System.Text;
using static CommonLib.Clients.Serialization.Enums;

namespace CommonLib.Clients.Serialization
{
    public sealed class ProtoWriter : IDisposable
    {
        private const int RecursionCheckDepth = 25;
        private Stream dest;
        private TypeModel model;
        private readonly NetObjectCache netCache = new NetObjectCache();
        private int fieldNumber;
        private int flushLock;
        private WireType wireType;
        private int depth;
        private MutableList recursionStack;
        private readonly SerializationContext context;
        private byte[] ioBuffer;
        private int ioIndex;
        private int position;
        private static readonly UTF8Encoding encoding = new UTF8Encoding();
        private int packedFieldNumber;
        internal NetObjectCache NetCache
        {
            get
            {
                return this.netCache;
            }
        }
        internal WireType WireType
        {
            get
            {
                return this.wireType;
            }
        }
        public SerializationContext Context
        {
            get
            {
                return this.context;
            }
        }
        public TypeModel Model
        {
            get
            {
                return this.model;
            }
        }
        public static void WriteObject(object value, int key, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (writer.model == null)
            {
                throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
            }
            SubItemToken token = ProtoWriter.StartSubItem(value, writer);
            if (key >= 0)
            {
                writer.model.Serialize(key, value, writer);
            }
            else
            {
                if (writer.model == null || !writer.model.TrySerializeAuxiliaryType(writer, value.GetType(), DataFormat.Default, 1, value, false))
                {
                    TypeModel.ThrowUnexpectedType(value.GetType());
                }
            }
            ProtoWriter.EndSubItem(token, writer);
        }
        public static void WriteRecursionSafeObject(object value, int key, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (writer.model == null)
            {
                throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
            }
            SubItemToken token = ProtoWriter.StartSubItem(null, writer);
            writer.model.Serialize(key, value, writer);
            ProtoWriter.EndSubItem(token, writer);
        }
        internal static void WriteObject(object value, int key, ProtoWriter writer, PrefixStyle style, int fieldNumber)
        {
            if (writer.model == null)
            {
                throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
            }
            if (writer.wireType != WireType.None)
            {
                throw ProtoWriter.CreateException(writer);
            }
            switch (style)
            {
                case PrefixStyle.Base128:
                    writer.wireType = WireType.String;
                    writer.fieldNumber = fieldNumber;
                    if (fieldNumber > 0)
                    {
                        ProtoWriter.WriteHeaderCore(fieldNumber, WireType.String, writer);
                    }
                    break;
                case PrefixStyle.Fixed32:
                case PrefixStyle.Fixed32BigEndian:
                    writer.fieldNumber = 0;
                    writer.wireType = WireType.Fixed32;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("style");
            }
            SubItemToken token = ProtoWriter.StartSubItem(value, writer, true);
            if (key < 0)
            {
                if (!writer.model.TrySerializeAuxiliaryType(writer, value.GetType(), DataFormat.Default, 1, value, false))
                {
                    TypeModel.ThrowUnexpectedType(value.GetType());
                }
            }
            else
            {
                writer.model.Serialize(key, value, writer);
            }
            ProtoWriter.EndSubItem(token, writer, style);
        }
        internal int GetTypeKey(ref Type type)
        {
            return this.model.GetKey(ref type);
        }
        public static void WriteFieldHeader(int fieldNumber, WireType wireType, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (writer.wireType != WireType.None)
            {
                throw new InvalidOperationException(string.Concat(new string[]
                {
                    "Cannot write a ",
                    wireType.ToString(),
                    " header until the ",
                    writer.wireType.ToString(),
                    " data has been written"
                }));
            }
            if (fieldNumber < 0)
            {
                throw new ArgumentOutOfRangeException("fieldNumber");
            }
            if (writer.packedFieldNumber == 0)
            {
                writer.fieldNumber = fieldNumber;
                writer.wireType = wireType;
                ProtoWriter.WriteHeaderCore(fieldNumber, wireType, writer);
                return;
            }
            if (writer.packedFieldNumber == fieldNumber)
            {
                switch (wireType)
                {
                    case WireType.Variant:
                    case WireType.Fixed64:
                        break;
                    default:
                        if (wireType != WireType.Fixed32 && wireType != WireType.SignedVariant)
                        {
                            throw new InvalidOperationException("Wire-type cannot be encoded as packed: " + wireType.ToString());
                        }
                        break;
                }
                writer.fieldNumber = fieldNumber;
                writer.wireType = wireType;
                return;
            }
            throw new InvalidOperationException("Field mismatch during packed encoding; expected " + writer.packedFieldNumber.ToString() + " but received " + fieldNumber.ToString());
        }
        internal static void WriteHeaderCore(int fieldNumber, WireType wireType, ProtoWriter writer)
        {
            uint header = (uint)(fieldNumber << 3 | (int)(wireType & (WireType)7));
            ProtoWriter.WriteUInt32Variant(header, writer);
        }
        public static void WriteBytes(byte[] data, ProtoWriter writer)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            ProtoWriter.WriteBytes(data, 0, data.Length, writer);
        }
        public static void WriteBytes(byte[] data, int offset, int length, ProtoWriter writer)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            switch (writer.wireType)
            {
                case WireType.Fixed64:
                    if (length != 8)
                    {
                        throw new ArgumentException("length");
                    }
                    goto IL_AE;
                case WireType.String:
                    ProtoWriter.WriteUInt32Variant((uint)length, writer);
                    writer.wireType = WireType.None;
                    if (length == 0)
                    {
                        return;
                    }
                    if (writer.flushLock == 0 && length > writer.ioBuffer.Length)
                    {
                        ProtoWriter.Flush(writer);
                        writer.dest.Write(data, offset, length);
                        writer.position += length;
                        return;
                    }
                    goto IL_AE;
                case WireType.Fixed32:
                    if (length != 4)
                    {
                        throw new ArgumentException("length");
                    }
                    goto IL_AE;
            }
            throw ProtoWriter.CreateException(writer);
        IL_AE:
            ProtoWriter.DemandSpace(length, writer);
            Helpers.BlockCopy(data, offset, writer.ioBuffer, writer.ioIndex, length);
            ProtoWriter.IncrementedAndReset(length, writer);
        }
        private static void CopyRawFromStream(Stream source, ProtoWriter writer)
        {
            byte[] buffer = writer.ioBuffer;
            int space = buffer.Length - writer.ioIndex;
            int bytesRead = 1;
            while (space > 0 && (bytesRead = source.Read(buffer, writer.ioIndex, space)) > 0)
            {
                writer.ioIndex += bytesRead;
                writer.position += bytesRead;
                space -= bytesRead;
            }
            if (bytesRead <= 0)
            {
                return;
            }
            if (writer.flushLock == 0)
            {
                ProtoWriter.Flush(writer);
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.dest.Write(buffer, 0, bytesRead);
                    writer.position += bytesRead;
                }
                return;
            }
            while (true)
            {
                ProtoWriter.DemandSpace(128, writer);
                if ((bytesRead = source.Read(writer.ioBuffer, writer.ioIndex, writer.ioBuffer.Length - writer.ioIndex)) <= 0)
                {
                    break;
                }
                writer.position += bytesRead;
                writer.ioIndex += bytesRead;
            }
        }
        private static void IncrementedAndReset(int length, ProtoWriter writer)
        {
            writer.ioIndex += length;
            writer.position += length;
            writer.wireType = WireType.None;
        }
        public static SubItemToken StartSubItem(object instance, ProtoWriter writer)
        {
            return ProtoWriter.StartSubItem(instance, writer, false);
        }
        private void CheckRecursionStackAndPush(object instance)
        {
            if (this.recursionStack == null)
            {
                this.recursionStack = new MutableList();
            }
            else
            {
                int hitLevel;
                if (instance != null && (hitLevel = this.recursionStack.IndexOfReference(instance)) >= 0)
                {
                    throw new ProtoException("Possible recursion detected (offset: " + (this.recursionStack.Count - hitLevel).ToString() + " level(s)): " + instance.ToString());
                }
            }
            this.recursionStack.Add(instance);
        }
        private void PopRecursionStack()
        {
            this.recursionStack.RemoveLast();
        }
        private static SubItemToken StartSubItem(object instance, ProtoWriter writer, bool allowFixed)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (++writer.depth > 25)
            {
                writer.CheckRecursionStackAndPush(instance);
            }
            if (writer.packedFieldNumber != 0)
            {
                throw new InvalidOperationException("Cannot begin a sub-item while performing packed encoding");
            }
            switch (writer.wireType)
            {
                case WireType.String:
                    writer.wireType = WireType.None;
                    ProtoWriter.DemandSpace(32, writer);
                    writer.flushLock++;
                    writer.position++;
                    return new SubItemToken(writer.ioIndex++);
                case WireType.StartGroup:
                    writer.wireType = WireType.None;
                    return new SubItemToken(-writer.fieldNumber);
                case WireType.Fixed32:
                    {
                        if (!allowFixed)
                        {
                            throw ProtoWriter.CreateException(writer);
                        }
                        ProtoWriter.DemandSpace(32, writer);
                        writer.flushLock++;
                        SubItemToken token = new SubItemToken(writer.ioIndex);
                        ProtoWriter.IncrementedAndReset(4, writer);
                        return token;
                    }
            }
            throw ProtoWriter.CreateException(writer);
        }
        public static void EndSubItem(SubItemToken token, ProtoWriter writer)
        {
            ProtoWriter.EndSubItem(token, writer, PrefixStyle.Base128);
        }
        private static void EndSubItem(SubItemToken token, ProtoWriter writer, PrefixStyle style)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (writer.wireType != WireType.None)
            {
                throw ProtoWriter.CreateException(writer);
            }
            int value = token.value;
            if (writer.depth <= 0)
            {
                throw ProtoWriter.CreateException(writer);
            }
            if (writer.depth-- > 25)
            {
                writer.PopRecursionStack();
            }
            writer.packedFieldNumber = 0;
            if (value < 0)
            {
                ProtoWriter.WriteHeaderCore(-value, WireType.EndGroup, writer);
                writer.wireType = WireType.None;
                return;
            }
            switch (style)
            {
                case PrefixStyle.Base128:
                    {
                        int len = writer.ioIndex - value - 1;
                        int offset = 0;
                        uint tmp = (uint)len;
                        while ((tmp >>= 7) != 0u)
                        {
                            offset++;
                        }
                        if (offset == 0)
                        {
                            writer.ioBuffer[value] = (byte)(len & 127);
                        }
                        else
                        {
                            ProtoWriter.DemandSpace(offset, writer);
                            byte[] blob = writer.ioBuffer;
                            Helpers.BlockCopy(blob, value + 1, blob, value + 1 + offset, len);
                            tmp = (uint)len;
                            do
                            {
                                blob[value++] = (byte)((tmp & 127u) | 128u);
                            }
                            while ((tmp >>= 7) != 0u);
                            blob[value - 1] = (byte)((int)blob[value - 1] & -129);
                            writer.position += offset;
                            writer.ioIndex += offset;
                        }
                        break;
                    }
                case PrefixStyle.Fixed32:
                    {
                        int len = writer.ioIndex - value - 4;
                        ProtoWriter.WriteInt32ToBuffer(len, writer.ioBuffer, value);
                        break;
                    }
                case PrefixStyle.Fixed32BigEndian:
                    {
                        int len = writer.ioIndex - value - 4;
                        byte[] buffer = writer.ioBuffer;
                        ProtoWriter.WriteInt32ToBuffer(len, buffer, value);
                        byte b = buffer[value];
                        buffer[value] = buffer[value + 3];
                        buffer[value + 3] = b;
                        b = buffer[value + 1];
                        buffer[value + 1] = buffer[value + 2];
                        buffer[value + 2] = b;
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException("style");
            }
            if (--writer.flushLock == 0 && writer.ioIndex >= 1024)
            {
                ProtoWriter.Flush(writer);
            }
        }
        public ProtoWriter(Stream dest, TypeModel model, SerializationContext context)
        {
            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }
            if (!dest.CanWrite)
            {
                throw new ArgumentException("Cannot write to stream", "dest");
            }
            this.dest = dest;
            this.ioBuffer = BufferPool.GetBuffer();
            this.model = model;
            this.wireType = WireType.None;
            if (context == null)
            {
                context = SerializationContext.Default;
            }
            else
            {
                context.Freeze();
            }
            this.context = context;
        }
        void IDisposable.Dispose()
        {
            this.Dispose();
        }
        private void Dispose()
        {
            if (this.dest != null)
            {
                ProtoWriter.Flush(this);
                this.dest = null;
            }
            this.model = null;
            BufferPool.ReleaseBufferToPool(ref this.ioBuffer);
        }
        internal static int GetPosition(ProtoWriter writer)
        {
            return writer.position;
        }
        private static void DemandSpace(int required, ProtoWriter writer)
        {
            if (writer.ioBuffer.Length - writer.ioIndex < required)
            {
                if (writer.flushLock == 0)
                {
                    ProtoWriter.Flush(writer);
                    if (writer.ioBuffer.Length - writer.ioIndex >= required)
                    {
                        return;
                    }
                }
                BufferPool.ResizeAndFlushLeft(ref writer.ioBuffer, required + writer.ioIndex, 0, writer.ioIndex);
            }
        }
        public void Close()
        {
            if (this.depth != 0 || this.flushLock != 0)
            {
                throw new InvalidOperationException("Unable to close stream in an incomplete state");
            }
            this.Dispose();
        }
        internal void CheckDepthFlushlock()
        {
            if (this.depth != 0 || this.flushLock != 0)
            {
                throw new InvalidOperationException("The writer is in an incomplete state");
            }
        }
        internal static void Flush(ProtoWriter writer)
        {
            if (writer.flushLock == 0 && writer.ioIndex != 0)
            {
                writer.dest.Write(writer.ioBuffer, 0, writer.ioIndex);
                writer.ioIndex = 0;
            }
        }
        private static void WriteUInt32Variant(uint value, ProtoWriter writer)
        {
            ProtoWriter.DemandSpace(5, writer);
            int count = 0;
            do
            {
                writer.ioBuffer[writer.ioIndex++] = (byte)((value & 127u) | 128u);
                count++;
            }
            while ((value >>= 7) != 0u);
            byte[] expr_4B_cp_0 = writer.ioBuffer;
            int expr_4B_cp_1 = writer.ioIndex - 1;
            expr_4B_cp_0[expr_4B_cp_1] &= 127;
            writer.position += count;
        }
        internal static uint Zig(int value)
        {
            return (uint)(value << 1 ^ value >> 31);
        }
        internal static ulong Zig(long value)
        {
            return (ulong)(value << 1 ^ value >> 63);
        }
        private static void WriteUInt64Variant(ulong value, ProtoWriter writer)
        {
            ProtoWriter.DemandSpace(10, writer);
            int count = 0;
            do
            {
                writer.ioBuffer[writer.ioIndex++] = (byte)((value & 127uL) | 128uL);
                count++;
            }
            while ((value >>= 7) != 0uL);
            byte[] expr_50_cp_0 = writer.ioBuffer;
            int expr_50_cp_1 = writer.ioIndex - 1;
            expr_50_cp_0[expr_50_cp_1] &= 127;
            writer.position += count;
        }
        public static void WriteString(string value, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (writer.wireType != WireType.String)
            {
                throw ProtoWriter.CreateException(writer);
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.Length == 0)
            {
                ProtoWriter.WriteUInt32Variant(0u, writer);
                writer.wireType = WireType.None;
                return;
            }
            int predicted = ProtoWriter.encoding.GetByteCount(value);
            ProtoWriter.WriteUInt32Variant((uint)predicted, writer);
            ProtoWriter.DemandSpace(predicted, writer);
            int actual = ProtoWriter.encoding.GetBytes(value, 0, value.Length, writer.ioBuffer, writer.ioIndex);
            ProtoWriter.IncrementedAndReset(actual, writer);
        }
        public static void WriteUInt64(ulong value, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            WireType wireType = writer.wireType;
            switch (wireType)
            {
                case WireType.Variant:
                    ProtoWriter.WriteUInt64Variant(value, writer);
                    writer.wireType = WireType.None;
                    return;
                case WireType.Fixed64:
                    ProtoWriter.WriteInt64((long)value, writer);
                    return;
                default:
                    if (wireType != WireType.Fixed32)
                    {
                        throw ProtoWriter.CreateException(writer);
                    }
                    ProtoWriter.WriteUInt32(checked((uint)value), writer);
                    return;
            }
        }
        public static void WriteInt64(long value, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            WireType wireType = writer.wireType;
            switch (wireType)
            {
                case WireType.Variant:
                    {
                        if (value >= 0L)
                        {
                            ProtoWriter.WriteUInt64Variant((ulong)value, writer);
                            writer.wireType = WireType.None;
                            return;
                        }
                        ProtoWriter.DemandSpace(10, writer);
                        byte[] buffer = writer.ioBuffer;
                        int index = writer.ioIndex;
                        buffer[index] = (byte)(value | 128L);
                        buffer[index + 1] = (byte)((int)(value >> 7) | 128);
                        buffer[index + 2] = (byte)((int)(value >> 14) | 128);
                        buffer[index + 3] = (byte)((int)(value >> 21) | 128);
                        buffer[index + 4] = (byte)((int)(value >> 28) | 128);
                        buffer[index + 5] = (byte)((int)(value >> 35) | 128);
                        buffer[index + 6] = (byte)((int)(value >> 42) | 128);
                        buffer[index + 7] = (byte)((int)(value >> 49) | 128);
                        buffer[index + 8] = (byte)((int)(value >> 56) | 128);
                        buffer[index + 9] = 1;
                        ProtoWriter.IncrementedAndReset(10, writer);
                        return;
                    }
                case WireType.Fixed64:
                    {
                        ProtoWriter.DemandSpace(8, writer);
                        byte[] buffer = writer.ioBuffer;
                        int index = writer.ioIndex;
                        buffer[index] = (byte)value;
                        buffer[index + 1] = (byte)(value >> 8);
                        buffer[index + 2] = (byte)(value >> 16);
                        buffer[index + 3] = (byte)(value >> 24);
                        buffer[index + 4] = (byte)(value >> 32);
                        buffer[index + 5] = (byte)(value >> 40);
                        buffer[index + 6] = (byte)(value >> 48);
                        buffer[index + 7] = (byte)(value >> 56);
                        ProtoWriter.IncrementedAndReset(8, writer);
                        return;
                    }
                default:
                    if (wireType == WireType.Fixed32)
                    {
                        ProtoWriter.WriteInt32(checked((int)value), writer);
                        return;
                    }
                    if (wireType != WireType.SignedVariant)
                    {
                        throw ProtoWriter.CreateException(writer);
                    }
                    ProtoWriter.WriteUInt64Variant(ProtoWriter.Zig(value), writer);
                    writer.wireType = WireType.None;
                    return;
            }
        }
        public static void WriteUInt32(uint value, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            WireType wireType = writer.wireType;
            switch (wireType)
            {
                case WireType.Variant:
                    ProtoWriter.WriteUInt32Variant(value, writer);
                    writer.wireType = WireType.None;
                    return;
                case WireType.Fixed64:
                    ProtoWriter.WriteInt64((long)value, writer);
                    return;
                default:
                    if (wireType == WireType.Fixed32)
                    {
                        ProtoWriter.WriteInt32((int)value, writer);
                        return;
                    }
                    throw ProtoWriter.CreateException(writer);
            }
        }
        public static void WriteInt16(short value, ProtoWriter writer)
        {
            ProtoWriter.WriteInt32((int)value, writer);
        }
        public static void WriteUInt16(ushort value, ProtoWriter writer)
        {
            ProtoWriter.WriteUInt32((uint)value, writer);
        }
        public static void WriteByte(byte value, ProtoWriter writer)
        {
            ProtoWriter.WriteUInt32((uint)value, writer);
        }
        public static void WriteSByte(sbyte value, ProtoWriter writer)
        {
            ProtoWriter.WriteInt32((int)value, writer);
        }
        private static void WriteInt32ToBuffer(int value, byte[] buffer, int index)
        {
            buffer[index] = (byte)value;
            buffer[index + 1] = (byte)(value >> 8);
            buffer[index + 2] = (byte)(value >> 16);
            buffer[index + 3] = (byte)(value >> 24);
        }
        public static void WriteInt32(int value, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            WireType wireType = writer.wireType;
            switch (wireType)
            {
                case WireType.Variant:
                    {
                        if (value >= 0)
                        {
                            ProtoWriter.WriteUInt32Variant((uint)value, writer);
                            writer.wireType = WireType.None;
                            return;
                        }
                        ProtoWriter.DemandSpace(10, writer);
                        byte[] buffer = writer.ioBuffer;
                        int index = writer.ioIndex;
                        buffer[index] = (byte)(value | 128);
                        buffer[index + 1] = (byte)(value >> 7 | 128);
                        buffer[index + 2] = (byte)(value >> 14 | 128);
                        buffer[index + 3] = (byte)(value >> 21 | 128);
                        buffer[index + 4] = (byte)(value >> 28 | 128);
                        buffer[index + 5] = (buffer[index + 6] = (buffer[index + 7] = (buffer[index + 8] = 255)));
                        buffer[index + 9] = 1;
                        ProtoWriter.IncrementedAndReset(10, writer);
                        return;
                    }
                case WireType.Fixed64:
                    {
                        ProtoWriter.DemandSpace(8, writer);
                        byte[] buffer = writer.ioBuffer;
                        int index = writer.ioIndex;
                        buffer[index] = (byte)value;
                        buffer[index + 1] = (byte)(value >> 8);
                        buffer[index + 2] = (byte)(value >> 16);
                        buffer[index + 3] = (byte)(value >> 24);
                        buffer[index + 4] = (buffer[index + 5] = (buffer[index + 6] = (buffer[index + 7] = 0)));
                        ProtoWriter.IncrementedAndReset(8, writer);
                        return;
                    }
                default:
                    if (wireType == WireType.Fixed32)
                    {
                        ProtoWriter.DemandSpace(4, writer);
                        ProtoWriter.WriteInt32ToBuffer(value, writer.ioBuffer, writer.ioIndex);
                        ProtoWriter.IncrementedAndReset(4, writer);
                        return;
                    }
                    if (wireType != WireType.SignedVariant)
                    {
                        throw ProtoWriter.CreateException(writer);
                    }
                    ProtoWriter.WriteUInt32Variant(ProtoWriter.Zig(value), writer);
                    writer.wireType = WireType.None;
                    return;
            }
        }
        public static void WriteDouble(double value, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            WireType wireType = writer.wireType;
            if (wireType == WireType.Fixed64)
            {
                ProtoWriter.WriteInt64(BitConverter.ToInt64(BitConverter.GetBytes(value), 0), writer);
                return;
            }
            if (wireType != WireType.Fixed32)
            {
                throw ProtoWriter.CreateException(writer);
            }
            float f = (float)value;
            if (Helpers.IsInfinity(f) && !Helpers.IsInfinity(value))
            {
                throw new OverflowException();
            }
            ProtoWriter.WriteSingle(f, writer);
        }
        public static void WriteSingle(float value, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            WireType wireType = writer.wireType;
            if (wireType == WireType.Fixed64)
            {
                ProtoWriter.WriteDouble((double)value, writer);
                return;
            }
            if (wireType == WireType.Fixed32)
            {
                ProtoWriter.WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(value), 0), writer);
                return;
            }
            throw ProtoWriter.CreateException(writer);
        }
        public static void ThrowEnumException(ProtoWriter writer, object enumValue)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            string rhs = (enumValue == null) ? "<null>" : (enumValue.GetType().FullName + "." + enumValue.ToString());
            throw new ProtoException("No wire-value is mapped to the enum " + rhs + " at position " + writer.position.ToString());
        }
        internal static Exception CreateException(ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            return new ProtoException("Invalid serialization operation with wire-type " + writer.wireType.ToString() + " at position " + writer.position.ToString());
        }
        public static void WriteBoolean(bool value, ProtoWriter writer)
        {
            ProtoWriter.WriteUInt32(value ? 1u : 0u, writer);
        }
        public static void AppendExtensionData(IExtensible instance, ProtoWriter writer)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (writer.wireType != WireType.None)
            {
                throw ProtoWriter.CreateException(writer);
            }
            IExtension extn = instance.GetExtensionObject(false);
            if (extn != null)
            {
                Stream source = extn.BeginQuery();
                try
                {
                    ProtoWriter.CopyRawFromStream(source, writer);
                }
                finally
                {
                    extn.EndQuery(source);
                }
            }
        }
        public static void SetPackedField(int fieldNumber, ProtoWriter writer)
        {
            if (fieldNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("fieldNumber");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.packedFieldNumber = fieldNumber;
        }
        internal string SerializeType(Type type)
        {
            return TypeModel.SerializeType(this.model, type);
        }
        public void SetRootObject(object value)
        {
            this.NetCache.SetKeyedObject(0, value);
        }
        public static void WriteType(Type value, ProtoWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            ProtoWriter.WriteString(writer.SerializeType(value), writer);
        }
    }
}
