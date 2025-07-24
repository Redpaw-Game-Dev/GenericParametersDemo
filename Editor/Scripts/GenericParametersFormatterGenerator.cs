using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using static LazyRedpaw.GenericParameters.Constants;

namespace LazyRedpaw.GenericParameters
{
    public class GenericParametersFormatterGenerator
    {
        [DidReloadScripts]
        public static void Generate()
        {
            StringBuilder strBuilder = new StringBuilder("//This file was generated automatically. Do not change it manually!\n");
            strBuilder.AppendLine("using MemoryPack;\nusing UnityEngine;\n\nnamespace LazyRedpaw.GenericParameters\n{");
            strBuilder.AppendLine($"\t[MemoryPackUnionFormatter(typeof({nameof(IParameter)}))]");
            IList<Type> types = typeof(IParameter).GetMemoryPackableNonAbstractChildrenTypes();
            for (int i = 0; i < types.Count; i++)
            {
                strBuilder.AppendLine($"\t[MemoryPackUnion({i}, typeof({types[i].Name}))]");
            }
            strBuilder.Append("\tpublic partial class GenericParametersFormatter\n\t{\n\t\t[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]\n\t\tstatic void Initialize()\n\t\t{\n\t\t\tGenericParametersFormatterInitializer.RegisterFormatter();\n\t\t}\n\t}\n}");
            string directory = Path.GetDirectoryName(FormatterFilePath);
            if(!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.Create(FormatterFilePath).Close();
            File.WriteAllText(FormatterFilePath, strBuilder.ToString());
            AssetDatabase.Refresh();
        }
    }
}