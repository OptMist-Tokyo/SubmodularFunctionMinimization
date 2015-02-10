#include "Submodular.h"


using namespace OnigiriSubmodular;

void Cut::SetVariables(int n, double* modular, double** weight){
	Cut::n = n;
	Cut::modular = new double[n];
	Cut::weight = new double*[n];
	for(int i=0;i<n;i++){
		Cut::modular[i] = modular[i];
		Cut::weight[i] = new double[n];
		for(int j=0;j<n;j++){
			Cut::weight[i][j] = weight[i][j];
		}
	}
	Cut::fOfEmpty = 0;
}

Cut::Cut(int n,double* modular,double** weight){
	SetVariables(n,modular,weight);
}

Cut::Cut(string path){ 
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
	double** weight = new double*[n];
	for(int i=0;i<n;i++){
		weight[i] = new double[n];
		for(int j=0;j<n;j++){
			file>>weight[i][j];
		}
	}
	file.close();
	SetVariables(n,modular,weight);
	delete[]modular;
	for(int i=0;i<n;i++){
		delete[] weight[i];
	}
	delete[] weight;
}

Cut::~Cut(){
	delete[]Cut::modular;
	for(int i=0;i<n;i++){
		delete[] Cut::weight[i];
	}
	delete[] Cut::weight;
}


double Cut::Value(const int* order,int cardinality){
	double res = 0;
	for (int i = 0; i < cardinality; i++)
	{
		res += modular[order[i]];
		for (int j = cardinality; j < n; j++)
		{
			res += weight[order[i]][order[j]];
		}//for j
	}//for i
	return res;
}

void Cut::Base(const int* order,double* base){
	for (int i = 0; i < n; i++)
	{
		int cur = order[i];
		base[cur] = modular[cur];
		for (int j = 0; j < i; j++)
		{
			base[cur] -= weight[order[j]][cur];
		}//for j
		for (int j = i + 1; j < n; j++)
		{
			base[cur] += weight[cur][order[j]];
		}//for j
	}//for i
}

#if _DEBUG

void Cut::TestCalcBase(const int* order){
	double* b0 = new double[n];
	double* b1 = new double[n];
	Cut::CalcBase(order,b0);
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