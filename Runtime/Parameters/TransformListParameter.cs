using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class TransformListParameter : Parameter<List<Transform>>
    {
        public TransformListParameter(int hash) : base(hash)
        {
            _value = new List<Transform>();
        }
        
        public TransformListParameter(int hash, List<Transform> value) : base(hash, value) { }
    }
}