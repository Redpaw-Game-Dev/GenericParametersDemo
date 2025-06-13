using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class MeshFilterListParameter : Parameter<List<MeshFilter>>
    {
        public MeshFilterListParameter(int hash) : base(hash)
        {
            _value = new List<MeshFilter>();
        }
        
        public MeshFilterListParameter(int hash, List<MeshFilter> value) : base(hash, value) { }
    }
}