#include "stdafx.h"
#include "CppUnitTest.h"
#include <iostream>
#include "Submodular.h"
#include <random>

#ifdef _DEBUG

using namespace std;
using namespace OnigiriSubmodular;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace TestSubmodular
{		
	TEST_CLASS(CalcBase)
	{

	private:
        static void ConvertModularToManual(int n, Modular &func,Manual &res)
        {
            double* values = new double[1 << n];
			int*order = new int[n];
            for (int mask = 0; mask < (1 << n); mask++)
            {
                int usedBit;
                SubmodularOracle::GetOrder(n, mask, usedBit,order);
                values[mask] = func.CalcValue(order, usedBit);
            }//for mask
			Manual tmp(n, values); 
			res.Copy(tmp);
			delete []order;
			delete[] values;
        }

	public:
		
		TEST_METHOD(CalcBase0)
		{
            const int n = 3;
            double array[n]={9,89,7};
            int order [n]= {0,1,2};

			double* b0 = new double[n];
			double* b1 = new double[n];

            Modular func(n, array);
			Manual manual(0,0);
            ConvertModularToManual(n, func,manual);
            func.CalcBase(order,b0);
            manual.CalcBase(order,b1);
			bool res = (memcmp(b0,b1,n)!=0);
			delete[] b0;
			delete[]b1;
            Assert::AreEqual(true,res);
		}

		TEST_METHOD(CalcBase1)
		{
            const int nMin = 1;
            const int nMax = 10;
            const int max = 10000;

            for (int i = nMin; i < nMax; i++)
            {
                int* order = new int[i];
				double* array = new double[i];
				double* b0 = new double[i];
				double* b1 = new double[i];
				for(int j=0;j<i;j++){
					order[j] = j;
					array[j] = rand()%max-rand()%max;
				}
                Modular func(i, array);
                Manual manual(0,0);
				ConvertModularToManual(i,  func,manual);
                func.CalcBase(order,b0);
                manual.CalcBase(order,b1);
				bool res = (memcmp(b0,b1,i)!=0);
                Assert::AreEqual(true,res);
				delete[]order;
				delete[] array;
				delete[]b0;
				delete[]b1;
            }//for i
		}
			

	};
}

#endif