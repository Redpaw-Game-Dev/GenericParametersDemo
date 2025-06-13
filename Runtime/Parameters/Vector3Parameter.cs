using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class Vector3Parameter : Parameter<Vector3>
    {
        public Vector3Parameter(int hash) : base(hash) { }
        public Vector3Parameter(int hash, Vector3 value) : base(hash, value) { }
    }
}