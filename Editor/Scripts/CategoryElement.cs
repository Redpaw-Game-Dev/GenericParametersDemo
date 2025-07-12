using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using static LazyRedpaw.GenericParameters.Constants;
using static LazyRedpaw.StaticHashes.StaticHashesHelper;

namespace LazyRedpaw.GenericParameters
{
    public class CategoryElement
    {
        private readonly VisualElement _root;
        private readonly VisualTreeAsset _parameterUXML;
        private readonly DropdownField _newParameterNameDropdown;
        private readonly DropdownField _newParameterTypeDropdown;
        private readonly DropdownField _categoryTypeDropdown;
        private readonly Button _deleteButton;
        // private readonly Button _revertButton;
        private readonly Button _createParameterButton;
        private readonly ScrollView _parametersScrollView;
        private readonly Button _expandButton;
        private readonly Label _parametersCountLabel;
        private readonly string _name;
        private readonly int _hash;
        private readonly List<Type> _parameterTypes;
        private readonly List<string> _parameterTypeNames;
        private readonly List<Type> _categoryTypes;
        private readonly List<string> _categoryTypeNames;
        private readonly List<string> _availableParameterNames;
        private readonly List<int> _availableParameterHashes;
        
        private List<ParameterElement> _parameters;
        // private List<ParameterElement> _deletedParameters;
        private bool _isExpanded;
        private string _assemblyQualifiedName;
        private string _savedTypeName;

        public VisualElement Root => _root;
        public List<ParameterElement> Parameters => _parameters;
        public string Name => _name;
        public int Hash => _hash;
        public bool IsTypeChanged => _categoryTypeDropdown.value != _savedTypeName;
        public string AssemblyQualifiedName => _assemblyQualifiedName;
        
        public event Action<CategoryElement> DeletionRequested; 
        
        public CategoryElement(string name, int hash, VisualElement root, List<Type> parameterTypes,
            List<string> parameterTypeNames, VisualTreeAsset parameterUXML, List<Type> categoryTypes,
            List<string> categoryTypeNames, string typeName, string assemblyQualifiedName, List<ParameterJson> parameters = null)
        {
            _availableParameterNames = new List<string>();
            _availableParameterHashes = new List<int>();
            _parameters = new List<ParameterElement>();
            // _deletedParameters = new List<ParameterElement>();
            _name = name;
            _hash = hash;
            _root = root;
            _parameterTypes = parameterTypes;
            _parameterTypeNames =  parameterTypeNames;
            _parameterUXML = parameterUXML;
            _categoryTypes = categoryTypes;
            _categoryTypeNames = categoryTypeNames;
            _savedTypeName = typeName;
            _assemblyQualifiedName = assemblyQualifiedName;
            
            _root.Q<Label>(CategoryNameLabel).text = _name;
            _newParameterNameDropdown = _root.Q<DropdownField>(NewParameterNameDropdown);
            _newParameterTypeDropdown = _root.Q<DropdownField>(NewParameterTypeDropdown);
            _categoryTypeDropdown = _root.Q<DropdownField>(CategoryTypeDropdown);
            _deleteButton = _root.Q<Button>(DeleteButton);
            // _revertButton = _root.Q<Button>(RevertButton);
            _createParameterButton = _root.Q<Button>(CreateParameterButton);
            _parametersScrollView = _root.Q<ScrollView>(ParametersList);
            _expandButton = _root.Q<Button>(ExpandButton);
            _parametersCountLabel = _root.Q<Label>(ParametersCount);

            LoadParameters(parameters);

            ProcessParametersCountChange();
            
            _categoryTypeDropdown.choices = _categoryTypeNames;
            _categoryTypeDropdown.SetValueWithoutNotify(typeName);
            _categoryTypeDropdown.RegisterValueChangedCallback(OnCategoryTypeFieldChanged);
            _newParameterTypeDropdown.choices = _parameterTypeNames;
            _newParameterTypeDropdown.value = _parameterTypeNames[0];
            _deleteButton.clicked += OnDeleteButtonClicked;
            // _revertButton.clicked += OnRevertButtonClicked;
            _createParameterButton.clicked += OnCreateParameterButtonClicked;
            _expandButton.clicked += OnExpandButtonClicked;
        }

        private void LoadParameters(List<ParameterJson> parameters)
        {
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    string parameterName = GetHashName(parameters[i].Hash);
                    Type parameterType = Type.GetType(parameters[i].AssemblyQualifiedName);
                    AddNewParameter(parameterName, parameterType, parameters[i].Hash);
                }
            }
        }

        public CategoryJson GetCategoryJson()
        {
            CategoryJson categoryJson = new CategoryJson()
            {
                Hash = _hash,
                AssemblyQualifiedName = _assemblyQualifiedName,
                Parameters = new List<ParameterJson>()
            };
            for (int i = 0; i < _parameters.Count; i++)
            {
                categoryJson.Parameters.Add(new ParameterJson()
                {
                    Hash = _parameters[i].Hash,
                    AssemblyQualifiedName = _parameters[i].AssemblyQualifiedName
                });
            }
            return categoryJson;
        }
        
        // private void OnRevertButtonClicked()
        // {
        //     for (int i = 0; i < _parameters.Count; i++)
        //     {
        //         _parameters[i].Revert();
        //     }
        // }

        private void OnExpandButtonClicked()
        {
            if (_isExpanded)
            {
                _isExpanded = false;
                _parametersScrollView.style.display = DisplayStyle.None;
                _expandButton.RemoveFromClassList(ExpandButtonExpanded);
            }
            else
            {
                _isExpanded = true;
                _parametersScrollView.style.display = DisplayStyle.Flex;
                _expandButton.AddToClassList(ExpandButtonExpanded);
            }
        }

        private void OnCreateParameterButtonClicked()
        {
            int paramHash = GetHashValue(_newParameterNameDropdown.value);
            Type paramType = GetTypeByName(_newParameterTypeDropdown.value);
            AddNewParameter(_newParameterNameDropdown.value, paramType, paramHash);
            ProcessParametersCountChange();
            SortParameters();
        }

        private void ProcessParametersCountChange()
        {
            UpdateAvailableParamNames();
            UpdateNewParamNameDropdown();
            UpdateCreateParamButtonState();
            _parametersCountLabel.text = "Items " + _parameters.Count;
        }
        
        private void SortParameters()
        {
            _parameters.Sort((a, b) => String.Compare(a.Name.ToLower(),
                b.Name.ToLower(), StringComparison.Ordinal));
            for (int i = 0; i < _parameters.Count; i++)
            {
                _parametersScrollView.Remove(_parameters[i].Root);
                _parametersScrollView.Insert(i, _parameters[i].Root);
            }
        }

        private void AddNewParameter(string paramName, Type paramType, int paramHash)
        {
            _parameterUXML.CloneTree(_parametersScrollView.contentContainer);
            VisualElement paramRoot = _parametersScrollView.contentContainer.Q<VisualElement>(ParameterRoot);
            paramRoot.name = paramName;
            // Type paramType = GetTypeByName(typeName);
            List<string> nameChoiceList = new List<string>(_availableParameterNames);
            for (int i = 0; i < _parameters.Count; i++)
            {
                _parameters[i].RemoveNameFromChoiceList(paramName);
            }
            ParameterElement newParam = new ParameterElement(paramRoot, paramHash, paramName, paramType.Name, 
                paramType.AssemblyQualifiedName, _parameterTypes, _parameterTypeNames, nameChoiceList);
            newParam.DeletionRequested += OnParameterItemDeletionRequested;
            newParam.NameChanged += OnParamNameChanged;
            _parameters.Add(newParam);
        }

        private void OnParamNameChanged(ChangeEvent<string> evt, int savedHash)
        {
            int oldHash = GetHashValue(evt.previousValue);
            int newHash = GetHashValue(evt.newValue);
            
            _availableParameterNames.Remove(evt.newValue);
            _availableParameterHashes.Remove(newHash);
            _availableParameterNames.Add(evt.previousValue);
            _availableParameterNames.Sort();
            for (int i = 0; i < _availableParameterNames.Count; i++)
            {
                if (string.Equals(_availableParameterNames[i], evt.previousValue))
                {
                    _availableParameterHashes.Insert(i, oldHash);
                    break;
                }
            }
            UpdateNewParamNameDropdown();
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].SavedHash != savedHash)
                {
                    _parameters[i].RemoveNameFromChoiceList(evt.newValue);
                    _parameters[i].AddNameToChoiceList(evt.previousValue);
                }
            }
        }

        private void OnParameterItemDeletionRequested(ParameterElement element)
        {
            element.Root.RemoveFromHierarchy();
            element.DeletionRequested -= OnParameterItemDeletionRequested;
            _parameters.Remove(element);
            // _deletedParameters.Add(element);
            ProcessParametersCountChange();
            for (int i = 0; i < _parameters.Count; i++)
            {
                _parameters[i].AddNameToChoiceList(element.Name);
            }
            // _availableParameterNames.Add(element.Name);
            // _availableParameterNames.Sort();
            // int hash = GetHashValue(element.Name);
            // for (int i = 0; i < _availableParameterNames.Count; i++)
            // {
            //     if (string.Equals(_availableParameterNames[i], element.Name))
            //     {
            //         _availableParameterHashes.Insert(i, hash);
            //         break;
            //     }
            // }
        }

        private void UpdateCreateParamButtonState() => _createParameterButton.SetEnabled(IsAnyAvailableCategory());
        private bool IsAnyAvailableCategory() => _availableParameterHashes != null && _availableParameterHashes.Count > 0;

        private void UpdateAvailableParamNames()
        {
            _availableParameterHashes.Clear();
            _availableParameterNames.Clear();
            int[] hashes = GetHashes(_hash);
            string[] names = GetHashNames(_hash);
            for (int i = 0; i < hashes.Length; i++)
            {
                if (!IsParameterExisting(hashes[i]))
                {
                    _availableParameterHashes.Add(hashes[i]);
                    _availableParameterNames.Add(names[i]);
                }
            }
        }
        
        private void UpdateNewParamNameDropdown()
        {
            if (_availableParameterHashes == null || _availableParameterHashes.Count == 0)
            {
                _newParameterNameDropdown.choices = NoAvailableChoices;
                _newParameterNameDropdown.value = NoAvailableChoices[0];
                _newParameterNameDropdown.SetEnabled(false);
            }
            else
            {
                _newParameterNameDropdown.choices = _availableParameterNames;
                _newParameterNameDropdown.value = _availableParameterNames[0];
                _newParameterNameDropdown.SetEnabled(true);
            }
        }
        
        private bool IsParameterExisting(int hash)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if(_parameters[i].Hash == hash) return true;
            }
            return false;
        }
        

        private void OnDeleteButtonClicked()
        {
            DeletionRequested?.Invoke(this);
        }

        private Type GetTypeByName(string typeName)
        {
            for (int i = 0; i < _parameterTypes.Count; i++)
            {
                if(_parameterTypes[i].Name == typeName) return _parameterTypes[i];
            }
            return null;
        }
        
        private void OnCategoryTypeFieldChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue != _savedTypeName) _categoryTypeDropdown.AddToClassList(ChangedBorder);
            else _categoryTypeDropdown.RemoveFromClassList(ChangedBorder);
            for (int i = 0; i < _categoryTypes.Count; i++)
            {
                if (_categoryTypes[i].Name == evt.newValue)
                {
                    _assemblyQualifiedName = _categoryTypes[i].AssemblyQualifiedName;
                }
            }
        }
    }
}