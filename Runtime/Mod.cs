using System;
using LazyRedpaw.GuidSerialization;
using MemoryPack;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(ModByte))]
    [MemoryPackUnion(1, typeof(ModInt))]
    [MemoryPackUnion(2, typeof(ModFloat))]
    [MemoryPackUnion(3, typeof(ModDouble))]
    [MemoryPackUnion(4, typeof(ModLong))]
    [MemoryPackUnion(5, typeof(ModShort))]
    [MemoryPackUnion(6, typeof(ModUInt))]
    [MemoryPackUnion(7, typeof(ModULong))]
    [MemoryPackUnion(8, typeof(ModUShort))]
    public abstract partial class Mod
    {
        [SerializeField] protected SerializableGuid _guid;
        [SerializeField] protected SerializableGuid _sourceGuid;
        [SerializeField] protected ModAction _action = ModAction.NumberAdd;

        protected Mod() => _guid = SerializableGuid.NewGuid();
        protected Mod(SerializableGuid sourceGuid) : this() => _sourceGuid = sourceGuid;
        [MemoryPackConstructor]
        protected Mod(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action) : this()
        {
            _guid = guid;
            _sourceGuid = sourceGuid;
            _action = action;
        }

        public virtual SerializableGuid Guid => _guid;
        public virtual SerializableGuid SourceGuid { get => _sourceGuid; set => _sourceGuid = value; }
        public virtual ModAction Action => _action;
        public virtual int Order => (int)_action;

        public abstract T GetValue<T>();
        public abstract Mod Copy();
    }
    
    [Serializable]
    public abstract class Mod<T> : Mod
    {
        [SerializeField] protected T _value;

        public virtual T Value => _value;

        protected Mod() : base() { }
        protected Mod(SerializableGuid sourceGuid) : base(sourceGuid) { }
        protected Mod(T value) : this() => _value = value;
        protected Mod(SerializableGuid sourceGuid, T value) : base(sourceGuid) => _value = value;
        protected Mod(T value, ModAction action) : this(value) => _action = action;
        protected Mod(SerializableGuid sourceGuid, T value, ModAction action) : this(sourceGuid, value) => _action = action;
        protected Mod(SerializableGuid guid, SerializableGuid sourceGuid, ModAction action, T value) 
            : base(guid, sourceGuid, action) => _value = value;

        public virtual T GetValue() => Value;
        public override T1 GetValue<T1>()
        {
            try
            {
                return (T1)Convert.ChangeType(Value, typeof(T1));
            }
            catch (Exception e)
            {
                Debug.LogError("MOD VALUE CONVERSION ERROR: " + e.Message);
                return (T1)Convert.ChangeType(0, typeof(T1));
            }
        }
    }
}
