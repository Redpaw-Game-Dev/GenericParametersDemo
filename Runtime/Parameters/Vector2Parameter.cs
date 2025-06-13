using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class Vector2Parameter : Parameter<Vector2>
    {
        public Vector2Parameter(int hash) : base(hash) { }
        public Vector2Parameter(int hash, Vector2 value) : base(hash, value) { }
    }
}