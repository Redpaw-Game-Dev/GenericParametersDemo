using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class Vector3ListParameter : Parameter<List<Vector3>>
    {
        public Vector3ListParameter(int hash) : base(hash)
        {
            _value = new List<Vector3>();
        }
        [MemoryPackConstructor]
        public Vector3ListParameter(int hash, List<Vector3> value) : base(hash, value) { }
    }
}