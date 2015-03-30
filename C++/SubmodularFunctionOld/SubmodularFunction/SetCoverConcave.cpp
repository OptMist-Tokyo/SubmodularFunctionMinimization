#include "Submodular.h"
using namespace OnigiriSubmodular;


void SetCoverConcave::SetVariables(int n,int m,double* modular,double* weight,int* length, int** edges){
	SetCoverConcave::n = n;
	SetCoverConcave::m = m;
	SetCoverConcave::modular = new double[n];
	SetCoverConcave::length = new int[n];
	SetCoverConcave::edges = new int*[n];
	for(int i=0;i<n;i++){
		SetCoverConcave::modular[i] = modular[i];
		SetCoverConcave::length[i] = length[i];
		SetCoverConcave::edges[i] = new int[length[i]];
		for(int j=0;j<length[i];j++){
			SetCoverConcave::edges[i][j] = edges[i][j];
		}
	}
	SetCoverConcave::weight = new double[m];
	for(int i=0;i<m;i++){
		SetCoverConcave::weight[i] = weight[i];
	}
	SetCoverConcave::fOfEmpty = 0;

	used = new bool[m];
}

SetCoverConcave::SetCoverConcave(int n,int m,double* modular,double* weight,int* length, int** edges,double pow,double coeff){
	SetCoverConcave::pow = pow;
	SetCoverConcave::coeff = coeff;
	SetVariables(n,m,modular,weight,length, edges);
}

SetCoverConcave:: SetCoverConcave(string path,double pow,double coeff){
	SetCoverConcave::pow = pow;
	SetCoverConcave::coeff = coeff;
	ifstream file(path);
	if(file.fail()) {
		cerr << path + " does not exist."<<endl;
		exit(0);
	}

	int n;
	file>>n;
	double* modular =new double[n];
	for(int i=0;i<n;i++){
		file>>modular[i];
	}

	int m;
	file>>m;
	double* weight = new double[m];
	for(int i=0;i<m;i++){
		file>>weight[i];
	}

	int* length = new int[n];
	int** edges =new int*[n];
	for(int i=0;i<n;i++){
		file>>length[i];
		edges[i] =new int[length[i]];
		for(int j=0;j<length[i];j++){
			file>>edges[i][j];
		}
	}
	file.close();
	SetVariables(n,m,modular,weight,length,edges);
	delete[]modular;
	delete[] weight;
	delete[] length;
	for(int i=0;i<n;i++){
		delete[] edges[i];
	}
	delete[] edges;
}

SetCoverConcave::~SetCoverConcave(){
	delete[]modular;
	delete[] weight;
	delete[] length;
	for(int i=0;i<n;i++){
		delete[] edges[i];
	}
	delete[] edges;

	delete[]used;
}

double SetCoverConcave::Calc(double left,double right){
	return left + coeff * std::pow(right, pow);
}

double SetCoverConcave::Value(const int* order,int cardinality){
	for(int i=0;i<m;i++){
		used[i] = false;
	}
	double left = 0;
	double right = 0;
	for (int i = 0; i < cardinality; i++)
	{
		int cur = order[i];
		left += modular[cur];
		for (int j = 0;j<length[cur];j++)
		{
			int neighboor = edges[cur][j];
			if (!used[neighboor])
			{
				used[neighboor] = true;
				right += weight[neighboor];
			}//if
		}//foreach neighboor
	}//for i
	double res = Calc(left,right);
	return res;
}

void SetCoverConcave::Base(const int* order, double* base){
	for(int i=0;i<m;i++){
		used[i] = false;
	}
            double prevVal = 0;
            double prevLeft = 0;
            double prevRight = 0;
            double left = 0;
            double right = 0;
	for (int i = 0; i < n; i++)
	{
		int cur = order[i];
                left = prevLeft + modular[cur];
                right = prevRight;
		for (int j = 0;j<length[cur];j++)
		{
			int neighboor = edges[cur][j];
			if (!used[neighboor])
			{
				used[neighboor] = true;
                        right += weight[neighboor];
			}//if
		}//foreach neighboor
                double curVal = Calc(left, right);
                base[cur] = curVal - prevVal;
                prevVal = curVal;
                prevLeft = left;
                prevRight = right;
	}//for i
}


#if _DEBUG
void SetCoverConcave::TestCalcBase(const int* order){
	double* b0 = new double[n];
	double* b1 = new double[n];
	SetCoverConcave::CalcBase(order,b0);
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