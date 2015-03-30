// SubmodularFunction.cpp : DLL アプリケーション用にエクスポートされる関数を定義します。
//

#include "stdafx.h"
#include "SubmodularFunction.h"

namespace Onigiri
{

	
	SetCoverConcave::SetCoverConcave(std::string path,double c,double pow):SubmodularOracle(c,pow){
		std::ifstream file(path);
		if(file.fail()) {
			std::cerr << path + " does not exist."<<std::endl;
			exit(0);
		}

		modularEmptyValue = 0;
		coverEmptyValue = 0;

		int n;
		file>>n;
		Initialize(n);
		modular =new double[n];
		for(int i=0;i<n;i++){
			file>>modular[i];
		}

		file>>m;
		used= new int[m];
		weight = new double[m];
		for(int i=0;i<m;i++){
			file>>weight[i];
			used[i] = 0;
		}

		length = new int[n];
		edges =new int*[n];
		for(int i=0;i<n;i++){
			file>>length[i];
			edges[i] =new int[length[i]];
			for(int j=0;j<length[i];j++){
				file>>edges[i][j];
			}
		}
		file.close();
	}

	SetCoverConcave:: ~SetCoverConcave(){
		delete[]modular;
		delete[] weight;
		delete[] length;
		delete[]used;
		for(int i=0;i<n;i++){
			delete[] edges[i];
		}
		delete[] edges;
	}

	double SetCoverConcave::CalcValue(double modular,double cover){
		return modular + c * pow(cover,alpha);
	}

	void SetCoverConcave::CalcBase(const int* order,double* base){
		double modularValue = modularEmptyValue;
		double coverValue = coverEmptyValue;
		double val = CalcValue(modularValue,coverValue);
		for(int i=0;i<count;i++){
			int cur = remainder[order[i]];
			modularValue+=modular[cur];
			for(int k=0;k<length[cur];k++){
				int next = edges[cur][k];
				if(used[next]==0){
					coverValue += weight[next];
				}
				used[next]++;
			}
			double nextVal = CalcValue(modularValue,coverValue);
			base[order[i]] = nextVal - val;
			val = nextVal;
		}
		for(int i=0;i<count;i++){
			int cur = remainder[order[i]];
			for(int k=0;k<length[cur];k++){
				int next = edges[cur][k];
				used[next]--;
			}
		}
	}

	
	void SetCoverConcave::Contract(std::vector<int> &list,int * reorder){

		

		std::sort(list.begin(),list.end());
		std::vector<int>::iterator iter = list.begin();
		int index = 0;
		for(int i=0;i<count;i++){
			if(iter!=list.end()&&i==*iter){
				int cur = remainder[i];
				contracted.push_back(cur);
				modularEmptyValue +=modular[cur];
				for(int j=0;j<length[cur];j++){
					int next = edges[cur][j];
					if(used[next]==0){
						coverEmptyValue +=weight[next];
					}
					used[next]++;
				}
				iter++;
			}
			else{
				remainder[index] = remainder[i];
				reorder[index++] = i;
			}
		}
		count -=list.size();
		cntContracted +=list.size();
	}



	void SetCoverConcave::Delete(std::vector<int> &list,int* reorder) {
		std::sort(list.begin(),list.end());
		std::vector<int>::iterator iter = list.begin();
		int index = 0;
		for(int i=0;i<count;i++){
			if(iter!=list.end()&&i==*iter){
				int cur = remainder[i];
				deleted.push_back(cur);
				iter++;
			}
			else{
				remainder[index] = remainder[i];
				reorder[index++] = i;
			}
		}
		count -=list.size();
		cntDeleted +=list.size();
	}

	void  SetCoverConcave::Copy(SubmodularOracle* original){
		SubmodularOracle::CopyBase(original);
		SetCoverConcave* cast = static_cast<SetCoverConcave*>(original);
		modularEmptyValue = cast->modularEmptyValue;
		coverEmptyValue = cast->coverEmptyValue;
		for(int i=0;i<m;i++){
			used[i] = cast->used[i];
		}		
	}
	



}


