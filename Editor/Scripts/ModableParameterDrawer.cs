using System;
using System.Reflection;
using LazyRedpaw.StaticHashes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static LazyRedpaw.GenericParameters.Constants;

namespace LazyRedpaw.GenericParameters
{
    [CustomPropertyDrawer(typeof(ModableParameter), true)]
    public class ModableParameterDrawer : ParameterDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Type type = property.GetPropertyFieldType();

            if (property.managedReferenceValue == null)
            {
                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            }

            bool isParentGeneric = property.managedReferenceValue.GetType().GetParentGenericType() == typeof(ModableParameter<>);
            if (!isParentGeneric) return base.CreatePropertyGUI(property);

            VisualElement root = new VisualElement();
            root.styleSheets.Add(_styleUSS);

            VisualElement header = new VisualElement();
            header.AddToClassList(ParameterHeader);
            header.AddToClassList(AlignHorizontal);
            root.Add(header);

            Button expandButton = new Button();
            expandButton.AddToClassList(FoldoutArrow);
            header.Add(expandButton);

            SerializedProperty hashProp = property.FindPropertyRelative("_hash");
            string name = StaticHashesHelper.GetHashName(hashProp.intValue) ?? property.displayName;
            Label label = new Label(name);
            label.AddToClassList(FlexGrow);
            label.AddToClassList(LabelUSS);
            header.Add(label);

            SerializedProperty baseValueProp = property.FindPropertyRelative("_baseValue");
            Label baseValueLabel = new Label(baseValueProp.displayName);
            baseValueLabel.AddToClassList(LabelUSS);
            baseValueLabel.AddToClassList(ModableParamLabel);
            header.Add(baseValueLabel);
            PropertyField baseValueField = new PropertyField(baseValueProp) { label = " " };
            baseValueField.AddToClassList(ModableParam);
            header.Add(baseValueField);

            SerializedProperty valueProp = property.FindPropertyRelative("_value");
            Label valueLabel = new Label(valueProp.displayName);
            valueLabel.AddToClassList(LabelUSS);
            valueLabel.AddToClassList(MarginLeft10);
            valueLabel.AddToClassList(ModableParamLabel);
            header.Add(valueLabel);
            PropertyField valueField = new PropertyField(valueProp) { label = string.Empty };
            valueField.AddToClassList(ModableParam);
            valueField.SetEnabled(false);
            header.Add(valueField);

            baseValueField.RegisterValueChangeCallback(prop => CalcFinalStatValue(property));

            VisualElement content = new VisualElement();
            content.AddToClassList(MarginLeft15);
            content.style.display = DisplayStyle.None;
            root.Add(content);

            expandButton.clicked += () =>
            {
                if (expandButton.ClassListContains(ExpandButtonExpanded))
                {
                    expandButton.RemoveFromClassList(ExpandButtonExpanded);
                    content.style.display = DisplayStyle.None;
                }
                else
                {
                    expandButton.AddToClassList(ExpandButtonExpanded);
                    content.style.display = DisplayStyle.Flex;
                }
            };
            SerializedProperty currentProperty = property.Copy();
            int initDepth = currentProperty.depth;
            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (currentProperty.name == "_hash" || currentProperty.depth > initDepth + 1 ||
                        currentProperty.name == "_value" || currentProperty.name == "_baseValue") continue;

                    PropertyField propField = new PropertyField(currentProperty)
                        { label = currentProperty.displayName };
                    if (currentProperty.name == "_mods")
                    {
                        propField.RegisterCallback<SerializedPropertyChangeEvent>(_ =>
                            CalcFinalStatValue(property));
                    }

                    content.Add(propField);
                } while (currentProperty.NextVisible(false) && currentProperty.depth > initDepth);
            }
            return root;
        }

        private void CalcFinalStatValue(SerializedProperty property)
        {
            object stat = property.GetTargetObjectOfProperty();
            MethodInfo method = stat.GetType().GetMethod("CalculateFinalValue", BindingFlags.NonPublic | BindingFlags.Instance);
            if(method == null) Debug.LogWarning("CalculateFinalValue method could not be found");
            else
            {
                method.Invoke(stat, null);
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            }
        }
    }
}