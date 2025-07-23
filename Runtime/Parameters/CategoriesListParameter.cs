using MemoryPack;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class CategoriesListParameter : Parameter
    {
        [SerializeField] private CategoriesList _categories;
        
        public CategoriesList Categories => _categories;
        
        public CategoriesListParameter(int hash) : base(hash)
        {
            _categories = new CategoriesList();
        }
        [MemoryPackConstructor]
        public CategoriesListParameter(int hash, CategoriesList categories) : base(hash)
        {
            _categories = categories;
        }
    }
}