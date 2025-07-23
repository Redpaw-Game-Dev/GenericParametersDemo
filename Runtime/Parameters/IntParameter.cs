using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class IntParameter : Parameter<int>
    {
        public IntParameter(int hash) : base(hash) { }
        [MemoryPackConstructor]
        public IntParameter(int hash, int value) : base(hash, value) { }
    }
}