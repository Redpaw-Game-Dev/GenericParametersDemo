using System.Collections.Generic;
using System.Numerics;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class Vector2ListParameter : Parameter<List<Vector2>>
    {
        public Vector2ListParameter(int hash) : base(hash)
        {
            _value = new List<Vector2>();
        }
        [MemoryPackConstructor]
        public Vector2ListParameter(int hash, List<Vector2> value) : base(hash, value) { }
    }
}