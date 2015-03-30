#include "Submodular.h"
using namespace OnigiriSubmodular;



void BinaryMatrixRank::SetVariables( int n,int row, double* modular,bool** columnVectors){
	BinaryMatrixRank::row = row;
	BinaryMatrixRank::n = n;
	BinaryMatrixRank::modular = new double[n];
	BinaryMatrixRank::columnVectors =new bool*[n];
	for(int i=0;i<n;i++){
		BinaryMatrixRank::modular[i] = modular[i];
		BinaryMatrixRank::columnVectors[i] = new bool[n];
		for(int j=0;j<n;j++){
			BinaryMatrixRank::columnVectors[i][j]= columnVectors[i][j];
		}
	}
	BinaryMatrixRank::fOfEmpty = 0;

	columnVector = new bool[row];
	coefficientColumnVectors=new bool*[row];
	for(int i=0;i<row;i++){
		coefficientColumnVectors[i] = new bool[row];
	}
}

BinaryMatrixRank::BinaryMatrixRank(int n,const  int row,double* modular, bool** columnVectors){
	SetVariables(n,row,modular,columnVectors);
}

BinaryMatrixRank::BinaryMatrixRank(string path){
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

	int row;
	file>>row;
	bool** columnVectors = new bool*[n];
	for(int i=0;i<n;i++){
		columnVectors[i] = new bool[row];
	}
	for(int j=0;j<n;j++){
		for(int i=0;i<n;i++){
			int tmp;
			file>>tmp;
			columnVectors[i][j] = (tmp==1);
		}
	}

	file.close();
	SetVariables(n,row,modular,columnVectors);
	delete[]modular;
	for(int i=0;i<n;i++){
		delete[] columnVectors[i];
	}
	delete[]columnVectors;
}

BinaryMatrixRank::~BinaryMatrixRank(){
	delete[]modular;
	for(int i=0;i<n;i++){
		delete[] columnVectors[i];
	}
	delete[]columnVectors;
	
	delete[] columnVector;
	for(int i=0;i<row;i++){
		delete[] coefficientColumnVectors[i] ;
	}
	delete[]coefficientColumnVectors;
}

double BinaryMatrixRank::Value(const int* order,int cardinality) {
	SetUnitMatrix();
	int rank = 0;
	double res = 0;
	for (int k = 0; k < cardinality; k++)
	{
		res+=modular[order[k]];
		CheckAndDoRankArgmentation(order[k], coefficientColumnVectors, rank);
	}//for k
	res += rank;
	return res;
}

void BinaryMatrixRank::Base(const int* order, double* base){
	SetUnitMatrix();
	int rank = 0;
	for (int i = 0; i < n; i++)
	{
		base[order[i]] = modular[order[i]];
		if (CheckAndDoRankArgmentation(order[i],coefficientColumnVectors,rank))
		{
			base[order[i]]++;
		}//if
	}//for i
}

void BinaryMatrixRank::Elimination(bool** coefficientColumnVectors, int rank, bool* columnVector)
{
	for (int i = rank + 1; i < row; i++)
	{
		if (columnVector[i])
		{
			for (int j = 0; j < row; j++)
			{
				coefficientColumnVectors[i][j] ^= coefficientColumnVectors[rank][j];
			}//for j
		}//if
	}//for i
}

void BinaryMatrixRank::Swap(bool** coefficientColumnVectors, int rank, bool* columnVector, int onePos){
	bool tmpVariable = columnVector[onePos];
	columnVector[onePos] = columnVector[rank];
	columnVector[rank] = tmpVariable;
	bool* tmpArray = coefficientColumnVectors[onePos];
	coefficientColumnVectors[onePos] = coefficientColumnVectors[rank];
	coefficientColumnVectors[rank] = tmpArray;
}

bool BinaryMatrixRank::CheckAndDoRankArgmentation(int pos ,bool** coefficientColumnVectors, int &rank)
{
	SetMultiplyTranspose(pos, coefficientColumnVectors);
	int onePos = GetOnePosition(rank, columnVector);
	if (onePos == -1)
	{
		return false;
	}//if
	Swap(coefficientColumnVectors, rank, columnVector, onePos);
	Elimination(coefficientColumnVectors, rank, columnVector);
	rank++;
	return true;
}

int BinaryMatrixRank::GetOnePosition(int rank, const bool* columnVector){
	for (int i = rank; i < row; i++)
	{
		if (columnVector[i])
		{
			return i;
		}//if
	}//for i
	return -1;
}

void BinaryMatrixRank::SetMultiplyTranspose(int pos,bool** coefficientColumnVectors){	
	for (int i = 0; i < row; i++)
	{
		columnVector[i] = 0;
		for (int j = 0; j < row; j++)
		{
			columnVector[i] ^= (coefficientColumnVectors[i][j] & columnVectors[pos][j]);
		}//for j
	}//for i
}

void BinaryMatrixRank::SetUnitMatrix(){
	for(int i=0;i<row;i++){
		for(int j=0;j<row;j++){
			coefficientColumnVectors[i][j] = false;
		}
		coefficientColumnVectors[i][i] = true;
	}
}


#if _DEBUG
void BinaryMatrixRank::TestCalcBase(const int* order){
	double* b0 = new double[n];
	double* b1 = new double[n];
	BinaryMatrixRank::CalcBase(order,b0);
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