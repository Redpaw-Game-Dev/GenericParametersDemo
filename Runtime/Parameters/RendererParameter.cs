using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class RendererParameter : Parameter<Renderer>
    {
        public RendererParameter(int hash) : base(hash) { }
        public RendererParameter(int hash, Renderer value) : base(hash, value) { }
    }
}