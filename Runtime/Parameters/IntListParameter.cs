using System.Collections.Generic;

namespace LazyRedpaw.GenericParameters
{
    public class IntListParameter : Parameter<List<int>>
    {
        public IntListParameter(int hash) : base(hash)
        {
            _value = new List<int>();
        }
        
        public IntListParameter(int hash, List<int> value) : base(hash, value) { }
    }
}