using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class FloatParameter : Parameter<float>
    {
        public FloatParameter(int hash) : base(hash) { }
        [MemoryPackConstructor]
        public FloatParameter(int hash, float value) : base(hash, value) { }
    }
}