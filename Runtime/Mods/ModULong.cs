using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    public partial class ModULong : Mod<ulong>
    {
        public ModULong() { }
        public ModULong(ulong value) : base(value) { }
        public ModULong(ulong value, ModAction action) : base(value, action) { }
        public ModULong(SerializableGuid sourceGuid, ulong value, ModAction action) : base(sourceGuid, value, action) { }
        [MemoryPackConstructor]
        public ModULong(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, ulong value)
            : base(guid, sourceGuid, action, value) { }
        public override Mod Copy() => new ModULong(SourceGuid, Value, Action);
    }
}