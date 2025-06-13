using System.Collections.Generic;
using System.Numerics;

namespace LazyRedpaw.GenericParameters
{
    public class Vector2ListParameter : Parameter<List<Vector2>>
    {
        public Vector2ListParameter(int hash) : base(hash)
        {
            _value = new List<Vector2>();
        }
        
        public Vector2ListParameter(int hash, List<Vector2> value) : base(hash, value) { }
    }
}