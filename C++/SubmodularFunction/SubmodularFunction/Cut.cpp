
#include "stdafx.h"
#include "SubmodularFunction.h"


namespace Onigiri{

	Cut::Cut(std::string path):SubmodularOracle(0,0){ 
		std::ifstream file(path);	
		if(file.fail()) {
			std::cerr << path + " does not exist."<<std::endl;
			exit(0);
		}

		emptyValue = 0;
		int n;
		file>>n;
		Initialize(n);
		modular =new double[n];
		for(int i=0;i<n;i++){
			file>>modular[i];
		}
		weight = new double*[n];
		for(int i=0;i<n;i++){
			weight[i] = new double[n];
			for(int j=0;j<n;j++){
				file>>weight[i][j];
			}
		}
		file.close();

		contractedOutEdges =new double[n];
		contractedInEdges   =new double[n];
		deletedOutEdges	 	= new double[n];
		deletedInEdges 	 =new double[n];

		for(int i=0;i<n;i++){
			contractedOutEdges[i]= contractedInEdges[i] =deletedOutEdges[i] =deletedInEdges[i] = 0;
		}
		for (int i = 0; i < n; i++)           {
			for (int j = 0; j < n; j++)               {
				contractedOutEdges[i] += weight[i][j];
				deletedInEdges[i] += weight[j][i];
			}
		}
	}

	Cut::~Cut(){
		delete[]Cut::modular;
		for(int i=0;i<n;i++){
			delete[] Cut::weight[i];
		}
		delete[] Cut::weight;
		delete[] Cut::contractedOutEdges;
		delete[] Cut::contractedInEdges;
		delete[] Cut::deletedOutEdges;
		delete[] Cut::deletedInEdges;
	}

	void Cut::CalcBase(const int* order, double* base) {
		for(int i=0;i<count;i++){
			int index = order[i];
			int cur = remainder[index];


			//std::string str;
			//str +=std::to_string(remainder[index])+" ";
			//str +=std::to_string(modular[cur])+" ";
			//str +=std::to_string(contractedOutEdges[cur])+" ";
			//str +=std::to_string(contractedInEdges[cur])+" ";
			//std::cout<<str;


			base[index] = modular[cur]+ contractedOutEdges[cur] - contractedInEdges[cur];
			for(int j=0;j<i;j++){
				int v = remainder[order[j]];
				base[index] -= weight[cur][v] + weight[v][cur];
			}
		}
	}


	void Cut::Contract(std::vector<int> &list,int* reorder){
		std::sort(list.begin(),list.end());
		std::vector<int>::iterator iter = list.begin();
		int index = 0;
		for(int i=0;i<count;i++){
			if(iter!=list.end()&&i==*iter){
				int cur = remainder[i];
				contracted.push_back(cur);
				emptyValue += modular[cur];
				emptyValue -= contractedInEdges[cur];
				emptyValue +=contractedOutEdges[cur];
				for(int j=0;j<count;j++){
					int v = remainder[j];
					contractedInEdges[v] += weight[cur][v];
					contractedOutEdges[v] -= weight[v][cur];
				}
				//std::string str;
				//for(int k=0;k<n;k++){
				//	str+=std::to_string(contractedInEdges[k])+ " ";
				//}
				//for(int k=0;k<n;k++){
				//	str+=std::to_string(contractedOutEdges[k])+ " ";
				//}
				//std::cout<<str;
				iter++;
			}
		}

		 iter = list.begin();
		for(int i=0;i<count;i++){
			if(iter!=list.end()&&i==*iter){
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


	void Cut::Delete(std::vector<int> &list,int* reorder) {
		std::sort(list.begin(),list.end());
		std::vector<int>::iterator iter = list.begin();
		int index = 0;
		for(int i=0;i<count;i++){
			if(iter!=list.end()&&i==*iter){
				int cur = remainder[i];
				deleted.push_back(cur);
				for(int j=0;j<count;j++){
					int v = remainder[j];
					deletedInEdges[v] -= weight[cur][v];
					deletedOutEdges[v] += weight[v][cur];
				}
				iter++;
			}
		}

		 iter = list.begin();
		for(int i=0;i<count;i++){
			if(iter!=list.end()&&i==*iter){
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

	void Cut::Copy(SubmodularOracle *original){
		SubmodularOracle::CopyBase(original);
		Cut* cast = static_cast<Cut*>(original);
		emptyValue = cast->emptyValue;
		for(int i=0;i<n;i++){
			contractedOutEdges[i] =   cast-> contractedOutEdges[i];
			contractedInEdges[i]	=	cast->	contractedInEdges[i];
			deletedOutEdges[i]		=	cast->   deletedOutEdges[i];
			deletedInEdges[i]	=		cast->	  deletedInEdges[i];
		}
	}


}