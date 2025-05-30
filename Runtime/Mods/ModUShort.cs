using System;
using LazyRedpaw.GuidSerialization;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModUShort : Mod<ushort>
    {
        public ModUShort() { }
        public ModUShort(ushort value) : base(value) { }
        public ModUShort(ushort value, ModAction action) : base(value, action) { }
        public ModUShort(SerializableGuid sourceGuid, ushort value, ModAction action) : base(sourceGuid, value, action) { }
        public override Mod Copy() => new ModUShort(SourceGuid, Value, Action);
    }
}