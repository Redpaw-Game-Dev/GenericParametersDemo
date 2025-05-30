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
    [CustomPropertyDrawer(typeof(CategoriesList))]
    public class CategoriesListDrawer : PropertyDrawer
    {
        // [SerializeField] protected StyleSheet _styleUSS;
        [SerializeField] private VisualTreeAsset _categoriesListTreeAsset;
        [SerializeField] private VisualTreeAsset _listItemTreeAsset;

        private static readonly Dictionary<string, CategoryJson> CategoriesJsonMap;

        static CategoriesListDrawer()
        {
            string json = File.ReadAllText(GenericParametersJsonFilePath);
            List<CategoryJson> categoriesJson = JsonConvert.DeserializeObject<MainJson>(json).Categories;
            CategoriesJsonMap = new Dictionary<string, CategoryJson>();
            for (int i = 0; i < categoriesJson.Count; i++)
            {
                CategoryJson category = categoriesJson[i];
                CategoriesJsonMap.Add(GetCategoryName(category.Hash), category);
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            List<string> availableNames = new List<string>();

            VisualElement root = new VisualElement();
            // root.styleSheets.Add(_styleUSS);
            
            SerializedProperty categoriesProp = property.FindPropertyRelative("_categories");
            UpdateAvailableNames();

            _categoriesListTreeAsset.CloneTree(root);
            Foldout header = root.Q<Foldout>(CategoriesFoldout);
            Label countLabel = root.Q<Label>(CategoriesCount);
            ScrollView scrollView = root.Q<ScrollView>(Constants.CategoriesList);

            header.text = property.displayName;
            countLabel.text = $"Items: {categoriesProp.arraySize}";


            VisualElement popupContainer = root.Q<VisualElement>(AddCategoryContainer);

            PopupField<string> popupField = new PopupField<string>();
            popupField.AddToClassList(FlexGrow);
            popupContainer.Add(popupField);

            UpdatePopupField();
            ResetList();

            List<string> GetNamesList()
            {
                UpdateAvailableNames();
                List<string> list = new List<string>();
                if (availableNames.Count > 0) list.Add(ChooseCategoryText);
                else list.Add(AllCategoriesAddedText);
                for (int i = 0; i < availableNames.Count; i++)
                {
                    list.Add(availableNames[i]);
                }

                return list;
            }

            Button addButton = new Button() { text = "Add" };
            addButton.clicked += OnAddButtonClicked;
            addButton.SetEnabled(false);
            popupContainer.Add(addButton);

            popupField.RegisterValueChangedCallback(OnNamePopupChangedCallback);

            void OnAddButtonClicked()
            {
                CreateNewElement(CategoriesJsonMap[popupField.value]);
                UpdatePopupField();
                ResetList();
            }

            void UpdatePopupField()
            {
                List<string> namesList = GetNamesList();
                popupField.choices = namesList;
                popupField.value = namesList[0];
                popupField.SetEnabled(namesList.Count > 1);
            }

            void CreateNewElement(CategoryJson categoryJson)
            {
                string nameLowerCase = GetCategoryName(categoryJson.Hash).ToLower();
                if (categoriesProp.arraySize == 0) CreateFirstElement(categoryJson.Hash);
                else
                {
                    bool isAdded = CreateAndInsertElement(categoryJson.Hash, nameLowerCase);
                    if (!isAdded) CreateElementAndAddAsLast(categoryJson.Hash);
                }
                categoriesProp.serializedObject.ApplyModifiedProperties();
                categoriesProp.serializedObject.Update();
            }

            void CreateElementAndAddAsLast(int hash)
            {
                categoriesProp.InsertArrayElementAtIndex(categoriesProp.arraySize);
                SerializedProperty newElem = categoriesProp.GetArrayElementAtIndex(categoriesProp.arraySize - 1);
                newElem.managedReferenceValue = Activator.CreateInstance(typeof(Category), hash);
                categoriesProp.serializedObject.ApplyModifiedProperties();
                categoriesProp.serializedObject.Update();
            }

            bool CreateAndInsertElement(int hash, string nameLowerCase)
            {
                bool isAdded = false;
                for (int l = 0; l < categoriesProp.arraySize; l++)
                {
                    string otherName = GetCategoryName(
                        categoriesProp.GetArrayElementAtIndex(l).FindPropertyRelative(HashPropName).intValue).ToLower();
                    if (string.Compare(nameLowerCase, otherName, StringComparison.Ordinal) < 0)
                    {
                        isAdded = true;
                        categoriesProp.InsertArrayElementAtIndex(l);
                        SerializedProperty newElem = categoriesProp.GetArrayElementAtIndex(l);
                        newElem.managedReferenceValue = Activator.CreateInstance(typeof(Category), hash);
                        categoriesProp.serializedObject.ApplyModifiedProperties();
                        categoriesProp.serializedObject.Update();
                        break;
                    }
                }

                return isAdded;
            }

            void CreateFirstElement(int hash)
            {
                categoriesProp.InsertArrayElementAtIndex(0);
                SerializedProperty newElem = categoriesProp.GetArrayElementAtIndex(0);
                newElem.managedReferenceValue = Activator.CreateInstance(typeof(Category), hash);
                categoriesProp.serializedObject.ApplyModifiedProperties();
                categoriesProp.serializedObject.Update();
            }

            void ResetList()
            {
                scrollView.contentContainer.Clear();
                if (categoriesProp.arraySize == 0) scrollView.Add(EmptyListLabel);
                for (int i = 0; i < categoriesProp.arraySize; i++)
                {
                    SerializedProperty elemProp = categoriesProp.GetArrayElementAtIndex(i);
                    _listItemTreeAsset.CloneTree(scrollView.contentContainer);
                    VisualElement elemRoot = root.Q<VisualElement>(CategoryListItemRoot);
                    int hash = elemProp.FindPropertyRelative(HashPropName).intValue;
                    elemRoot.name = GetCategoryName(hash);
                    Button removeButton = elemRoot.Q<Button>(RemoveButton);
                    removeButton.clicked += () => DeleteCategory(hash);
                    PropertyField propField = elemRoot.Q<PropertyField>(CategoryField);
                    propField.BindProperty(elemProp);
                }
            }

            void DeleteCategory(int hash)
            {
                for (int i = 0; i < categoriesProp.arraySize; i++)
                {
                    SerializedProperty paramProp = categoriesProp.GetArrayElementAtIndex(i);
                    int otherHash = paramProp.FindPropertyRelative(HashPropName).intValue;
                    if (hash == otherHash)
                    {
                        categoriesProp.DeleteArrayElementAtIndex(i);
                        categoriesProp.serializedObject.ApplyModifiedProperties();
                        categoriesProp.serializedObject.Update();
                        UpdatePopupField();
                        ResetList();
                        break;
                    }
                }
            }

            void OnNamePopupChangedCallback(ChangeEvent<string> evt)
            {
                if (GetCategoryId(evt.newValue) != 0) addButton.SetEnabled(true);
                else addButton.SetEnabled(false);
            }

            void UpdateAvailableNames()
            {
                categoriesProp.serializedObject.Update();
                List<int> hashes = new List<int>(CategoryIdsArray);
                for (int i = 0; i < categoriesProp.arraySize; i++)
                {
                    SerializedProperty element = categoriesProp.GetArrayElementAtIndex(i);
                    hashes.Remove(element.FindPropertyRelative(HashPropName).intValue);
                }

                availableNames.Clear();
                for (int i = 0; i < hashes.Count; i++)
                {
                    availableNames.Add(GetCategoryName(hashes[i]));
                }
            }
            return root;
        }
    }
}