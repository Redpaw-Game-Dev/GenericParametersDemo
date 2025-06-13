using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class MeshFilterParameter : Parameter<MeshFilter>
    {
        public MeshFilterParameter(int hash) : base(hash) { }
        public MeshFilterParameter(int hash, MeshFilter value) : base(hash, value) { }
    }
}