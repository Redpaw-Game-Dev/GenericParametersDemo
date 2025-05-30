using System;
using LazyRedpaw.GuidSerialization;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModDouble : Mod<double>
    {
        public ModDouble() { }
        public ModDouble(double value) : base(value) { }
        public ModDouble(double value, ModAction action) : base(value, action) { }
        public ModDouble(SerializableGuid sourceGuid, double value, ModAction action) : base(sourceGuid, value, action) { }
        public override Mod Copy() => new ModDouble(SourceGuid, Value, Action);
    }
}