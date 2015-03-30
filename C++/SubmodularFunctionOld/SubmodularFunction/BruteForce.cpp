#include "Submodular.h"
using namespace OnigiriSubmodular;

string ConvertToString(int minimizer,int n)
{
	string sb = "";
	for (int i = 0; i < n; i++)
	{
		sb+=(((minimizer & 1) == 1) ? "1" : "0");
		minimizer >>= 1;
	}//for i
	return sb;
}

BruteForce::BruteForce()
{
}

BruteForce::~BruteForce()
{
}

SFMResult BruteForce::Minimization(SubmodularOracle* oracle)
{     
	SubmodularFunctionMinimization::SetOracle(oracle);

	double minValue = DBL_MAX;
	int minimizer = -1;
	int iteration = 0;
	int* order = new int[N()];
	for (int mask = 0; mask < (1 << N()); mask++)
	{
		iteration++;
		int usedBit ;
		SubmodularOracle::GetOrder(N(), mask, usedBit,order);
		double cur = oracle->CalcValue(order, usedBit);
		if (cur<minValue)
		{
			minValue = cur;
			minimizer = mask;
		}//if
	}//for mask
	string min = ConvertToString(minimizer,N());
	SetResult(NULL,min,iteration);
	delete[] order;
	return Result();
}