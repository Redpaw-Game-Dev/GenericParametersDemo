using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static LazyRedpaw.GenericParameters.Constants;

namespace LazyRedpaw.GenericParameters
{
    [CustomPropertyDrawer(typeof(Mod), true)]
    public class ModDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, Type> _typeMap;
        private static readonly string[] _typeNames;
        
        [SerializeField] private VisualTreeAsset _modUXML;

        static ModDrawer()
        {
            _typeMap = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && typeof(Mod).IsAssignableFrom(t))
                .ToDictionary(t => t.Name, t => t);

            _typeNames = _typeMap.Keys.ToArray();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            _modUXML?.CloneTree(root);
            
            root.Q<Label>(ModLabel).text = property.displayName;

            List<string> typeNamesList = _typeNames.ToList();
            if (property.managedReferenceValue == null)
            {
                property.managedReferenceValue = Activator.CreateInstance(_typeMap[typeNamesList[2]]);
                
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            }
            
            object instance = property.managedReferenceValue;

            string currentTypeName = instance?.GetType().Name;
            if (string.IsNullOrEmpty(currentTypeName) || !typeNamesList.Contains(currentTypeName))
            {
                currentTypeName = typeNamesList[0];
            }

            VisualElement popupContainer = root.Q<VisualElement>(TypePopupContainer);
            PopupField<string> popup = new PopupField<string>(typeNamesList, currentTypeName);
            popup.AddToClassList(FlexGrow);
            popupContainer.Add(popup);

            VisualElement valueFieldContainer = root.Q<VisualElement>(ValueContainer);

            void RebuildValueField()
            {
                valueFieldContainer.Clear();

                instance = property.managedReferenceValue;
                if (instance == null) return;

                property.serializedObject.Update();

                SerializedProperty valueProp = property.FindPropertyRelative("_value");
                if (valueProp != null)
                {
                    var valueField = new PropertyField(valueProp);
                    valueField.AddToClassList(FlexGrow);
                    valueField.Bind(property.serializedObject);
                    valueFieldContainer.Add(valueField);
                }
            }

            RebuildValueField();

            EnumField actionField = root.Q<EnumField>(ModActionField);
            actionField.BindProperty(property.FindPropertyRelative("_action"));

            popup.RegisterValueChangedCallback(evt =>
            {
                Type newType = _typeMap[evt.newValue];
                object oldInstance = property.managedReferenceValue;

                object newInstance = Activator.CreateInstance(newType);
                
                int oldActionValue = property.FindPropertyRelative("_action").enumValueFlag;

                if (oldInstance != null)
                {
                    FieldInfo oldField = FindValueField(oldInstance.GetType());
                    FieldInfo newField = FindValueField(newType);

                    if (oldField != null && newField != null)
                    {
                        try
                        {
                            object oldValue = oldField.GetValue(oldInstance);
                            object convertedValue = Convert.ChangeType(oldValue, newField.FieldType);
                            newField.SetValue(newInstance, convertedValue);
                        }
                        catch(Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }
                }

                property.managedReferenceValue = newInstance;
                
                property.FindPropertyRelative("_action").enumValueFlag = oldActionValue;
                
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();

                RebuildValueField();
            });
            
            SerializedProperty guidProp = property.FindPropertyRelative("_guid");
            SerializedProperty sourceGuidProp = property.FindPropertyRelative("_sourceGuid");
            PropertyField guidField = new PropertyField(guidProp, guidProp.displayName);
            PropertyField sourceGuidField = new PropertyField(sourceGuidProp, sourceGuidProp.displayName);
            
            VisualElement debugContainer = root.Q<VisualElement>(DebugContainer);
            debugContainer.style.display = DisplayStyle.None;
            debugContainer.Add(guidField);
            debugContainer.Add(sourceGuidField);
            
            Button expandButton = root.Q<Button>(ExpandButton);
            expandButton.clicked += () =>
            {
                if (expandButton.ClassListContains(ExpandButtonExpanded))
                {
                    expandButton.RemoveFromClassList(ExpandButtonExpanded);
                    debugContainer.style.display = DisplayStyle.None;
                }
                else
                {
                    expandButton.AddToClassList(ExpandButtonExpanded);
                    debugContainer.style.display = DisplayStyle.Flex;
                }
            };

            return root;
        }
        
        private FieldInfo FindValueField(Type type)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }

    }
}