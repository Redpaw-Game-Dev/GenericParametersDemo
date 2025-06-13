using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class Vector3ListParameter : Parameter<List<Vector3>>
    {
        public Vector3ListParameter(int hash) : base(hash)
        {
            _value = new List<Vector3>();
        }
        
        public Vector3ListParameter(int hash, List<Vector3> value) : base(hash, value) { }
    }
}