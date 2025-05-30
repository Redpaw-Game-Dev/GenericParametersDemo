using System;
using LazyRedpaw.GuidSerialization;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModFloat : Mod<float>
    {
        public ModFloat() { }
        public ModFloat(float value) : base(value) { }
        public ModFloat(float value, ModAction action) : base(value, action) { }
        public ModFloat(SerializableGuid sourceGuid, float value, ModAction action) : base(sourceGuid, value, action) { }
        public override Mod Copy() => new ModFloat(SourceGuid, Value, Action);
    }
}