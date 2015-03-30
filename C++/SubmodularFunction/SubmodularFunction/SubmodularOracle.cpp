// SubmodularFunction.cpp : DLL アプリケーション用にエクスポートされる関数を定義します。
//

#include "stdafx.h"
#include "SubmodularFunction.h"


namespace Onigiri
{
	int SubmodularOracle::N(){
		return n;
	}

	int SubmodularOracle::Count(){
		return count;
	}

	int* SubmodularOracle::Remainder(){
		return remainder;
	}
		
	SubmodularOracle::SubmodularOracle(double c,double pow){
		SubmodularOracle::c = c;
		SubmodularOracle::alpha = pow;
		contracted.clear();
		deleted.clear();
	}

	void SubmodularOracle::Initialize(int n){
		SubmodularOracle::n  =n;
		SubmodularOracle::count = n;
		cntContracted = 0;
		cntDeleted = 0;
		SubmodularOracle::remainder = new int[n];
		for(int i=0;i<n;i++){
			remainder[i] = i;
		}
	}

	SubmodularOracle::~SubmodularOracle(){
		delete[] remainder;
	}

	
	void SubmodularOracle::CopyBase(SubmodularOracle* original){
		n = original->N();
		cntContracted = original->cntContracted;
		cntDeleted = original->cntDeleted;
		count = original->Count();
		c = original ->c;
		alpha = original -> alpha;
		for(int i=0;i<count;i++){
			remainder[i] = original->remainder[i];
		}		
	}
	

	std::vector<int> SubmodularOracle::DeletedList(){
		return deleted;
	}

	std::vector<int> SubmodularOracle::ContractedList(){
		return contracted;
	}


}


