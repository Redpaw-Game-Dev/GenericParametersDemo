using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class ColorListParameter : Parameter<List<Color>>
    {
        public ColorListParameter(int hash) : base(hash)
        {
            _value = new List<Color>();
        }
        
        [MemoryPackConstructor]
        public ColorListParameter(int hash, List<Color> value) : base(hash, value) { }
    }
}