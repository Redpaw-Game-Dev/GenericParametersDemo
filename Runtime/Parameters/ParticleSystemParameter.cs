using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class ParticleSystemParameter : Parameter<ParticleSystem>
    {
        public ParticleSystemParameter(int hash) : base(hash) { }
        public ParticleSystemParameter(int hash, ParticleSystem value) : base(hash, value) { }
    }
}