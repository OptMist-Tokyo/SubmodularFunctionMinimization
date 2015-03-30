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
	TEST_CLASS(IsSubmodularOverride)
	{

	private: 
		
        static const int nMin = 1;
       static const int nMax = 10;
       static const int kMin = 0;
       static const int kMax = 24;



		void CheckSubmodular(void (*action)(int n,int k,const string &methodName),const string &methodName)
        {
            for (int n = nMin; n < nMax; n++)
            {
                for (int k = kMin; k < kMax; k++)
                {
                    (*action)(n, k,methodName);
                }//for k
            }//for n
        }
		
	public:	
		
		static void DelegateModular(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				Modular func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideModular)
        {
            string methodName("Modular");
			void (*action)(int n,int k,const string &methodName) = &DelegateModular;
            CheckSubmodular(action,methodName);
        }
	
		static void DelegateUndirectedCut(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				UndirectedCut func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideUndirectedCut)
        {
            string methodName("UndirectedCut");
			void (*action)(int n,int k,const string &methodName) = &DelegateUndirectedCut;
            CheckSubmodular(action,methodName);
        }

			
		static void DelegateDirectedCut(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				DirectedCut func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideDirectedCut)
        {
            string methodName("DirectedCut");
			void (*action)(int n,int k,const string &methodName) = &DelegateDirectedCut;
            CheckSubmodular(action,methodName);
        }
			
		static void DelegateConnectedDetachment(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				ConnectedDetachment func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideConnectedDetachment)
        {
            string methodName("ConnectedDetachment");
			void (*action)(int n,int k,const string &methodName) = &DelegateConnectedDetachment;
            CheckSubmodular(action,methodName);
        }
	
		static void DelegateFacilityLocation(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				FacilityLocation func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideFacilityLocation)
        {
            string methodName("FacilityLocation");
			void (*action)(int n,int k,const string &methodName) = &DelegateFacilityLocation;
            CheckSubmodular(action,methodName);
        }

		static void DelegateGraphicMatroid(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				GraphicMatroid func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideGraphicMatroid)
        {
            string methodName("GraphicMatroid");
			void (*action)(int n,int k,const string &methodName) = &DelegateGraphicMatroid;
            CheckSubmodular(action,methodName);
        }

		static void DelegateBinaryMatrixRank(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				BinaryMatrixRank func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideBinaryMatrixRank)
        {
            string methodName("BinaryMatrixRank");
			void (*action)(int n,int k,const string &methodName) = &DelegateBinaryMatrixRank;
            CheckSubmodular(action,methodName);
        }

		static void DelegateNonPositiveSymmetricMatrixSummation(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				NonPositiveSymmetricMatrixSummation func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideNegativeSymmetricMatrixSummation)
        {
            string methodName("NegativeSymmetricMatrixSummation");
			void (*action)(int n,int k,const string &methodName) = &DelegateNonPositiveSymmetricMatrixSummation;
            CheckSubmodular(action,methodName);
        }

		static void DelegateSetCover(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				SetCover func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideSetCover)
        {
            string methodName("SetCover");
			void (*action)(int n,int k,const string &methodName) = &DelegateSetCover;
            CheckSubmodular(action,methodName);
        }

		static void DelegateSetCoverConcave(int n,int k,const string &methodName){
                string path(DataPrefix + methodName + "\\" + Converter::IntToString(n) + "_" + Converter::IntToString(k));
				SetCoverConcave func(path);
                Assert::AreEqual(true, func.IsSubmodular());
		}

        TEST_METHOD(IsSubmodularOverrideSetCoverConcave)
        {
            string methodName("SetCoverConcave");
			void (*action)(int n,int k,const string &methodName) = &DelegateSetCoverConcave;
            CheckSubmodular(action,methodName);
        }


	};
}

#endif