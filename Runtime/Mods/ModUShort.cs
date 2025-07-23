using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    public partial class ModUShort : Mod<ushort>
    {
        public ModUShort() { }
        public ModUShort(ushort value) : base(value) { }
        public ModUShort(ushort value, ModAction action) : base(value, action) { }
        public ModUShort(SerializableGuid sourceGuid, ushort value, ModAction action) : base(sourceGuid, value, action) { }
        [MemoryPackConstructor]
        public ModUShort(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, ushort value)
            : base(guid, sourceGuid, action, value) { }
        public override Mod Copy() => new ModUShort(SourceGuid, Value, Action);
    }
}