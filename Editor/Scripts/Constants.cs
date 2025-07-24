using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace LazyRedpaw.GenericParameters
{
    public static class Constants
    {
        public const string WindowTitle = "Generic Parameters";
        public const string ModLabel = "ModLabel";
        public const string ModType = "ModType";
        public const string ModActionField = "ModActionField";
        public const string ModValue = "ModValue";
        public const string TypePopupContainer = "TypePopupContainer";
        public const string ValueContainer = "ValueContainer";
        public const string ExpandButton = "ExpandButton";
        public const string DebugContainer = "DebugContainer";
        public const string ParameterLabel = "ParameterLabel";
        public const string NewCategoryNameDropdown = "NewCategoryNameDropdown";
        public const string CreateCategoryButton = "CreateCategoryButton";
        public const string CategoriesScrollView = "CategoriesScrollView";
        public const string CategoriesCount = "CategoriesCount";
        public const string CategoriesFoldout = "CategoriesFoldout";
        public const string CategoriesList = "CategoriesList";
        public const string CategoryRoot = "CategoryRoot";
        public const string CategoryNameLabel = "CategoryNameLabel";
        public const string ParametersCount = "ParametersCount";
        public const string NewParameterNameDropdown = "NewParameterNameDropdown";
        public const string NewParameterTypeDropdown = "NewParameterTypeDropdown";
        public const string ParametersList = "ParametersList";
        public const string ParameterRoot = "ParameterRoot";
        public const string ParametersFoldout = "ParametersFoldout";
        public const string ParameterHashDropdown = "ParameterNameDropdown";
        public const string ParameterTypeDropdown = "ParameterTypeDropdown";
        public const string CategoryTypeDropdown = "CategoryTypeDropdown";
        public const string RevertButton = "RevertButton";
        public const string DeleteButton = "DeleteButton";
        public const string AddButton = "AddButton";
        public const string RemoveButton = "RemoveButton";
        public const string CreateParameterButton = "CreateParameterButton";
        public const string SaveButtonTmp = "SaveButtonTmp";
        public const string AddParamContainer = "AddParamContainer";
        public const string ParamListItemRoot = "ParamListItemRoot";
        public const string CategoryListItemRoot = "CategoryListItemRoot";
        public const string ParamField = "ParamField";
        public const string CategoryField = "CategoryField";
        public const string AddCategoryContainer = "AddCategoryContainer";
        
        public const string HashPropName = "_hash";
        public const string CategoriesPropName = "_categories";
        public const string ParametersPropName = "_parameters";
        
        public const string AlignHorizontal = "align-horizontal";
        public const string FlexGrow = "flex-grow";
        public const string ExpandButtonExpanded = "expand-button-expanded";
        public const string ChangedBorder = "changed-border";
        public const string FoldoutArrow = "foldout-arrow";
        public const string MarginLeft15 = "margin-left-15";
        public const string MarginLeft10 = "margin-left-10";
        public const string LabelUSS = "label";
        public const string ParameterHeader = "parameter-header";
        public const string ModableParam = "modable-param";
        public const string ModableParamLabel = "modable-param-label";
        public const string ParamListItem = "param-list-item";

        public static readonly List<string> NoAvailableChoices = new() { "There is no available option" };
        public const string NullParameterText = "Parameter is null";
        public const string CategoryNotFoundText = "Category not found";
        public const string ChooseParameterText = "Choose parameter";
        public const string ChooseCategoryText = "Choose category";
        public const string AllParamsAddedText = "All parameters are added";
        public const string AllCategoriesAddedText = "All categories are added";
        
        public static readonly Label EmptyListLabel = new Label("List is empty")
        {
            style =
            {
                unityTextAlign = TextAnchor.MiddleLeft,
                marginBottom = 3,
                marginLeft = 5,
                marginRight = 3,
                marginTop = 3,
                flexGrow = 1
            }
        };
        
        public static readonly string GenericParametersJsonFilePath = Path.Combine(Application.dataPath, "Scripts/LazyRedpaw/GenericParameters/GenericParametersStorage.cs");
        public static readonly string FormatterFilePath = Path.Combine(Application.dataPath, "Scripts/LazyRedpaw/GenericParameters/GenericParametersFormatter.cs");
        public const int JsonRowIndex = 6;
        public static readonly Regex JsonRegex = new Regex("^\"(.*)\"$");
        
        public static readonly string[] StorageTemplate = new string[]
        {
            "//This file was generated automatically. Do not change it manually!",
            "namespace LazyRedpaw.GenericParameters",
            "{",
            "\tpublic static class GenericParametersStorage",
            "\t{",
            "\t\tpublic const string Json =",
            // "\t\t\tstring.Empty;",
            "\t}",
            "}"
        };
    }
}