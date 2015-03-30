
#include "Submodular.h"

using namespace OnigiriSubmodular;

DirectedCut::DirectedCut(int n,double* modular,double** weight):
	Cut(n,modular,weight)
{
}

DirectedCut::DirectedCut(string path):
	Cut(path)
{
}
