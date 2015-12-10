using UnityEngine;
using System.Collections;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Modified.Mono.CSharp;
using System;
using System.Reflection;
using Completed;
using System.Collections.Generic;

public class ListAI  {

    // Use this for initialization
    public static Boolean init;
    public static Dictionary<string, AIBase> AIPrograms;
	public static void initAI() {
        if (!init)
        {
            AIPrograms = new Dictionary<string, AIBase>();
            CSharpCodeCompiler provider = new CSharpCodeCompiler();
            CompilerParameters parameters = new CompilerParameters();
            foreach (string file in Directory.GetFiles("AI"))
            {
                parameters = new CompilerParameters();
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    parameters.ReferencedAssemblies.Add(asm.Location);
                }
                Debug.Log(Path.GetFileNameWithoutExtension(file));
                StreamReader theReader = new StreamReader(file, Encoding.Default);
                string code = theReader.ReadToEnd();
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);
                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError CompErr in results.Errors)
                    {
                        Debug.Log("Line number " + CompErr.Line +
                            ", Error Number: " + CompErr.ErrorNumber +
                            ", '" + CompErr.ErrorText + ";");
                    }
                }
                Assembly assembly = results.CompiledAssembly;
                Debug.Log("Assembly "+assembly);
                Type program = assembly.GetType(Path.GetFileNameWithoutExtension(file));
                ConstructorInfo main = program.GetConstructor(new Type[0]);
                AIBase ai = (AIBase)main.Invoke(new System.Object[0]);
                AIPrograms.Add(Path.GetFileNameWithoutExtension(file), ai);
            }
        }
    }
}
