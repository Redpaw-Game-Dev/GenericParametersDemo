using MemoryPack;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class Vector3Parameter : Parameter<Vector3>
    {
        public Vector3Parameter(int hash) : base(hash) { }
        [MemoryPackConstructor]
        public Vector3Parameter(int hash, Vector3 value) : base(hash, value) { }
    }
}