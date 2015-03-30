#ifdef _DEBUG

#include "Submodular.h"

using namespace OnigiriSubmodular;

Manual::Manual(int n,const double* array)
{
	Manual::n = n;
	Manual::values = new double[1<<n];
	for(int i=0;i<n;i++){
		Manual::values[i] = array[i];
	}
	Manual::fOfEmpty = values[0];
}

Manual::~Manual(){
	delete[] Manual::values;
}

double Manual::Value(const int* order,int cardinality)
{
	int mask = 0;
	for (int i = 0; i < cardinality; i++)
	{
		mask |= 1 << order[i];
	}//for i
	return values[mask];
}

double Manual:: Value(int mask)
{
	return values[mask];
}

void Manual::Copy(const Manual &other){
	Manual::n = other.n;
	Manual::values = new double[n];
	for(int i=0;i<n;i++){
		values[i] = other.values[i];
	}
}

#endif