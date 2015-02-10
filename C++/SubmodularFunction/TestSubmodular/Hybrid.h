#ifndef HYBLID_INCLUDED
#define HYBLID_INCLUDED

#include "Submodular.h"

using namespace OnigiriSubmodular;


class Hybrid:public SubmodularFunctionMinimization
{
private:
	int N;
	int K;
	int M;
	int**     lst;
	double**  y;
	double**  f;
	double**  phi;
	int*p;
	double*kappa;
	double** a;
	int reduce(double** pt,double* lambda,int n,int k,int* q);

public:
	DLLImport Hybrid();
	DLLImport virtual SFMResult Minimization(SubmodularOracle* oracle) final override;
	DLLImport ~Hybrid();
};

#endif