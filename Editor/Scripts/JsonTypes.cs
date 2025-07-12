using System;
using System.Collections.Generic;

namespace LazyRedpaw.GenericParameters
{
    [Serializable]
    public class ParameterJson
    {
        public int Hash;
        public string AssemblyQualifiedName;
    }

    [Serializable]
    public class CategoryJson
    {
        public int Hash;
        public string AssemblyQualifiedName;
        public List<ParameterJson> Parameters;
    }
    
    [Serializable]
    public class MainJson
    {
        public List<CategoryJson> Categories;
    }
}