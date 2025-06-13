using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class RendererListParameter : Parameter<List<Renderer>>
    {
        public RendererListParameter(int hash) : base(hash)
        {
            _value = new List<Renderer>();
        }
        
        public RendererListParameter(int hash, List<Renderer> value) : base(hash, value) { }
    }
}