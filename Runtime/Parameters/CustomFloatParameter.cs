using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class CustomFloatParameter : Parameter<float>
    {
        [SerializeField] private float _max;
        [SerializeField] private float _min;
        [SerializeField] private string[] _strings;
        
        public CustomFloatParameter() { }
        public CustomFloatParameter(int hash) : base(hash) { }
    }
}