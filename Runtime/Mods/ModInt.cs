using System;
using LazyRedpaw.GuidSerialization;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModInt : Mod<int>
    {
        public ModInt() { }
        public ModInt(int value) : base(value) { }
        public ModInt(int value, ModAction action) : base(value, action) { }
        public ModInt(SerializableGuid sourceGuid, int value, ModAction action) : base(sourceGuid, value, action) { }
        public override Mod Copy() => new ModInt(SourceGuid, Value, Action);
    }
}