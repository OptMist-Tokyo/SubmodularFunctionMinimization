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
	TEST_CLASS(IsSubmodular)
	{

	private:
		static bool IsSubmodularTrivially(Manual &func)
		{
			int max = 1 << func.N();
			for (int i = 0; i < max; i++)
			{
				for (int j = i + 1; j < max; j++)
				{
					if (func.Value(i) + func.Value(j) < func.Value(i & j) + func.Value(i | j))
					{
						return false;
					}//if
				}//for j
			}//for i
			return true;
		}

		static bool RepeatMany(int n, int rep, int max)
		{
			bool res = true;
			double* array = new double[1 << n];
			for (int i = 0; i < rep; i++)
			{
				for (int k = 0; k < (1<<n); k++)
				{
					array[k] = rand()%max - rand()%max;
					//array[k] = rand.Next(max) - rand.Next(max);
				}//for k
				Manual func(n, array);
				bool want = IsSubmodular::IsSubmodularTrivially(func);
				bool ans = func.IsSubmodular();
				res &= (want == ans);
			}//for i
			delete[] array;
			return res;
		}


	public:

		TEST_METHOD(IsSubmodular0)
		{
			const int n = 3;
			double array[1<<n] = { 0, 0, 0, 0, 0, 0, 0, 0 };

			Manual func (n, array);
			Assert::AreEqual(IsSubmodular::IsSubmodularTrivially(func),func.IsSubmodular());
		}

		TEST_METHOD(IsSubmodular1)
		{
			const int n = 3;
			double array[1<<n] = { 0, 0, 0, 0, 0, 0, 0, 3 };

			Manual func (n, array);
			Assert::AreEqual(IsSubmodular::IsSubmodularTrivially(func),func.IsSubmodular());
		}

		TEST_METHOD(IsSubmodular2)
		{
			const int n = 3;
			double array[1<<n] = { 0, 2,3,4,6,-1,5,-10};

			Manual func (n, array);
			Assert::AreEqual(IsSubmodular::IsSubmodularTrivially(func),func.IsSubmodular());
		}

		TEST_METHOD(IsSubmodular3)
		{			
			const int nMin = 1;
			const int nMax = 9;
			const int rep = 10000;
			const int max = 10000;
			for (int i = nMin; i < nMax; i++)
			{
				bool res = RepeatMany(i, rep, max);
				Assert::AreEqual(true, res);                
			}//for i
		}

	};
}

#endif