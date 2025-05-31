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
    [CustomPropertyDrawer(typeof(Parameter), true)]
    public class ParameterDrawer : PropertyDrawer
    {
        protected static readonly StyleSheet StyleUSS;

        static ParameterDrawer()
        {
            StyleUSS = Resources.Load<StyleSheet>("Styles");
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            root.styleSheets.Add(StyleUSS);

            Type type = property.GetPropertyFieldType();

            if (property.managedReferenceValue == null)
            {
                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
            }

            int propertiesCount = property.GetChildrenPropertiesCount();
            SerializedProperty hashProp = property.FindPropertyRelative("_hash");
            string name = StaticHashesHelper.GetHashName(hashProp.intValue) ?? property.displayName;
            Label label = new Label(name);
            label.AddToClassList(LabelUSS);
            if (propertiesCount == 1)
            {
                label.AddToClassList(ParameterHeader);
                root.Add(label);
            }

            bool isValuePropInline = property.managedReferenceValue.GetType().ContainsTypeAsAncestor(typeof(Parameter<>));
            if (isValuePropInline)
            {
                if (propertiesCount == 2)
                {
                    SerializedProperty valueProp = property.FindPropertyRelative("_value");
                    PropertyField valueField = new PropertyField(valueProp) { label = name };
                    root.Add(valueField);
                }
                else if (propertiesCount > 2)
                {
                    VisualElement header = CreateFoldoutHeader(root, out Button expandButton);

                    SerializedProperty valueProp = property.FindPropertyRelative("_value");
                    PropertyField valueField = new PropertyField(valueProp) { label = name };
                    valueField.AddToClassList(FlexGrow);
                    header.Add(valueField);

                    CreateContent(property, root, expandButton);
                }
            }
            else if(propertiesCount > 1)
            {
                VisualElement header = CreateFoldoutHeader(root, out Button expandButton);

                SerializedProperty valueProp = property.FindPropertyRelative("_value");
                header.Add(label);

                CreateContentWithValue(property, root, expandButton);
            }
            return root;
        }

        private void CreateContent(SerializedProperty property, VisualElement root, Button expandButton)
        {
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
                    if(currentProperty.name == "_hash" || currentProperty.depth > initDepth + 1 ||
                       currentProperty.name == "_value") continue;
                    PropertyField propField = new PropertyField(currentProperty) { label = currentProperty.displayName };
                    content.Add(propField);
                } while (currentProperty.NextVisible(false) && currentProperty.depth > initDepth);
            }
        }
        
        private void CreateContentWithValue(SerializedProperty property, VisualElement root, Button expandButton)
        {
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
                    if(currentProperty.name == "_hash" || currentProperty.depth > initDepth + 1) continue;
                    PropertyField propField = new PropertyField(currentProperty) { label = currentProperty.displayName };
                    content.Add(propField);
                } while (currentProperty.NextVisible(false) && currentProperty.depth > initDepth);
            }
        }

        private VisualElement CreateFoldoutHeader(VisualElement root, out Button expandButton)
        {
            VisualElement header = new VisualElement();
            header.AddToClassList(ParameterHeader);
            header.AddToClassList(AlignHorizontal);
            root.Add(header);
                            
            expandButton = new Button();
            expandButton.AddToClassList(FoldoutArrow);
            header.Add(expandButton);
            return header;
        }
    }
}