using System.Collections.Generic;

namespace LazyRedpaw.GenericParameters
{
    public class StringListParameter : Parameter<List<string>>
    {
        public StringListParameter(int hash) : base(hash)
        {
            _value = new List<string>();
        }
        
        public StringListParameter(int hash, List<string> value) : base(hash, value) { }
    }
}