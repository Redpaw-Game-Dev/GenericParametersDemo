using System;
using LazyRedpaw.GuidSerialization;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModByte : Mod<byte>
    {
        public ModByte() { }
        public ModByte(byte value) : base(value) { }
        public ModByte(byte value, ModAction action) : base(value, action) { }
        public ModByte(SerializableGuid sourceGuid, byte value, ModAction action) : base(sourceGuid, value, action) { }
        public override Mod Copy() => new ModByte(SourceGuid, Value, Action);
    }
}