using System;
using System.Collections.Generic;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public class ModableInt : ModableParameter<int>
    {
        public ModableInt() { }
        public ModableInt(int hash) : base(hash) { }
        public ModableInt(int baseValue, int hash) : base(baseValue, hash) { }
        public ModableInt(int baseValue, int hash, IEnumerable<Mod> mods) : base(baseValue, hash, mods) { }
        public ModableInt(object baseValue, int hash, IEnumerable<Mod> mods) : base(baseValue, hash, mods) { }

        public override void SetBaseValue(object value)
        {
            try
            {
                BaseValue = Convert.ToInt32(value);
            }
            catch
            {
                BaseValue = 0;
            }
#if UNITY_EDITOR
            if(!Application.isPlaying) CalculateFinalValue();
#endif
        }

        public override void AddToBaseValue(int value) => BaseValue += value;

        public override ModableParameter CopyParameter()
        {
            List<Mod> mods = new List<Mod>(_mods.Count);
            for (int i = 0; i < _mods.Count; i++)
            {
                mods.Add(_mods[i].Copy());
            }
            return new ModableInt(BaseValue, _hash, mods);
        }
        
        protected override void CalculateFinalValue()
        {
            float finalValue = BaseValue;
            if (_mods != null)
            {
                int sumRatioAdd = 0;
                for (int i = 0; i < _mods.Count; i++)
                {
                    if(_mods[i] == null) continue;
                    int modValue = _mods[i].GetValue<int>();
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
            _value = (int)Math.Round(finalValue, 4);
        }
    }
}