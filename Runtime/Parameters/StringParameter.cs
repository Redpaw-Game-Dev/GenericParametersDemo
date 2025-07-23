using MemoryPack;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class StringParameter : Parameter<string>
    {
        public StringParameter(int hash) : base(hash) { }
        [MemoryPackConstructor]
        public StringParameter(int hash, string value) : base(hash, value) { }
    }
}