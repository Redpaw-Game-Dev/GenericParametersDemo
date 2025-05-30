using System;
using LazyRedpaw.GuidSerialization;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModULong : Mod<ulong>
    {
        public ModULong() { }
        public ModULong(ulong value) : base(value) { }
        public ModULong(ulong value, ModAction action) : base(value, action) { }
        public ModULong(SerializableGuid sourceGuid, ulong value, ModAction action) : base(sourceGuid, value, action) { }
        public override Mod Copy() => new ModULong(SourceGuid, Value, Action);
    }
}