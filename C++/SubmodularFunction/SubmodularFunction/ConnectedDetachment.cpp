#include "Submodular.h"


using namespace OnigiriSubmodular;

void ConnectedDetachment::SetVariables(int n, double* modular,map<int,bool>* edges){
	map<int,bool>::iterator itr;
	ConnectedDetachment::n = n;
	ConnectedDetachment::modular = new double[n];
	ConnectedDetachment::edges = new map<int,bool>[n];
	for(int i=0;i<n;i++){
		ConnectedDetachment::modular[i] = modular[i];
		ConnectedDetachment::edges[i] = *new map<int,bool>();
		for(itr = edges[i].begin();itr!=edges[i].end();itr++)
		{
			int u = itr->first;
			ConnectedDetachment::edges[i][u] = true;
		}//foreach u
	}
	ConnectedDetachment::fOfEmpty = 0;

	uf =new UnionFind(n);
    used=new bool[n];
    numComplement=new int[n];
}

ConnectedDetachment::ConnectedDetachment(int n,double* modular,map<int,bool>* edges){
	SetVariables(n,modular,edges);
}

ConnectedDetachment::ConnectedDetachment(string path){ 
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

	map<int,bool>* edges = new map<int,bool>[n];
	for(int i=0;i<n;i++){
		int cnt;
		file>>cnt;
		edges[i] = map<int,bool>();
		for(int j=0;j<cnt;j++){
			int cur;
			file>>cur;
			edges[i][cur] = true;
		}
	}
	file.close();
	SetVariables(n,modular,edges);
	delete[]modular;
	for(int i=0;i<n;i++){
		edges[i].clear();
	}
	delete[] edges;
}

ConnectedDetachment::~ConnectedDetachment(){
	delete[]ConnectedDetachment::modular;
	for(int i=0;i<n;i++){
		edges[i].clear();
	}
	delete[] ConnectedDetachment::edges;

	delete uf;
	delete[]used;
	delete[]numComplement;
}


double ConnectedDetachment::Value(const int* order,int cardinality){
	double res = 1;
	for (int i = 0; i < cardinality; i++)
	{
		res -= modular[order[i]];
	}//for i
	res += CountIncidentEdge(order, cardinality);
	res -= CountComponentOfComplementGraph(order, cardinality);
	return res;
}


void ConnectedDetachment::Base(const int* order,double* base){
	map<int,bool>::iterator itr;
	SetArrayOfComplementConnectedComponent(order);
	SetUsed();
	for (int i = 0; i < n; i++)
	{
		int v = order[i];
		double cur = 0;
		cur -= modular[v];

		for(itr = edges[v].begin();itr!=edges[v].end();itr++)
		{
			if (!used[itr->first])
			{
				cur++;
			}//if
		}//foreach u
		cur -= numComplement[v] - (i == 0 ? 1 : numComplement[order[i - 1]]);
		used[v] = true;
		base[v] = cur;
	}//for i
}

void ConnectedDetachment::SetArrayOfComplementConnectedComponent(const int*order)
{
	map<int,bool>::iterator itr;
	SetUsed();
	uf->Clear();
	int component = 0;
	for(int i=0;i<n;i++){
		numComplement[i] = 0;
	}
	for (int i = n - 1; i > 0; i--)
	{
		component++;
		int v = order[i];
		for(itr = edges[v].begin();itr!=edges[v].end();itr++)
		{
			int u = itr->first;
			if (used[u]&&!uf->Same(v,u))
			{
				component--;
				uf->Unite(v, u);
			}//if
		}//foreach u
		used[v] = true;
		numComplement[order[i-1]] = component;
	}//forrev i
}

int ConnectedDetachment::CountIncidentEdge(const int* order, int cardinality)
{
	map<int,bool>::iterator itr;
	SetUsed();
	int res = 0;
	for (int i = 0; i < cardinality; i++)
	{
		int v = order[i];
		for(itr = edges[v].begin();itr!=edges[v].end();itr++)
		{
			if (!used[ itr->first])
			{
				res++;
			}//if
		}//foreach u
		used[v] = true;
	}//for v
	return res;
}

int ConnectedDetachment::CountComponentOfComplementGraph(const int* order, int cardinality)
{
	map<int,bool>::iterator itr;
	uf->Clear();
	int res = n - cardinality;
	SetUsed();
	for (int i = cardinality; i < n; i++)
	{
		used[order[i]] = true;
	}//for i
	for (int i = cardinality; i < n; i++)
	{
		int v = order[i];
		for(itr = edges[v].begin();itr!=edges[v].end();itr++)
		{
			int u = itr->first;
			if (used[u]&&!uf->Same(order[i],u))
			{
				uf->Unite(order[i], u);
				res--;
			}//if
		}//foreach u
	}//for i
	return res;
}

void ConnectedDetachment::SetUsed(){
	for(int i=0;i<n;i++){
		used[i] = false;
	}
}


#if _DEBUG

void ConnectedDetachment::TestCalcBase(const int* order){
	double* b0 = new double[n];
	double* b1 = new double[n];
	ConnectedDetachment::CalcBase(order,b0);
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