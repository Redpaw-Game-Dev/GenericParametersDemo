using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    public partial class ModInt : Mod<int>
    {
        public ModInt() { }
        public ModInt(int value) : base(value) { }
        public ModInt(int value, ModAction action) : base(value, action) { }
        public ModInt(SerializableGuid sourceGuid, int value, ModAction action) : base(sourceGuid, value, action) { }
        [MemoryPackConstructor]
        public ModInt(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, int value)
            : base(guid, sourceGuid, action, value) { }
        public override Mod Copy() => new ModInt(SourceGuid, Value, Action);
    }
}