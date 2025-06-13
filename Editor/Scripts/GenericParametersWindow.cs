using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static LazyRedpaw.GenericParameters.Constants;
using static LazyRedpaw.StaticHashes.StaticHashesHelper;
using Object = UnityEngine.Object;

namespace LazyRedpaw.GenericParameters
{
    public class GenericParametersWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _windowUXML;
        [SerializeField] private VisualTreeAsset _categoryUXML;
        [SerializeField] private VisualTreeAsset _parameterUXML;

        private DropdownField _newCategoryDropdown;
        private Button _createCategoryButton;
        private ScrollView _categoriesScrollView;
        private Button _expandButton;
        private Button _saveButton;
        private Label _categoriesCountLabel;
        private List<CategoryElement> _categories;
        private List<CategoryElement> _deletedCategories;
        private List<string> _availableCategoryNames;
        private List<int> _availableCategoryIds;
        private bool _isExpanded;
        private List<Type> _parameterTypes;
        private List<string> _parameterTypeNames;
        private List<CategoryJson> _categoryJsons;

        [MenuItem("Window/Generic Parameters")]
        private static void OpenWindow()
        {
            GetWindow<GenericParametersWindow>(WindowTitle);
        }

        private void CreateGUI()
        {
            typeof(Parameter).GetNonAbstractChildrenAndSelfTypesAndTheirNames(out Type[] types, out string[] names);
            _parameterTypes = types.ToList();
            _parameterTypeNames = names.ToList();
            _categories = new List<CategoryElement>();
            _deletedCategories = new List<CategoryElement>();
            _categoryJsons = new List<CategoryJson>();
            InitUiElements();
        }

        private void InitUiElements()
        {
            _windowUXML.CloneTree(rootVisualElement);
            _newCategoryDropdown = rootVisualElement.Q<DropdownField>(NewCategoryNameDropdown);
            _createCategoryButton = rootVisualElement.Q<Button>(CreateCategoryButton);
            _categoriesScrollView = rootVisualElement.Q<ScrollView>(CategoriesScrollView);
            _categoriesCountLabel = rootVisualElement.Q<Label>(CategoriesCount);
            _expandButton = rootVisualElement.Q<Button>(ExpandButton);
            _saveButton = rootVisualElement.Q<Button>(SaveButtonTmp);

            LoadJson();
            ProcessCategoriesCountChange();
            
            _createCategoryButton.clicked += OnCreateCategoryButtonClicked;
            _expandButton.clicked += OnExpandButtonClicked;
            _saveButton.clicked += OnSaveButtonClicked;
        }

        private void LoadJson()
        {
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
            _categoryJsons = JsonConvert.DeserializeObject<MainJson>(json).Categories;
            for (int i = 0; i < _categoryJsons.Count; i++)
            {
                LoadCategory(_categoryJsons[i]);
            }
        }

        private void OnSaveButtonClicked()
        {
            for (int i = 0; i < _categoryJsons.Count; i++)
            {
                CategoryJson categoryJson = _categoryJsons[i];
                CategoryElement category = null;
                for (int j = 0; j < _categories.Count; j++)
                {
                    if (categoryJson.Hash == _categories[j].Hash)
                    {
                        category = _categories[j];
                        break;
                    }
                }

                if (category == null)
                {
                    UpdateParameterFields(categoryJson.Hash, null);
                }
                else
                {
                    for (int j = 0; j < categoryJson.Parameters.Count; j++)
                    {
                        ParameterJson parameterJson = categoryJson.Parameters[j];
                        ParameterElement parameter = null;
                        for (int k = 0; k < category.Parameters.Count; k++)
                        {
                            if (parameterJson.Hash == category.Parameters[k].Hash)
                            {
                                parameter = category.Parameters[k];
                                if (parameter.IsTypeChanged)
                                {
                                    UpdateParameterFields(parameter.Hash, Type.GetType(parameter.AssemblyQualifiedName));
                                }
                                break;
                            }
                        }

                        if (parameter == null)
                        {
                            UpdateParameterFields(parameterJson.Hash, null);
                        }
                    }
                }
            }
            
            MainJson mainJson = new MainJson() { Categories = new List<CategoryJson>() };
            for (int i = 0; i < _categories.Count; i++)
            {
                mainJson.Categories.Add(_categories[i].GetCategoryJson());
            }
            string json = JsonConvert.SerializeObject(mainJson);
            for (int i = 0; i < json.Length; i++)
            {
                if (json[i] == '"')
                {
                    json = json.Insert(i, "\\");
                    i++;
                }
            }
            List<string> lines = new List<string>(StorageTemplate);
            lines.Insert(JsonRowIndex, $"\"{json}\";");
            File.WriteAllLines(GenericParametersJsonFilePath, lines);
            AssetDatabase.Refresh();
        }

        private void OnExpandButtonClicked()
        {
            if (_isExpanded)
            {
                _isExpanded = false;
                _categoriesScrollView.style.display = DisplayStyle.None;
                _expandButton.RemoveFromClassList(ExpandButtonExpanded);
            }
            else
            {
                _isExpanded = true;
                _categoriesScrollView.style.display = DisplayStyle.Flex;
                _expandButton.AddToClassList(ExpandButtonExpanded);
            }
        }

        private void OnCreateCategoryButtonClicked()
        {
            int categoryHash = GetCategoryId(_newCategoryDropdown.value);
            var categoryElement = _deletedCategories.FirstOrDefault(h => h.Hash == categoryHash);
            if (categoryElement != null) AddExistingCategory(categoryElement);
            else AddNewCategory(_newCategoryDropdown.value, categoryHash);
            ProcessCategoriesCountChange();
            SortCategories();
        }

        private void ProcessCategoriesCountChange()
        {
            UpdateAvailableCategories();
            UpdateNewCategoryDropdown();
            UpdateCreateCategoryState();
            _categoriesCountLabel.text = "Items " + _categories.Count;
        }

        private void AddNewCategory(string categoryName, int categoryHash)
        {
            _categoryUXML.CloneTree(_categoriesScrollView.contentContainer);
            VisualElement categoryRoot = _categoriesScrollView.contentContainer.Q<VisualElement>(CategoryRoot);
            categoryRoot.name = categoryName;
            CategoryElement newCategory = new CategoryElement(categoryName, categoryHash, categoryRoot,
                _parameterTypes, _parameterTypeNames, _parameterUXML);
            newCategory.DeletionRequested += OnCategoryItemDeletionRequested;
            _categories.Add(newCategory);
        }
        
        private void LoadCategory(CategoryJson category)
        {
            _categoryUXML.CloneTree(_categoriesScrollView.contentContainer);
            VisualElement categoryRoot = _categoriesScrollView.contentContainer.Q<VisualElement>(CategoryRoot);
            string categoryName = GetCategoryName(category.Hash);
            categoryRoot.name = categoryName;
            CategoryElement newCategory = new CategoryElement(categoryName, category.Hash, categoryRoot,
                _parameterTypes, _parameterTypeNames, _parameterUXML, category.Parameters);
            newCategory.DeletionRequested += OnCategoryItemDeletionRequested;
            _categories.Add(newCategory);
        }
        
        public void AddExistingCategory(CategoryElement element)
        {
            _deletedCategories.Remove(element);
            _categoriesScrollView.contentContainer.Add(element.Root);
            element.DeletionRequested += OnCategoryItemDeletionRequested;
            _categories.Add(element);
        }
        
        private void OnCategoryItemDeletionRequested(CategoryElement element)
        {
            element.Root.RemoveFromHierarchy();
            element.DeletionRequested -= OnCategoryItemDeletionRequested;
            _categories.Remove(element);
            _deletedCategories.Add(element);
            ProcessCategoriesCountChange();
        }

        private void UpdateCreateCategoryState() => _createCategoryButton.SetEnabled(IsAnyAvailableCategory());
        private bool IsAnyAvailableCategory() => _availableCategoryIds != null && _availableCategoryIds.Count > 0;

        private void UpdateAvailableCategories()
        {
            if(_availableCategoryNames == null) _availableCategoryNames = new List<string>();
            else _availableCategoryNames.Clear();
            if(_availableCategoryIds == null) _availableCategoryIds = new List<int>();
            else _availableCategoryIds.Clear();
            for (int i = 0; i < CategoryIdsArray.Length; i++)
            {
                if (!IsCategoryExisting(CategoryIdsArray[i]))
                {
                    _availableCategoryIds.Add(CategoryIdsArray[i]);
                    _availableCategoryNames.Add(CategoriesNamesArray[i]);
                }
            }
        }

        private void UpdateNewCategoryDropdown()
        {
            if (_availableCategoryIds == null || _availableCategoryIds.Count == 0)
            {
                _newCategoryDropdown.choices = NoAvailableChoices;
                _newCategoryDropdown.value = NoAvailableChoices[0];
                _newCategoryDropdown.SetEnabled(false);
            }
            else
            {
                _newCategoryDropdown.choices = _availableCategoryNames;
                _newCategoryDropdown.value = _availableCategoryNames[0];
                _newCategoryDropdown.SetEnabled(true);
            }
        }

        private bool IsCategoryExisting(int hash)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if(_categories[i].Hash == hash) return true;
            }
            return false;
        }
        
        private void SortCategories()
        {
            _categories.Sort((a, b) => String.Compare(a.Name.ToLower(),
                b.Name.ToLower(), StringComparison.Ordinal));
            for (int i = 0; i < _categories.Count; i++)
            {
                _categoriesScrollView.Remove(_categories[i].Root);
                _categoriesScrollView.Insert(i, _categories[i].Root);
            }
        }
        
        private void UpdateParameterFields(int hash, Type newType)
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < assetPaths.Length; i++)
            {
                if (assetPaths[i].StartsWith("Assets") &&
                    (assetPaths[i].EndsWith(".prefab") || assetPaths[i].EndsWith(".asset") || assetPaths[i].EndsWith(".unity")))
                {
                    UpdateField(assetPaths[i], hash, newType);
                }
            }
            Debug.unityLogger.logEnabled = false;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.unityLogger.logEnabled = true;
            EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
        }

        private void UpdateField(string path, int hash, Type newType)
        {
            Object asset = AssetDatabase.LoadMainAssetAtPath(path);
            if(asset == null) return;
            bool assetChanged = false;

            if (asset is GameObject)
            {
                GameObject gameObject = (GameObject)asset;
                assetChanged = UpdateFieldInGameObject(gameObject, hash, newType);
            }
            else if (path.EndsWith(".unity"))
            {
                Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                List<GameObject> sceneObjects = GetAllSceneObjects(scene);
                for (int i = 0; i < sceneObjects.Count; i++)
                {
                    assetChanged |= UpdateFieldInGameObject(sceneObjects[i], hash, newType);
                }
                if (assetChanged)
                {
                    Debug.unityLogger.logEnabled = false;
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                    Debug.unityLogger.logEnabled = true;
                }
            }
            else
            {
                SerializedObject serializedObject = new SerializedObject(asset);
                assetChanged = UpdateFieldInSerializedObject(serializedObject, hash, newType);
            }

            if (assetChanged) EditorUtility.SetDirty(asset);
        }

        private bool UpdateFieldInGameObject(GameObject gameObject, int hash, Type newType)
        {
            bool objectChanged = false;
            Component[] components = gameObject.GetComponentsInChildren<Component>(true);

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null) continue; // In case of missing scripts
                SerializedObject serializedObject = new SerializedObject(components[i]);
                objectChanged |= UpdateFieldInSerializedObject(serializedObject, hash, newType);
            }
            return objectChanged;
        }

        private bool UpdateFieldInSerializedObject(SerializedObject serializedObject, int hash, Type newType)
        {
            bool objectChanged = false;
            SerializedProperty property = serializedObject.GetIterator();
            if (newType == null)
            {
                while (property.NextVisible(true))
                {
                    if (property.type == nameof(Constants.CategoriesList))
                    {
                        SerializedProperty categoriesProp = property.FindPropertyRelative(CategoriesPropName);
                        for (int i = 0; i < categoriesProp.arraySize; i++)
                        {
                            SerializedProperty categoryProp = categoriesProp.GetArrayElementAtIndex(i);
                            SerializedProperty hashProp = categoryProp.FindPropertyRelative(HashPropName);
                            if (hashProp.intValue == hash)
                            {
                                categoriesProp.DeleteArrayElementAtIndex(i);
                                objectChanged = true;
                                break;
                            }
                        }

                        if (!objectChanged)
                        {
                            for (int i = 0; i < categoriesProp.arraySize; i++)
                            {
                                SerializedProperty categoryProp = categoriesProp.GetArrayElementAtIndex(i);
                                SerializedProperty parametersProp = categoryProp.FindPropertyRelative(ParametersPropName);
                                for (int j = 0; j < parametersProp.arraySize; j++)
                                {
                                    SerializedProperty parameterProp = parametersProp.GetArrayElementAtIndex(j);
                                    SerializedProperty hashProp = parameterProp.FindPropertyRelative(HashPropName);
                                    if (hashProp.intValue == hash)
                                    {
                                        parametersProp.DeleteArrayElementAtIndex(j);
                                        objectChanged = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                while (property.NextVisible(true))
                {
                    SerializedProperty hashProp = property.FindPropertyRelative(HashPropName);
                    if (hashProp != null && hashProp.intValue == hash)
                    {
                        property.managedReferenceValue = Activator.CreateInstance(newType, hash);
                        objectChanged = true;
                    }
                }
            }

            if (objectChanged) serializedObject.ApplyModifiedProperties();
            return objectChanged;
        }

        private List<GameObject> GetAllSceneObjects(Scene scene)
        {
            List<GameObject> sceneObjects = new List<GameObject>();
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                sceneObjects.AddRange(obj.GetComponentsInChildren<Transform>(true)
                    .Select(t => t.gameObject));
            }
            return sceneObjects;
        }
    }
}