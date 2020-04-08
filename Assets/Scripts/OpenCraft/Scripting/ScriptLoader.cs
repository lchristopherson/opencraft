using RoslynCSharp;

using UnityEngine;

// using OpenCraft.API.V1;

public class ScriptLoader : MonoBehaviour
{
    private ScriptDomain scriptDomain;

    // Start is called before the first frame update
    void Start()
    {
        scriptDomain = ScriptDomain.CreateDomain("MyDomain");
        // scriptDomain.RoslynCompilerService.ReferenceAssemblies.Add(typeof(Common).Assembly);
        // scriptDomain.RoslynCompilerService.

        var path = "/Users/lchristopherson/OpenCraft/Assets/Scripts/OpenCraft/Scripting/Test.cs";

        // ScriptType type = domain.CompileAndLoadMainSource(cSharpSource);
        
        ScriptType type = scriptDomain.CompileAndLoadMainFile(path);


        // ScriptAssembly type = scriptDomain.CompileAndLoadFile(path);
        // var types = type.FindAllTypes();
        
        // Debug.Log(types.Length);
        

        // ScriptType type = scriptDomain.CompileAndLoadMainSource(source);
        ScriptProxy proxy = type.CreateInstance();

        proxy.Call("SayHello");

        // Common.Print("Hello!");
    }
}
