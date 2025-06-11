using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static LazyRedpaw.GenericParameters.Constants;
using static LazyRedpaw.StaticHashes.StaticHashesHelper;

namespace LazyRedpaw.GenericParameters
{
    [CustomPropertyDrawer(typeof(Category))]
    public class CategoryDrawer : PropertyDrawer
    {
        private static readonly StyleSheet StyleUSS;
        private static readonly VisualTreeAsset CategoryTreeAsset;
        private static readonly VisualTreeAsset ParamListItemTreeAsset;
        
        // private static readonly List<CategoryJson> CategoriesJsonList;

        static CategoryDrawer()
        {
            StyleUSS = Resources.Load<StyleSheet>("Styles");
            CategoryTreeAsset = Resources.Load<VisualTreeAsset>("CategoryUXML");
            ParamListItemTreeAsset = Resources.Load<VisualTreeAsset>("ParamListItemUXML");
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            List<string> availableHashNames = new List<string>();

            VisualElement root = new VisualElement();
            root.styleSheets.Add(StyleUSS);
            
            SerializedProperty hashProp = property.FindPropertyRelative(HashPropName);
            int categoryHash = hashProp.intValue;

            bool isCategoryExisting = CategoryIdsArray.Contains(categoryHash);
            
            if (isCategoryExisting)
            {
                string[] lines = File.ReadAllLines(GenericParametersJsonFilePath);
                int startIndex = lines[JsonRowIndex].IndexOf("\"", StringComparison.Ordinal);
                int endIndex = lines[JsonRowIndex].LastIndexOf("\"", StringComparison.Ordinal);
                string value = lines[JsonRowIndex].Substring(startIndex + 1, endIndex - startIndex - 1);
                string json = value.Replace("\\", "");
                List<CategoryJson> categoriesJsonList = JsonConvert.DeserializeObject<MainJson>(json).Categories;
                
                Dictionary<string, ParameterJson> paramJsonMap = new Dictionary<string, ParameterJson>();
                for (int i = 0; i < categoriesJsonList.Count; i++)
                {
                    CategoryJson category = categoriesJsonList[i];
                    if (category.Hash == categoryHash)
                    {
                        for (int j = 0; j < category.Parameters.Count; j++)
                        {
                            ParameterJson parameter = category.Parameters[j];
                            paramJsonMap.Add(GetHashName(parameter.Hash), parameter);
                        }
                    }
                }
                
                string name = GetCategoryName(categoryHash);
                SerializedProperty paramsProp = property.FindPropertyRelative("_parameters");
                UpdateAvailableParamNames();
                
                CategoryTreeAsset.CloneTree(root);
                Foldout parametersHeader = root.Q<Foldout>(ParametersFoldout);
                Label parametersCountLabel = root.Q<Label>(ParametersCount);
                ScrollView parametersScrollView = root.Q<ScrollView>(ParametersList);

                parametersHeader.text = name;
                parametersCountLabel.text = $"Items: {paramsProp.arraySize}";
                
                
                VisualElement popupContainer = root.Q<VisualElement>(AddParamContainer);

                PopupField<string> popupField = new PopupField<string>();
                popupField.AddToClassList(FlexGrow);
                popupContainer.Add(popupField);
                
                UpdatePopupField();
                ResetParametersList();

                List<string> GetParameterNamesList()
                {
                    UpdateAvailableParamNames();
                    List<string> list = new List<string>();
                    if(availableHashNames.Count > 0) list.Add(ChooseParameterText);
                    else list.Add(AllParamsAddedText);
                    for (int i = 0; i < availableHashNames.Count; i++)
                    {
                        list.Add(availableHashNames[i]);
                    }
                    return list;
                }
                
                Button addButton = new Button() { text = "Add" };
                addButton.clicked += OnAddButtonClicked;
                addButton.SetEnabled(false);
                popupContainer.Add(addButton);

                popupField.RegisterValueChangedCallback(OnParamNamePopupChangedCallback);
                
                void OnAddButtonClicked()
                {
                    CreateNewParam(paramJsonMap[popupField.value]);
                    UpdatePopupField();
                    ResetParametersList();
                }

                void UpdatePopupField()
                {
                    List<string> paramNamesList = GetParameterNamesList();
                    popupField.choices = paramNamesList;
                    popupField.value = paramNamesList[0];
                    popupField.SetEnabled(paramNamesList.Count > 1);
                }

                void CreateNewParam(ParameterJson parameterJson)
                {
                    string paramNameLower = GetHashName(parameterJson.Hash).ToLower();
                    Type paramType = Type.GetType(parameterJson.AssemblyQualifiedName);
                    if (paramsProp.arraySize == 0) CreateFirstParam(paramType, parameterJson.Hash);
                    else
                    {
                        bool isAdded = CreateAndInsertParam(paramType, parameterJson.Hash, paramNameLower);
                        if (!isAdded) CreateParamAndAddAsLast(paramType, parameterJson.Hash);
                    }
                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();
                }

                void CreateParamAndAddAsLast(Type paramType, int paramHash)
                {
                    paramsProp.InsertArrayElementAtIndex(paramsProp.arraySize);
                    SerializedProperty newElem = paramsProp.GetArrayElementAtIndex(paramsProp.arraySize - 1);
                    newElem.managedReferenceValue = Activator.CreateInstance(paramType, paramHash);
                    paramsProp.serializedObject.ApplyModifiedProperties();
                    paramsProp.serializedObject.Update();
                }

                bool CreateAndInsertParam(Type paramType, int paramHash, string paramNameLower)
                {
                    bool isAdded = false;
                    for (int l = 0; l < paramsProp.arraySize; l++)
                    {
                        string otherName = GetHashName(
                            paramsProp.GetArrayElementAtIndex(l).FindPropertyRelative(HashPropName).intValue).ToLower();
                        if (string.Compare(paramNameLower, otherName, StringComparison.Ordinal) < 0)
                        {
                            isAdded = true;
                            paramsProp.InsertArrayElementAtIndex(l);
                            SerializedProperty newElem = paramsProp.GetArrayElementAtIndex(l);
                            newElem.managedReferenceValue = Activator.CreateInstance(paramType, paramHash);
                            paramsProp.serializedObject.ApplyModifiedProperties();
                            paramsProp.serializedObject.Update();
                            break;
                        }
                    }
                    return isAdded;
                }

                void CreateFirstParam(Type paramType, int paramHash)
                {
                    paramsProp.InsertArrayElementAtIndex(0);
                    SerializedProperty newElem = paramsProp.GetArrayElementAtIndex(0);
                    newElem.managedReferenceValue = Activator.CreateInstance(paramType, paramHash);
                    paramsProp.serializedObject.ApplyModifiedProperties();
                    paramsProp.serializedObject.Update();
                }
                
                void ResetParametersList()
                {
                    parametersScrollView.contentContainer.Clear();
                    if(paramsProp.arraySize == 0) parametersScrollView.Add(EmptyListLabel);
                    for (int i = 0; i < paramsProp.arraySize; i++)
                    {
                        SerializedProperty paramProp = paramsProp.GetArrayElementAtIndex(i);
                        ParamListItemTreeAsset.CloneTree(parametersScrollView.contentContainer);
                        VisualElement paramRoot = root.Q<VisualElement>(ParamListItemRoot);
                        int paramHash = paramProp.FindPropertyRelative(HashPropName).intValue;
                        paramRoot.name = GetHashName(paramHash);
                        PropertyField paramField = paramRoot.Q<PropertyField>(ParamField);
                        paramField.BindProperty(paramProp);
                        Button removeButton = paramRoot.Q<Button>(RemoveButton);
                        removeButton.clicked += () => DeleteParameter(paramHash);
                    }
                }

                void DeleteParameter(int paramHash)
                {
                    for (int i = 0; i < paramsProp.arraySize; i++)
                    {
                        SerializedProperty paramProp = paramsProp.GetArrayElementAtIndex(i);
                        int otherHash = paramProp.FindPropertyRelative(HashPropName).intValue;
                        if (paramHash == otherHash)
                        {
                            paramsProp.DeleteArrayElementAtIndex(i);
                            paramsProp.serializedObject.ApplyModifiedProperties();
                            paramsProp.serializedObject.Update();
                            UpdatePopupField();
                            ResetParametersList();
                            break;
                        }
                    }
                }
                
                void OnParamNamePopupChangedCallback(ChangeEvent<string> evt)
                {
                    if (GetHashValue(evt.newValue) != 0) addButton.SetEnabled(true);
                    else addButton.SetEnabled(false);
                }

                void UpdateAvailableParamNames()
                {
                    paramsProp.serializedObject.Update();
                    List<ParameterJson> parameterJsons = new List<ParameterJson>(paramJsonMap.Values);
                    for (int i = 0; i < paramsProp.arraySize; i++)
                    {
                        SerializedProperty element = paramsProp.GetArrayElementAtIndex(i);
                        for (int j = parameterJsons.Count - 1; j >= 0; j--)
                        {
                            if (parameterJsons[j].Hash == element.FindPropertyRelative(HashPropName).intValue)
                            {
                                parameterJsons.RemoveAt(j);
                            } 
                        }
                    }
                    availableHashNames.Clear();
                    for (int i = 0; i < parameterJsons.Count; i++)
                    {
                        availableHashNames.Add(GetHashName(parameterJsons[i].Hash));
                    }
                }
            }
            else
            {
                Label label = new Label(CategoryNotFoundText);
                label.AddToClassList(LabelUSS);
                root.Add(label);
            }
            return root;
        }
    }
}