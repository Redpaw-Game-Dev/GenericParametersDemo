using System;
using LazyRedpaw.GuidSerialization;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModUInt : Mod<uint>
    {
        public ModUInt() { }
        public ModUInt(uint value) : base(value) { }
        public ModUInt(uint value, ModAction action) : base(value, action) { }
        public ModUInt(SerializableGuid sourceGuid, uint value, ModAction action) : base(sourceGuid, value, action) { }
        public override Mod Copy() => new ModUInt(SourceGuid, Value, Action);
    }
}