using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    public partial class ModDouble : Mod<double>
    {
        public ModDouble() { }
        public ModDouble(double value) : base(value) { }
        public ModDouble(double value, ModAction action) : base(value, action) { }
        public ModDouble(SerializableGuid sourceGuid, double value, ModAction action) : base(sourceGuid, value, action) { }
        [MemoryPackConstructor]
        public ModDouble(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, double value)
            : base(guid, sourceGuid, action, value) { }
        public override Mod Copy() => new ModDouble(SourceGuid, Value, Action);
    }
}