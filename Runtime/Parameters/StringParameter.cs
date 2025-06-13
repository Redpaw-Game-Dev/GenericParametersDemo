namespace LazyRedpaw.GenericParameters
{
    public class StringParameter : Parameter<string>
    {
        public StringParameter(int hash) : base(hash) { }
        public StringParameter(int hash, string value) : base(hash, value) { }
    }
}