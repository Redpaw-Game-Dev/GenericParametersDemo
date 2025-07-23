using MemoryPack;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class Vector2Parameter : Parameter<Vector2>
    {
        public Vector2Parameter(int hash) : base(hash) { }
        [MemoryPackConstructor]
        public Vector2Parameter(int hash, Vector2 value) : base(hash, value) { }
    }
}