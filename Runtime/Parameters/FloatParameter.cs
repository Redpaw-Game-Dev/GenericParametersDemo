namespace LazyRedpaw.GenericParameters
{
    public class FloatParameter : Parameter<float>
    {
        public FloatParameter(int hash) : base(hash) { }
        public FloatParameter(int hash, float value) : base(hash, value) { }
    }
}