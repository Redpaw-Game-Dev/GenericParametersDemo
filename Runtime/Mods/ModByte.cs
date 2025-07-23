using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    public partial class ModByte : Mod<byte>
    {
        public ModByte() { }
        public ModByte(byte value) : base(value) { }
        public ModByte(byte value, ModAction action) : base(value, action) { }
        public ModByte(SerializableGuid sourceGuid, byte value, ModAction action) : base(sourceGuid, value, action) { }
        [MemoryPackConstructor]
        public ModByte(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, byte value)
            : base(guid, sourceGuid, action, value) { }

        public override Mod Copy() => new ModByte(SourceGuid, Value, Action);
    }
}