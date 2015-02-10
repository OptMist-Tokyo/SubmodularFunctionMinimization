#ifndef Subomdular_ORACLE_INCLUDED
#define Subomdular_ORACLE_INCLUDED
#include <stdexcept>
#include <fstream>
#include <iostream>
#include <string>
#include <map>
#include <algorithm>
#include <time.h>
#include "Library.h"


#define DLLImport __declspec(dllexport)

using namespace OnigiriLibrary;
using namespace std;

namespace OnigiriSubmodular
{

#pragma region SubmodularOracle

	class SubmodularOracle
	{
	private:
		long memoTime;
	protected:
		double fOfEmpty;
		SubmodularOracle();
		virtual double Value(const int* order,int cardinality) = 0;
		virtual void Base(const int* order, double* base);
		int n;
	public:
		long oracleTime;
		long oracleCall;
		long baseCall;
		DLLImport virtual ~SubmodularOracle();
		DLLImport int N();
		DLLImport double CalcValue(const int* order,int cardinality);
		DLLImport void CalcBase(const int* order, double* base);
		DLLImport bool IsSubmodular();
		static DLLImport void GetOrder( int n,int mask, int &usedBit,int* order);
		static DLLImport void GetOrder(int n, string &minimizer,int &usedBit,int* order);
	};

	//class Modular: public SubmodularOracle
	//{
	//private:
	//	double* modular;
	//public:
	//	Modular(int n,double*modular);
	//	Modular(string path);
	//	void SetVariables(int n,double* modular);
	//	virtual ~Modular();
	//	virtual double CalcValue(const int* order,int cardinality) final override;
	//	virtual void CalcBase(const int* order,double* base) final override;
	//};

	class Modular:public SubmodularOracle
	{
	private:
		double* modular;
	public:
		DLLImport Modular(int n,double*modular);
		DLLImport Modular(string path);
		void SetVariables(int n,double* modular);
		DLLImport virtual ~Modular();
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport virtual void Base(const int* order,double* base) final override;
#if _DEBUG
		DLLImport void TestCalcBase(const int* order);
#endif
	};

	class Cut: public SubmodularOracle
	{
	private:
		double* modular;
		double** weight;
	public:
		DLLImport Cut(int n, double* modular, double** weight);
		DLLImport Cut(string path);
		void SetVariables(int n,double* modular,double** weight);
		DLLImport virtual ~Cut();
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport virtual void Base(const int* order, double* base) final override;
#if _DEBUG
		DLLImport void TestCalcBase(const int* order);
#endif
	};

	class UndirectedCut: public Cut
	{
	public:
		DLLImport UndirectedCut(int n,double*modular,double** weight);
		DLLImport UndirectedCut(string path);
	};

	class DirectedCut: public Cut
	{
	public:
		DLLImport DirectedCut(int n,double*modular,double** weight);
		DLLImport DirectedCut(string path);
	};

	class ConnectedDetachment: public SubmodularOracle
	{
	private:
		double* modular;
		map<int,bool>* edges;
		UnionFind *uf;   //for calculation
		bool* used;    //for calculation
		int* numComplement;    //for calculation
		int CountIncidentEdge(const int* order,int cardinality);
		int CountComponentOfComplementGraph(const int* order,int cardinality);
		void SetArrayOfComplementConnectedComponent(const int* order);
		void SetUsed();
	public :
		DLLImport ConnectedDetachment(int n, double* modular, map<int,bool>* edges);
		DLLImport ConnectedDetachment(string path);
		DLLImport virtual ~ConnectedDetachment();
		void SetVariables(int n,double* modular,map<int,bool>* edges);
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport virtual void Base(const int* order, double* base) final override;
#if _DEBUG
		DLLImport void TestCalcBase(const int* order);
#endif
	};

	class FacilityLocation:public SubmodularOracle
	{
	private:
		double* modular;
		double** matrix;
		double* maxRows;	//for calculation
	public:
		DLLImport FacilityLocation(int n, double* modular, double** matrix);
		DLLImport FacilityLocation(string path);
		DLLImport virtual ~FacilityLocation();
		void SetVariables(int n,double* modular,double** matrix);
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport virtual void Base(const int* order, double* base) final override;
#if _DEBUG
		DLLImport void TestCalcBase(const int* order);
#endif
	};

	class GraphicMatroid: public SubmodularOracle
	{
	private:
		int V;
		double* modular;
		int* heads;
		int* tails;
		UnionFind *uf;
	public:
		DLLImport GraphicMatroid(int n, int V, double* modular, int* heads, int*tails);
		DLLImport GraphicMatroid(string path);
		DLLImport virtual ~GraphicMatroid();
		void SetVariables(int n,int V,double* modular,int* heads,int* tails);
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport virtual void Base(const int* order, double* base) final override;
#if _DEBUG
		DLLImport void TestCalcBase(const int* order);
#endif
	};

	class BinaryMatrixRank: public SubmodularOracle
	{
	private:
		int row;
		double* modular;
		bool** columnVectors;
		bool** coefficientColumnVectors;  //for caluculation
		bool* columnVector;    //or caluculation
		void SetUnitMatrix();
		void SetMultiplyTranspose(int pos,bool** coefficientColumnVectors);
		bool CheckAndDoRankArgmentation(int pos ,  bool** coefficientColumnVectors, int &rank);
		int GetOnePosition(int rank, const bool* columnVector);
		void Swap(bool** coefficientColumnVectors, int rank, bool* columnVector, int onePos);
		void Elimination(bool** coefficientColumnVectors, int rank, bool* columnVector);
	public:
		DLLImport BinaryMatrixRank(int n, int row, double* modular, bool** columnVectors);
		DLLImport BinaryMatrixRank(string path);
		DLLImport ~BinaryMatrixRank();
		void SetVariables(int n,int row,double* modular,bool** columnVectors);
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport virtual void Base(const int* order, double* base) final override;
#if _DEBUG
		DLLImport void TestCalcBase(const int* order);
#endif
	};

	class NonPositiveSymmetricMatrixSummation:public SubmodularOracle
	{
	private:
		double* modular;
		double** matrix;
	public :
		DLLImport NonPositiveSymmetricMatrixSummation(int n, double* modular, double** matrix);
		DLLImport NonPositiveSymmetricMatrixSummation(string path);
		DLLImport ~NonPositiveSymmetricMatrixSummation();
		void SetVariables(int n,double* modular,double** matrix);
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport virtual void Base(const int* order, double* base) final override;
#if _DEBUG
		DLLImport void TestCalcBase(const int* order);
#endif
	};

	class SetCover:public SubmodularOracle
	{
	private:
		int m;
		double* modular;
		double* weight;
		int* length;
		int** edges;
		bool* used;	//for calculation
	public :
		DLLImport SetCover(int n, int m, double* modular, double* weight, int* length, int** edges);
		DLLImport SetCover(string path);
		DLLImport ~SetCover();
		void SetVariables(int n,int m,double* modular,double* weight,int* length, int** edges);
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport virtual void Base(const int* order, double* base) final override;
#if _DEBUG
		DLLImport void TestCalcBase(const int* order);
#endif
	};

	class SetCoverConcave:public SubmodularOracle
	{
	private:
	   #define  POW 0.75
	   #define COEFF 10
		double pow;
		double coeff;
		int m;
		double* modular;
		double* weight;
		int* length;
		int** edges;
		bool* used;	//for calculation
		DLLImport double Calc(double left, double right);
	public :
		DLLImport SetCoverConcave(int n, int m, double* modular, double* weight, int* length, int** edges,double pow = POW,double coeff = COEFF);
		DLLImport SetCoverConcave(string path,double pow = POW, double coeff = COEFF);
		DLLImport ~SetCoverConcave();
		void SetVariables(int n,int m,double* modular,double* weight,int* length, int** edges);
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport virtual void Base(const int* order, double* base) final override;
#if _DEBUG
		DLLImport void TestCalcBase(const int* order);
#endif
	};


#ifdef _DEBUG
	class Manual: public SubmodularOracle
	{
	private :
		__readonly double* values;
	public:
		DLLImport  virtual ~Manual();
		DLLImport Manual(int n,const double* array);
		DLLImport virtual double Value(const int* order,int cardinality) final override;
		DLLImport double Value(int mask);
		DLLImport void Copy(const Manual &other);
	};
#endif

#pragma endregion SubmodularOracle

#pragma region SFMResult

	class SFMResult
	{
	private:
		int outputLength;
		long memoOracleCallTime; //for calculation
		long memoBaseCallTime;  //for calculation
		long memoReductionTime; //for calculation
		double* x;
		string minimizer;
		double minimumValue;
		long iteration;
		long oracleCall;
		long baseCall;
		long reductionCall;
		long startTime;
		long executionTime;
		long reductionTime;
		long oracleTime;
		double dualValue;
		int n;
		void OutputVariable(string tittle, long value,ofstream &file);
		void OutputVariableDouble(string tittle, double value,ofstream &file);
		void OutputVariable(string tittle, string value,ofstream &file);
		void OutputArray(string tittle, double* value,ofstream &file);

	public :
		SFMResult(int n);
		SFMResult(SFMResult &result);
		DLLImport	virtual ~SFMResult();
		void SetResult(double* x,string &minimizer,double minimumValue,long iteration,SubmodularOracle &oracle);
		//void StartOracle();
		//void StopOracle();
		//void StartBase();
		//void StopBase();
		//void StartReduction();
		//void StopReduction();
		DLLImport string Minimizer();
		DLLImport double MinimumValue();
		DLLImport void X(double* &x);
		DLLImport long Iteration();
		DLLImport long OracleCall();
		DLLImport long BaseCall();
		DLLImport long ReductionCall();
		DLLImport long ExecutionTime();
		DLLImport long ReductionTime();
		DLLImport long OracleTime();
		DLLImport double DualValue();
		DLLImport int N();
		DLLImport void Output(string path, bool withX = true);
	};

#pragma endregion SFMResult

#pragma region SubmodularFunctionMinimization

	class SubmodularFunctionMinimization{
	private:
		SubmodularOracle* oracle;
		SFMResult* result;
	protected:
		DLLImport SubmodularFunctionMinimization();
		DLLImport int N();
		DLLImport SFMResult Result();
		//DLLImport double CalcValue(int* order,int cordinality);
		//DLLImport void CalcBase(int*order,double* base);
		DLLImport void SetOracle(SubmodularOracle *oracle);
		//DLLImport void SetResult(double* x,long iteration);
		DLLImport void SetResult(double* x,string &minimizer,long iteration);
	public :
		DLLImport virtual SFMResult Minimization(SubmodularOracle* oracle) = 0;
		DLLImport virtual ~SubmodularFunctionMinimization();
	};

	class BruteForce:public SubmodularFunctionMinimization
	{
	public:
	   DLLImport BruteForce();
		DLLImport virtual SFMResult Minimization(SubmodularOracle* oracle) final override;
		DLLImport ~BruteForce();
	};


#pragma endregion SubmodularFunctionMinimization


}


#endif //Subomdular_FUNCTION_INCLUDED