#include "Submodular.h"
using namespace OnigiriSubmodular;



void NonPositiveSymmetricMatrixSummation::SetVariables(int n, double* modular, double** matrix){
	NonPositiveSymmetricMatrixSummation::n = n;
	NonPositiveSymmetricMatrixSummation::modular =new double[n];
	NonPositiveSymmetricMatrixSummation::matrix = new double*[n];
	for(int i=0;i<n;i++){
		NonPositiveSymmetricMatrixSummation::modular[i ] = modular[i];
		NonPositiveSymmetricMatrixSummation::matrix[i] = new double[n];
		for(int j=0;j<n;j++){
			NonPositiveSymmetricMatrixSummation::matrix[i][j] = matrix[i][j];
		}
	}
	NonPositiveSymmetricMatrixSummation::fOfEmpty = 0;
}

NonPositiveSymmetricMatrixSummation::NonPositiveSymmetricMatrixSummation( int n, double* modular, double** matrix){
	SetVariables(n,modular,matrix);
}

NonPositiveSymmetricMatrixSummation::NonPositiveSymmetricMatrixSummation(string path){
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

	double** matrix = new double*[n];
	for(int i=0;i<n;i++){
		matrix[i] = new double[n];
		for(int j=0;j<n;j++){
			file>>matrix[i][j];
		}
	}
	file.close();
	SetVariables(n,modular,matrix);
	delete[]modular;
	for(int i=0;i<n;i++){
		delete[]matrix[i];
	}
	delete[] matrix;
}

NonPositiveSymmetricMatrixSummation::~NonPositiveSymmetricMatrixSummation(){
	delete[]modular;
	for(int i=0;i<n;i++){
		delete[] matrix[i];
	}
	delete [] matrix;
}

double NonPositiveSymmetricMatrixSummation::Value(const int* order,int cardinality){
	double res = 0;
	for (int i = 0; i < cardinality; i++)
	{
		res += modular[order[i]];
		for (int j = 0; j < cardinality; j++)
		{
			res += matrix[order[i]][order[j]];
		}//for j
	}//for i
	return res;
}

void NonPositiveSymmetricMatrixSummation::Base(const int* order, double* base){
	for (int i = 0; i < n; i++)
	{
		int cur = order[i];
		base[cur] = modular[cur];
		base[cur] += matrix[cur][cur];
		for (int j = 0; j < i; j++)
		{
			base[cur] += matrix[cur][order[j]] + matrix[order[j]][cur];
		}//for j
	}//for i
}

#if _DEBUG
void NonPositiveSymmetricMatrixSummation::TestCalcBase(const int* order){
	double* b0 = new double[n];
	double* b1 = new double[n];
	NonPositiveSymmetricMatrixSummation::CalcBase(order,b0);
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