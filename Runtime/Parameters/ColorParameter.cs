using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class ColorParameter : Parameter<Color>
    {
        public ColorParameter(int hash) : base(hash) { }
        public ColorParameter(int hash, Color value) : base(hash, value) { }
    }
}