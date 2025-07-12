using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class CategoriesList
    {
        [SerializeReference] protected List<Category> _categories;
        
        public virtual Category this[int i] => _categories[i];
        public virtual int Count => _categories.Count;

        public virtual event Action<Category> CategoryAdded;
        public virtual event Action<Category> CategoryRemoved;
        public virtual event Action<Category, Category> CategoryReplaced;
        public virtual event Action<Parameter, Category> ParameterAdded;
        public virtual event Action<Parameter, Category> ParameterRemoved;
        public virtual event Action<Parameter, Parameter, Category> ParameterReplaced;
        
        public CategoriesList()
        {
            _categories = new List<Category>();
        }

        public virtual List<Category> GetAllCategories() => new List<Category>(_categories);

        public virtual Category GetCategory(int index) => this[index];
        
        public virtual T GetCategory<T>(int index) where T : Category => (T)this[index];
        
        public virtual Category GetCategoryByHash(int hash)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Hash == hash) return _categories[i];
            }
            return null;
        }

        public virtual T GetCategoryByHash<T>(int hash) where T : Category => (T)GetCategoryByHash(hash);
        
        public virtual List<Category> GetCategoriesByHash(int[] hashes)
        {
            if (hashes == null) return null;
            List<Category> result = new List<Category>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetCategoryByHash(hashes[i]));
            }
            return result;
        }
        
        public virtual List<T> GetCategoriesByHash<T>(int[] hashes) where T : Category
        {
            if (hashes == null) return null;
            List<T> result = new List<T>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetCategoryByHash<T>(hashes[i]));
            }
            return result;
        }
        
        public virtual bool TryGetCategoryByHash(int hash, out Category category)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Hash == hash)
                {
                    category = _categories[i];
                    return true;
                }
            }
            category = null;
            return false;
        }
        
        public virtual bool TryGetCategoryByHash<T>(int hash, out T category) where T : Category
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Hash == hash)
                {
                    category = (T)_categories[i];
                    return true;
                }
            }
            category = null;
            return false;
        }
        
        public virtual bool[] TryGetCategoriesByHash(int[] hashes, out List<Category> categories)
        {
            categories = new List<Category>();
            if (hashes == null) return null;
            bool[] isFound = new bool[hashes.Length];
            for (int i = 0; i < hashes.Length; i++)
            {
                categories.Add(GetCategoryByHash(hashes[i]));
                isFound[i] = categories[i] != null;
            }
            return isFound;
        }
        
        public virtual bool[] TryGetCategoriesByHash<T>(int[] hashes, out List<T> categories) where T : Category
        {
            categories = new List<T>();
            if (hashes == null) return null;
            bool[] isFound = new bool[hashes.Length];
            for (int i = 0; i < hashes.Length; i++)
            {
                categories.Add(GetCategoryByHash<T>(hashes[i]));
                isFound[i] = categories[i] != null;
            }
            return isFound;
        }

        public virtual void AddCategory(Category value)
        {
            if (!IsContainingCategory(value.Hash))
            {
                _categories.Add(value);
                value.ParamAdded += param => ParameterAdded?.Invoke(param, value);
                value.ParamRemoved += param => ParameterRemoved?.Invoke(param, value);
                value.ParamReplaced += (replacedParam, newParam) => ParameterReplaced?.Invoke(replacedParam, newParam, value);
                CategoryAdded?.Invoke(value);
            }
        }
        
        public virtual void AddCategories(List<Category> values)
        {
            for (var i = 0; i < values.Count; i++)
            {
                AddCategory(values[i]);
            }
        }
        
        public virtual void ReplaceCategory(Category newValue)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Hash == newValue.Hash)
                {
                    Category replacedCategory = _categories[i];
                    replacedCategory.ParamAdded -= param => ParameterAdded?.Invoke(param, replacedCategory);
                    replacedCategory.ParamRemoved -= param => ParameterRemoved?.Invoke(param, replacedCategory);
                    replacedCategory.ParamReplaced -= (replacedParam, newParam) => ParameterReplaced?.Invoke(replacedParam, newParam, replacedCategory);
                    _categories[i] = newValue;
                    newValue.ParamAdded += param => ParameterAdded?.Invoke(param, newValue);
                    newValue.ParamRemoved += param => ParameterRemoved?.Invoke(param, newValue);
                    newValue.ParamReplaced += (replacedParam, newParam) => ParameterReplaced?.Invoke(replacedParam, newParam, newValue);
                    CategoryReplaced?.Invoke(replacedCategory, newValue);
                    return;
                }
            }
        }
        
        public virtual void ReplaceCategories(List<Category> newValues)
        {
            for (int i = 0; i < newValues.Count; i++)
            {
                ReplaceCategory(newValues[i]);
            }
        }
        
        public virtual void ReplaceOrAddCategory(Category newValue)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Hash == newValue.Hash)
                {
                    Category replacedCategory = _categories[i];
                    replacedCategory.ParamAdded -= param => ParameterAdded?.Invoke(param, replacedCategory);
                    replacedCategory.ParamRemoved -= param => ParameterRemoved?.Invoke(param, replacedCategory);
                    replacedCategory.ParamReplaced -= (replacedParam, newParam) => ParameterReplaced?.Invoke(replacedParam, newParam, replacedCategory);
                    _categories[i] = newValue;
                    newValue.ParamAdded += param => ParameterAdded?.Invoke(param, newValue);
                    newValue.ParamRemoved += param => ParameterRemoved?.Invoke(param, newValue);
                    newValue.ParamReplaced += (replacedParam, newParam) => ParameterReplaced?.Invoke(replacedParam, newParam, newValue);
                    CategoryReplaced?.Invoke(replacedCategory, newValue);
                    return;
                }
            }
            _categories.Add(newValue);
            CategoryAdded?.Invoke(newValue);
        }
        
        public virtual void ReplaceOrAddCategories(List<Category> newValues)
        {
            for (int i = 0; i < newValues.Count; i++)
            {
                ReplaceOrAddCategory(newValues[i]);
            }
        }
        
        public virtual bool RemoveCategory(Category value) => RemoveCategory(value.Hash);
        
        public virtual bool RemoveCategory(int hash)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (hash == _categories[i].Hash)
                {
                    Category removedCategory = _categories[i];
                    removedCategory.ParamAdded -= param => ParameterAdded?.Invoke(param, removedCategory);
                    removedCategory.ParamRemoved -= param => ParameterRemoved?.Invoke(param, removedCategory);
                    removedCategory.ParamReplaced -= (replacedParam, newParam) => ParameterReplaced?.Invoke(replacedParam, newParam, removedCategory);
                    _categories.RemoveAt(i);
                    CategoryRemoved?.Invoke(removedCategory);
                    return true;
                }
            }
            return false;
        }
        
        public virtual bool[] RemoveCategories(List<Category> values)
        {
            bool[] isRemoved = new bool[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                isRemoved[i] |= RemoveCategory(values[i].Hash);
            }
            return isRemoved;
        }
        
        public virtual bool[] RemoveCategories(int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            bool[] isRemoved = new bool[hashes.Length];
            for (var i = 0; i < hashes.Length; i++)
            {
                isRemoved[i] |= RemoveCategory(hashes[i]);
            }
            return isRemoved;
        }
        
        public virtual void RemoveCategoryAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                Category removedCategory = _categories[index];
                removedCategory.ParamAdded -= param => ParameterAdded?.Invoke(param, removedCategory);
                removedCategory.ParamRemoved -= param => ParameterRemoved?.Invoke(param, removedCategory);
                removedCategory.ParamReplaced -= (replacedParam, newParam) => ParameterReplaced?.Invoke(replacedParam, newParam, removedCategory);
                _categories.RemoveAt(index);
                CategoryRemoved?.Invoke(removedCategory);
            }
        }
        
        public virtual void RemoveAllCategories()
        {
            for (int i = _categories.Count - 1; i >= 0; i--)
            {
                RemoveCategoryAt(i);
            }
        }
        
        public virtual bool IsContainingCategory(int hash)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (hash == _categories[i].Hash) return true;
            }
            return false;
        }
        
        public virtual bool[] IsContainingCategories(int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            bool[] isContaining = new bool[hashes.Length];
            for (int i = 0; i < hashes.Length; i++)
            {
                isContaining[i] |= IsContainingCategory(hashes[i]);
            }
            return isContaining;
        }

        public virtual T GetCategoryCopy<T>(int hash) where T : Category => (T)GetCategoryCopy(hash);

        public virtual Category GetCategoryCopy(int hash)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (hash == _categories[i].Hash)
                {
                    return _categories[i].CopyCategory();
                }
            }
            return null;
        }
        
        public virtual IEnumerable<T> GetCategoriesCopy<T>(int[] hashes) where T : Category
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<T> result = new List<T>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetCategoryCopy<T>(hashes[i]));
            }
            return result;
        }

        public virtual IEnumerable<Category> GetCategoriesCopy(int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<Category> result = new List<Category>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetCategoryCopy(hashes[i]));
            }
            return result;
        }
    }
}