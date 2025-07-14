using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        private static readonly VisualTreeAsset CategoriesListTreeAsset;
        private static readonly VisualTreeAsset ListItemTreeAsset;

        static CategoriesListDrawer()
        {
            CategoriesListTreeAsset = Resources.Load<VisualTreeAsset>("CategoriesUXML");
            ListItemTreeAsset = Resources.Load<VisualTreeAsset>("CategoryListItemUXML");
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            List<string> availableNames = new List<string>();

            VisualElement root = new VisualElement();
            string json = string.Empty;
            if (!File.Exists(GenericParametersJsonFilePath))
            {
                string directory = Path.GetDirectoryName(GenericParametersJsonFilePath);
                if(!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                File.Create(GenericParametersJsonFilePath).Close();
                MainJson mainJson = new MainJson() { Categories = new List<CategoryJson>() };
                json = JsonConvert.SerializeObject(mainJson);
                string jsonForFile = json;
                for (int i = 0; i < jsonForFile.Length; i++)
                {
                    if (jsonForFile[i] == '"')
                    {
                        jsonForFile = jsonForFile.Insert(i, "\\");
                        i++;
                    }
                }
                List<string> lines = new List<string>(StorageTemplate);
                lines.Insert(JsonRowIndex, $"\"{jsonForFile}\";");
                File.WriteAllLines(GenericParametersJsonFilePath, lines);
                AssetDatabase.Refresh();
            }
            else
            {
                string[] lines = File.ReadAllLines(GenericParametersJsonFilePath);
                int startIndex = lines[JsonRowIndex].IndexOf("\"", StringComparison.Ordinal);
                int endIndex = lines[JsonRowIndex].LastIndexOf("\"", StringComparison.Ordinal);
                string value = lines[JsonRowIndex].Substring(startIndex + 1, endIndex - startIndex - 1);
                json = value.Replace("\\", "");
            }
            List<CategoryJson> categoriesJsonList = JsonConvert.DeserializeObject<MainJson>(json).Categories;
            AssetDatabase.Refresh();
            
            Dictionary<string, CategoryJson> categoriesJsonMap = new Dictionary<string, CategoryJson>();
            for (int i = 0; i < categoriesJsonList.Count; i++)
            {
                CategoryJson category = categoriesJsonList[i];
                categoriesJsonMap.Add(GetCategoryName(category.Hash), category);
            }
            
            SerializedProperty categoriesProp = property.FindPropertyRelative("_categories");
            UpdateAvailableNames();

            CategoriesListTreeAsset.CloneTree(root);
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
                CreateNewElement(categoriesJsonMap[popupField.value]);
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
                Type categoryType = Type.GetType(categoryJson.AssemblyQualifiedName);
                if (categoriesProp.arraySize == 0) CreateFirstElement(categoryJson.Hash, categoryType);
                else
                {
                    bool isAdded = CreateAndInsertElement(categoryJson.Hash, categoryType, nameLowerCase);
                    if (!isAdded) CreateElementAndAddAsLast(categoryJson.Hash, categoryType);
                }
                categoriesProp.serializedObject.ApplyModifiedProperties();
                categoriesProp.serializedObject.Update();
            }

            void CreateElementAndAddAsLast(int hash, Type categoryType)
            {
                categoriesProp.InsertArrayElementAtIndex(categoriesProp.arraySize);
                SerializedProperty newElem = categoriesProp.GetArrayElementAtIndex(categoriesProp.arraySize - 1);
                newElem.managedReferenceValue = Activator.CreateInstance(categoryType, hash);
                categoriesProp.serializedObject.ApplyModifiedProperties();
                categoriesProp.serializedObject.Update();
            }

            bool CreateAndInsertElement(int hash, Type categoryType, string nameLowerCase)
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
                        newElem.managedReferenceValue = Activator.CreateInstance(categoryType, hash);
                        categoriesProp.serializedObject.ApplyModifiedProperties();
                        categoriesProp.serializedObject.Update();
                        break;
                    }
                }

                return isAdded;
            }

            void CreateFirstElement(int hash, Type categoryType)
            {
                categoriesProp.InsertArrayElementAtIndex(0);
                SerializedProperty newElem = categoriesProp.GetArrayElementAtIndex(0);
                newElem.managedReferenceValue = Activator.CreateInstance(categoryType, hash);
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
                    ListItemTreeAsset.CloneTree(scrollView.contentContainer);
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
                List<CategoryJson> categoryJsons = new List<CategoryJson>(categoriesJsonMap.Values);
                for (int i = 0; i < categoriesProp.arraySize; i++)
                {
                    SerializedProperty element = categoriesProp.GetArrayElementAtIndex(i);
                    for (int j = categoryJsons.Count - 1; j >= 0; j--)
                    {
                        if (categoryJsons[j].Hash == element.FindPropertyRelative(HashPropName).intValue)
                        {
                            categoryJsons.RemoveAt(j);
                        }
                    }
                }

                availableNames.Clear();
                for (int i = 0; i < categoryJsons.Count; i++)
                {
                    availableNames.Add(GetCategoryName(categoryJsons[i].Hash));
                }
            }
            return root;
        }
    }
}