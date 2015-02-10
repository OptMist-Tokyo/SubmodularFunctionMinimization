
#include "Submodular.h"

using namespace OnigiriSubmodular;

SubmodularOracle::SubmodularOracle()
{
	fOfEmpty = 0;
	oracleCall = 0;
	oracleTime  = 0;
	baseCall = 0;
}


SubmodularOracle::~SubmodularOracle(){
}

int SubmodularOracle::N()
{
	return n;
}

double SubmodularOracle::CalcValue(const int* order,int cardinality)
{
	memoTime = clock();
	oracleCall++;
	double res = Value(order,cardinality);
	oracleTime+=clock()-memoTime;
	return res;
}

void SubmodularOracle::CalcBase(const int* order,double* base)
{
	memoTime = clock();
	baseCall++;
	Base(order,base);
	oracleTime+=clock()-memoTime;
}
 

void SubmodularOracle::Base(const int* order,double* base)
{
	double sum = fOfEmpty;
	for (int i = 0; i < n; i++)
	{
		double curValue = Value(order, i+1);
		base[order[i]] = curValue - sum;    
		sum = curValue;
	}//for i
}


static void Swap(int* order, int x, int y)
{
	int tmp = order[x];
	order[x] = order[y];
	order[y] = tmp;
}


bool SubmodularOracle::IsSubmodular()
{
	int* order = new int[1<<n];
	for (int mask = 0; mask < (1<<n); mask++)
	{
		int usedBit;
		GetOrder(n, mask,usedBit,order);
		for (int i = usedBit; i < n; i++)
		{
			Swap(order, usedBit, i);
			double a =CalcValue(order, usedBit);
			double b =CalcValue(order, usedBit+1);
			for (int j = i+1; j < n; j++)
			{
				Swap(order, usedBit + 1, j);
				double d = CalcValue(order, usedBit + 2);
				Swap(order, usedBit, usedBit + 1);
				double c = CalcValue(order, usedBit+1);
				Swap(order, usedBit, usedBit + 1);
				if (b+c<a+d-1e-9)
				{
					delete[] order;
					return false;
				}//if
				Swap(order, usedBit + 1, j);
			}//for j
			Swap(order, usedBit, i);
		}//for i
	}//for mask
	delete[]order;
	return true;
}

void SubmodularOracle::GetOrder( int n,int mask, int &usedBit,int* order)
{
	usedBit = 0;
	for (int i = 0; i < n; i++)
	{
		if ((mask >> i & 1) == 1)
		{
			order[usedBit++] = i;
		}//if
		else
		{
			order[n - 1 - (i - usedBit)] = i;
		}//else
	}//for i
}

void SubmodularOracle::GetOrder( int n,string &mask, int &usedBit,int* order)
{
	usedBit = 0;
	for (int i = 0; i < n; i++)
	{
		if (mask[i]=='0')
		{
			order[n - 1 - (i - usedBit)] = i;
		}//if
		else
		{
			order[usedBit++] = i;
		}//else
	}//for i
}