using MemoryPack;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class ColorParameter : Parameter<Color>
    {
        public ColorParameter(int hash) : base(hash) { }
        [MemoryPackConstructor]
        public ColorParameter(int hash, Color value) : base(hash, value) { }
    }
}