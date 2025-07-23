using System;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    [MemoryPackable]
    public partial class ModableFloat : ModableParameter<float>
    {
        public ModableFloat() { }
        public ModableFloat(int hash) : base(hash) { }
        public ModableFloat(float baseValue, int hash) : base(baseValue, hash) { }
        [MemoryPackConstructor]
        public ModableFloat(float baseValue, int hash, IEnumerable<Mod> mods) : base(baseValue, hash, mods) { }
        public ModableFloat(object baseValue, int hash, IEnumerable<Mod> mods) : base(baseValue, hash, mods) { }

        public override void SetBaseValue(object value)
        {
            try
            {
                BaseValue = Convert.ToSingle(value);
            }
            catch
            {
                BaseValue = 0f;
            }
#if UNITY_EDITOR
            if(!Application.isPlaying) CalculateFinalValue();
#endif
        }

        public override void AddToBaseValue(float value) => BaseValue += value;

        public override ModableParameter CopyParameter()
        {
            List<Mod> mods = new List<Mod>(_mods.Count);
            for (int i = 0; i < _mods.Count; i++)
            {
                mods.Add(_mods[i].Copy());
            }
            return new ModableFloat(BaseValue, _hash, mods);
        }
        
        protected override void CalculateFinalValue()
        {
            float finalValue = BaseValue;
            if (_mods != null)
            {
                float sumRatioAdd = 0;
                for (int i = 0; i < _mods.Count; i++)
                {
                    if(_mods[i] == null) continue;
                    float modValue = _mods[i].GetValue<float>();
                    switch (_mods[i].Action)
                    {
                        case ModAction.NumberAdd:
                            finalValue += modValue;
                            break;
                        case ModAction.OnePlusRatioAdd:
                            sumRatioAdd += modValue;
                            if (i + 1 >= _mods.Count || _mods[i + 1].Action != ModAction.OnePlusRatioAdd)
                            {
                                finalValue *= 1f + sumRatioAdd;
                                sumRatioAdd = 0;
                            }
                            break;
                        case ModAction.RatioAdd:
                            sumRatioAdd += modValue;
                            if (i + 1 >= _mods.Count || _mods[i + 1].Action != ModAction.RatioAdd)
                            {
                                finalValue *= sumRatioAdd;
                                sumRatioAdd = 0;
                            }
                            break;
                        case ModAction.OnePlusRatioMult:
                            finalValue *= 1 + modValue;
                            break;
                        case ModAction.RatioMult:
                            finalValue *= modValue;
                            break;
                    }
                }
            }
            _value = (float)Math.Round(finalValue, 4);
        }
    }
}