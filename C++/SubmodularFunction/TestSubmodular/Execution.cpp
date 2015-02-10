#include "stdafx.h"
#include "CppUnitTest.h"
#include <iostream>
#include "Submodular.h"
#include <time.h>
#include <direct.h>
#include <fstream>
#include "Hybrid.h"
#include "FW.h"
#include "Const.h"

using namespace std;
using namespace OnigiriSubmodular;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace TestSubmodular
{		
	TEST_CLASS(Execution)
	{

	private:
		string oracles[MAXLEN]; 
		string algorithms [MAXLEN];

		void SetOracles()
		{
			for(int i=0;i<MAXLEN;i++){
				oracles[i] = "";
			}
	//		oracles[0] = "Modular";
			oracles[1] = "UndirectedCut";
		/*	oracles[2] = "DirectedCut";
			oracles[3] = "ConnectedDetachment";
			oracles[4] = "FacilityLocation";
			oracles[5] = "GraphicMatroid";
			oracles[6] = "BinaryMatrixRank";
			oracles[7] = "NonPositiveSymmetricMatrixSummation";
			oracles[8] = "SetCover"*/;
		}

		void SetAlgorithms()
		{
			for(int i=0;i<MAXLEN;i++){
				algorithms[i] = "";
			}
			//algorithms[0] = "BruteForce";
			algorithms[1] = "Hybrid";
			//algorithms[2] = "FW";
		}

		SubmodularFunctionMinimization* GetAlgo(string algoName)
		{
			if(algoName=="BruteForce"){
				BruteForce* algo = new BruteForce();
				return algo;
			}else if(algoName=="Hybrid"){
				Hybrid* algo = new Hybrid();
				return algo;
			}else if(algoName=="FW"){
				FW* algo = new FW();
				return algo;
			}else {
			exit(0);
			}
		}


		SubmodularOracle* GetOracle(string oracleName, int n, int k)
		{
			string path = DataPrefix + oracleName+ "\\" +Converter::IntToString(n) + "_" + Converter::IntToString(k);
			if(oracleName=="Modular"){
				Modular* oracle =new Modular(path);
				return oracle;
			}else if(oracleName=="UndirectedCut"){
				UndirectedCut* oracle = new UndirectedCut(path);
				return oracle;
			}else if(oracleName=="DirectedCut"){
				DirectedCut* oracle = new DirectedCut(path);
				return oracle;
			}else if(oracleName=="ConnectedDetachment"){
				ConnectedDetachment* oracle = new ConnectedDetachment(path);
				return oracle;
			}else if(oracleName=="FacilityLocation"){
				FacilityLocation* oracle = new FacilityLocation(path);
				return oracle;
			}else if(oracleName=="GraphicMatroid"){
				GraphicMatroid* oracle = new GraphicMatroid(path);
				return oracle;
			}else if(oracleName=="BinaryMatrixRank"){
				BinaryMatrixRank* oracle = new BinaryMatrixRank(path);
				return oracle;
			}else if(oracleName=="NonPositiveSymmetricMatrixSummation"){
				NonPositiveSymmetricMatrixSummation* oracle = new NonPositiveSymmetricMatrixSummation(path);
				return oracle;
			}else if(oracleName=="SetCover"){
				SetCover* oracle =new SetCover(path);
				return oracle;
			}else {
				exit(0);
			}

			//int n =3;
			//string path0;string path1;
			//double* modular;
			//double** matrix;
			//
			//	UndirectedCut* oracle0 = new UndirectedCut(path0);				
			//	SetCover* oracle1 =new SetCover(path1);
			//	FacilityLocation* oracle2 = new FacilityLocation(n,modular,matrix);
			    //            SubmodularOracle* oracle = GetOracle(oracleName, n, k);


							//Hybrid* algo0 = new Hybrid();
							//BruteForce* algo1 = new BruteForce();
							//SFMResult sfmResult0 = algo0->Minimization(oracle);
							//SFMResult sfmResult1 = algo1->Minimization(oracle);

		}

		string GetTime(){
			time_t timer;
			struct tm local;
			timer = time(NULL);
			localtime_s(&local,&timer);
			string time = "";
			time+=Converter::IntToString( local.tm_year + 1900);
			time+=+"_"+Converter::IntToString( local.tm_mon + 1);
			time+="_"+Converter::IntToString( local.tm_mday);
			time+="_"+Converter::IntToString(local.tm_hour);
			time+="_"+Converter::IntToString (local.tm_min);
			time+="_"+Converter::IntToString( local.tm_sec);
			time+="_"+Converter::IntToString( local.tm_isdst);
			return time;
		}

		
        void CheckResult(SFMResult &result, int n, int k, string oracle)
        {
            string path = AnsPath+ oracle + "\\" + Converter::IntToString(n)+ "_" +Converter::IntToString(k);
			
            string minimizer;     string minimumValue;
			ifstream file(path);
			file>>minimizer;
			file>>minimumValue;
            bool ok = (std::stod(minimumValue) == result.MinimumValue()) || (minimizer == result.Minimizer());
			Assert::AreEqual(true,ok);
			file.close();
        }

	public:

		TEST_METHOD(ExecutionAll)
		{
			const int nMin = 100;
			const int nMax = 100;
			const int kMin = 0;
			const int kMax = 10;

			SetOracles();
			SetAlgorithms();
			string time = GetTime();
			string directly =  ResultPrefix + time;
			_mkdir(directly.c_str());

			for(int i=0;i<MAXLEN;i++)
			{
				string oracleName = oracles[i];
				for(int j=0;j<MAXLEN;j++)
			    {
					string algoName = algorithms[j];
					if(oracleName==""||algoName ==""){
						continue;
					}
			        string path = directly + "\\" + oracleName+ "_" + algoName+ ".txt";
			        for (int n = nMin; n <= nMax; n++)
			        {
			            for (int k = kMin; k < kMax; k++)
			            {
							if(k==0){
								continue;
							}

			                SubmodularOracle* oracle = GetOracle(oracleName, n, k);
			                SubmodularFunctionMinimization* algo = GetAlgo(algoName);
			                SFMResult result =  algo->Minimization(oracle);
							delete oracle;
							delete algo;
							ofstream file(path,ios_base::app);
							result.Output(path);
							file.close();
//							Execution::CheckResult(result,n,k,oracleName);
			            }//for k
			        }//for n
			    }//foreach algoName
			}//foreach item
		}

		


	};
}
