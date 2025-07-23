using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LazyRedpaw.GuidSerialization;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ModableParameter : Parameter
    {
        [SerializeReference] protected List<Mod> _mods;

        protected readonly ReadOnlyCollection<Mod> _readOnlyMods;
        protected bool _isChanged = true;

        public virtual ReadOnlyCollection<Mod> Mods => _readOnlyMods;

        protected virtual bool IsChanged
        {
            get => _isChanged;
            set
            {
                _isChanged = value;
                if (_isChanged) InvokeChanged();
            }
        }

        public ModableParameter()
        {
            _mods = new List<Mod>();
            _readOnlyMods = _mods.AsReadOnly();
        }

        public ModableParameter(int hash) : base(hash)
        {
            _mods = new List<Mod>();
            _readOnlyMods = _mods.AsReadOnly();
        }
        
        public ModableParameter(int hash, IEnumerable<Mod> mods) : this()
        {
            _hash = hash;
            foreach (Mod mod in mods)
            {
                _mods.Add(mod);
            }
        }

        public virtual List<Mod> CopyMods() => new(_mods);
        
        public virtual void AddMod(Mod mod)
        {
            if (ContainsMod(mod.Guid)) return;
            _mods.Add(mod);
            _mods.Sort(CompareModOrder);
            IsChanged = true;
        }

        public virtual bool RemoveMod(Mod mod) => RemoveMod(mod.Guid);
        public virtual void RemoveMod(int index)
        {
            if (index >= 0 && index < _mods.Count)
            {
                _mods.RemoveAt(index);
                IsChanged = true;
            }
        }

        public virtual bool RemoveMod(SerializableGuid modGuid)
        {
            for (int i = 0; i < _mods.Count; i++)
            {
                if (_mods[i].Guid == modGuid)
                {
                    _mods.RemoveAt(i);
                    IsChanged = true;
                    return true;
                }
            }
            return false;
        }

        public virtual bool RemoveAllModsFromSource(SerializableGuid sourceGuid)
        {
            bool didRemove = false;
            for (int i = _mods.Count - 1; i >= 0; i--)
            {
                if (_mods[i].SourceGuid == sourceGuid)
                {
                    _mods.RemoveAt(i);
                    didRemove = true;
                    IsChanged = true;
                }
            }
            return didRemove;
        }
        
        public virtual bool RemoveAllMods()
        {
            bool didRemove = false;
            for (int i = _mods.Count - 1; i >= 0; i--)
            {
                didRemove = true;
                _mods.RemoveAt(i);
            }
            if(didRemove) IsChanged = true;
            return didRemove;
        }

        public virtual bool ContainsMod(SerializableGuid modGuid)
        {
            if(modGuid == SerializableGuid.Empty) return false;
            for (int i = 0; i < _mods.Count; i++)
            {
                if (_mods[i].Guid == modGuid) return true;
            }
            return false;
        }

        protected virtual int CompareModOrder(Mod a, Mod b)
        {
            return a.Order < b.Order ? -1 : a.Order > b.Order ? 1 : 0;
        }

        public virtual void SetBaseValue(object value) { }

        public virtual void GetBaseValue(out object value)
        {
            value = null;
        }

        public virtual void GetValue(out object value)
        {
            value = null;
        }

        public virtual void Override<T>(T otherStat) where T : ModableParameter
        {
            _mods = new List<Mod>(otherStat._mods);
        }
        public virtual ModableParameter CopyParameter() => new ModableParameter(_hash, new List<Mod>(_mods));
        public override IParameter Copy() => CopyParameter();
    }
    
    [Serializable]
    public abstract class ModableParameter<TValue> : ModableParameter
    {
        [SerializeField] protected TValue _baseValue;
        [SerializeField] protected TValue _value;

        public virtual TValue BaseValue
        {
            get => _baseValue;
            protected set
            {
                _baseValue = value;
                IsChanged = true;
            }
        }
        
        public virtual TValue Value
        {
            get
            {
                if (IsChanged)
                {
                    CalculateFinalValue();
                    IsChanged = false;
                }
                return _value;
            }
        }
        
        protected ModableParameter() => CalculateFinalValue();
        protected ModableParameter(int hash) : base(hash) => CalculateFinalValue();
        
        
        protected ModableParameter(TValue baseValue, int hash) : this(hash)
        {
            BaseValue = baseValue;
        }
        protected ModableParameter(TValue baseValue, int hash, IEnumerable<Mod> mods) : base(hash, mods)
        {
            BaseValue = baseValue;
            if(!Application.isPlaying) CalculateFinalValue();
        }
        protected ModableParameter(object baseValue, int hash, IEnumerable<Mod> mods) : base(hash, mods)
        {
            SetBaseValue(baseValue);
        }

        protected abstract void CalculateFinalValue();

        public override void GetBaseValue(out object value) => value = BaseValue;
        public override void GetValue(out object value) => value = Value;

        public override void Override<T>(T otherStat)
        {
            if (otherStat is ModableParameter<TValue> stat)
            {
                SetBaseValue(stat.BaseValue);
                RemoveAllMods();
                for (int j = 0; j < stat.Mods.Count; j++)
                {
                    AddMod(stat.Mods[j]);
                }
            }
        }

        public abstract void AddToBaseValue(TValue value);
        
        public virtual TValue GetBaseValue() => _baseValue;

        public virtual void SetBaseValue(TValue value)
        {
            BaseValue = value;
#if UNITY_EDITOR
            if(!Application.isPlaying) CalculateFinalValue();
#endif
        }
    }
}