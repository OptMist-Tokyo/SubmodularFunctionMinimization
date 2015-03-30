#include "Submodular.h"
using namespace OnigiriSubmodular;


void SetCover::SetVariables(int n,int m,double* modular,double* weight,int* length, int** edges){
	SetCover::n = n;
	SetCover::m = m;
	SetCover::modular = new double[n];
	SetCover::length = new int[n];
	SetCover::edges = new int*[n];
	for(int i=0;i<n;i++){
		SetCover::modular[i] = modular[i];
		SetCover::length[i] = length[i];
		SetCover::edges[i] = new int[length[i]];
		for(int j=0;j<length[i];j++){
			SetCover::edges[i][j] = edges[i][j];
		}
	}
	SetCover::weight = new double[m];
	for(int i=0;i<m;i++){
		SetCover::weight[i] = weight[i];
	}
	SetCover::fOfEmpty = 0;

	used = new bool[m];
}

SetCover::SetCover(int n,int m,double* modular,double* weight,int* length, int** edges){
	SetVariables(n,m,modular,weight,length, edges);
}

SetCover:: SetCover(string path){
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

SetCover::~SetCover(){
	delete[]modular;
	delete[] weight;
	delete[] length;
	for(int i=0;i<n;i++){
		delete[] edges[i];
	}
	delete[] edges;

	delete[]used;
}

double SetCover::Value(const int* order,int cardinality){
	double res = 0;
	for(int i=0;i<m;i++){
		used[i] = false;
	}
	for (int i = 0; i < cardinality; i++)
	{
		int cur = order[i];
		res += modular[cur];
		for (int j = 0;j<length[cur];j++)
		{
			int neighboor = edges[cur][j];
			if (!used[neighboor])
			{
				used[neighboor] = true;
				res += weight[neighboor];
			}//if
		}//foreach neighboor
	}//for i
	return res;
}

void SetCover::Base(const int* order, double* base){
	for(int i=0;i<m;i++){
		used[i] = false;
	}
	for (int i = 0; i < n; i++)
	{
		int cur = order[i];
		base[cur] = modular[cur];
		for (int j = 0;j<length[cur];j++)
		{
			int neighboor = edges[cur][j];
			if (!used[neighboor])
			{
				used[neighboor] = true;
				base[cur] += weight[neighboor];
			}//if
		}//foreach neighboor
	}//for i
}


#if _DEBUG
void SetCover::TestCalcBase(const int* order){
	double* b0 = new double[n];
	double* b1 = new double[n];
	SetCover::CalcBase(order,b0);
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