using System.Collections.Generic;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class IntListParameter : Parameter<List<int>>
    {
        public IntListParameter(int hash) : base(hash)
        {
            _value = new List<int>();
        }
        
        [MemoryPackConstructor]
        public IntListParameter(int hash, List<int> value) : base(hash, value) { }
    }
}