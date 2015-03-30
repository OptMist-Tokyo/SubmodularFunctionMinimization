#include "Submodular.h"


using namespace OnigiriSubmodular;


//Modular::Modular(int n,  double* modular){
//	Modular::n = n;
//	Modular::modular = new double[n];
//	for(int i=0;i<n;i++){
//		Modular::modular[i] = modular[i];
//	}
//	Modular::fOfEmpty = 0;
//}

void Modular::SetVariables(int n,  double* modular){
	Modular::n = n;
	Modular::modular = new double[n];
	for(int i=0;i<n;i++){
		Modular::modular[i] = modular[i];
	}
	Modular::fOfEmpty = 0;
}

Modular::Modular(int n,double* modular){
	SetVariables(n,modular);
}

Modular::Modular(string path){ 
	ifstream file(path);
	int n;
	if(file.fail()) {
		cerr << path + " does not exist."<<endl;
		exit(0);
	}
	file>>n;
	double* modular =new double[n];
	for(int i=0;i<n;i++){
		file>>modular[i];
	}
	file.close();
	SetVariables(n,modular);
	delete[]modular;
}

Modular::~Modular(){
	delete[]Modular::modular;
}


double Modular::Value(const int* order,int cardinality){
	double res = 0;
	for (int i = 0; i < cardinality; i++)
	{
		res += modular[order[i]];
	}//for i
	return res;
}

void Modular::Base(const int* order,double* base)
{
	for (int i = 0; i < n; i++)
	{
		base[i] = modular[i];
	}//for i
}

#if _DEBUG

void Modular::TestCalcBase(const int* order){
	double* b0 = new double[n];
	double* b1 = new double[n];
	Modular::CalcBase(order,b0);
	SubmodularOracle::CalcBase(order,b1);
	bool same  = true;
	for(int i=0;i<n;i++){
		same&=(b0[i]==b1[i]);
	}
	delete[]b0;
	delete[]b1;
	if (!same)
	{
		throw new exception();
	}//if
}
#endif