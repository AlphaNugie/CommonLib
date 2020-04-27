using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace CommonLib.Clients.Serialization.Meta
{
    public sealed class RuntimeTypeModel : TypeModel
    {
        private sealed class Singleton
        {
            internal static readonly RuntimeTypeModel Value = new RuntimeTypeModel(true);
            private Singleton()
            {
            }
        }
        private sealed class BasicType
        {
            private readonly Type type;
            private readonly IProtoSerializer serializer;
            public Type Type
            {
                get
                {
                    return this.type;
                }
            }
            public IProtoSerializer Serializer
            {
                get
                {
                    return this.serializer;
                }
            }
            public BasicType(Type type, IProtoSerializer serializer)
            {
                this.type = type;
                this.serializer = serializer;
            }
        }
        private const byte OPTIONS_InferTagFromNameDefault = 1;
        private const byte OPTIONS_IsDefaultModel = 2;
        private const byte OPTIONS_Frozen = 4;
        private const byte OPTIONS_AutoAddMissingTypes = 8;
        private const byte OPTIONS_UseImplicitZeroDefaults = 32;
        private const byte OPTIONS_AllowParseableTypes = 64;
        private const byte OPTIONS_AutoAddProtoContractTypesOnly = 128;
        private byte options;
        private static readonly BasicList.MatchPredicate MetaTypeFinder = new BasicList.MatchPredicate(RuntimeTypeModel.MetaTypeFinderImpl);
        private static readonly BasicList.MatchPredicate BasicTypeFinder = new BasicList.MatchPredicate(RuntimeTypeModel.BasicTypeFinderImpl);
        private BasicList basicTypes = new BasicList();
        private readonly BasicList types = new BasicList();
        private int metadataTimeoutMilliseconds = 5000;
        private int contentionCounter = 1;
        private MethodInfo defaultFactory;
        public event LockContentedEventHandler LockContended;
        public bool InferTagFromNameDefault
        {
            get
            {
                return this.GetOption(1);
            }
            set
            {
                this.SetOption(1, value);
            }
        }
        public bool AutoAddProtoContractTypesOnly
        {
            get
            {
                return this.GetOption(128);
            }
            set
            {
                this.SetOption(128, value);
            }
        }
        public bool UseImplicitZeroDefaults
        {
            get
            {
                return this.GetOption(32);
            }
            set
            {
                if (!value && this.GetOption(2))
                {
                    throw new InvalidOperationException("UseImplicitZeroDefaults cannot be disabled on the default model");
                }
                this.SetOption(32, value);
            }
        }
        public bool AllowParseableTypes
        {
            get
            {
                return this.GetOption(64);
            }
            set
            {
                this.SetOption(64, value);
            }
        }
        public static RuntimeTypeModel Default
        {
            get
            {
                return RuntimeTypeModel.Singleton.Value;
            }
        }
        public MetaType this[Type type]
        {
            get
            {
                return (MetaType)this.types[this.FindOrAddAuto(type, true, false, false)];
            }
        }
        public bool AutoAddMissingTypes
        {
            get
            {
                return this.GetOption(8);
            }
            set
            {
                if (!value && this.GetOption(2))
                {
                    throw new InvalidOperationException("The default model must allow missing types");
                }
                this.ThrowIfFrozen();
                this.SetOption(8, value);
            }
        }
        public int MetadataTimeoutMilliseconds
        {
            get
            {
                return this.metadataTimeoutMilliseconds;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("MetadataTimeoutMilliseconds");
                }
                this.metadataTimeoutMilliseconds = value;
            }
        }
        private bool GetOption(byte option)
        {
            return (this.options & option) == option;
        }
        private void SetOption(byte option, bool value)
        {
            if (value)
            {
                this.options |= option;
                return;
            }
            this.options &= ~option;
        }
        public IEnumerable GetTypes()
        {
            return this.types;
        }
        public override string GetSchema(Type type)
        {
            BasicList requiredTypes = new BasicList();
            MetaType primaryType = null;
            bool isInbuiltType = false;
            if (type == null)
            {
                BasicList.NodeEnumerator enumerator = this.types.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    MetaType meta = (MetaType)enumerator.Current;
                    MetaType tmp = meta.GetSurrogateOrBaseOrSelf(false);
                    if (!requiredTypes.Contains(tmp))
                    {
                        requiredTypes.Add(tmp);
                        this.CascadeDependents(requiredTypes, tmp);
                    }
                }
            }
            else
            {
                Type tmp2 = Helpers.GetUnderlyingType(type);
                if (tmp2 != null)
                {
                    type = tmp2;
                }
                WireType defaultWireType;
                isInbuiltType = (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out defaultWireType, false, false, false, false) != null);
                if (!isInbuiltType)
                {
                    int index = this.FindOrAddAuto(type, false, false, false);
                    if (index < 0)
                    {
                        throw new ArgumentException("The type specified is not a contract-type", "type");
                    }
                    primaryType = ((MetaType)this.types[index]).GetSurrogateOrBaseOrSelf(false);
                    requiredTypes.Add(primaryType);
                    this.CascadeDependents(requiredTypes, primaryType);
                }
            }
            StringBuilder headerBuilder = new StringBuilder();
            string package = null;
            if (!isInbuiltType)
            {
                IEnumerable typesForNamespace = (primaryType == null) ? this.types : requiredTypes;
                foreach (MetaType meta2 in typesForNamespace)
                {
                    if (!meta2.IsList)
                    {
                        string tmp3 = meta2.Type.Namespace;
                        if (!Helpers.IsNullOrEmpty(tmp3) && !tmp3.StartsWith("System."))
                        {
                            if (package == null)
                            {
                                package = tmp3;
                            }
                            else
                            {
                                if (!(package == tmp3))
                                {
                                    package = null;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (!Helpers.IsNullOrEmpty(package))
            {
                headerBuilder.Append("package ").Append(package).Append(';');
                Helpers.AppendLine(headerBuilder);
            }
            bool requiresBclImport = false;
            StringBuilder bodyBuilder = new StringBuilder();
            MetaType[] metaTypesArr = new MetaType[requiredTypes.Count];
            requiredTypes.CopyTo(metaTypesArr, 0);
            Array.Sort<MetaType>(metaTypesArr, MetaType.Comparer.Default);
            if (isInbuiltType)
            {
                Helpers.AppendLine(bodyBuilder).Append("message ").Append(type.Name).Append(" {");
                MetaType.NewLine(bodyBuilder, 1).Append("optional ").Append(this.GetSchemaTypeName(type, DataFormat.Default, false, false, ref requiresBclImport)).Append(" value = 1;");
                Helpers.AppendLine(bodyBuilder).Append('}');
            }
            else
            {
                for (int i = 0; i < metaTypesArr.Length; i++)
                {
                    MetaType tmp4 = metaTypesArr[i];
                    if (!tmp4.IsList || tmp4 == primaryType)
                    {
                        tmp4.WriteSchema(bodyBuilder, 0, ref requiresBclImport);
                    }
                }
            }
            if (requiresBclImport)
            {
                headerBuilder.Append("import \"bcl.proto\"; // schema for protobuf-net's handling of core .NET types");
                Helpers.AppendLine(headerBuilder);
            }
            return Helpers.AppendLine(headerBuilder.Append(bodyBuilder)).ToString();
        }
        private void CascadeDependents(BasicList list, MetaType metaType)
        {
            if (metaType.IsList)
            {
                Type itemType = TypeModel.GetListItemType(this, metaType.Type);
                WireType defaultWireType;
                if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, itemType, out defaultWireType, false, false, false, false) == null)
                {
                    int index = this.FindOrAddAuto(itemType, false, false, false);
                    if (index >= 0)
                    {
                        MetaType tmp = ((MetaType)this.types[index]).GetSurrogateOrBaseOrSelf(false);
                        if (!list.Contains(tmp))
                        {
                            list.Add(tmp);
                            this.CascadeDependents(list, tmp);
                            return;
                        }
                    }
                }
            }
            else
            {
                MetaType tmp;
                if (metaType.IsAutoTuple)
                {
                    MemberInfo[] mapping;
                    if (MetaType.ResolveTupleConstructor(metaType.Type, out mapping) != null)
                    {
                        for (int i = 0; i < mapping.Length; i++)
                        {
                            Type type = null;
                            if (mapping[i] is PropertyInfo)
                            {
                                type = ((PropertyInfo)mapping[i]).PropertyType;
                            }
                            else
                            {
                                if (mapping[i] is FieldInfo)
                                {
                                    type = ((FieldInfo)mapping[i]).FieldType;
                                }
                            }
                            WireType defaultWireType2;
                            if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out defaultWireType2, false, false, false, false) == null)
                            {
                                int index2 = this.FindOrAddAuto(type, false, false, false);
                                if (index2 >= 0)
                                {
                                    tmp = ((MetaType)this.types[index2]).GetSurrogateOrBaseOrSelf(false);
                                    if (!list.Contains(tmp))
                                    {
                                        list.Add(tmp);
                                        this.CascadeDependents(list, tmp);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (ValueMember member in metaType.Fields)
                    {
                        Type type2 = member.ItemType;
                        if (type2 == null)
                        {
                            type2 = member.MemberType;
                        }
                        WireType defaultWireType3;
                        if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type2, out defaultWireType3, false, false, false, false) == null)
                        {
                            int index3 = this.FindOrAddAuto(type2, false, false, false);
                            if (index3 >= 0)
                            {
                                tmp = ((MetaType)this.types[index3]).GetSurrogateOrBaseOrSelf(false);
                                if (!list.Contains(tmp))
                                {
                                    list.Add(tmp);
                                    this.CascadeDependents(list, tmp);
                                }
                            }
                        }
                    }
                }
                if (metaType.HasSubtypes)
                {
                    SubType[] subtypes = metaType.GetSubtypes();
                    for (int j = 0; j < subtypes.Length; j++)
                    {
                        SubType subType = subtypes[j];
                        tmp = subType.DerivedType.GetSurrogateOrSelf();
                        if (!list.Contains(tmp))
                        {
                            list.Add(tmp);
                            this.CascadeDependents(list, tmp);
                        }
                    }
                }
                tmp = metaType.BaseType;
                if (tmp != null)
                {
                    tmp = tmp.GetSurrogateOrSelf();
                }
                if (tmp != null && !list.Contains(tmp))
                {
                    list.Add(tmp);
                    this.CascadeDependents(list, tmp);
                }
            }
        }
        internal RuntimeTypeModel(bool isDefault)
        {
            this.AutoAddMissingTypes = true;
            this.UseImplicitZeroDefaults = true;
            this.SetOption(2, isDefault);
        }
        internal MetaType FindWithoutAdd(Type type)
        {
            BasicList.NodeEnumerator enumerator = this.types.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MetaType metaType = (MetaType)enumerator.Current;
                if (metaType.Type == type)
                {
                    if (metaType.Pending)
                    {
                        this.WaitOnLock(metaType);
                    }
                    return metaType;
                }
            }
            Type underlyingType = TypeModel.ResolveProxies(type);
            if (!(underlyingType == null))
            {
                return this.FindWithoutAdd(underlyingType);
            }
            return null;
        }
        private static bool MetaTypeFinderImpl(object value, object ctx)
        {
            return ((MetaType)value).Type == (Type)ctx;
        }
        private static bool BasicTypeFinderImpl(object value, object ctx)
        {
            return ((RuntimeTypeModel.BasicType)value).Type == (Type)ctx;
        }
        private void WaitOnLock(MetaType type)
        {
            int opaqueToken = 0;
            try
            {
                this.TakeLock(ref opaqueToken);
            }
            finally
            {
                this.ReleaseLock(opaqueToken);
            }
        }
        internal IProtoSerializer TryGetBasicTypeSerializer(Type type)
        {
            int idx = this.basicTypes.IndexOf(RuntimeTypeModel.BasicTypeFinder, type);
            if (idx >= 0)
            {
                return ((RuntimeTypeModel.BasicType)this.basicTypes[idx]).Serializer;
            }
            IProtoSerializer result;
            lock (this.basicTypes)
            {
                idx = this.basicTypes.IndexOf(RuntimeTypeModel.BasicTypeFinder, type);
                if (idx >= 0)
                {
                    result = ((RuntimeTypeModel.BasicType)this.basicTypes[idx]).Serializer;
                }
                else
                {
                    MetaType.AttributeFamily family = MetaType.GetContractFamily(this, type, null);
                    WireType defaultWireType;
                    IProtoSerializer ser = (family == MetaType.AttributeFamily.None) ? ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out defaultWireType, false, false, false, false) : null;
                    if (ser != null)
                    {
                        this.basicTypes.Add(new RuntimeTypeModel.BasicType(type, ser));
                    }
                    result = ser;
                }
            }
            return result;
        }
        internal int FindOrAddAuto(Type type, bool demand, bool addWithContractOnly, bool addEvenIfAutoDisabled)
        {
            int key = this.types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type);
            if (key >= 0)
            {
                MetaType metaType = (MetaType)this.types[key];
                if (metaType.Pending)
                {
                    this.WaitOnLock(metaType);
                }
                return key;
            }
            bool shouldAdd = this.AutoAddMissingTypes || addEvenIfAutoDisabled;
            if (Helpers.IsEnum(type) || this.TryGetBasicTypeSerializer(type) == null)
            {
                Type underlyingType = TypeModel.ResolveProxies(type);
                if (underlyingType != null)
                {
                    key = this.types.IndexOf(RuntimeTypeModel.MetaTypeFinder, underlyingType);
                    type = underlyingType;
                }
                if (key < 0)
                {
                    int opaqueToken = 0;
                    try
                    {
                        this.TakeLock(ref opaqueToken);
                        MetaType metaType;
                        if ((metaType = this.RecogniseCommonTypes(type)) == null)
                        {
                            MetaType.AttributeFamily family = MetaType.GetContractFamily(this, type, null);
                            if (family == MetaType.AttributeFamily.AutoTuple)
                            {
                                addEvenIfAutoDisabled = (shouldAdd = true);
                            }
                            if (!shouldAdd || (!Helpers.IsEnum(type) && addWithContractOnly && family == MetaType.AttributeFamily.None))
                            {
                                if (demand)
                                {
                                    TypeModel.ThrowUnexpectedType(type);
                                }
                                return key;
                            }
                            metaType = this.Create(type);
                        }
                        metaType.Pending = true;
                        bool weAdded = false;
                        int winner = this.types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type);
                        if (winner < 0)
                        {
                            this.ThrowIfFrozen();
                            key = this.types.Add(metaType);
                            weAdded = true;
                        }
                        else
                        {
                            key = winner;
                        }
                        if (weAdded)
                        {
                            metaType.ApplyDefaultBehaviour();
                            metaType.Pending = false;
                        }
                    }
                    finally
                    {
                        this.ReleaseLock(opaqueToken);
                    }
                    return key;
                }
                return key;
            }
            if (shouldAdd && !addWithContractOnly)
            {
                throw MetaType.InbuiltType(type);
            }
            return -1;
        }
        private MetaType RecogniseCommonTypes(Type type)
        {
            return null;
        }
        private MetaType Create(Type type)
        {
            this.ThrowIfFrozen();
            return new MetaType(this, type, this.defaultFactory);
        }
        public MetaType Add(Type type, bool applyDefaultBehaviour)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            MetaType newType = this.FindWithoutAdd(type);
            if (newType != null)
            {
                return newType;
            }
            int opaqueToken = 0;
            if (type.IsInterface && base.MapType(MetaType.ienumerable).IsAssignableFrom(type) && TypeModel.GetListItemType(this, type) == null)
            {
                throw new ArgumentException("IEnumerable[<T>] data cannot be used as a meta-type unless an Add method can be resolved");
            }
            try
            {
                newType = this.RecogniseCommonTypes(type);
                if (newType != null)
                {
                    if (!applyDefaultBehaviour)
                    {
                        throw new ArgumentException("Default behaviour must be observed for certain types with special handling; " + type.FullName, "applyDefaultBehaviour");
                    }
                    applyDefaultBehaviour = false;
                }
                if (newType == null)
                {
                    newType = this.Create(type);
                }
                newType.Pending = true;
                this.TakeLock(ref opaqueToken);
                if (this.FindWithoutAdd(type) != null)
                {
                    throw new ArgumentException("Duplicate type", "type");
                }
                this.ThrowIfFrozen();
                this.types.Add(newType);
                if (applyDefaultBehaviour)
                {
                    newType.ApplyDefaultBehaviour();
                }
                newType.Pending = false;
            }
            finally
            {
                this.ReleaseLock(opaqueToken);
            }
            return newType;
        }
        private void ThrowIfFrozen()
        {
            if (this.GetOption(4))
            {
                throw new InvalidOperationException("The model cannot be changed once frozen");
            }
        }
        public void Freeze()
        {
            if (this.GetOption(2))
            {
                throw new InvalidOperationException("The default model cannot be frozen");
            }
            this.SetOption(4, true);
        }
        protected override int GetKeyImpl(Type type)
        {
            return this.GetKey(type, false, true);
        }
        internal int GetKey(Type type, bool demand, bool getBaseKey)
        {
            int result;
            try
            {
                int typeIndex = this.FindOrAddAuto(type, demand, true, false);
                if (typeIndex >= 0)
                {
                    MetaType mt = (MetaType)this.types[typeIndex];
                    if (getBaseKey)
                    {
                        mt = MetaType.GetRootType(mt);
                        typeIndex = this.FindOrAddAuto(mt.Type, true, true, false);
                    }
                }
                result = typeIndex;
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf(type.FullName) >= 0)
                {
                    throw;
                }
                throw new ProtoException(ex.Message + " (" + type.FullName + ")", ex);
            }
            return result;
        }
        protected internal override void Serialize(int key, object value, ProtoWriter dest)
        {
            ((MetaType)this.types[key]).Serializer.Write(value, dest);
        }
        protected internal override object Deserialize(int key, object value, ProtoReader source)
        {
            IProtoSerializer ser = ((MetaType)this.types[key]).Serializer;
            if (value == null && Helpers.IsValueType(ser.ExpectedType))
            {
                if (ser.RequiresOldValue)
                {
                    value = Activator.CreateInstance(ser.ExpectedType);
                }
                return ser.Read(value, source);
            }
            return ser.Read(value, source);
        }
        internal bool IsPrepared(Type type)
        {
            MetaType meta = this.FindWithoutAdd(type);
            return meta != null && meta.IsPrepared();
        }
        internal EnumSerializer.EnumPair[] GetEnumMap(Type type)
        {
            int index = this.FindOrAddAuto(type, false, false, false);
            if (index >= 0)
            {
                return ((MetaType)this.types[index]).GetEnumMap();
            }
            return null;
        }
        internal void TakeLock(ref int opaqueToken)
        {
            opaqueToken = 0;
            if (Monitor.TryEnter(this.types, this.metadataTimeoutMilliseconds))
            {
                opaqueToken = this.GetContention();
                return;
            }
            this.AddContention();
            throw new TimeoutException("Timeout while inspecting metadata; this may indicate a deadlock. This can often be avoided by preparing necessary serializers during application initialization, rather than allowing multiple threads to perform the initial metadata inspection; please also see the LockContended event");
        }
        private int GetContention()
        {
            return Interlocked.CompareExchange(ref this.contentionCounter, 0, 0);
        }
        private void AddContention()
        {
            Interlocked.Increment(ref this.contentionCounter);
        }
        internal void ReleaseLock(int opaqueToken)
        {
            if (opaqueToken != 0)
            {
                Monitor.Exit(this.types);
                if (opaqueToken != this.GetContention())
                {
                    LockContentedEventHandler handler = this.LockContended;
                    if (handler != null)
                    {
                        string stackTrace;
                        try
                        {
                            throw new ProtoException();
                        }
                        catch (Exception ex)
                        {
                            stackTrace = ex.StackTrace;
                        }
                        handler(this, new LockContentedEventArgs(stackTrace));
                    }
                }
            }
        }
        internal void ResolveListTypes(Type type, ref Type itemType, ref Type defaultType)
        {
            if (type == null)
            {
                return;
            }
            if (Helpers.GetTypeCode(type) != ProtoTypeCode.Unknown)
            {
                return;
            }
            if (this[type].IgnoreListHandling)
            {
                return;
            }
            if (type.IsArray)
            {
                if (type.GetArrayRank() != 1)
                {
                    throw new NotSupportedException("Multi-dimension arrays are supported");
                }
                itemType = type.GetElementType();
                if (itemType == base.MapType(typeof(byte)))
                {
                    Type type2;
                    itemType = (type2 = null);
                    defaultType = type2;
                }
                else
                {
                    defaultType = type;
                }
            }
            if (itemType == null)
            {
                itemType = TypeModel.GetListItemType(this, type);
            }
            if (itemType != null)
            {
                Type nestedItemType = null;
                Type nestedDefaultType = null;
                this.ResolveListTypes(itemType, ref nestedItemType, ref nestedDefaultType);
                if (nestedItemType != null)
                {
                    throw TypeModel.CreateNestedListsNotSupported();
                }
            }
            if (itemType != null && defaultType == null)
            {
                if (type.IsClass && !type.IsAbstract && Helpers.GetConstructor(type, Helpers.EmptyTypes, true) != null)
                {
                    defaultType = type;
                }
                if (defaultType == null && type.IsInterface)
                {
                    Type[] genArgs;
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == base.MapType(typeof(IDictionary<,>)) && itemType == base.MapType(typeof(KeyValuePair<,>)).MakeGenericType(genArgs = type.GetGenericArguments()))
                    {
                        defaultType = base.MapType(typeof(Dictionary<,>)).MakeGenericType(genArgs);
                    }
                    else
                    {
                        defaultType = base.MapType(typeof(List<>)).MakeGenericType(new Type[]
                        {
                            itemType
                        });
                    }
                }
                if (defaultType != null && !Helpers.IsAssignableFrom(type, defaultType))
                {
                    defaultType = null;
                }
            }
        }
        internal string GetSchemaTypeName(Type effectiveType, DataFormat dataFormat, bool asReference, bool dynamicType, ref bool requiresBclImport)
        {
            Type tmp = Helpers.GetUnderlyingType(effectiveType);
            if (tmp != null)
            {
                effectiveType = tmp;
            }
            if (effectiveType == base.MapType(typeof(byte[])))
            {
                return "bytes";
            }
            WireType wireType;
            IProtoSerializer ser = ValueMember.TryGetCoreSerializer(this, dataFormat, effectiveType, out wireType, false, false, false, false);
            if (ser == null)
            {
                if (asReference || dynamicType)
                {
                    requiresBclImport = true;
                    return "bcl.NetObjectProxy";
                }
                return this[effectiveType].GetSurrogateOrBaseOrSelf(true).GetSchemaTypeName();
            }
            else
            {
                if (!(ser is ParseableSerializer))
                {
                    ProtoTypeCode typeCode = Helpers.GetTypeCode(effectiveType);
                    switch (typeCode)
                    {
                        case ProtoTypeCode.Boolean:
                            return "bool";
                        case ProtoTypeCode.Char:
                        case ProtoTypeCode.Byte:
                        case ProtoTypeCode.UInt16:
                        case ProtoTypeCode.UInt32:
                            if (dataFormat == DataFormat.FixedSize)
                            {
                                return "fixed32";
                            }
                            return "uint32";
                        case ProtoTypeCode.SByte:
                        case ProtoTypeCode.Int16:
                        case ProtoTypeCode.Int32:
                            switch (dataFormat)
                            {
                                case DataFormat.ZigZag:
                                    return "sint32";
                                case DataFormat.FixedSize:
                                    return "sfixed32";
                            }
                            return "int32";
                        case ProtoTypeCode.Int64:
                            switch (dataFormat)
                            {
                                case DataFormat.ZigZag:
                                    return "sint64";
                                case DataFormat.FixedSize:
                                    return "sfixed64";
                            }
                            return "int64";
                        case ProtoTypeCode.UInt64:
                            if (dataFormat == DataFormat.FixedSize)
                            {
                                return "fixed64";
                            }
                            return "uint64";
                        case ProtoTypeCode.Single:
                            return "float";
                        case ProtoTypeCode.Double:
                            return "double";
                        case ProtoTypeCode.Decimal:
                            requiresBclImport = true;
                            return "bcl.Decimal";
                        case ProtoTypeCode.DateTime:
                            requiresBclImport = true;
                            return "bcl.DateTime";
                        case (ProtoTypeCode)17:
                            break;
                        case ProtoTypeCode.String:
                            if (asReference)
                            {
                                requiresBclImport = true;
                            }
                            if (!asReference)
                            {
                                return "string";
                            }
                            return "bcl.NetObjectProxy";
                        default:
                            switch (typeCode)
                            {
                                case ProtoTypeCode.TimeSpan:
                                    requiresBclImport = true;
                                    return "bcl.TimeSpan";
                                case ProtoTypeCode.Guid:
                                    requiresBclImport = true;
                                    return "bcl.Guid";
                            }
                            break;
                    }
                    throw new NotSupportedException("No .proto map found for: " + effectiveType.FullName);
                }
                if (asReference)
                {
                    requiresBclImport = true;
                }
                if (!asReference)
                {
                    return "string";
                }
                return "bcl.NetObjectProxy";
            }
        }
        public void SetDefaultFactory(MethodInfo methodInfo)
        {
            this.VerifyFactory(methodInfo, null);
            this.defaultFactory = methodInfo;
        }
        internal void VerifyFactory(MethodInfo factory, Type type)
        {
            if (factory != null)
            {
                if (type != null && Helpers.IsValueType(type))
                {
                    throw new InvalidOperationException();
                }
                if (!factory.IsStatic)
                {
                    throw new ArgumentException("A factory-method must be static", "factory");
                }
                if (type != null && factory.ReturnType != type && factory.ReturnType != base.MapType(typeof(object)))
                {
                    throw new ArgumentException("The factory-method must return object" + ((type == null) ? "" : (" or " + type.FullName)), "factory");
                }
                if (!CallbackSet.CheckCallbackParameters(this, factory))
                {
                    throw new ArgumentException("Invalid factory signature in " + factory.DeclaringType.FullName + "." + factory.Name, "factory");
                }
            }
        }
    }
}
