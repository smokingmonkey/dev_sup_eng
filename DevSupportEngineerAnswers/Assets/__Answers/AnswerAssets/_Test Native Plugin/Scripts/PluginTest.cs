//This script is an example of the use of an implemented CPP Unity plugin
//Calls a function in the CPP plugin passing a struct by reference.
//The passed struct is defined with three strings.
//After calling the function, the third string will contain the concatenated content
//of the other two strings

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class PluginTest : MonoBehaviour
{
    const string dll = "DLLTEST";
    
    //You can try different strings in inspector
    [SerializeField]
    string testString1;
    [SerializeField]
    string testString2;

    [StructLayout(LayoutKind.Sequential)]
    public struct TwoStrings
    {
        public string string1;
        public string string2;
        public string concatenated;

        public TwoStrings(string String1, string String2)
        {
            string1 = String1;
            string2 = String2;
            concatenated = "";
        }
    }
    
    // CallingConvention.Cdecl needed to align both sides of the inteface because, in C#  CallingConvention it's set by default to Winapi that turns into StdCall 
    // meanwhile for C and C++ the default calling convention is Cdecl.
    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void ConcatStrings(ref TwoStrings twoStrings);

    // Start is called before the first frame update
    void Start()
    {
        TwoStrings tw = new TwoStrings(testString1, testString2);
        Debug.Log("Concat before call: " + tw.concatenated);

        //The call to our extern function
        ConcatStrings(ref tw);
        Debug.Log("Concat after call: " + tw.concatenated);
    }
    
}
