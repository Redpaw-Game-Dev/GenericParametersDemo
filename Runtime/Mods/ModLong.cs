using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    public partial class ModLong : Mod<long>
    {
        public ModLong() { }
        public ModLong(long value) : base(value) { }
        public ModLong(long value, ModAction action) : base(value, action) { }
        public ModLong(SerializableGuid sourceGuid, long value, ModAction action) : base(sourceGuid, value, action) { }
        [MemoryPackConstructor]
        public ModLong(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, long value)
            : base(guid, sourceGuid, action, value) { }
        public override Mod Copy() => new ModLong(SourceGuid, Value, Action);
    }
}