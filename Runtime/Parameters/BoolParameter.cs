using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class BoolParameter : Parameter<bool>
    {
        public BoolParameter(int hash) : base(hash) { }
        [MemoryPackConstructor]
        public BoolParameter(int hash, bool value) : base(hash, value) { }
    }
}