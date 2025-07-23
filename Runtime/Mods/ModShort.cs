using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    public partial class ModShort : Mod<short>
    {
        public ModShort() { }
        public ModShort(short value) : base(value) { }
        public ModShort(short value, ModAction action) : base(value, action) { }
        public ModShort(SerializableGuid sourceGuid, short value, ModAction action) : base(sourceGuid, value, action) { }
        [MemoryPackConstructor]
        public ModShort(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, short value)
            : base(guid, sourceGuid, action, value) { }
        public override Mod Copy() => new ModShort(SourceGuid, Value, Action);
    }
}