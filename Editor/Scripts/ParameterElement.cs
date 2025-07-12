using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using static LazyRedpaw.GenericParameters.Constants;
using static LazyRedpaw.StaticHashes.StaticHashesHelper;

namespace LazyRedpaw.GenericParameters
{
    public class ParameterElement
    {
        private readonly VisualElement _root;
        private readonly DropdownField _nameField;
        private readonly DropdownField _typeField;
        // private readonly Button _revertButton;
        private readonly Button _deleteButton;

        private readonly int _savedHash;
        private readonly string _savedFullType;
        private readonly List<Type> _parameterTypes;
        private readonly List<string> _parameterTypeNames;

        private int _hash;
        private string _assemblyQualifiedName;
        private string _savedName;
        private string _savedTypeName;

        public VisualElement Root => _root;
        public int SavedHash => _savedHash;
        public string Name => _nameField.value;
        public bool IsTypeChanged => _typeField.value != _savedTypeName;
        public int Hash => _hash;
        public string AssemblyQualifiedName => _assemblyQualifiedName;

        public event Action<ParameterElement> DeletionRequested; 
        public event Action<ChangeEvent<string>, int> NameChanged; 
        
        public ParameterElement(VisualElement root, int hash, string name, string typeName, string assemblyQualifiedName,
            List<Type> parameterTypes, List<string> parameterTypeNames, List<string> nameChoiceList)
        {
            _root = root;
            _nameField = _root.Q<DropdownField>(ParameterHashDropdown);
            _typeField = _root.Q<DropdownField>(ParameterTypeDropdown);
            // _revertButton = _root.Q<Button>(RevertButton);
            _deleteButton = _root.Q<Button>(DeleteButton);
            
            _savedHash = hash;
            _savedName = name;
            _savedTypeName = typeName;
            _hash = hash;
            _assemblyQualifiedName = assemblyQualifiedName;

            _parameterTypes = parameterTypes;
            _parameterTypeNames = parameterTypeNames;
            _nameField.choices = nameChoiceList;
            _nameField.SetValueWithoutNotify(name);
            _typeField.choices = _parameterTypeNames;
            _typeField.SetValueWithoutNotify(typeName);

            _typeField.RegisterValueChangedCallback(OnTypeFieldChanged);
            _nameField.RegisterValueChangedCallback(OnNameFieldChanged);
            // _revertButton.clicked += OnRevertButtonClicked;
            _deleteButton.clicked += OnDeleteButtonClicked;
        }

        // public void Revert()
        // {
        //     _nameField.value = _savedName;
        //     _typeField.value = _savedTypeName;
        // }

        private void OnDeleteButtonClicked()
        {
            DeletionRequested?.Invoke(this);
        }
        
        // private void OnRevertButtonClicked()
        // {
        //     Revert();
        // }

        private void OnTypeFieldChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue != _savedTypeName) _typeField.AddToClassList(ChangedBorder);
            else _typeField.RemoveFromClassList(ChangedBorder);
            for (int i = 0; i < _parameterTypes.Count; i++)
            {
                if (_parameterTypes[i].Name == evt.newValue)
                {
                    _assemblyQualifiedName = _parameterTypes[i].AssemblyQualifiedName;
                }
            }
            // UpdateRevertButtonState();
        }
        
        private void OnNameFieldChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue != _savedName) _nameField.AddToClassList(ChangedBorder);
            else _nameField.RemoveFromClassList(ChangedBorder);
            _hash = GetHashValue(evt.newValue);
            // UpdateRevertButtonState();
            NameChanged?.Invoke(evt, _savedHash);
        }

        // private void UpdateRevertButtonState() => _revertButton.SetEnabled(_savedTypeName != _typeField.value || _nameField.value != _savedName);

        public void AddNameToChoiceList(string name)
        {
            _nameField.choices.Add(name);
        }
        
        public void RemoveNameFromChoiceList(string name)
        {
            _nameField.choices.Remove(name);
        }
    }
}