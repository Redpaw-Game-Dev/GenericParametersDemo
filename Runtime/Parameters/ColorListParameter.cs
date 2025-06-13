using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class ColorListParameter : Parameter<List<Color>>
    {
        public ColorListParameter(int hash) : base(hash)
        {
            _value = new List<Color>();
        }
        
        public ColorListParameter(int hash, List<Color> value) : base(hash, value) { }
    }
}