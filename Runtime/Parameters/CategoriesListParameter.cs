using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class CategoriesListParameter : Parameter
    {
        [SerializeField] private CategoriesList _categories;

        public CategoriesListParameter(int hash) : base(hash)
        {
            _categories = new CategoriesList();
        }
    }
}