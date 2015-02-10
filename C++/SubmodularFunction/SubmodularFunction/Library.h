#ifndef Onigiri_LIBRARY_INCLUDED
#define Onigiri_LIBRARY_INCLUDED
#define DLLImport __declspec(dllexport)
#include <string>
#include <stdio.h>
#include <sstream>
using namespace std;

namespace OnigiriLibrary{

	class UnionFind
	{
	private:
		int len;
		int* parent;
	public :
		UnionFind(int n=0);
		~UnionFind();
		int Find(int x);
		bool Unite(int x, int y);
		bool Same(int x, int y);
		void Clear();
	};


	class Converter
	{
	public:
		DLLImport static string IntToString(long number);
		DLLImport static string DoubleToString(double number);
	};


}

#endif