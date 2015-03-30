#include "stdafx.h"
#include "SubmodularFunction.h"

namespace Onigiri
{
	const int contractType = 1;
	const int deleteType  =2;
	const int anyType  =3;
	const double Eps = 1e-10;
	const double CutRatio  = 0.35;
	const int naiveSize = 5;
     const double RepRatio = 0.02;
	 const double RepRatioSmall = 0.01;
        const double CutRatioSmall = 0.20;
	 

	FW::FW()
	{
	}

	FW::~FW()
	{
	}

	void 
		swap2 (int *s,int i,int j)
		//int *s;
		//int i;
		//int j;
	{
		int temp;

		/* temp = A[i];
		A[i]  = A[j];
		A[j] = temp;*/

		temp = s[i];
		s[i] = s[j];
		s[j] = temp;

	}

	int partition2(double* A,int* s,int left,int right)
		//double *A;
		//int *s;
		//int left;
		//int right; 
	{
		int i = left-1;    /* left to right pointer */
		int j = right;     /* right to left pointer */

		for(;;) {
			while (A[s[++i]] < A[s[right]]);   /*  find element on left to swap */
			while (A[s[right]] < A[s[--j]])    /* look for element on right to swap, but don't run off end */
				if (j == left)     
					break;

			if (i >= j)
				break;    /* pointers cross */
			swap2(s, i, j);
		} 
		swap2(s, i, right);  /*  swap partition  element */ 
		return i;  
	}


	void quicksort(double* A,int* s,int left,int right)
		//double *A;
		//int *s; /* permutation array */
		//int left;
		//int right;
	{
		int q;

		if (right  > left) {
			q = partition2 (A, s, left, right);
			quicksort(A, s, left, q-1);
			quicksort(A, s,  q+1, right);
		}
	}


	void SetSigns(double* x,double &minimum,double &maximum,		
		std::vector<int> &negativeList, std::vector<int> &nonnegativeList, int count){
			minimum = DBL_MAX;
			maximum = -DBL_MAX;
			negativeList.clear();
			nonnegativeList.clear();
			for(int i=0;i<count;i++){
				if(x[i]<0){
					minimum = min(minimum,x[i]);
					negativeList.push_back(i);
				}
				else{
					maximum = max(maximum,x[i]);
					nonnegativeList.push_back(i);
				}
			}
	}

	void Contract(SubmodularOracle  *oracle,SubmodularOracle  *dummy,std::vector<int> &negativeList,int* reorder){
		int index = 0;
		std::vector<int> contracted;
		contracted.clear();

		for(int i=0;i<oracle->Count();i++){
			if(index<(int)negativeList.size() && oracle->Remainder()[i]==dummy->Remainder()[negativeList[index]]){
				contracted.push_back(i);
				index++;
			}
		}
		oracle->Contract(contracted,reorder);
	}

	void Delete(SubmodularOracle  *oracle,SubmodularOracle  *dummy,std::vector<int> &nonnegativeList, int* reorder){
		int index = 0;
		std::vector<int> deleted;
		deleted.clear();
		for(int i=0;i<oracle->Count();i++){
			if(index<(int)nonnegativeList.size() && oracle->Remainder()[i]==dummy->Remainder()[nonnegativeList[index]]){
				deleted.push_back(i);
				index++;
			}
		}
		oracle->Delete(deleted,reorder);
	}


	void SetX(double* x, std::vector<double> &lambdas, std::vector<double*> &bases, int count,int numComponents){
		for(int i=0;i<count;i++){
			x[i] = 0;
		}
		for(int i=0;i<numComponents;i++){
			for(int j=0;j<count;j++){
				x[j]+=lambdas[i]*bases[i][j];
			}
		}
	}

	double CalcNorm(double* x, int count){
		double norm = 0;
		for(int i=0;i<count;i++){
			norm+=x[i]*x[i];
		}
		return norm;
	}

	void CalcLinearMinimzer(SubmodularOracle  *oracle,double *x, int* order,double* base, double* sortX,int count){
		for(int i=0;i<count;i++){
			order[i] = i;
			sortX[i] = x[i];
		}
		quicksort(sortX,order,0,count-1);
		oracle->CalcBase(order,base);
	}

	double CalcInnerProduct(double* x,double* base,int count){
		double innerProd = 0;
		for(int i=0;i<count;i++){
			innerProd+=x[i]*base[i];
		}
		return innerProd;
	}

	double IsMinimizer(double norm,double* x,double* base,int count){
		double innerProd = CalcInnerProduct(x,base,count);
		if(abs(norm-innerProd) <= max( norm, abs(innerProd) ) *Eps){
			return true;
		}
		return norm <=  innerProd;
	}

	double Normalize(double* vector,int length){		
		double res = 0;
		for(int i=0;i<length;i++){
			res+=vector[i]*vector[i];
		}
		res = sqrt(res);
		for(int i=0;i<length;i++){
			vector[i]/=res;
		}
		return res;
	}



	bool AddToMatrix(std::vector<double> &lambdas,double* base,std::vector<double*> &Q,std::vector<double*> &R,int & numComponents, int count,std::vector<double*> bases){		


		if(numComponents==0){
			for(int i=0;i<count;i++){
				Q[0][i] = base[i];
			}
			Q[0][count] = 1;
		}
		else{
			lambdas[numComponents ] = 0;
			for(int i=0;i<numComponents;i++){
				R[numComponents][i] = Q[i][count];
				for(int j=0;j<count;j++){
					R[numComponents][i] +=base[j]*Q[i][j];
				}
			}
			for(int i=0;i<=count;i++){
				Q[numComponents][i] = 0;
				for(int j=0;j<numComponents;j++){
					Q[numComponents][i]+=Q[j][i]*R[numComponents][j];
				}
			}
			for(int i=0;i<count;i++){
				Q[numComponents][i] = base[i] - Q[numComponents][i];
			}
			Q[numComponents][count] = 1 - Q[numComponents][count];
		}
		R[numComponents][numComponents] = Normalize(Q[numComponents],count+1);
		return R[numComponents][numComponents]>=abs(R[0][0])*Eps;
	}

	void CalcBestLambdas(std::vector<double*> &Q,std::vector<double*> & R,int & numComponents,double* bestLambdas, int count){
		for(int i=numComponents-1;i>=0;i--){
			double sum = 0;
			for(int j=i+1;j<numComponents;j++){
				sum+=R[j][i]*bestLambdas[j];
			}
			bestLambdas[i] =(Q[i][count]-sum)/R[i][i];
		}


		double sum = 0;
		for(int i=0;i<numComponents;i++){
			sum+=bestLambdas[i];
		}
		for(int i=0;i<numComponents;i++){
			bestLambdas[i]/=sum;
		}
	}

	bool IsInRelativeInterior(double* bestLambdas, int numComponents){		
		bool res = true;
		for (int i = 0; i < numComponents; i++)
		{
			if (bestLambdas[i] < Eps)
			{
				res = false;
				if (bestLambdas[i] > -Eps)
				{
					bestLambdas[i] = 0;
				}//if
			}//if
		}//for i
		return res;
	}


	void Elimination(int pivotRow, int eraseRow, int endCol,std::vector<double*> & Q,std::vector<double*> & R,int count)
	{
		double a = R[pivotRow][pivotRow];
		double b = R[pivotRow][eraseRow];
		double sq = sqrt((a * a + b * b));
		if (sq == 0)
		{
			return;
		}
		double cos = a / sq;
		double sin = b / sq;
		for (int j = pivotRow; j < endCol; j++)
		{
			double pivot =  R[j][pivotRow];
			double erase = R[j][eraseRow];
			R[j][pivotRow] = pivot * cos + erase * sin;
			R[j][eraseRow] = -pivot * sin + erase * cos;
		}	

		R[pivotRow][eraseRow] = 0;

		 for (int j = 0; j <= count; j++){
                double pivot = Q[pivotRow][j];
                double erase = Q[eraseRow][j];
                Q[pivotRow][j] = pivot * cos + erase * sin;
                Q[eraseRow][j] = -pivot * sin + erase * cos;
            }

	}

	void Delete(int index,std::vector<double>& lambdas,std::vector<double*> &bases,std::vector<int*>&orders,std::vector<double*> & Q,std::vector<double*> & R,int &numComponents,double* bestLambdas, int count){
		for (int i = index; i < numComponents - 1; i++)
		{
			double tmpDouble;
			tmpDouble = lambdas[i];
			lambdas[i] = lambdas[i+1];
			lambdas[i+1] = tmpDouble;

			double* tmpDoubleArray;
			tmpDoubleArray = bases[i];
			bases[i] = bases[i+1];
			bases[i+1] = tmpDoubleArray;

			tmpDoubleArray = R[i];
			R[i] = R[i+1];
			R[i+1] = tmpDoubleArray;

			int* tmpIntArray;
			tmpIntArray = orders[i];
			orders[i] = orders[i+1];
			orders[i+1] = tmpIntArray;
		}
		for (int i = index; i < numComponents - 1; i++)
		{
			Elimination(i, i + 1, numComponents - 1,Q,R,count);
		}
		numComponents--;
	}


	//void Delete(std::vector<int> list,std::vector<double>& lambdas,double* base,std::vector<double*> &bases,std::vector<int*>&orders,std::vector<double*> & Q,std::vector<double*> & R,int & numComponents,double* bestLambdas, int count){
	//	double sum = 1;
	//	for(int i=list.size()-1;i>=0;i--){
	//		sum-=lambdas[i];
	//		Delete(list[i],lambdas,  bases,orders, Q,R, numComponents, bestLambdas,  count);
	//	}
	//	//numComponents = numComponents-(int)list.size();
	//	for(int i=0;i<numComponents;i++){
	//		lambdas[i]/=sum;
	//	}
	//}


	void FindIntersection(std::vector<double>& lambdas,std::vector<double*> &bases,std::vector<int*>&orders,std::vector<double*> & Q,std::vector<double*> & R,int &numComponents,double* bestLambdas, int count){

		double portion = 0;
		for (int i = 0; i < numComponents; i++){
			if (lambdas[i] - bestLambdas[i] > Eps){
				portion = max(portion, -bestLambdas[i] / (lambdas[i] - bestLambdas[i]));
			}
		}
		for (int i = numComponents - 1; i >= 0; i--){
			lambdas[i] = portion * lambdas[i] + (1 - portion) * bestLambdas[i];
			if (lambdas[i] < Eps){
				lambdas[i] = 0;
				Delete(i,lambdas,  bases,orders, Q,R, numComponents, bestLambdas,  count);
			}
		}
		if (portion >= 1 - Eps){
			int minIndex = 0;
			for (int i = 0; i < numComponents; i++){
				if (lambdas[i] < lambdas[minIndex]){
					minIndex = i;
				}
			}
			lambdas[minIndex] = 0;
			Delete(minIndex, lambdas,  bases,orders, Q,R, numComponents, bestLambdas,  count);
		}

	}

	void ExecuteAlgorithm(std::vector<double>& lambdas,std::vector<double*> &bases,std::vector<int*>&orders,std::vector<double*> & Q,std::vector<double*> & R,int & numComponents,double* bestLambdas, int count){
		while(numComponents>1){
			CalcBestLambdas( Q, R,numComponents,bestLambdas, count);
			if(IsInRelativeInterior(bestLambdas,numComponents)){
				for(int i=0;i<numComponents;i++){
					lambdas[i] = bestLambdas[i];
				}
				break;
			}
			FindIntersection(lambdas, bases,orders, Q,R, numComponents, bestLambdas,  count);
		}
		if(numComponents==1){
			for(int i=0;i<(int)lambdas.size();i++){
				lambdas[i] = 0;
			}
			lambdas[0] = 1;
		}

	}

	bool Update(std::vector<double>& lambdas,double* base,int* order,std::vector<double*> &bases,std::vector<int*>&orders,std::vector<double*> &Q,std::vector<double*> &R,int & numComponents,double* bestLambdas, int count,int n){
		if(((int)Q.size())==numComponents){
			Q.push_back(new double[n+1]);
			R.push_back(new double[n+1]);
			for(int i=0;i<n+1;i++){
				Q[numComponents][i] = R[numComponents][i] = 0;
			}
			lambdas.push_back(0);
			orders.push_back(new int[n]);
			bases.push_back(new double[n]);
			Q[numComponents][numComponents] = 1;

		}	
		for(int i=0;i<count;i++){
			orders[numComponents ][i] = order[i];
			bases[numComponents ][i] = base[i];
		}
		if(!AddToMatrix(lambdas,base,Q,R,numComponents,count,bases)){
			return false;
		}		
		numComponents++;
		ExecuteAlgorithm(lambdas,  bases,orders, Q,R, numComponents, bestLambdas,  count);
		return true;
	}

	void FW::SetInitialBase(SubmodularOracle  *oracle,int &numComponents, double* x,std::vector<double>& lambdas,std::vector<int*> &orders,std::vector<double*> &bases,std::vector<double*> &Q,std::vector<double*>&R){
		for(int i=0;i<oracle->Count();i++){
			order[i] = i;
		}
		numComponents = 0;
		oracle->CalcBase(order,base);

		
		for(int i=0;i<(int)lambdas.size();i++){
			lambdas[i] = 0;
		}
		if(lambdas.size()==0){
			lambdas.push_back(0);
		}
		lambdas[0] = 1;

		if(orders.size()==0){
			orders.push_back(new int[oracle->N()]);
			bases.push_back(new double[oracle->N()]);
		}
		for(int i=0;i<oracle->Count();i++){
			orders[0][i] = order[i];
			bases[0][i] = base[i];
		}

		Update(lambdas, base, order,bases,orders,Q,R,numComponents,bestLambdas, oracle->Count(),oracle->N());
	}


	void IsIdeal(std::vector<int*> orders,int* cnt,int &remainder, int &filledIndex, int index,int numComponents)       {
		for (int k = 0; k < numComponents; k++)          {
			int cur = orders[k][index];
			if (cnt[cur] == 0)              {
				remainder++;
			}
			cnt[cur]++;
			if (cnt[cur] == numComponents)              {
				remainder--;
			}
		}
		if (remainder == 0)          {
			filledIndex = index;
		}
	}

	int SetUpperIndex(int* cnt,std::vector<int*> orders,double* x,int count,int numComponents)       {
		for (int i = 0; i < count; i++)        {
			cnt[i] = 0;
		}
		int remainder = 0;
		int upperIndex = count;
		for (int i = count - 1; i >= 0; i--)         {
			if (x[orders[0][i]] <= 0)              {
				break;
			}
			IsIdeal(orders,cnt,remainder, upperIndex, i,numComponents);                
		}
		return upperIndex;
	}

	int SetLowerIndex(int* cnt,std::vector<int*> orders,double* x,int count,int numComponents)       {
		for (int i = 0; i < count; i++) {
			cnt[i] = 0;
		}

		/*
		std::string str;
		for(int i=0;i<=count;i++){
		str +=std::to_string(x[i])+" ";
		}
		std::cout<<str;
		*/

		int remainder = 0;
		int lowerIndex = -1;
		for (int i = 0; i < count; i++)        {
			if (x[orders[0][i]] > 0)             {
				break;
			}
			IsIdeal(orders,cnt,remainder, lowerIndex, i,numComponents);                
		}
		return lowerIndex;
	}

	void Reorder(std::vector<int*>orders,std::vector<double*> &bases,int* reorder,int* inverse,int count,int nextCount,int numComponents){
		for(int i=0;i<nextCount;i++){
			int cur = reorder[i];
			for(int j=0;j<numComponents;j++){
				bases[j][i] = bases[j][cur];
			}
		}

		for(int i=0;i<count;i++){
			inverse[i] = -1;
		}
		for(int i=0;i<nextCount;i++){
			inverse[reorder[i]] = i;
		}
		for(int i=0;i<numComponents;i++){
			int index = 0;
			for(int j=0;j<count;j++){
				int cur = orders[i][j];
				if(inverse[cur]==-1)
					continue;
				orders[i][index++] =inverse[cur];
			}
		}

	}

	bool HeuristicDelete(SubmodularOracle *oracle,  int* cnt,std::vector<int*> orders,std::vector<double*> & bases,double* x, int* reorder, int count,int numComponents,bool*used){
		int cntPositive = 0;
		for(int i=0;i<count;i++){
			used[i] = (x[i]>0);
			if(x[i]>0){
				cntPositive++;
			}
		}
		std::vector<int> badIndices;
		for(int i=0;i<numComponents;i++){
			for(int j = count - cntPositive ;j<count;j++){
				if(!used[orders[i][j]]){
					badIndices.push_back(i);
					break;
				}
			}
		}
		if((int)badIndices.size()>0 && badIndices.size() < CutRatio*numComponents){
			oracle->Delete(badIndices,reorder);
			return true;
		}
		return false;
	}


	bool HeuristicContract(SubmodularOracle *oracle,  int* cnt,std::vector<int*> orders,std::vector<double*> & bases,double* x, int* reorder, int count,int numComponents,bool*used){
		int cntNonpositive = 0;
		for(int i=0;i<count;i++){
			used[i] = (x[i]<=0);
			if(x[i]<=0){
				cntNonpositive++;
			}
		}
		std::vector<int> badIndices;
		for(int i=0;i<numComponents;i++){
			for(int j=0;j<cntNonpositive;j++){
				if(!used[orders[i][j]]){
					badIndices.push_back(i);
					break;
				}
			}
		}
		if((int)badIndices.size()>0 && badIndices.size() < CutRatio*numComponents){
			oracle->Contract(badIndices,reorder);
			return true;
		}
		return false;
	}


	bool HeuristicReduce(SubmodularOracle *oracle,  int* cnt,std::vector<int*> orders,std::vector<double*> & bases,double* x, int* reorder, int count,int numComponents,int type,bool*used){
		if(type==deleteType){
			return HeuristicDelete(oracle,   cnt, orders, bases,x,  reorder,  oracle->Count(), numComponents,used);
		}
		else if(type==contractType){
			return HeuristicContract(oracle,   cnt, orders, bases,x,  reorder,  oracle->Count(), numComponents,used);
		}
		return false;
	}


	bool Reduce(SubmodularOracle *oracle,  int* cnt,std::vector<int*> orders,std::vector<double*> & bases,double* x, int* reorder, int*inverse,int count,int numComponents,int type){
		bool flg = false;
		int prevCount = oracle->Count();
		int lowerIndex  =  SetLowerIndex( cnt, orders, x, count, numComponents)  ; 
		int upperIndex  =  SetUpperIndex( cnt, orders, x, count, numComponents)  ; 
		if((type&contractType)>0 && lowerIndex !=-1){
			std::vector<int> contractList ;
			contractList.clear();
			for(int i=0;i<=lowerIndex;i++){
				contractList.push_back(orders[0][i]);
			}
			oracle->Contract(contractList,reorder);
			Reorder(orders,bases,reorder,inverse,prevCount,oracle->Count(),numComponents);
			flg = true;	
		}
		if(!flg && (type&deleteType)>0 && upperIndex !=count){
			std::vector<int> deleteList ;
			deleteList.clear();
			for(int i=upperIndex;i<count;i++){
				deleteList.push_back(orders[0][i]);
			}
			oracle->Delete(deleteList,reorder);
			Reorder(orders,bases,reorder,inverse,prevCount,oracle->Count(),numComponents);
			flg = true;
		}
		return flg;
	}

	void ReconstractMatrix(std::vector<double> &lambdas,std::vector<int*> &orders,std::vector<double*> &bases,std::vector<double*> &Q,std::vector<double*> &R,int & numComponents,double* bestLambdas, int count){
		int memoCount = numComponents;
		int lastIndex = memoCount - 1;
		numComponents = 0;
		for (int i = 0; i < memoCount; i++)
		{
			if (AddToMatrix(lambdas,bases[numComponents],Q,R,numComponents,count,bases))
			{
				numComponents++;
			}
			else
			{
				for (int j = numComponents; j < lastIndex; j++)
				{
					//double tmpDouble;
					//tmpDouble = lambdas[j];
					//lambdas[j] = lambdas[j+1];
					//lambdas[j+1] = tmpDouble;

					double* tmpDoubleArray;
					tmpDoubleArray = bases[j];
					bases[j] = bases[j+1];
					bases[j+1] = tmpDoubleArray;

					/*    	tmpDoubleArray = R[j];
					R[j] = R[j+1];
					R[j+1] = tmpDoubleArray;*/

					int* tmpIntArray;
					tmpIntArray = orders[j];
					orders[j] = orders[j+1];
					orders[j+1] = tmpIntArray;
				}
				lastIndex--;
			}
		}

		for(int i=0;i<numComponents;i++){
			lambdas[i] = 1.0/numComponents;
		}
		ExecuteAlgorithm(lambdas,bases,orders,Q,R,numComponents,bestLambdas,count);
	}

	void ForcedReduce(SubmodularOracle  *oracle, int count ,int* reorder, int &numComponents,std::vector<double>& lambdas,std::vector<int*> &orders,int type){
		int index = 0;
		for(int i=0;i<numComponents;i++){
			if(lambdas[i] > lambdas[index]){
				index = i;
			}
		}

		if(type == contractType){
			std::vector<int> contracted;
			contracted.clear();
             int end = (int)(floor(CutRatioSmall * count));
			 for(int i=0;i<end;i++){
				 contracted.push_back(orders[index][i]);
			 }
			 oracle->Contract(contracted,reorder);
		}
		if(type == deleteType){
			std::vector<int> deleted;
			deleted.clear();
            int start = (int)(ceil((1 - CutRatioSmall) * count));
			 for(int i=start;i<count;i++){
				 deleted.push_back(orders[index][i]);
			 }
			 oracle->Delete(deleted,reorder);
		}

	}

	void FW::ExecuteWolfeAlgorithm(SubmodularOracle  *oracle, int &numComponents,double* x,std::vector<double>& lambdas,std::vector<int*> &orders,std::vector<double*> &bases, std::vector<double*> &Q,std::vector<double*> &R,long maxRepeat,int type){
		double currentNorm =DBL_MAX;
		int iteration = 0;
        int cutIteration = max(10, (int)(oracle ->N() * RepRatioSmall));
		std::vector<int> negative;
		std::vector<int>nonnegative;
		double minimum;double maximum;
		while(iteration<maxRepeat && oracle->Count()>0){
			iteration++;
			SetX(x,lambdas,bases,oracle->Count(),numComponents);
			SetSigns(x,minimum,maximum,negative,nonnegative,oracle->Count());
			if(maximum <=0|| minimum>0){
				break;
			}
			if(Reduce(oracle, cnt, orders, bases,x,  reorder, inverse, oracle->Count(), numComponents, type)){
				ReconstractMatrix(lambdas,orders,bases,Q,R,numComponents,bestLambdas,oracle->Count());
				continue;
			}
			if(HeuristicReduce(oracle,   cnt, orders, bases,x,  reorder,  oracle->Count(), numComponents, type,used)){
				SetInitialBase(oracle, numComponents,x,lambdas,orders,bases,Q,R);
				currentNorm   = DBL_MAX;
				continue;
			}
			if(iteration % cutIteration == cutIteration -1 && type != anyType){
				ForcedReduce( oracle,oracle->Count(),reorder,numComponents, lambdas,orders,  type);
				SetInitialBase(oracle, numComponents,x,lambdas,orders,bases,Q,R);
				currentNorm   = DBL_MAX;
				continue;
			}
			double nextNorm = CalcNorm(x,oracle->Count());
			if(currentNorm <=nextNorm){
				break;
			}
			currentNorm = nextNorm;
			CalcLinearMinimzer(oracle,x,order,base,sortX,oracle->Count());
			if(IsMinimizer(currentNorm,x,base,oracle->Count())){
				break;
			}
			if(!Update(lambdas,base,order,bases,orders,Q,R,numComponents,bestLambdas,oracle->Count(),oracle->N())){
				break;
			}
		}
		SetX(x,lambdas,bases,oracle->Count(),numComponents);
	}

	std::vector<int> FW::Minimization(SubmodularOracle &oracle,SubmodularOracle &dummy)
	{
		int n = oracle.N();
		order = new int[n];
		base = new double[n];
		reorder = new int[n];
		sortX = new double[n];
		bestLambdas = new double[n+1];
		cnt = new int[n];
		used = new bool[n];
		inverse = new int[n];

		std::vector<int> negativeList;
		std::vector<int> nonnegativeList;
		std::vector<int> smallNegativeList;
		std::vector<int> smallNonnegativeList;
		negativeList.clear();
		nonnegativeList.clear();
		smallNegativeList.clear();
		smallNonnegativeList.clear();
		double minimumValue;
		double maximumValue;

		int numComponents = -1;
		double* x = new double[n];
		std::vector<double> lambdas;
		std::vector<int*> orders;
		std::vector<double*> bases;
		std::vector<double*> Q;
		std::vector<double*> R;
		std::vector<double*> smallQ;
		std::vector<double*>smallR;
		lambdas.clear();
		orders.clear();
		bases.clear();
		Q.clear();
		R.clear();
		smallQ.clear();
		smallR.clear();

		int numSmallComponents = -1;
		double* smallX = new double[n];
		std::vector<double> smallLamdas;
		std::vector<int*> smallOrders;
		std::vector<double*> smallBases;
		smallLamdas.clear();
		smallOrders.clear();
		smallBases.clear();


		SetInitialBase(&oracle, numComponents,x,lambdas,orders,bases,Q,R);
		while(true){
			if(oracle.Count() < naiveSize){
				ExecuteWolfeAlgorithm(&oracle, numComponents,x,lambdas,orders,bases,Q,R,LONG_MAX,anyType);
				break;
			}
			else{
				 int smallReputation = max(10, (int)(oracle.Count() * RepRatio));
				ExecuteWolfeAlgorithm(&oracle,numComponents, x,lambdas,orders,bases,Q,R,smallReputation,anyType);
				SetSigns(x,minimumValue,maximumValue,negativeList,nonnegativeList,oracle.Count());
		    	if(oracle.Count()==0||minimumValue>0 ||maximumValue<=0){
					break;
				}

				//for contraction
				dummy.Copy(&oracle);
				dummy.Delete(nonnegativeList,reorder);
				SetInitialBase(&dummy,numSmallComponents,smallX,smallLamdas,smallOrders,smallBases,smallQ,smallR);
				ExecuteWolfeAlgorithm(&dummy,numSmallComponents,smallX,smallLamdas,smallOrders,smallBases,smallQ,smallR,LONG_MAX,deleteType);
				SetSigns(smallX,minimumValue,maximumValue,smallNegativeList,smallNonnegativeList,dummy.Count());
				if(smallNegativeList.size()>0){
					Contract(&oracle,&dummy,smallNegativeList,reorder);
					SetInitialBase(&oracle,numComponents, x,lambdas,orders,bases,Q,R);
					continue;
				}


				//for deletion
				dummy.Copy(&oracle);
				dummy.Contract(negativeList,reorder);
				SetInitialBase(&dummy,numSmallComponents,smallX,smallLamdas,smallOrders,smallBases,smallQ,smallR);
				ExecuteWolfeAlgorithm(&dummy,numSmallComponents,smallX,smallLamdas,smallOrders,smallBases,smallQ,smallR,LONG_MAX,contractType);
				SetSigns(smallX,minimumValue,maximumValue,smallNegativeList,smallNonnegativeList,dummy.Count());
				if(smallNonnegativeList.size()>0){
					Delete(&oracle,&dummy,smallNonnegativeList,reorder);
					SetInitialBase(&oracle,numComponents, x,lambdas,orders,bases,Q,R);
					continue;
				}
			}
		}

		std::vector<int> res;
		res = oracle.ContractedList();
		for(int i=0;i<oracle.Count();i++){
			if(x[i]<=0){
				res.push_back(oracle.Remainder()[i]);
			}
		}


		delete[] order;
		delete[]base;
		delete[]reorder;
		delete[]x;
		delete[]smallX;
		delete[]sortX;
		delete[]bestLambdas;
		delete[]cnt;
		delete[]used;
		delete[]inverse;
		for(int i=0;i<(int)orders.size();i++){
			delete[] orders[i];
			delete[] bases[i];
		}
		for(int i=0;i<(int)Q.size();i++){
			delete[] Q[i];
			delete[] R[i];
		}			
		for(int i=0;i<(int)smallOrders.size();i++){
			delete[]smallOrders[i];
			delete[]smallBases[i];
		}
		for(int i=0;i<(int)smallQ.size();i++){
			delete[]smallQ[i];
			delete[]smallR[i];
		}		

		return res;
	}




}