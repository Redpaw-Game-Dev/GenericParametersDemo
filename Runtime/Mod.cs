using System;
using LazyRedpaw.GuidSerialization;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public abstract class Mod
    {
        [SerializeField] protected SerializableGuid _guid;
        [SerializeField] protected SerializableGuid _sourceGuid;
        [SerializeField] protected ModAction _action = ModAction.NumberAdd;

        protected Mod() => _guid = SerializableGuid.NewGuid();
        protected Mod(SerializableGuid sourceGuid) : this() => _sourceGuid = sourceGuid;

        public SerializableGuid Guid => _guid;
        public SerializableGuid SourceGuid { get => _sourceGuid; set => _sourceGuid = value; }
        public ModAction Action => _action;
        public int Order => (int)_action;

        public abstract T GetValue<T>();
        public abstract Mod Copy();
    }
    
    [Serializable]
    public abstract class Mod<T> : Mod
    {
        [SerializeField] private T _value;

        public T Value => _value;

        protected Mod() : base() { }
        protected Mod(SerializableGuid sourceGuid) : base(sourceGuid) { }
        protected Mod(T value) : this() => _value = value;
        protected Mod(SerializableGuid sourceGuid, T value) : base(sourceGuid) => _value = value;
        protected Mod(T value, ModAction action) : this(value) => _action = action;
        protected Mod(SerializableGuid sourceGuid, T value, ModAction action) : this(sourceGuid, value) => _action = action;
        public T GetValue() => Value;
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
