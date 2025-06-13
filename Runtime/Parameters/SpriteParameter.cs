using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class SpriteParameter : Parameter<Sprite>
    {
        public SpriteParameter(int hash) : base(hash) { }
        public SpriteParameter(int hash, Sprite value) : base(hash, value) { }
    }
}