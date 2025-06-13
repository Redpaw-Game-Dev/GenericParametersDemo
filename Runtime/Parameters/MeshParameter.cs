using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class MeshParameter : Parameter<Mesh>
    {
        public MeshParameter(int hash) : base(hash) { }
        public MeshParameter(int hash, Mesh value) : base(hash, value) { }
    }
}