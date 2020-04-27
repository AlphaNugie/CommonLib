using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using static CommonLib.Clients.Serialization.Enums;
using static CommonLib.Clients.Serialization.Meta.TypeFormatEvent;

namespace CommonLib.Clients.Serialization.Meta
{
    public abstract class TypeModel
    {
        private class DeserializeItemsIterator : IEnumerator, IEnumerable
        {
            private bool haveObject;
            private object current;
            private readonly Stream source;
            private readonly Type type;
            private readonly PrefixStyle style;
            private readonly int expectedField;
            private readonly Serializer.TypeResolver resolver;
            private readonly TypeModel model;
            private readonly SerializationContext context;
            public object Current
            {
                get
                {
                    return this.current;
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }
            public bool MoveNext()
            {
                if (this.haveObject)
                {
                    int bytesRead;
                    this.current = this.model.DeserializeWithLengthPrefix(this.source, null, this.type, this.style, this.expectedField, this.resolver, out bytesRead, out this.haveObject, this.context);
                }
                return this.haveObject;
            }
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }
            public DeserializeItemsIterator(TypeModel model, Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
            {
                this.haveObject = true;
                this.source = source;
                this.type = type;
                this.style = style;
                this.expectedField = expectedField;
                this.resolver = resolver;
                this.model = model;
                this.context = context;
            }
        }
        private sealed class DeserializeItemsIterator<T> : TypeModel.DeserializeItemsIterator, IEnumerator<T>, IDisposable, IEnumerator, IEnumerable<T>, IEnumerable
        {
            public new T Current
            {
                get
                {
                    return (T)((object)base.Current);
                }
            }
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this;
            }
            void IDisposable.Dispose()
            {
            }
            public DeserializeItemsIterator(TypeModel model, Stream source, PrefixStyle style, int expectedField, SerializationContext context) : base(model, source, model.MapType(typeof(T)), style, expectedField, null, context)
            {
            }
        }
        protected internal enum CallbackType
        {
            BeforeSerialize,
            AfterSerialize,
            BeforeDeserialize,
            AfterDeserialize
        }
        private static readonly Type ilist = typeof(IList);
        public event TypeFormatEventHandler DynamicTypeFormatting;
        protected internal Type MapType(Type type)
        {
            return this.MapType(type, true);
        }
        protected internal virtual Type MapType(Type type, bool demand)
        {
            return type;
        }
        private WireType GetWireType(ProtoTypeCode code, DataFormat format, ref Type type, out int modelKey)
        {
            modelKey = -1;
            if (type.IsEnum)
            {
                modelKey = this.GetKey(ref type);
                return WireType.Variant;
            }
            switch (code)
            {
                case ProtoTypeCode.Boolean:
                case ProtoTypeCode.Char:
                case ProtoTypeCode.SByte:
                case ProtoTypeCode.Byte:
                case ProtoTypeCode.Int16:
                case ProtoTypeCode.UInt16:
                case ProtoTypeCode.Int32:
                case ProtoTypeCode.UInt32:
                    if (format != DataFormat.FixedSize)
                    {
                        return WireType.Variant;
                    }
                    return WireType.Fixed32;
                case ProtoTypeCode.Int64:
                case ProtoTypeCode.UInt64:
                    if (format != DataFormat.FixedSize)
                    {
                        return WireType.Variant;
                    }
                    return WireType.Fixed64;
                case ProtoTypeCode.Single:
                    return WireType.Fixed32;
                case ProtoTypeCode.Double:
                    return WireType.Fixed64;
                case ProtoTypeCode.Decimal:
                case ProtoTypeCode.DateTime:
                case ProtoTypeCode.String:
                    break;
                case (ProtoTypeCode)17:
                    goto IL_94;
                default:
                    switch (code)
                    {
                        case ProtoTypeCode.TimeSpan:
                        case ProtoTypeCode.ByteArray:
                        case ProtoTypeCode.Guid:
                        case ProtoTypeCode.Uri:
                            break;
                        default:
                            goto IL_94;
                    }
                    break;
            }
            return WireType.String;
        IL_94:
            if ((modelKey = this.GetKey(ref type)) >= 0)
            {
                return WireType.String;
            }
            return WireType.None;
        }
        internal bool TrySerializeAuxiliaryType(ProtoWriter writer, Type type, DataFormat format, int tag, object value, bool isInsideList)
        {
            if (type == null)
            {
                type = value.GetType();
            }
            ProtoTypeCode typecode = Helpers.GetTypeCode(type);
            int modelKey;
            WireType wireType = this.GetWireType(typecode, format, ref type, out modelKey);
            if (modelKey >= 0)
            {
                if (Helpers.IsEnum(type))
                {
                    this.Serialize(modelKey, value, writer);
                    return true;
                }
                ProtoWriter.WriteFieldHeader(tag, wireType, writer);
                switch (wireType)
                {
                    case WireType.None:
                        throw ProtoWriter.CreateException(writer);
                    case WireType.String:
                    case WireType.StartGroup:
                        {
                            SubItemToken token = ProtoWriter.StartSubItem(value, writer);
                            this.Serialize(modelKey, value, writer);
                            ProtoWriter.EndSubItem(token, writer);
                            return true;
                        }
                }
                this.Serialize(modelKey, value, writer);
                return true;
            }
            else
            {
                if (wireType != WireType.None)
                {
                    ProtoWriter.WriteFieldHeader(tag, wireType, writer);
                }
                ProtoTypeCode protoTypeCode = typecode;
                switch (protoTypeCode)
                {
                    case ProtoTypeCode.Boolean:
                        ProtoWriter.WriteBoolean((bool)value, writer);
                        return true;
                    case ProtoTypeCode.Char:
                        ProtoWriter.WriteUInt16((ushort)((char)value), writer);
                        return true;
                    case ProtoTypeCode.SByte:
                        ProtoWriter.WriteSByte((sbyte)value, writer);
                        return true;
                    case ProtoTypeCode.Byte:
                        ProtoWriter.WriteByte((byte)value, writer);
                        return true;
                    case ProtoTypeCode.Int16:
                        ProtoWriter.WriteInt16((short)value, writer);
                        return true;
                    case ProtoTypeCode.UInt16:
                        ProtoWriter.WriteUInt16((ushort)value, writer);
                        return true;
                    case ProtoTypeCode.Int32:
                        ProtoWriter.WriteInt32((int)value, writer);
                        return true;
                    case ProtoTypeCode.UInt32:
                        ProtoWriter.WriteUInt32((uint)value, writer);
                        return true;
                    case ProtoTypeCode.Int64:
                        ProtoWriter.WriteInt64((long)value, writer);
                        return true;
                    case ProtoTypeCode.UInt64:
                        ProtoWriter.WriteUInt64((ulong)value, writer);
                        return true;
                    case ProtoTypeCode.Single:
                        ProtoWriter.WriteSingle((float)value, writer);
                        return true;
                    case ProtoTypeCode.Double:
                        ProtoWriter.WriteDouble((double)value, writer);
                        return true;
                    case ProtoTypeCode.Decimal:
                        BclHelpers.WriteDecimal((decimal)value, writer);
                        return true;
                    case ProtoTypeCode.DateTime:
                        BclHelpers.WriteDateTime((DateTime)value, writer);
                        return true;
                    case (ProtoTypeCode)17:
                        break;
                    case ProtoTypeCode.String:
                        ProtoWriter.WriteString((string)value, writer);
                        return true;
                    default:
                        switch (protoTypeCode)
                        {
                            case ProtoTypeCode.TimeSpan:
                                BclHelpers.WriteTimeSpan((TimeSpan)value, writer);
                                return true;
                            case ProtoTypeCode.ByteArray:
                                ProtoWriter.WriteBytes((byte[])value, writer);
                                return true;
                            case ProtoTypeCode.Guid:
                                BclHelpers.WriteGuid((Guid)value, writer);
                                return true;
                            case ProtoTypeCode.Uri:
                                ProtoWriter.WriteString(((Uri)value).AbsoluteUri, writer);
                                return true;
                        }
                        break;
                }
                IEnumerable sequence = value as IEnumerable;
                if (sequence == null)
                {
                    return false;
                }
                if (isInsideList)
                {
                    throw TypeModel.CreateNestedListsNotSupported();
                }
                foreach (object item in sequence)
                {
                    if (item == null)
                    {
                        throw new NullReferenceException();
                    }
                    if (!this.TrySerializeAuxiliaryType(writer, null, format, tag, item, true))
                    {
                        TypeModel.ThrowUnexpectedType(item.GetType());
                    }
                }
                return true;
            }
        }
        private void SerializeCore(ProtoWriter writer, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            Type type = value.GetType();
            int key = this.GetKey(ref type);
            if (key >= 0)
            {
                this.Serialize(key, value, writer);
                return;
            }
            if (!this.TrySerializeAuxiliaryType(writer, type, DataFormat.Default, 1, value, false))
            {
                TypeModel.ThrowUnexpectedType(type);
            }
        }
        public void Serialize(Stream dest, object value)
        {
            this.Serialize(dest, value, null);
        }
        public void Serialize(Stream dest, object value, SerializationContext context)
        {
            using (ProtoWriter writer = new ProtoWriter(dest, this, context))
            {
                writer.SetRootObject(value);
                this.SerializeCore(writer, value);
                writer.Close();
            }
        }
        public void Serialize(ProtoWriter dest, object value)
        {
            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }
            dest.CheckDepthFlushlock();
            dest.SetRootObject(value);
            this.SerializeCore(dest, value);
            dest.CheckDepthFlushlock();
            ProtoWriter.Flush(dest);
        }
        public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int fieldNumber)
        {
            int bytesRead;
            return this.DeserializeWithLengthPrefix(source, value, type, style, fieldNumber, null, out bytesRead);
        }
        public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
        {
            int bytesRead;
            return this.DeserializeWithLengthPrefix(source, value, type, style, expectedField, resolver, out bytesRead);
        }
        public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, out int bytesRead)
        {
            bool haveObject;
            return this.DeserializeWithLengthPrefix(source, value, type, style, expectedField, resolver, out bytesRead, out haveObject, null);
        }
        private object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, out int bytesRead, out bool haveObject, SerializationContext context)
        {
            haveObject = false;
            bytesRead = 0;
            if (type == null && (style != PrefixStyle.Base128 || resolver == null))
            {
                throw new InvalidOperationException("A type must be provided unless base-128 prefixing is being used in combination with a resolver");
            }
            while (true)
            {
                bool expectPrefix = expectedField > 0 || resolver != null;
                int actualField;
                int tmpBytesRead;
                int len = ProtoReader.ReadLengthPrefix(source, expectPrefix, style, out actualField, out tmpBytesRead);
                if (tmpBytesRead == 0)
                {
                    break;
                }
                bytesRead += tmpBytesRead;
                if (len < 0)
                {
                    return value;
                }
                bool skip;
                if (style == PrefixStyle.Base128)
                {
                    if (expectPrefix && expectedField == 0 && type == null && resolver != null)
                    {
                        type = resolver(actualField);
                        skip = (type == null);
                    }
                    else
                    {
                        skip = (expectedField != actualField);
                    }
                }
                else
                {
                    skip = false;
                }
                if (skip)
                {
                    if (len == 2147483647)
                    {
                        goto Block_12;
                    }
                    ProtoReader.Seek(source, len, null);
                    bytesRead += len;
                }
                if (!skip)
                {
                    goto Block_13;
                }
            }
            return value;
        Block_12:
            throw new InvalidOperationException();
        Block_13:
            ProtoReader reader = null;
            object result;
            try
            {
                int len;
                reader = ProtoReader.Create(source, this, context, len);
                int key = this.GetKey(ref type);
                if (key >= 0 && !Helpers.IsEnum(type))
                {
                    value = this.Deserialize(key, value, reader);
                }
                else
                {
                    if (!this.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, true, false, true, false) && len != 0)
                    {
                        TypeModel.ThrowUnexpectedType(type);
                    }
                }
                bytesRead += reader.Position;
                haveObject = true;
                result = value;
            }
            finally
            {
                ProtoReader.Recycle(reader);
            }
            return result;
        }
        public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
        {
            return this.DeserializeItems(source, type, style, expectedField, resolver, null);
        }
        public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
        {
            return new TypeModel.DeserializeItemsIterator(this, source, type, style, expectedField, resolver, context);
        }
        public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField)
        {
            return this.DeserializeItems<T>(source, style, expectedField, null);
        }
        public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField, SerializationContext context)
        {
            return new TypeModel.DeserializeItemsIterator<T>(this, source, style, expectedField, context);
        }
        public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber)
        {
            this.SerializeWithLengthPrefix(dest, value, type, style, fieldNumber, null);
        }
        public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber, SerializationContext context)
        {
            if (type == null)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                type = this.MapType(value.GetType());
            }
            int key = this.GetKey(ref type);
            using (ProtoWriter writer = new ProtoWriter(dest, this, context))
            {
                switch (style)
                {
                    case PrefixStyle.None:
                        this.Serialize(key, value, writer);
                        break;
                    case PrefixStyle.Base128:
                    case PrefixStyle.Fixed32:
                    case PrefixStyle.Fixed32BigEndian:
                        ProtoWriter.WriteObject(value, key, writer, style, fieldNumber);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("style");
                }
                writer.Close();
            }
        }
        public object Deserialize(Stream source, object value, Type type)
        {
            return this.Deserialize(source, value, type, null);
        }
        public object Deserialize(Stream source, object value, Type type, SerializationContext context)
        {
            bool autoCreate = this.PrepareDeserialize(value, ref type);
            ProtoReader reader = null;
            object result;
            try
            {
                reader = ProtoReader.Create(source, this, context, -1);
                if (value != null)
                {
                    reader.SetRootObject(value);
                }
                object obj = this.DeserializeCore(reader, type, value, autoCreate);
                reader.CheckFullyConsumed();
                result = obj;
            }
            finally
            {
                ProtoReader.Recycle(reader);
            }
            return result;
        }
        private bool PrepareDeserialize(object value, ref Type type)
        {
            if (type == null)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("type");
                }
                type = this.MapType(value.GetType());
            }
            bool autoCreate = true;
            Type underlyingType = Helpers.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                type = underlyingType;
                autoCreate = false;
            }
            return autoCreate;
        }
        public object Deserialize(Stream source, object value, Type type, int length)
        {
            return this.Deserialize(source, value, type, length, null);
        }
        public object Deserialize(Stream source, object value, Type type, int length, SerializationContext context)
        {
            bool autoCreate = this.PrepareDeserialize(value, ref type);
            ProtoReader reader = null;
            object result;
            try
            {
                reader = ProtoReader.Create(source, this, context, length);
                if (value != null)
                {
                    reader.SetRootObject(value);
                }
                object obj = this.DeserializeCore(reader, type, value, autoCreate);
                reader.CheckFullyConsumed();
                result = obj;
            }
            finally
            {
                ProtoReader.Recycle(reader);
            }
            return result;
        }
        public object Deserialize(ProtoReader source, object value, Type type)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            bool autoCreate = this.PrepareDeserialize(value, ref type);
            if (value != null)
            {
                source.SetRootObject(value);
            }
            object obj = this.DeserializeCore(source, type, value, autoCreate);
            source.CheckFullyConsumed();
            return obj;
        }
        private object DeserializeCore(ProtoReader reader, Type type, object value, bool noAutoCreate)
        {
            int key = this.GetKey(ref type);
            if (key >= 0 && !Helpers.IsEnum(type))
            {
                return this.Deserialize(key, value, reader);
            }
            this.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, true, false, noAutoCreate, false);
            return value;
        }
        internal static MethodInfo ResolveListAdd(TypeModel model, Type listType, Type itemType, out bool isList)
        {
            isList = model.MapType(TypeModel.ilist).IsAssignableFrom(listType);
            Type[] types = new Type[]
            {
                itemType
            };
            MethodInfo add = Helpers.GetInstanceMethod(listType, "Add", types);
            if (add == null)
            {
                bool forceList = listType.IsInterface && listType == model.MapType(typeof(IEnumerable<>)).MakeGenericType(types);
                Type constuctedListType = model.MapType(typeof(ICollection<>)).MakeGenericType(types);
                if (forceList || constuctedListType.IsAssignableFrom(listType))
                {
                    add = Helpers.GetInstanceMethod(constuctedListType, "Add", types);
                }
            }
            if (add == null)
            {
                Type[] interfaces = listType.GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    Type interfaceType = interfaces[i];
                    if (interfaceType.Name == "IProducerConsumerCollection`1" && interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition().FullName == "System.Collections.Concurrent.IProducerConsumerCollection`1")
                    {
                        add = Helpers.GetInstanceMethod(interfaceType, "TryAdd", types);
                        if (add != null)
                        {
                            break;
                        }
                    }
                }
            }
            if (add == null)
            {
                types[0] = model.MapType(typeof(object));
                add = Helpers.GetInstanceMethod(listType, "Add", types);
            }
            if (add == null && isList)
            {
                add = Helpers.GetInstanceMethod(model.MapType(TypeModel.ilist), "Add", types);
            }
            return add;
        }
        internal static Type GetListItemType(TypeModel model, Type listType)
        {
            if (listType == model.MapType(typeof(string)) || listType.IsArray || !model.MapType(typeof(IEnumerable)).IsAssignableFrom(listType))
            {
                return null;
            }
            BasicList candidates = new BasicList();
            MethodInfo[] methods = listType.GetMethods();
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                if (!method.IsStatic && !(method.Name != "Add"))
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    Type paramType;
                    if (parameters.Length == 1 && !candidates.Contains(paramType = parameters[0].ParameterType))
                    {
                        candidates.Add(paramType);
                    }
                }
            }
            string name = listType.Name;
            if (name == null || (name.IndexOf("Queue") < 0 && name.IndexOf("Stack") < 0))
            {
                TypeModel.TestEnumerableListPatterns(model, candidates, listType);
                Type[] interfaces = listType.GetInterfaces();
                for (int j = 0; j < interfaces.Length; j++)
                {
                    Type iType = interfaces[j];
                    TypeModel.TestEnumerableListPatterns(model, candidates, iType);
                }
            }
            PropertyInfo[] properties = listType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int k = 0; k < properties.Length; k++)
            {
                PropertyInfo indexer = properties[k];
                if (!(indexer.Name != "Item") && !candidates.Contains(indexer.PropertyType))
                {
                    ParameterInfo[] args = indexer.GetIndexParameters();
                    if (args.Length == 1 && !(args[0].ParameterType != model.MapType(typeof(int))))
                    {
                        candidates.Add(indexer.PropertyType);
                    }
                }
            }
            switch (candidates.Count)
            {
                case 0:
                    return null;
                case 1:
                    return (Type)candidates[0];
                case 2:
                    if (TypeModel.CheckDictionaryAccessors(model, (Type)candidates[0], (Type)candidates[1]))
                    {
                        return (Type)candidates[0];
                    }
                    if (TypeModel.CheckDictionaryAccessors(model, (Type)candidates[1], (Type)candidates[0]))
                    {
                        return (Type)candidates[1];
                    }
                    break;
            }
            return null;
        }
        private static void TestEnumerableListPatterns(TypeModel model, BasicList candidates, Type iType)
        {
            if (iType.IsGenericType)
            {
                Type typeDef = iType.GetGenericTypeDefinition();
                if (typeDef == model.MapType(typeof(IEnumerable<>)) || typeDef == model.MapType(typeof(ICollection<>)) || typeDef.FullName == "System.Collections.Concurrent.IProducerConsumerCollection`1")
                {
                    Type[] iTypeArgs = iType.GetGenericArguments();
                    if (!candidates.Contains(iTypeArgs[0]))
                    {
                        candidates.Add(iTypeArgs[0]);
                    }
                }
            }
        }
        private static bool CheckDictionaryAccessors(TypeModel model, Type pair, Type value)
        {
            return pair.IsGenericType && pair.GetGenericTypeDefinition() == model.MapType(typeof(KeyValuePair<,>)) && pair.GetGenericArguments()[1] == value;
        }
        private bool TryDeserializeList(TypeModel model, ProtoReader reader, DataFormat format, int tag, Type listType, Type itemType, ref object value)
        {
            bool isList;
            MethodInfo addMethod = TypeModel.ResolveListAdd(model, listType, itemType, out isList);
            if (addMethod == null)
            {
                throw new NotSupportedException("Unknown list variant: " + listType.FullName);
            }
            bool found = false;
            object nextItem = null;
            IList list = value as IList;
            object[] args = isList ? null : new object[1];
            BasicList arraySurrogate = listType.IsArray ? new BasicList() : null;
            while (this.TryDeserializeAuxiliaryType(reader, format, tag, itemType, ref nextItem, true, true, true, true))
            {
                found = true;
                if (value == null && arraySurrogate == null)
                {
                    value = TypeModel.CreateListInstance(listType, itemType);
                    list = (value as IList);
                }
                if (list != null)
                {
                    list.Add(nextItem);
                }
                else
                {
                    if (arraySurrogate != null)
                    {
                        arraySurrogate.Add(nextItem);
                    }
                    else
                    {
                        args[0] = nextItem;
                        addMethod.Invoke(value, args);
                    }
                }
                nextItem = null;
            }
            if (arraySurrogate != null)
            {
                if (value != null)
                {
                    if (arraySurrogate.Count != 0)
                    {
                        Array existing = (Array)value;
                        Array newArray = Array.CreateInstance(itemType, existing.Length + arraySurrogate.Count);
                        Array.Copy(existing, newArray, existing.Length);
                        arraySurrogate.CopyTo(newArray, existing.Length);
                        value = newArray;
                    }
                }
                else
                {
                    Array newArray = Array.CreateInstance(itemType, arraySurrogate.Count);
                    arraySurrogate.CopyTo(newArray, 0);
                    value = newArray;
                }
            }
            return found;
        }
        private static object CreateListInstance(Type listType, Type itemType)
        {
            Type concreteListType = listType;
            if (listType.IsArray)
            {
                return Array.CreateInstance(itemType, 0);
            }
            if (!listType.IsClass || listType.IsAbstract || Helpers.GetConstructor(listType, Helpers.EmptyTypes, true) == null)
            {
                bool handled = false;
                string fullName;
                if (listType.IsInterface && (fullName = listType.FullName) != null && fullName.IndexOf("Dictionary") >= 0)
                {
                    if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    {
                        Type[] genericTypes = listType.GetGenericArguments();
                        concreteListType = typeof(Dictionary<,>).MakeGenericType(genericTypes);
                        handled = true;
                    }
                    if (!handled && listType == typeof(IDictionary))
                    {
                        concreteListType = typeof(Hashtable);
                        handled = true;
                    }
                }
                if (!handled)
                {
                    concreteListType = typeof(List<>).MakeGenericType(new Type[]
                    {
                        itemType
                    });
                    handled = true;
                }
                if (!handled)
                {
                    concreteListType = typeof(ArrayList);
                }
            }
            return Activator.CreateInstance(concreteListType);
        }
        internal bool TryDeserializeAuxiliaryType(ProtoReader reader, DataFormat format, int tag, Type type, ref object value, bool skipOtherFields, bool asListItem, bool autoCreate, bool insideList)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            ProtoTypeCode typecode = Helpers.GetTypeCode(type);
            int modelKey;
            WireType wiretype = this.GetWireType(typecode, format, ref type, out modelKey);
            bool found = false;
            if (wiretype == WireType.None)
            {
                Type itemType = TypeModel.GetListItemType(this, type);
                if (itemType == null && type.IsArray && type.GetArrayRank() == 1 && type != typeof(byte[]))
                {
                    itemType = type.GetElementType();
                }
                if (itemType != null)
                {
                    if (insideList)
                    {
                        throw TypeModel.CreateNestedListsNotSupported();
                    }
                    found = this.TryDeserializeList(this, reader, format, tag, type, itemType, ref value);
                    if (!found && autoCreate)
                    {
                        value = TypeModel.CreateListInstance(type, itemType);
                    }
                    return found;
                }
                else
                {
                    TypeModel.ThrowUnexpectedType(type);
                }
            }
            while (!found || !asListItem)
            {
                int fieldNumber = reader.ReadFieldHeader();
                if (fieldNumber <= 0)
                {
                    break;
                }
                if (fieldNumber != tag)
                {
                    if (!skipOtherFields)
                    {
                        throw ProtoReader.AddErrorData(new InvalidOperationException("Expected field " + tag.ToString() + ", but found " + fieldNumber.ToString()), reader);
                    }
                    reader.SkipField();
                }
                else
                {
                    found = true;
                    reader.Hint(wiretype);
                    if (modelKey >= 0)
                    {
                        switch (wiretype)
                        {
                            case WireType.String:
                            case WireType.StartGroup:
                                {
                                    SubItemToken token = ProtoReader.StartSubItem(reader);
                                    value = this.Deserialize(modelKey, value, reader);
                                    ProtoReader.EndSubItem(token, reader);
                                    break;
                                }
                            default:
                                value = this.Deserialize(modelKey, value, reader);
                                break;
                        }
                    }
                    else
                    {
                        ProtoTypeCode protoTypeCode = typecode;
                        switch (protoTypeCode)
                        {
                            case ProtoTypeCode.Boolean:
                                value = reader.ReadBoolean();
                                break;
                            case ProtoTypeCode.Char:
                                value = (char)reader.ReadUInt16();
                                break;
                            case ProtoTypeCode.SByte:
                                value = reader.ReadSByte();
                                break;
                            case ProtoTypeCode.Byte:
                                value = reader.ReadByte();
                                break;
                            case ProtoTypeCode.Int16:
                                value = reader.ReadInt16();
                                break;
                            case ProtoTypeCode.UInt16:
                                value = reader.ReadUInt16();
                                break;
                            case ProtoTypeCode.Int32:
                                value = reader.ReadInt32();
                                break;
                            case ProtoTypeCode.UInt32:
                                value = reader.ReadUInt32();
                                break;
                            case ProtoTypeCode.Int64:
                                value = reader.ReadInt64();
                                break;
                            case ProtoTypeCode.UInt64:
                                value = reader.ReadUInt64();
                                break;
                            case ProtoTypeCode.Single:
                                value = reader.ReadSingle();
                                break;
                            case ProtoTypeCode.Double:
                                value = reader.ReadDouble();
                                break;
                            case ProtoTypeCode.Decimal:
                                value = BclHelpers.ReadDecimal(reader);
                                break;
                            case ProtoTypeCode.DateTime:
                                value = BclHelpers.ReadDateTime(reader);
                                break;
                            case (ProtoTypeCode)17:
                                break;
                            case ProtoTypeCode.String:
                                value = reader.ReadString();
                                break;
                            default:
                                switch (protoTypeCode)
                                {
                                    case ProtoTypeCode.TimeSpan:
                                        value = BclHelpers.ReadTimeSpan(reader);
                                        break;
                                    case ProtoTypeCode.ByteArray:
                                        value = ProtoReader.AppendBytes((byte[])value, reader);
                                        break;
                                    case ProtoTypeCode.Guid:
                                        value = BclHelpers.ReadGuid(reader);
                                        break;
                                    case ProtoTypeCode.Uri:
                                        value = new Uri(reader.ReadString());
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            if (!found && !asListItem && autoCreate && type != typeof(string))
            {
                value = Activator.CreateInstance(type);
            }
            return found;
        }
        public static RuntimeTypeModel Create()
        {
            return new RuntimeTypeModel(false);
        }
        protected internal static Type ResolveProxies(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type.IsGenericParameter)
            {
                return null;
            }
            Type tmp = Helpers.GetUnderlyingType(type);
            if (tmp != null)
            {
                return tmp;
            }
            string fullName = type.FullName;
            if (fullName != null && fullName.StartsWith("System.Data.Entity.DynamicProxies."))
            {
                return type.BaseType;
            }
            Type[] interfaces = type.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                string fullName2;
                if ((fullName2 = interfaces[i].FullName) != null && (fullName2 == "NHibernate.Proxy.INHibernateProxy" || fullName2 == "NHibernate.Proxy.DynamicProxy.IProxy" || fullName2 == "NHibernate.Intercept.IFieldInterceptorAccessor"))
                {
                    return type.BaseType;
                }
            }
            return null;
        }
        public bool IsDefined(Type type)
        {
            return this.GetKey(ref type) >= 0;
        }
        protected internal int GetKey(ref Type type)
        {
            if (type == null)
            {
                return -1;
            }
            int key = this.GetKeyImpl(type);
            if (key < 0)
            {
                Type normalized = TypeModel.ResolveProxies(type);
                if (normalized != null)
                {
                    type = normalized;
                    key = this.GetKeyImpl(type);
                }
            }
            return key;
        }
        protected abstract int GetKeyImpl(Type type);
        protected internal abstract void Serialize(int key, object value, ProtoWriter dest);
        protected internal abstract object Deserialize(int key, object value, ProtoReader source);
        public object DeepClone(object value)
        {
            if (value == null)
            {
                return null;
            }
            Type type = value.GetType();
            int key = this.GetKey(ref type);
            object result;
            if (key >= 0 && !Helpers.IsEnum(type))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ProtoWriter writer = new ProtoWriter(ms, this, null))
                    {
                        writer.SetRootObject(value);
                        this.Serialize(key, value, writer);
                        writer.Close();
                    }
                    ms.Position = 0L;
                    ProtoReader reader = null;
                    try
                    {
                        reader = ProtoReader.Create(ms, this, null, -1);
                        result = this.Deserialize(key, null, reader);
                        return result;
                    }
                    finally
                    {
                        ProtoReader.Recycle(reader);
                    }
                }
            }
            if (type == typeof(byte[]))
            {
                byte[] orig = (byte[])value;
                byte[] clone = new byte[orig.Length];
                Helpers.BlockCopy(orig, 0, clone, 0, orig.Length);
                return clone;
            }
            int modelKey;
            if (this.GetWireType(Helpers.GetTypeCode(type), DataFormat.Default, ref type, out modelKey) != WireType.None && modelKey < 0)
            {
                return value;
            }
            using (MemoryStream ms2 = new MemoryStream())
            {
                using (ProtoWriter writer2 = new ProtoWriter(ms2, this, null))
                {
                    if (!this.TrySerializeAuxiliaryType(writer2, type, DataFormat.Default, 1, value, false))
                    {
                        TypeModel.ThrowUnexpectedType(type);
                    }
                    writer2.Close();
                }
                ms2.Position = 0L;
                ProtoReader reader2 = null;
                try
                {
                    reader2 = ProtoReader.Create(ms2, this, null, -1);
                    value = null;
                    this.TryDeserializeAuxiliaryType(reader2, DataFormat.Default, 1, type, ref value, true, false, true, false);
                    result = value;
                }
                finally
                {
                    ProtoReader.Recycle(reader2);
                }
            }
            return result;
        }
        protected internal static void ThrowUnexpectedSubtype(Type expected, Type actual)
        {
            if (expected != TypeModel.ResolveProxies(actual))
            {
                throw new InvalidOperationException("Unexpected sub-type: " + actual.FullName);
            }
        }
        protected internal static void ThrowUnexpectedType(Type type)
        {
            string fullName = (type == null) ? "(unknown)" : type.FullName;
            if (type != null)
            {
                Type baseType = type.BaseType;
                if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name == "GeneratedMessage`2")
                {
                    throw new InvalidOperationException("Are you mixing protobuf-net and protobuf-csharp-port? See http://stackoverflow.com/q/11564914; type: " + fullName);
                }
            }
            throw new InvalidOperationException("Type is not expected, and no contract can be inferred: " + fullName);
        }
        internal static Exception CreateNestedListsNotSupported()
        {
            return new NotSupportedException("Nested or jagged lists and arrays are not supported");
        }
        public static void ThrowCannotCreateInstance(Type type)
        {
            throw new ProtoException("No parameterless constructor found for " + ((type == null) ? "(null)" : type.Name));
        }
        internal static string SerializeType(TypeModel model, Type type)
        {
            if (model != null)
            {
                TypeFormatEventHandler handler = model.DynamicTypeFormatting;
                if (handler != null)
                {
                    TypeFormatEventArgs args = new TypeFormatEventArgs(type);
                    handler(model, args);
                    if (!Helpers.IsNullOrEmpty(args.FormattedName))
                    {
                        return args.FormattedName;
                    }
                }
            }
            return type.AssemblyQualifiedName;
        }
        internal static Type DeserializeType(TypeModel model, string value)
        {
            if (model != null)
            {
                TypeFormatEventHandler handler = model.DynamicTypeFormatting;
                if (handler != null)
                {
                    TypeFormatEventArgs args = new TypeFormatEventArgs(value);
                    handler(model, args);
                    if (args.Type != null)
                    {
                        return args.Type;
                    }
                }
            }
            return Type.GetType(value);
        }
        public bool CanSerializeContractType(Type type)
        {
            return this.CanSerialize(type, false, true, true);
        }
        public bool CanSerialize(Type type)
        {
            return this.CanSerialize(type, true, true, true);
        }
        public bool CanSerializeBasicType(Type type)
        {
            return this.CanSerialize(type, true, false, true);
        }
        private bool CanSerialize(Type type, bool allowBasic, bool allowContract, bool allowLists)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            Type tmp = Helpers.GetUnderlyingType(type);
            if (tmp != null)
            {
                type = tmp;
            }
            switch (Helpers.GetTypeCode(type))
            {
                case ProtoTypeCode.Empty:
                case ProtoTypeCode.Unknown:
                    {
                        int modelKey = this.GetKey(ref type);
                        if (modelKey >= 0)
                        {
                            return allowContract;
                        }
                        if (allowLists)
                        {
                            Type itemType = null;
                            if (type.IsArray)
                            {
                                if (type.GetArrayRank() == 1)
                                {
                                    itemType = type.GetElementType();
                                }
                            }
                            else
                            {
                                itemType = TypeModel.GetListItemType(this, type);
                            }
                            if (itemType != null)
                            {
                                return this.CanSerialize(itemType, allowBasic, allowContract, false);
                            }
                        }
                        return false;
                    }
                default:
                    return allowBasic;
            }
        }
        public virtual string GetSchema(Type type)
        {
            throw new NotSupportedException();
        }
        internal virtual Type GetType(string fullName, Assembly context)
        {
            return TypeModel.ResolveKnownType(fullName, this, context);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Type ResolveKnownType(string name, TypeModel model, Assembly assembly)
        {
            if (Helpers.IsNullOrEmpty(name))
            {
                return null;
            }
            try
            {
                Type type = Type.GetType(name);
                if (type != null)
                {
                    Type result = type;
                    return result;
                }
            }
            catch
            {
            }
            try
            {
                int i = name.IndexOf(',');
                string fullName = ((i > 0) ? name.Substring(0, i) : name).Trim();
                if (assembly == null)
                {
                    assembly = Assembly.GetCallingAssembly();
                }
                Type type2 = (assembly == null) ? null : assembly.GetType(fullName);
                if (type2 != null)
                {
                    Type result = type2;
                    return result;
                }
            }
            catch
            {
            }
            return null;
        }
    }
}
