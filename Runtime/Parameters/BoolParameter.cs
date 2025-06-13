namespace LazyRedpaw.GenericParameters
{
    public class BoolParameter : Parameter<bool>
    {
        public BoolParameter(int hash) : base(hash) { }
        public BoolParameter(int hash, bool value) : base(hash, value) { }
    }
}