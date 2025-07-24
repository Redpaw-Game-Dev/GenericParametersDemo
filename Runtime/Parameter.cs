using System;
using LazyRedpaw.StaticHashes;
using MemoryPack;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(BoolParameter))]
    [MemoryPackUnion(1, typeof(CategoriesListParameter))]
    [MemoryPackUnion(2, typeof(ColorListParameter))]
    [MemoryPackUnion(3, typeof(ColorParameter))]
    [MemoryPackUnion(4, typeof(FloatListParameter))]
    [MemoryPackUnion(5, typeof(FloatParameter))]
    [MemoryPackUnion(6, typeof(IntListParameter))]
    [MemoryPackUnion(7, typeof(IntParameter))]
    [MemoryPackUnion(8, typeof(ModableFloat))]
    [MemoryPackUnion(9, typeof(ModableInt))]
    [MemoryPackUnion(10, typeof(StringListParameter))]
    [MemoryPackUnion(11, typeof(StringParameter))]
    [MemoryPackUnion(12, typeof(Vector2ListParameter))]
    [MemoryPackUnion(13, typeof(Vector2Parameter))]
    [MemoryPackUnion(14, typeof(Vector3ListParameter))]
    [MemoryPackUnion(15, typeof(Vector3Parameter))]
    public partial interface IParameter
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