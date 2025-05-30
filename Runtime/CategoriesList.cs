using System;
using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class CategoriesList
    {
        [SerializeReference] protected List<Category> _categories;
        
        public Category this[int i] => _categories[i];
        public int Count => _categories.Count;

        public event Action<Category> OnCategoryAdded;
        public event Action<Category> OnCategoryRemoved;
        public event Action<Category, Category> OnCategoryReplaced;
        
        public CategoriesList()
        {
            _categories = new List<Category>();
        }

        public Category Get(int index) => this[index];
        
        public T Get<T>(int index) where T : Category => (T)this[index];
        
        public Category GetByHash(int hash)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Hash == hash) return _categories[i];
            }
            return null;
        }

        public T GetByHash<T>(int hash) where T : Category => (T)GetByHash(hash);
        
        public bool TryGetByHash(int hash, out Category category)
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
        
        public bool TryGetByHash<T>(int hash, out T category) where T : Category
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

        public void Add(Category value)
        {
            if (!IsContaining(value.Hash))
            {
                _categories.Add(value);
                OnCategoryAdded?.Invoke(value);
            }
        }
        
        public void Replace(Category newValue)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Hash == newValue.Hash)
                {
                    _categories[i] = newValue;
                    return;
                }
            }
        }
        
        public void ReplaceOrAdd(Category newValue)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Hash == newValue.Hash)
                {
                    Category replacedCategory = _categories[i];
                    _categories[i] = newValue;
                    OnCategoryReplaced?.Invoke(replacedCategory, newValue);
                    return;
                }
            }
            _categories.Add(newValue);
            OnCategoryAdded?.Invoke(newValue);
        }
        
        public bool Remove(Category value) => Remove(value.Hash);
        
        public bool Remove(int hash)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (hash == _categories[i].Hash)
                {
                    Category removedCategory = _categories[i];
                    _categories.RemoveAt(i);
                    OnCategoryRemoved?.Invoke(removedCategory);
                    return true;
                }
            }
            return false;
        }
        
        public void RemoveAt(int index)
        {
            if (index > 0 && index < Count)
            {
                Category removedCategory = _categories[index];
                _categories.RemoveAt(index);
                OnCategoryRemoved?.Invoke(removedCategory);
            }
        }
        
        public void RemoveAll()
        {
            for (int i = _categories.Count - 1; i >= 0; i--)
            {
                Category removedCategory = _categories[i];
                _categories.RemoveAt(i);
                OnCategoryRemoved?.Invoke(removedCategory);
            }
        }
        
        public bool IsContaining(int hash)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (hash == _categories[i].Hash) return true;
            }
            return false;
        }

        public T GetParamCopy<T>(int hash) where T : Category => (T)GetParamCopy(hash);

        public Category GetParamCopy(int hash)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (hash == _categories[i].Hash)
                {
                    return _categories[i].Copy();
                }
            }
            return null;
        }
    }
}