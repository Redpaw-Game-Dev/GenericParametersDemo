using System;
using System.Collections.Generic;
using LazyRedpaw.StaticHashes;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class Category
    {
        [SerializeField, StaticHash] private int _hash;
        [SerializeReference] private List<Parameter> _parameters;

        public int Hash => _hash;
        public Parameter this[int i] => _parameters[i];
        public int Count => _parameters.Count;

        public event Action<Parameter> OnParamAdded;
        public event Action<Parameter> OnParamRemoved;
        public event Action<Parameter, Parameter> OnParamReplaced;
        
        public Category()
        {
            _parameters = new List<Parameter>();
        }

        public Category(int hash) : this()
        {
            _hash = hash;
        }

        public Parameter Get(int index) => this[index];
        
        public T Get<T>(int index) where T : Parameter => (T)this[index];
        
        public Parameter GetByHash(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == hash) return _parameters[i];
            }
            return null;
        }

        public T GetByHash<T>(int hash) where T : Parameter => (T)GetByHash(hash);
        
        public bool TryGetByHash(int hash, out Parameter parameter)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == hash)
                {
                    parameter = _parameters[i];
                    return true;
                }
            }
            parameter = null;
            return false;
        }
        
        public bool TryGetByHash<T>(int hash, out T parameter) where T : Parameter
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == hash)
                {
                    parameter = (T)_parameters[i];
                    return true;
                }
            }
            parameter = null;
            return false;
        }

        public void Add(Parameter value)
        {
            if (!IsContaining(value.Hash))
            {
                _parameters.Add(value);
                OnParamAdded?.Invoke(value);
            }
        }
        
        public void Replace(Parameter newValue)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == newValue.Hash)
                {
                    _parameters[i] = newValue;
                    return;
                }
            }
        }
        
        public void ReplaceOrAdd(Parameter newValue)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == newValue.Hash)
                {
                    Parameter replacedParam = _parameters[i];
                    _parameters[i] = newValue;
                    OnParamReplaced?.Invoke(replacedParam, newValue);
                    return;
                }
            }
            _parameters.Add(newValue);
            OnParamAdded?.Invoke(newValue);
        }
        
        public bool Remove(Parameter value) => Remove(value.Hash);
        
        public bool Remove(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (hash == _parameters[i].Hash)
                {
                    Parameter removedParam = _parameters[i];
                    _parameters.RemoveAt(i);
                    OnParamRemoved?.Invoke(removedParam);
                    return true;
                }
            }
            return false;
        }
        
        public void RemoveAt(int index)
        {
            if (index > 0 && index < Count)
            {
                Parameter removedParam = _parameters[index];
                _parameters.RemoveAt(index);
                OnParamRemoved?.Invoke(removedParam);
            }
        }
        
        public void RemoveAll()
        {
            for (int i = _parameters.Count - 1; i >= 0; i--)
            {
                Parameter removedParam = _parameters[i];
                _parameters.RemoveAt(i);
                OnParamRemoved?.Invoke(removedParam);
            }
        }
        
        public bool IsContaining(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (hash == _parameters[i].Hash) return true;
            }
            return false;
        }

        public T GetParamCopy<T>(int hash) where T : Parameter => (T)GetParamCopy(hash);

        public Parameter GetParamCopy(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (hash == _parameters[i].Hash)
                {
                    return _parameters[i].Copy();
                }
            }
            return null;
        }

        public Category Copy()
        {
            Category category = new Category(_hash);
            for (int i = 0; i < _parameters.Count; i++)
            {
                category.Add(_parameters[i].Copy());
            }
            return category;
        }
    }
}