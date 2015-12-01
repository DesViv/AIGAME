using UnityEngine;
using System.Collections;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using System;
using System.Reflection;

public class ListAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CSharpCodeProvider provider = new CSharpCodeProvider();
        CompilerParameters parameters = new CompilerParameters();
        foreach (string file in Directory.GetFiles("AI"))
        {
            parameters = new CompilerParameters();
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                parameters.ReferencedAssemblies.Add(asm.Location);
            }
            Debug.Log(file);
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
            Type program = assembly.GetType("AISimple");
            MethodInfo main = program.GetMethod("AISimple");
        }
    }

	// Update is called once per frame
	void Update () {

	}
}
