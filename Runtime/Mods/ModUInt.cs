using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    public partial class ModUInt : Mod<uint>
    {
        public ModUInt() { }
        public ModUInt(uint value) : base(value) { }
        public ModUInt(uint value, ModAction action) : base(value, action) { }
        public ModUInt(SerializableGuid sourceGuid, uint value, ModAction action) : base(sourceGuid, value, action) { }
        [MemoryPackConstructor]
        public ModUInt(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, uint value)
            : base(guid, sourceGuid, action, value) { }
        public override Mod Copy() => new ModUInt(SourceGuid, Value, Action);
    }
}