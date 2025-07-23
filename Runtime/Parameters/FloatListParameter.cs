using System.Collections.Generic;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class FloatListParameter : Parameter<List<float>>
    {
        public FloatListParameter(int hash) : base(hash)
        {
            _value = new List<float>();
        }
        
        [MemoryPackConstructor]
        public FloatListParameter(int hash, List<float> value) : base(hash, value) { }
    }
}