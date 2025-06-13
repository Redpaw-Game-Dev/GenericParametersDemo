namespace LazyRedpaw.GenericParameters
{
    public class IntParameter : Parameter<int>
    {
        public IntParameter(int hash) : base(hash) { }
        public IntParameter(int hash, int value) : base(hash, value) { }
    }
}