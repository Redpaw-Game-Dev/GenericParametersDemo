using System.Collections.Generic;
using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class StringListParameter : Parameter<List<string>>
    {
        public StringListParameter(int hash) : base(hash)
        {
            _value = new List<string>();
        }
        [MemoryPackConstructor]
        public StringListParameter(int hash, List<string> value) : base(hash, value) { }
    }
}