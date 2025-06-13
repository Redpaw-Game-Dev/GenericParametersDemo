using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class MeshListParameter : Parameter<List<Mesh>>
    {
        public MeshListParameter(int hash) : base(hash)
        {
            _value = new List<Mesh>();
        }
        
        public MeshListParameter(int hash, List<Mesh> value) : base(hash, value) { }
    }
}