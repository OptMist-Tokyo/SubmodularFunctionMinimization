
#include "Submodular.h"

using namespace OnigiriSubmodular;

UndirectedCut::UndirectedCut(int n, double* modular,double** weight):
	Cut(n,modular,weight)
{
}

UndirectedCut::UndirectedCut(string path):
	Cut(path)
{
}
