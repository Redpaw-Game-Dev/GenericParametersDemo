using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class MaterialParameter : Parameter<Material>
    {
        public MaterialParameter(int hash) : base(hash) { }
        public MaterialParameter(int hash, Material value) : base(hash, value) { }
    }
}