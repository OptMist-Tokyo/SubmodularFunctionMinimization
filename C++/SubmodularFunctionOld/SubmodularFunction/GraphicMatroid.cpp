#include "Submodular.h"

using namespace OnigiriSubmodular;


void GraphicMatroid::SetVariables(int n,int V, double* modular, int* heads,int* tails){
	GraphicMatroid::n = n;
	GraphicMatroid::V = V;
	GraphicMatroid::modular = new double[n];
	GraphicMatroid::heads = new int[n];
	GraphicMatroid::tails = new int[n];
	for(int i=0;i<n;i++){
		GraphicMatroid::modular[i]= modular[i];
		GraphicMatroid::heads[i] = heads[i];
		GraphicMatroid::tails[i] =tails[i];
	}
	GraphicMatroid::fOfEmpty = 0;

	uf = new UnionFind(V);
}

GraphicMatroid::GraphicMatroid(int n,int V,double* modular,int* heads, int*tails){
	SetVariables(n,V,modular,heads,tails);
}

GraphicMatroid::GraphicMatroid(string path){
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

	file>>V;
	int* heads = new int[n];
	int* tails = new int[n];
	for(int i=0;i<n;i++){
		file>>heads[i];
		file>>tails[i];
	}

	file.close();
	SetVariables(n,V,modular,heads,tails);
	delete[]modular;
	delete[] heads;
	delete[]tails;
}

GraphicMatroid::~GraphicMatroid(){
	delete[] modular;
	delete[]heads;
	delete[]tails;

	delete uf;
}

double GraphicMatroid::Value(const int* order,int cardinality){
	double res = cardinality;
	uf->Clear();
	for (int i = 0; i < cardinality; i++)
	{
		res += modular[order[i]];
		if (uf->Same(heads[order[i]],tails[order[i]]))
		{
			res--;
		}//if
		else
		{
			uf->Unite(heads[order[i]], tails[order[i]]);
		}//else
	}//for i
	return res;
}

void GraphicMatroid::Base(const int* order, double* base){
	uf->Clear();
	for (int i = 0; i < n; i++)
	{
		base[order[i]] = modular[order[i]];
		if (!uf->Same(heads[order[i]], tails[order[i]]))
		{
			uf->Unite(heads[order[i]], tails[order[i]]);
			base[order[i]]++;
		}//if
	}//for i
}


#if _DEBUG
void GraphicMatroid::TestCalcBase(const int* order){
	double* b0 = new double[n];
	double* b1 = new double[n];
	GraphicMatroid::CalcBase(order,b0);
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