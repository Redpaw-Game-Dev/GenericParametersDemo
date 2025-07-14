using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace LazyRedpaw.GenericParameters
{
    public static class SerializedPropertyExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetChildrenPropertiesCount(this SerializedProperty property,
            string[] excludedPropNames = null)
        {
            SerializedProperty currentProperty = property.Copy();
            int propertiesCount = 0;
            int depth = currentProperty.depth;
            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (currentProperty.depth == depth + 1)
                    {
                        bool areExcludedNamesContain = false;
                        if (excludedPropNames != null)
                        {
                            for (int i = 0; i < excludedPropNames.Length; i++)
                            {
                                if (currentProperty.name == excludedPropNames[i])
                                {
                                    areExcludedNamesContain = true;
                                    break;
                                }
                            }
                        }

                        if (!areExcludedNamesContain)
                        {
                            propertiesCount++;
                        }
                    }
                } while (currentProperty.NextVisible(false) && currentProperty.depth > depth);
            }

            return propertiesCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetParentObjectOfProperty(this SerializedProperty property)
        {
            string path = property.propertyPath;
            object obj = property.serializedObject.targetObject;

            string[] fieldNames = path.Split('.');
            for (int i = 0; i < fieldNames.Length - 1; i++)
            {
                FieldInfo field = obj.GetType().GetField(fieldNames[i],
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    obj = field.GetValue(obj);
                }
            }

            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetTargetObjectOfProperty(this SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                        .Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }

            return enm.Current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetPropertyFieldType(this SerializedProperty property)
        {
            if (property == null || property.serializedObject?.targetObject == null)
                return null;

            object target = property.serializedObject.targetObject;
            string path = property.propertyPath;

            return GetFieldTypeFromPath(target.GetType(), path, property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type GetFieldTypeFromPath(Type rootType, string path, SerializedProperty property)
        {
            string[] elements = path.Split('.');
            Type currentType = rootType;

            for (int i = 0; i < elements.Length; i++)
            {
                string element = elements[i];

                if (element == "Array" && i + 1 < elements.Length && elements[i + 1].StartsWith("data["))
                {
                    i++;
                    if (IsList(currentType, out Type elementType))
                        currentType = elementType;
                    else if (currentType.IsArray)
                        currentType = currentType.GetElementType();
                    else
                        return null;
                    continue;
                }

                FieldInfo field = GetFieldInfo(currentType, element);
                if (field == null)
                    return null;

                currentType = field.FieldType;

                if (i == elements.Length - 1 && property.propertyType == SerializedPropertyType.ManagedReference)
                {
                    string typename = property.managedReferenceFullTypename;
                    if (!string.IsNullOrEmpty(typename))
                    {
                        var split = typename.Split(' ');
                        if (split.Length == 2)
                        {
                            string assemblyName = split[0];
                            string className = split[1];
                            Type resolved = Type.GetType($"{className}, {assemblyName}");
                            if (resolved != null)
                                return resolved;
                        }
                    }
                }
            }

            return currentType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FieldInfo GetFieldInfo(Type type, string name)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                    return field;
                type = type.BaseType;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsList(Type type, out Type elementType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }

            elementType = null;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetManagedReferenceType(this SerializedProperty property)
        {
            var fullTypeName = property.managedReferenceFullTypename;
            if (string.IsNullOrEmpty(fullTypeName)) return null;

            var parts = fullTypeName.Split(' ');
            if (parts.Length != 2) return null;

            string assemblyName = parts[0];
            string className = parts[1];

            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == assemblyName);

            if (assembly == null)
            {
                Debug.LogWarning($"Assembly '{assemblyName}' not found.");
                return null;
            }

            var type = assembly.GetType(className);
            if (type == null)
            {
                Debug.LogWarning($"Type '{className}' not found in assembly '{assemblyName}'.");
            }

            return type;
        }
    }
}