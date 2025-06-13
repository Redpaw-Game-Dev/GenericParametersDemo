using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class ParticleSystemListParameter : Parameter<List<ParticleSystem>>
    {
        public ParticleSystemListParameter(int hash) : base(hash)
        {
            _value = new List<ParticleSystem>();
        }
        
        public ParticleSystemListParameter(int hash, List<ParticleSystem> value) : base(hash, value) { }
    }
}