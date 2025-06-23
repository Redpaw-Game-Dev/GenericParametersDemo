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

        public List<Parameter> GetAllParams() => new List<Parameter>(_parameters);
        
        public Parameter GetParam(int index) => this[index];
        
        public T GetParam<T>(int index) where T : Parameter => (T)this[index];
        
        public Parameter GetParamByHash(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == hash) return _parameters[i];
            }
            return null;
        }

        public T GetParamByHash<T>(int hash) where T : Parameter => (T)GetParamByHash(hash);

        public List<Parameter> GetParamsByHash(int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<Parameter> result = new List<Parameter>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetParamByHash(hashes[i]));
            }
            return result;
        }
        
        public List<T> GetParamsByHash<T>(int[] hashes) where T : Parameter
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<T> result = new List<T>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetParamByHash<T>(hashes[i]));
            }
            return result;
        }
        
        public bool TryGetParamByHash(int hash, out Parameter parameter)
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
        
        public bool TryGetParamByHash<T>(int hash, out T parameter) where T : Parameter
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
        
        public bool[] TryGetParamsByHash(int[] hashes, out List<Parameter> parameters)
        {
            parameters = new List<Parameter>();
            if (hashes == null) return null;
            bool[] isParamFound = new bool[hashes.Length];
            for (int i = 0; i < hashes.Length; i++)
            {
                parameters.Add(GetParamByHash(hashes[i]));
                isParamFound[i] = parameters[i] != null;
            }
            return isParamFound;
        }
        
        public bool[] TryGetParamsByHash<T>(int[] hashes, out List<T> parameters) where T : Parameter
        {
            parameters = new List<T>();
            if (hashes == null) return null;
            bool[] isParamFound = new bool[hashes.Length];
            for (int i = 0; i < hashes.Length; i++)
            {
                parameters.Add(GetParamByHash<T>(hashes[i]));
                isParamFound[i] = parameters[i] != null;
            }
            return isParamFound;
        }

        public void AddParam(Parameter value)
        {
            if (!IsContainingParam(value.Hash))
            {
                _parameters.Add(value);
                OnParamAdded?.Invoke(value);
            }
        }
        
        public void AddParams(List<Parameter> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                AddParam(values[i]);
            }
        }
        
        public void ReplaceParam(Parameter newValue)
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
        
        public void ReplaceParams(List<Parameter> newValues)
        {
            for (int i = 0; i < newValues.Count; i++)
            {
                ReplaceParam(newValues[i]);
            }
        }
        
        public void ReplaceOrAddParam(Parameter newValue)
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
        
        public void ReplaceOrAddParams(List<Parameter> newValues)
        {
            for (int i = 0; i < newValues.Count; i++)
            {
                ReplaceOrAddParam(newValues[i]);
            }
        }
        
        public bool RemoveParam(Parameter value) => RemoveParam(value.Hash);
        
        public bool RemoveParam(int hash)
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

        public bool[] RemoveParams(List<Parameter> values)
        {
            bool[] isRemoved = new bool[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                isRemoved[i] |= RemoveParam(values[i].Hash);
            }
            return isRemoved;
        }
        
        public bool[] RemoveParams(int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            bool[] isRemoved = new bool[hashes.Length];
            for (var i = 0; i < hashes.Length; i++)
            {
                isRemoved[i] |= RemoveParam(hashes[i]);
            }
            return isRemoved;
        }
        
        public void RemoveParamAt(int index)
        {
            if (index > 0 && index < Count)
            {
                Parameter removedParam = _parameters[index];
                _parameters.RemoveAt(index);
                OnParamRemoved?.Invoke(removedParam);
            }
        }
        
        public void RemoveAllParams()
        {
            for (int i = _parameters.Count - 1; i >= 0; i--)
            {
                Parameter removedParam = _parameters[i];
                _parameters.RemoveAt(i);
                OnParamRemoved?.Invoke(removedParam);
            }
        }
        
        public bool IsContainingParam(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (hash == _parameters[i].Hash) return true;
            }
            return false;
        }
        
        public bool[] IsContainingParams(int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            bool[] isContaining = new bool[hashes.Length];
            for (int i = 0; i < hashes.Length; i++)
            {
                isContaining[i] |= IsContainingParam(hashes[i]);
            }
            return isContaining;
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

        public IEnumerable<T> GetParamsCopy<T>(int[] hashes) where T : Parameter
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<T> result = new List<T>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetParamCopy<T>(hashes[i]));
            }
            return result;
        }

        public IEnumerable<Parameter> GetParamsCopy (int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<Parameter> result = new List<Parameter>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetParamCopy(hashes[i]));
            }
            return result;
        }

        public Category CopyCategory()
        {
            Category category = new Category(_hash);
            for (int i = 0; i < _parameters.Count; i++)
            {
                category.AddParam(_parameters[i].Copy());
            }
            return category;
        }
    }
}