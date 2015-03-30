#include "stdafx.h"
#include "CppUnitTest.h"
#include "SubmodularFunction.h"
#include <iostream>
#include <fstream>

using namespace Onigiri;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace UnitTest1
{		
	TEST_CLASS(UnitTest1)
	{
	public:

		TEST_METHOD(TestMethod1)
		{
			std::ofstream output(("E:\\Submodular\\res.txt"));	
			for(int i=1;i<=20;i++){
				for(int k=0;k<50;k++){				
					//if(i<6||k<5){
					//	continue;
					//}


					Cut cut("E:\\Submodular\\DataDouble\\DirectedCut\\" + std::to_string(i) + "_" + std::to_string(k) );
					Cut dummy("E:\\Submodular\\DataDouble\\DirectedCut\\"+ std::to_string(i) + "_" + std::to_string(k) );
					FW algo;
					std::vector<int> res;
					res = algo.Minimization(cut,dummy);
					
					std::ifstream file(("E:\\Submodular\\AnswersDirectedCut\\"+ std::to_string(i) + "_" + std::to_string(k) ));	
					std::string minimizer;
					file>>minimizer;
					std::sort(res.begin(),res.end());
					bool flg = true;
					int index = 0;
					for(int j=0;j<(int)minimizer.length();j++){
						if(index<(int)res.size()&&res[index]==j){
							flg &= (minimizer[j]=='1');
							index++;
						}
						else{
							flg &= (minimizer[j]=='0');
						}
					}

					
					output << (std::to_string(i) + "_" + std::to_string(k) + " "+ std::to_string(flg) )<< std::endl;
				}
			}


		}

		
		TEST_METHOD(TestMethod2)
		{

			std::ofstream output(("E:\\Submodular\\res.txt"));	
			//for(int i=1;i<=20;i++){
			for(int i=1;i<=1024;i*=2){
				for(int k=0;k<50;k++){				
					//if(i<16||k<9){
					//	continue;
					//}


					SetCoverConcave cover("E:\\Submodular\\DataDouble\\SetCoverConcave\\" + std::to_string(i) + "_" + std::to_string(k),100,0.5 );
					SetCoverConcave dummy("E:\\Submodular\\DataDouble\\SetCoverConcave\\"+ std::to_string(i) + "_" + std::to_string(k),100,0.5 );
					FW algo;
					std::vector<int> res;
					res = algo.Minimization(cover,dummy);
					
					std::ifstream file(("E:\\Submodular\\AnswersSetCoverConcave\\"+ std::to_string(i) + "_" + std::to_string(k) ));	
					std::string minimizer;
					file>>minimizer;
					std::sort(res.begin(),res.end());
					bool flg = true;
					int index = 0;
					for(int j=0;j<(int)minimizer.length();j++){
						if(index<(int)res.size()&&res[index]==j){
							flg &= (minimizer[j]=='1');
							index++;
						}
						else{
							flg &= (minimizer[j]=='0');
						}
					}

					
					output << (std::to_string(i) + "_" + std::to_string(k) + " "+ std::to_string(flg) )<< std::endl;
				}
			}


		}

	};
}