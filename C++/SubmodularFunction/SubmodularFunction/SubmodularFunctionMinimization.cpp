#include "Submodular.h"

using namespace OnigiriSubmodular;

SubmodularFunctionMinimization::SubmodularFunctionMinimization(){
	result = new SFMResult(0);
}

SubmodularFunctionMinimization::~SubmodularFunctionMinimization(){
	delete result;
}

int SubmodularFunctionMinimization::N(){
	return oracle->N();
}

SFMResult SubmodularFunctionMinimization::Result(){
	return *result;
}

//double SubmodularFunctionMinimization::CalcValue(int* order,int cordinality){
//	result->StartOracle();
//	double res = oracle->CalcValue(order,cordinality);
//	result->StopOracle();
//	return res;
//}
//
//void SubmodularFunctionMinimization::CalcBase(int*order,double* base){
//	result->StartBase();
//	oracle->CalcBase(order,base);
//	result->StopBase();
//}

void SubmodularFunctionMinimization::SetOracle(SubmodularOracle *oracle){
	SubmodularFunctionMinimization::oracle = oracle;
	delete result;
	SubmodularFunctionMinimization::result = new SFMResult(oracle->N());
}

//void SubmodularFunctionMinimization::SetResult(double* x,long iteration){
//	string minimizer = "";
//	for(int i=0;i<N();i++){
//		minimizer+=(x[i]<=0?"1":"0");
//	}
//	SetResult(x,minimizer,iteration);
//}

void SubmodularFunctionMinimization::SetResult(double* x,string &minimizer,long iteration){
	int usedBit = 0;
	int* order =new int[N()];
	SubmodularOracle::GetOrder(N(),minimizer,usedBit,order);
	double minimumValue = oracle->CalcValue(order,usedBit);
	result->SetResult(x,minimizer,minimumValue,iteration,*oracle);
	delete[]order;
}
