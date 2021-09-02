//This script defines a CPP plugin for Unity C#
//Receives a struct from a C# script with three strings
//Then concatenate the first two strings in the third string
#include <iostream>
#include "pch.h"
using namespace std;


#define DllExport __declspec (dllexport)

extern "C"
{
	//We define an C# side analog struct 
	//In CPP we use char* instead of string
	struct DllExport TwoStrings
	{
		char* string1;
		char* string2;
		char* concatenated;
	};

	//The function receives a passed by reference struct
	DllExport void ConcatStrings(TwoStrings &twoStrings)
	{
		strcpy_s(twoStrings.concatenated, strlen(twoStrings.string1) + 1, twoStrings.string1);
		strcat_s(twoStrings.concatenated, strlen(twoStrings.string1) + strlen(twoStrings.string2) + 1, twoStrings.string2);
	}
}
