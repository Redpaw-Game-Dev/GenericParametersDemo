using System.Collections.Generic;

namespace LazyRedpaw.GenericParameters
{
    public class FloatListParameter : Parameter<List<float>>
    {
        public FloatListParameter(int hash) : base(hash)
        {
            _value = new List<float>();
        }
        
        public FloatListParameter(int hash, List<float> value) : base(hash, value) { }
    }
}