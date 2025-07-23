using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LazyRedpaw.StaticHashes;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class Category
    {
        [SerializeField, StaticHash] protected int _hash;
        [SerializeReference] protected List<IParameter> _parameters;

        public virtual int Hash => _hash;
        public virtual IParameter this[int i] => _parameters[i];
        public virtual int Count => _parameters.Count;

        public virtual event Action<IParameter> ParamAdded;
        public virtual event Action<IParameter> ParamRemoved;
        public virtual event Action<IParameter, IParameter> ParamReplaced;
        
        public Category()
        {
            _parameters = new List<IParameter>();
        }

        public Category(int hash) : this()
        {
            _hash = hash;
        }
        
        public Category(int hash, List<IParameter> parameters)
        {
            _hash = hash;
            _parameters = new List<IParameter>(parameters);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual List<IParameter> GetAllParams() => new List<IParameter>(_parameters);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual IParameter GetParam(int index) => this[index];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T GetParam<T>(int index) where T : IParameter => (T)this[index];
        
        public virtual IParameter GetParamByHash(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == hash) return _parameters[i];
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T GetParamByHash<T>(int hash) where T : IParameter => (T)GetParamByHash(hash);

        public virtual List<IParameter> GetParamsByHash(int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<IParameter> result = new List<IParameter>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetParamByHash(hashes[i]));
            }
            return result;
        }
        
        public virtual List<T> GetParamsByHash<T>(int[] hashes) where T : IParameter
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<T> result = new List<T>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetParamByHash<T>(hashes[i]));
            }
            return result;
        }
        
        public virtual List<T> GetParamsWithType<T>() where T : IParameter
        {
            Type reqType = typeof(T);
            List<T> result = new List<T>();
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].GetType().ContainsTypeAsAncestor(reqType))
                {
                    result.Add((T)_parameters[i]);
                }
            }
            return result;
        }
        
        public virtual bool TryGetParamByHash(int hash, out IParameter parameter)
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
        
        public virtual bool TryGetParamByHash<T>(int hash, out T parameter) where T : IParameter
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == hash)
                {
                    parameter = (T)_parameters[i];
                    return true;
                }
            }
            parameter = default;
            return false;
        }
        
        public virtual bool[] TryGetParamsByHash(int[] hashes, out List<IParameter> parameters)
        {
            parameters = new List<IParameter>();
            if (hashes == null) return null;
            bool[] isParamFound = new bool[hashes.Length];
            for (int i = 0; i < hashes.Length; i++)
            {
                parameters.Add(GetParamByHash(hashes[i]));
                isParamFound[i] = parameters[i] != null;
            }
            return isParamFound;
        }
        
        public virtual bool[] TryGetParamsByHash<T>(int[] hashes, out List<T> parameters) where T : IParameter
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
        
        public virtual bool TryGetParamsWithType<T>(out List<T> parameters) where T : IParameter
        {
            parameters = new List<T>();
            Type reqType = typeof(T);
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].GetType().ContainsTypeAsAncestor(reqType))
                {
                    parameters.Add((T)_parameters[i]);
                }
            }
            return parameters.Count > 0;
        }

        public virtual void AddParam(IParameter value)
        {
            if (!IsContainingParam(value.Hash))
            {
                _parameters.Add(value);
                ParamAdded?.Invoke(value);
            }
        }
        
        public virtual void AddParams(List<IParameter> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                AddParam(values[i]);
            }
        }
        
        public virtual void ReplaceParam(IParameter newValue)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == newValue.Hash)
                {
                    IParameter replacedParam = _parameters[i];
                    _parameters[i] = newValue;
                    ParamReplaced?.Invoke(replacedParam, newValue);
                    return;
                }
            }
        }
        
        public virtual void ReplaceParams(List<IParameter> newValues)
        {
            for (int i = 0; i < newValues.Count; i++)
            {
                ReplaceParam(newValues[i]);
            }
        }
        
        public virtual void ReplaceOrAddParam(IParameter newValue)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Hash == newValue.Hash)
                {
                    IParameter replacedParam = _parameters[i];
                    _parameters[i] = newValue;
                    ParamReplaced?.Invoke(replacedParam, newValue);
                    return;
                }
            }
            _parameters.Add(newValue);
            ParamAdded?.Invoke(newValue);
        }
        
        public virtual void ReplaceOrAddParams(List<IParameter> newValues)
        {
            for (int i = 0; i < newValues.Count; i++)
            {
                ReplaceOrAddParam(newValues[i]);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool RemoveParam(IParameter value) => RemoveParam(value.Hash);
        
        public virtual bool RemoveParam(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (hash == _parameters[i].Hash)
                {
                    IParameter removedParam = _parameters[i];
                    _parameters.RemoveAt(i);
                    ParamRemoved?.Invoke(removedParam);
                    return true;
                }
            }
            return false;
        }

        public virtual bool[] RemoveParams(List<IParameter> values)
        {
            bool[] isRemoved = new bool[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                isRemoved[i] |= RemoveParam(values[i].Hash);
            }
            return isRemoved;
        }
        
        public virtual bool[] RemoveParams(int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            bool[] isRemoved = new bool[hashes.Length];
            for (var i = 0; i < hashes.Length; i++)
            {
                isRemoved[i] |= RemoveParam(hashes[i]);
            }
            return isRemoved;
        }
        
        public virtual void RemoveParamAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                IParameter removedParam = _parameters[index];
                _parameters.RemoveAt(index);
                ParamRemoved?.Invoke(removedParam);
            }
        }
        
        public virtual void RemoveAllParams()
        {
            for (int i = _parameters.Count - 1; i >= 0; i--)
            {
                RemoveParamAt(i);
            }
        }
        
        public virtual bool IsContainingParam(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (hash == _parameters[i].Hash) return true;
            }
            return false;
        }
        
        public virtual bool[] IsContainingParams(int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            bool[] isContaining = new bool[hashes.Length];
            for (int i = 0; i < hashes.Length; i++)
            {
                isContaining[i] |= IsContainingParam(hashes[i]);
            }
            return isContaining;
        }

        public virtual T GetParamCopy<T>(int hash) where T : IParameter => (T)GetParamCopy(hash);

        public virtual IParameter GetParamCopy(int hash)
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

        public virtual IEnumerable<T> GetParamsCopy<T>(int[] hashes) where T : IParameter
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<T> result = new List<T>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetParamCopy<T>(hashes[i]));
            }
            return result;
        }

        public virtual IEnumerable<IParameter> GetParamsCopy (int[] hashes)
        {
            if (hashes == null || hashes.Length == 0) return null;
            List<IParameter> result = new List<IParameter>();
            for (int i = 0; i < hashes.Length; i++)
            {
                result.Add(GetParamCopy(hashes[i]));
            }
            return result;
        }

        public virtual Category CopyCategory()
        {
            Category category = new Category(_hash);
            for (int i = 0; i < _parameters.Count; i++)
            {
                category.AddParam(_parameters[i].Copy());
            }
            return category;
        }
        
        public virtual T CopyCategory<T>() where T : Category
        {
            T category = Activator.CreateInstance(typeof(T), _hash) as T;
            for (int i = 0; i < _parameters.Count; i++)
            {
                category.AddParam(_parameters[i].Copy());
            }
            return category;
        }
    }
}