using System;
using LazyRedpaw.StaticHashes;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public interface IParameter
    {
        public int Hash { get; }

        public IParameter Copy();
    }
    
    [Serializable]
    public class Parameter : IParameter
    {
        [SerializeField, StaticHash] protected int _hash;
        
        public virtual int Hash => _hash;

        public virtual event Action Changed;
        
        public Parameter() { }
        public Parameter(int hash) => _hash = hash;

        public virtual IParameter Copy() => new Parameter(_hash);
        
        protected virtual void InvokeChanged() => Changed?.Invoke();
    }

    public abstract class Parameter<T> : Parameter
    {
        [SerializeField] protected T _value;

        public virtual T Value
        {
            get => _value;
            set
            {
                _value = value;
                InvokeChanged();
            }
        }

        protected Parameter() { }
        protected Parameter(int hash) : base(hash) { }
        protected Parameter(T value) : this() => _value = value;
        protected Parameter(int hash, T value) : base(hash) => _value = value;
    }
}