#ifndef FW_INCLUDED
#define FW_INCLUDED

#include "Submodular.h"

using namespace OnigiriSubmodular;


class FW:public SubmodularFunctionMinimization
{
private:
	void quicksort2(double* A,int* s,int left,int right);
public:
	DLLImport FW();
	DLLImport virtual SFMResult Minimization(SubmodularOracle* oracle) final override;
	DLLImport ~FW();
};

#endif