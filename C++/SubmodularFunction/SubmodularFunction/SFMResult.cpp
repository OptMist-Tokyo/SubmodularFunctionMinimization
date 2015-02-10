#include "Submodular.h"

using namespace OnigiriSubmodular;

SFMResult::SFMResult(int n){
	outputLength = 20;
	SFMResult::n = n;
	iteration=0;
	oracleCall=0;
	baseCall=0;
	reductionCall=0;
	executionTime=0;
	reductionTime=0;
	oracleTime=0;
	startTime = clock();
	x = NULL;
}

SFMResult::SFMResult(SFMResult &result){
	SFMResult::outputLength = result.outputLength;
	SFMResult::minimizer = result.minimizer;
	SFMResult::	minimumValue= result.minimumValue;
	SFMResult:: iteration= result.iteration;
	SFMResult::oracleCall=result.oracleCall;
	SFMResult:: baseCall = result.baseCall;
	SFMResult:: reductionCall = result.reductionCall;
	SFMResult:: executionTime= result.executionTime;
	SFMResult:: reductionTime=result.reductionTime;
	SFMResult::oracleTime= result.oracleTime;
	SFMResult:: dualValue = result.dualValue;
	SFMResult:: n = result.n;
	SFMResult:: x = new double[n];
	result.X(SFMResult::x);
}

SFMResult::	~SFMResult(){
	if(x!=NULL){
		delete[] x;
	}
}

void SFMResult:: SetResult(double* x,string &minimizer,double minimumValue,long iteration,SubmodularOracle &oracle){
	executionTime = clock()-startTime;
	SFMResult:: minimizer =minimizer;
	SFMResult::minimumValue = minimumValue;
	SFMResult::iteration = iteration;
	SFMResult::dualValue = 0;
	SFMResult::oracleCall = oracle.oracleCall;
	SFMResult::baseCall = oracle.baseCall;
	SFMResult::oracleTime= oracle.oracleTime;
	if(x!=NULL){
		SFMResult::x = new double[n];
		for(int i=0;i<n;i++){
			SFMResult::x[i] = x[i];
			if(x[i]<=0){
				SFMResult::dualValue +=x[i];
			}
		}
	}else{
		SFMResult::x = NULL;
	}
}

void SFMResult::OutputArray(string tittle, double* x,ofstream& file){
	tittle.resize(SFMResult::outputLength,' ');
	file<<tittle<<" : "<<endl;
	if(x!=NULL){
		for(int i=0;i<n;i++){
			file<<Converter::DoubleToString(x[i])<<endl;
		}
	}
}

void SFMResult::OutputVariable(string tittle, string value,ofstream& file){
	tittle.resize(SFMResult::outputLength,' ');
	file<<tittle<<" : "<<value<<endl;
}

void SFMResult::OutputVariable(string tittle, long value,ofstream& file){
	tittle.resize(SFMResult::outputLength,' ');
	file<<tittle<<" : "<<Converter::IntToString(value)<<endl;
}

void SFMResult::OutputVariableDouble(string tittle, double value,ofstream& file){
	tittle.resize(SFMResult::outputLength,' ');
	file<<tittle<<" : "<<Converter::DoubleToString(value)<<endl;
}

void SFMResult::Output(string path,bool withX){	
	ofstream file(path,ios_base::app);
	OutputVariable("N",N(),file);
	OutputVariable("Iteration", Iteration(), file);
	OutputVariable("Execurtion Time", ExecutionTime(), file);
	OutputVariable("Oracle Time", OracleTime(), file);
	OutputVariable("Reduction Time", ReductionTime(), file);
	OutputVariable("Oracle Call", OracleCall(), file);
	OutputVariable("Base Call", BaseCall(), file);
	OutputVariable("Reduce Call",ReductionCall(),file);
	OutputVariableDouble("Minimum Value", MinimumValue(), file);
	OutputVariable("Minimizer", Minimizer(), file);
	if(withX){
		OutputVariableDouble("Dual Value", DualValue(), file);
	}
	double* x = new double[N()];
	X(x);
	OutputArray("x", x, file);
	delete[] x;
	file.close();
}

//void SFMResult::  StartOracle(){
//
//	memoOracleCallTime = clock();
//	oracleCall++;
//}
//
//void SFMResult::  StopOracle(){
//	oracleTime += clock() - memoOracleCallTime;
//}
//
//void SFMResult::  StartBase(){
//	memoBaseCallTime = clock();
//	baseCall++;
//}
//
//void SFMResult::  StopBase(){
//	oracleTime += clock() - memoBaseCallTime;
//}
//
//void SFMResult::  StartReduction(){
//	memoReductionTime = clock();
//	reductionCall++;
//}
//
//void SFMResult::  StopReduction(){
//	reductionTime += clock() - memoReductionTime;
//}

string SFMResult:: Minimizer(){
	return minimizer;
}

double SFMResult::  MinimumValue(){
	return minimumValue;
}

void SFMResult::  X(double* &x){
	if(SFMResult::x==NULL){
		delete[]x;
		x = NULL;
	}else{
		for(int i=0;i<n;i++){
			x[i] = SFMResult::x[i];
		}
	}
}

long SFMResult::  Iteration(){
	return iteration;
}

long SFMResult::  OracleCall(){
	return oracleCall;
}

long SFMResult::  BaseCall(){
	return baseCall;
}

long SFMResult::  ReductionCall(){
	return reductionCall;
}

long SFMResult::  ExecutionTime(){
	return executionTime;
}

long SFMResult::  ReductionTime(){
	return reductionCall;
}

long SFMResult::  OracleTime(){
	return oracleTime;
}

double SFMResult:: DualValue(){
	return dualValue;
}

int SFMResult::  N(){
	return n;
}
