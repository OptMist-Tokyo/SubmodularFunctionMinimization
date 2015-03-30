#include "Submodular.h"


using namespace OnigiriSubmodular;



void FacilityLocation::SetVariables(int n,double* modular,double** matrix){
	FacilityLocation::n = n;
	FacilityLocation::modular = new double[n];
	FacilityLocation::matrix = new double*[n];
	for(int i=0;i<n;i++){
		FacilityLocation::modular[i] = modular[i];
		FacilityLocation::matrix[i] = new double[n];
		for(int j=0;j<n;j++){
			FacilityLocation::matrix[i][j] = matrix[i][j];
		}
	}
	FacilityLocation::fOfEmpty = 0;

	maxRows = new double[n];
}

FacilityLocation::FacilityLocation(int n,double* modular,double** matrix){
	SetVariables(n,modular,matrix);
}

FacilityLocation::FacilityLocation(string path){
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


FacilityLocation::~FacilityLocation(){
	delete[]modular;
	for(int i=0;i<n;i++){
		delete[] matrix[i];
	}
	delete[] matrix;

	delete[] maxRows;
}


double FacilityLocation::Value(const int* order,int cardinality) {           
	double res = 0;
            for (int i = 0; i < cardinality; i++)
            {
                res += modular[order[i]];
            }//for i
            for (int i = 0; i < n; i++)
            {
                double maxRow = 0;
                for (int j = 0; j < cardinality; j++)
                {
                    maxRow = max(maxRow, matrix[i][order[j]]);
                }//for j
				res +=maxRow;
            }//for i
            return res;
}

void FacilityLocation::Base(const int* order, double* base) {
			for(int i=0;i<n;i++){
				maxRows[i] = 0;
			}
            for (int i = 0; i < n; i++)
            {
                base[order[i]] = modular[order[i]];
                for (int j = 0; j < n; j++)
                {
                    double nextRow =max(maxRows[j], matrix[j][order[i]]);
                    base[order[i]] += nextRow - maxRows[j];
                    maxRows[j] = nextRow;
                }//for j
            }//for i
}


#if _DEBUG
void FacilityLocation::TestCalcBase(const int* order){
	double* b0 = new double[n];
	double* b1 = new double[n];
	FacilityLocation::CalcBase(order,b0);
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