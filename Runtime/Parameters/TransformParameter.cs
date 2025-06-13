using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class TransformParameter : Parameter<Transform>
    {
        public TransformParameter(int hash) : base(hash) { }
        public TransformParameter(int hash, Transform value) : base(hash, value) { }
    }
}