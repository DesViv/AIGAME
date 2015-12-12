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
    public static Boolean init;//Check if code is complied, so that it doesn't have to do it everytime
    public static Dictionary<string, AIBase> AIPrograms;//HashMap of AIScripts to be able to access
    /*
    Compiles the files in the AI sub-dirctory
    */
	public static void initAI() {
        if (!init)
        {
            AIPrograms = new Dictionary<string, AIBase>();//Create the hashmap
            CSharpCodeCompiler provider = new CSharpCodeCompiler(); //Get the complier
            CompilerParameters parameters = new CompilerParameters();
            foreach (string file in Directory.GetFiles("AI"))
            {
              //For every file in the AI directory
				Debug.Log ("file: " + file);
                parameters = new CompilerParameters();//Generate new parametes
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    parameters.ReferencedAssemblies.Add(asm.Location);//Link all the assmeblies to this new file that we are compiling
                }
                Debug.Log(Path.GetFileNameWithoutExtension(file));
                //Read file
                StreamReader theReader = new StreamReader(file, Encoding.Default);
                string code = theReader.ReadToEnd();
                //Actually compile
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);
                //Check for errors and print them. It would be good to display these later.
                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError CompErr in results.Errors)
                    {
                        Debug.Log("Line number " + CompErr.Line +
                            ", Error Number: " + CompErr.ErrorNumber +
                            ", '" + CompErr.ErrorText + ";");
                    }
                }
                //Create results. Note this will cause a crash if there are errors in scripts. Should change
                Assembly assembly = results.CompiledAssembly;
                Debug.Log("Assembly "+assembly);
                //Get the name of the C# script assembly. 
                Type program = assembly.GetType(Path.GetFileNameWithoutExtension(file));
                //Get the constructor for that script to generate a new class
                ConstructorInfo main = program.GetConstructor(new Type[0]);
                //Convert it to an AI script
                AIBase ai = (AIBase)main.Invoke(new System.Object[0]);
                //Add to hashmap under its name.
                AIPrograms.Add(Path.GetFileNameWithoutExtension(file), ai);
            }
        }
    }
}
