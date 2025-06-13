using System;
using System.Collections.Generic;
using System.Reflection;
using Object = System.Object;

namespace LazyRedpaw.GenericParameters
{
    public static class TypeExtensions
    {
        public static bool ContainsTypeAsAncestor(this Type type, Type ancestor)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            Type lastType = typeof(Object);
            Type baseType = type.BaseType;
            while (baseType != lastType)
            {
                if (ancestor.IsGenericType && baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == ancestor)
                    return true;
                if (!ancestor.IsGenericType && baseType == ancestor)
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }
        
        public static Type GetParentGenericType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type baseType = type.BaseType;
            if (baseType != null && baseType.IsGenericType)
            {
                return baseType.GetGenericTypeDefinition();
            }

            return null; // Not a generic parent type
        }

        public static Type[] GetParentGenericArguments(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type baseType = type.BaseType;
            if (baseType != null && baseType.IsGenericType)
            {
                return baseType.GetGenericArguments();
            }

            return null; // No generic arguments
        }
        
        public static void GetNonAbstractChildrenTypesAndTheirNames(this Type parentType, out Type[] types, out string[] typeNames)
        {
            List<Type> typesList = new List<Type>();
            List<string> typeNamesList = new List<string>();
            Type[] allTypes = Assembly.GetAssembly(parentType).GetTypes();
            for (int i = 0; i < allTypes.Length; i++)
            {
                if (allTypes[i].IsClass &&
                    !allTypes[i].IsAbstract &&
                    allTypes[i].IsSubclassOf(parentType))
                {
                    typeNamesList.Add(allTypes[i].Name);
                    typesList.Add(allTypes[i]);
                }
            }
            types = typesList.ToArray();
            typeNames = typeNamesList.ToArray();
        }
        
        public static void GetNonAbstractChildrenAndSelfTypesAndTheirNames(this Type parentType, out Type[] types, out string[] typeNames)
        {
            List<Type> typesList = new List<Type>();
            List<string> typeNamesList = new List<string>();
            if (parentType.IsClass &&
                !parentType.IsAbstract)
            {
                typeNamesList.Add(parentType.Name);
                typesList.Add(parentType);
            }
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                Type[] allTypes = assembly.GetTypes();
                for (int j = 0; j < allTypes.Length; j++)
                {
                    if (allTypes[j].IsClass &&
                        !allTypes[j].IsAbstract &&
                        allTypes[j].IsSubclassOf(parentType))
                    {
                        typeNamesList.Add(allTypes[j].Name);
                        typesList.Add(allTypes[j]);
                    }
                }
            }
            types = typesList.ToArray();
            typeNames = typeNamesList.ToArray();
        }
    }
}