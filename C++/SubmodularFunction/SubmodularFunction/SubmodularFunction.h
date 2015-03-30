#ifdef SubmodularFunctionDLL_EXPORTS
#define SubmodularFunctionDLL_API __declspec(dllexport) 
#else
#define SubmodularFunctionDLL_API __declspec(dllimport) 
#endif


#include <stdexcept>
#include <string>
#include <vector>
#include <iostream>
#include <fstream>
#include <algorithm>

namespace Onigiri
{

	class SubmodularOracle
	{
	private:
		long time;
	protected:
		int n;
		double c;
		double alpha;
		int cntContracted;
		int cntDeleted;
		int count;
		int* remainder;
		std::vector<int> contracted;
		std::vector<int> deleted;
		SubmodularOracle(double c, double pow);
		void Initialize(int n);
		void CopyBase(SubmodularOracle *original);
	public : 
		SubmodularFunctionDLL_API int N();
		SubmodularFunctionDLL_API int Count();
		SubmodularFunctionDLL_API int CountContracted();
		SubmodularFunctionDLL_API int CountDeleted();
		SubmodularFunctionDLL_API int* Remainder();
		SubmodularFunctionDLL_API std::vector<int> ContractedList();
		SubmodularFunctionDLL_API std::vector<int> DeletedList();
		SubmodularFunctionDLL_API virtual ~SubmodularOracle();
		SubmodularFunctionDLL_API virtual void CalcBase(const int* order,double* base) = 0;
		SubmodularFunctionDLL_API virtual void Delete(std::vector<int> &list,int* reorder) = 0;
		SubmodularFunctionDLL_API virtual void Contract(std::vector<int> &list,int* reorder) = 0;
		SubmodularFunctionDLL_API virtual void Copy(SubmodularOracle *original) = 0;
	};

	class SetCoverConcave:public SubmodularOracle{
	private:
		int m;
		double modularEmptyValue;
		double coverEmptyValue;
		double* modular;
		double* weight;
		int* length;
		int** edges;
		int* used;
		double CalcValue(double modular,double cover);
	public:
		SubmodularFunctionDLL_API SetCoverConcave(std::string path,double c,double pow);
		SubmodularFunctionDLL_API ~SetCoverConcave();
		SubmodularFunctionDLL_API virtual void CalcBase(const int* order,double* base) final override;
		SubmodularFunctionDLL_API virtual void Delete(std::vector<int> &list, int* reorder) final override;
		SubmodularFunctionDLL_API virtual void Contract(std::vector<int> &list, int* reorder) final override;
		SubmodularFunctionDLL_API virtual void Copy(SubmodularOracle *original) final override;
	};

	class Cut: public SubmodularOracle{
	private:
		double emptyValue;
		double* modular;
		double** weight;
		double* contractedOutEdges;
		double* contractedInEdges;
		double* deletedOutEdges;
		double* deletedInEdges;
	public:
		SubmodularFunctionDLL_API Cut(std::string path);
		SubmodularFunctionDLL_API ~Cut();
		SubmodularFunctionDLL_API virtual void CalcBase(const int* order, double* base) final override;
		SubmodularFunctionDLL_API virtual void Contract(std::vector<int> &list,int* reorder) final override;
		SubmodularFunctionDLL_API virtual void Delete(std::vector<int> &list,int* reorder) final override;
		SubmodularFunctionDLL_API virtual void Copy(SubmodularOracle *original) final override;
	};

	
	class FW
	{
	private:
		int* order;
		int* reorder;
		double* base;
		double* sortX;
		double* bestLambdas;
		int* inverse;
		int* cnt;
		bool* used;
		void ExecuteWolfeAlgorithm(SubmodularOracle  *oracle,int &numComponents, double* x,std::vector<double>& lambdas,std::vector<int*> &orders,std::vector<double*> &bases,std::vector<double*> &Q,std::vector<double*> &R, long smallReputation,int type);
		void SetInitialBase(SubmodularOracle  *oracle,int &numComponents, double* x,std::vector<double>& lambdas,std::vector<int*> &orders,std::vector<double*> &bases,std::vector<double*> &Q,std::vector<double*>&R);
	protected:
	public:
		SubmodularFunctionDLL_API FW();
		SubmodularFunctionDLL_API ~FW();
		SubmodularFunctionDLL_API std::vector<int> Minimization(SubmodularOracle  &oracle,SubmodularOracle &dummy);

	};

}