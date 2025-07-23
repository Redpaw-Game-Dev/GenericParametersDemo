using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    public partial class ModFloat : Mod<float>
    {
        public ModFloat() { }
        public ModFloat(float value) : base(value) { }
        public ModFloat(float value, ModAction action) : base(value, action) { }
        public ModFloat(SerializableGuid sourceGuid, float value, ModAction action) : base(sourceGuid, value, action) { }
        [MemoryPackConstructor]
        public ModFloat(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, float value)
            : base(guid, sourceGuid, action, value) { }
        public override Mod Copy() => new ModFloat(SourceGuid, Value, Action);
    }
}