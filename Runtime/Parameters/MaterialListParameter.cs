using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class MaterialListParameter : Parameter<List<Material>>
    {
        public MaterialListParameter(int hash) : base(hash)
        {
            _value = new List<Material>();
        }
        
        public MaterialListParameter(int hash, List<Material> value) : base(hash, value) { }
    }
}