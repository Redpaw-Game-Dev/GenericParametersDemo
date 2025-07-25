﻿using System;
using LazyRedpaw.GuidSerialization;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModShort : Mod<short>
    {
        public ModShort() { }
        public ModShort(short value) : base(value) { }
        public ModShort(short value, ModAction action) : base(value, action) { }
        public ModShort(SerializableGuid sourceGuid, short value, ModAction action) : base(sourceGuid, value, action) { }
        public override Mod Copy() => new ModShort(SourceGuid, Value, Action);
    }
}