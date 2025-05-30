using System;
using LazyRedpaw.GuidSerialization;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModLong : Mod<long>
    {
        public ModLong() { }
        public ModLong(long value) : base(value) { }
        public ModLong(long value, ModAction action) : base(value, action) { }
        public ModLong(SerializableGuid sourceGuid, long value, ModAction action) : base(sourceGuid, value, action) { }
        public override Mod Copy() => new ModLong(SourceGuid, Value, Action);
    }
}