using System;

namespace CommonLib.Clients.Serialization
{
    public sealed class SerializationContext
    {
        private bool frozen;
        private object context;
        private static readonly SerializationContext @default;
        public object Context
        {
            get
            {
                return this.context;
            }
            set
            {
                if (this.context != value)
                {
                    this.ThrowIfFrozen();
                    this.context = value;
                }
            }
        }
        internal static SerializationContext Default
        {
            get
            {
                return SerializationContext.@default;
            }
        }
        internal void Freeze()
        {
            this.frozen = true;
        }
        private void ThrowIfFrozen()
        {
            if (this.frozen)
            {
                throw new InvalidOperationException("The serialization-context cannot be changed once it is in use");
            }
        }
        static SerializationContext()
        {
            SerializationContext.@default = new SerializationContext();
            SerializationContext.@default.Freeze();
        }
    }
}
