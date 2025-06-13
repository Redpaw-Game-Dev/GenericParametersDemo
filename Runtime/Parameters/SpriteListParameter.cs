using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class SpriteListParameter : Parameter<List<Sprite>>
    {
        public SpriteListParameter(int hash) : base(hash)
        {
            _value = new List<Sprite>();
        }
        
        public SpriteListParameter(int hash, List<Sprite> value) : base(hash, value) { }
    }
}