#include "stdafx.h"
#include "CppUnitTest.h"
#include <iostream>
#include "Submodular.h"
#include <random>
#include <string>
#include <stdio.h>

#ifdef _DEBUG
#include "Const.h"

using namespace std;
using namespace OnigiriSubmodular;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace TestSubmodular
{		
	TEST_CLASS(CalcBaseOverride)
	{

	private: 

		static const int nMin = 1;
		static const int nMax = 70;
		static const int kMin = 0;
		static const int kMax = 24;


		static void TestCalcBase(void (*action)(int n,int k,const string &methodNamee,int *order) ,string methodName)
		{
			for (int n = nMin; n < nMax; n++)
			{
				int* order0 = new int[n];
				int* order1 = new int[n];
				for(int i=0;i<n;i++){
					order0[i] = i;
					order1[i] = n-1-i;
				}
				int* order2 = new int[n];
				int pos = 0;
				for(int i=0;i<n;i+=2){
					order2[pos++] = i;
				}
				for(int i=1;i<n;i+=2){
					order2[pos++] = i;
				}
				for (int k = kMin; k <= kMax; k++)
				{
					action(n, k, methodName,order0);
					action(n, k,methodName, order1);
					action(n, k,methodName, order2);
				}//for k
				delete[]order0;
				delete[]order1;
				delete[]order2;
			}//for n
		}



	public:	

		static void DelegateModular(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			Modular func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideModular)
		{
			string methodName("Modular");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateModular;
			TestCalcBase(action,methodName);
		}

		static void DelegateUndirectedCut(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			UndirectedCut func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideUndirectedCut)
		{
			string methodName("UndirectedCut");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateUndirectedCut;
			TestCalcBase(action,methodName);
		}

		static void DelegateDirectedCut(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			DirectedCut func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideDirectedCut)
		{
			string methodName("DirectedCut");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateDirectedCut;
			TestCalcBase(action,methodName);
		}

		static void DelegateConnectedDetachment(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			ConnectedDetachment func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideConnectedDetachment)
		{
			string methodName("ConnectedDetachment");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateConnectedDetachment;
			TestCalcBase(action,methodName);
		}

		static void DelegateFacilityLocation(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			FacilityLocation func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideFacilityLocation)
		{
			string methodName("FacilityLocation");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateFacilityLocation;
			TestCalcBase(action,methodName);
		}

		static void DelegateGraphicMatroid(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			GraphicMatroid func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideGraphicMatroid)
		{
			string methodName("GraphicMatroid");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateGraphicMatroid;
			TestCalcBase(action,methodName);
		}
		
		static void DelegateBinaryMatrixRank(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			BinaryMatrixRank func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideBinaryMatrixRank)
		{
			string methodName("BinaryMatrixRank");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateBinaryMatrixRank;
			TestCalcBase(action,methodName);
		}
		
		static void DelegateNonPositiveSymmetricMatrixSummation(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			NonPositiveSymmetricMatrixSummation func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideNegativeSymmetricMatrixSummation)
		{
			string methodName("NegativeSymmetricMatrixSummation");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateNonPositiveSymmetricMatrixSummation;
			TestCalcBase(action,methodName);
		}
		
		static void DelegateSetCover(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			SetCover func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideSetCover)
		{
			string methodName("SetCover");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateSetCover;
			TestCalcBase(action,methodName);
		}

		static void DelegateSetCoverConcave(int n,int k,const string &methodName,int* order){
			string path(DataPrefix + methodName+ "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
			SetCoverConcave func(path);
			func.TestCalcBase(order);
		}

		TEST_METHOD(CalcBaseOverrideSetCoverConcave)
		{
			string methodName("SetCoverConcave");
			void (*action)(int n,int k,const string &methodNamee,int *order) = DelegateSetCoverConcave;
			TestCalcBase(action,methodName);
		}



	};
}

#endif