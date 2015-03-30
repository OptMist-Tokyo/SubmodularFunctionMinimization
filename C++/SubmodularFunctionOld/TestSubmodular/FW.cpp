#include "stdafx.h"
#include "CppUnitTest.h"
#include "FW.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

FW::FW(){};
FW::~FW(){};


SFMResult FW::Minimization(SubmodularOracle* oracle){
	SetOracle(oracle);


	int n = oracle->N();
	double*x = new double[n+1];
	int* order = new int[n];

	const double EPS = 1e-10;
	const bool TRUE =true;
	const bool FALSE = false;

	int i, ih, it, ic, ix, ixh, ii, j, jj, kx;
	int jk, k, kk, ipk, flag, flg;//, ik;
	double sf_new, sf_previous;
	double theta, pp, xx, xpj, rr, usum, aid, bid, cid, rid, xxp, ss;

	int *ip =new int[n+2];
	int *ib=new int[n+2];
	int *s=new int[n+1];
	double *xz=new double[n+1];
	double *u=new double[n+1];
	double *ubar=new double[n+1];
	double *v=new double[n+1];
	double *w=new double[n+1];
	double *pss=new double[n+2];
	double **r=new double*[n+2];
	double **ps =new double*[n+2];
	for(i=0;i<n+2;i++){
		r[i] = new double[n+2];
		ps[i] = new double[n+2];
	}


	for (j = 1; j <= n + 1; j++){
		ip[j] = j+1;
		ib[j] = 0;
	}
	ip[n+1] = 0;

	kx = 0;
	xxp = 1.0E+10;
	ixh = 0;

	for (j = 1; j <= n; j++){
		x[j]  = 0.0;
	}

	for(j = 0; j <= n; j++){
		s[j] = j;
	}
	for(j=0;j<n;j++){
		order[j] = j;
	}
	/**************   step 0 *********************/
	xx = 1.0;
	sf_previous = 0.0;
	for (j = 1; j <= n ; j++){
		sf_new = oracle->CalcValue(order,j);
		ps[1][j] = sf_new - sf_previous;
		sf_previous = sf_new;
		xx += ps[1][j]*ps[1][j];
	}
	pss[1] = xx - 1.0;
	r[1][1] = sqrt(xx);
	w[1] = ih = it  = 1;
	ic = 2;
	ip[ih] = 0;
	k = 1;

	/**************   step 1 *********************/

	/******************* (A) ********************/
	long iteration = 0;
	while(1){/* 1000 */
		iteration++;
		for (j = 1; j <= n; j++){
			xz[j]  = x[j];
			x[j] = 0.0;
		}

		i = ih;
		while( i != 0){
			for(j = 1; j <= n; j++){
				x[j]  += ps[i][j]*w[i];
			}
			i = ip[i];
		}

		/******************* (B) ********************/

		/* sort x[1..n] using quicksort algorithm:  
		s[i] contains the index of i-th smallest value of x */
		for(j = 0; j <= n; j++){
			s[j] = j;
		}   
		quicksort2(x,s,1,n);
		for(j=0;j<n;j++){
			order[j] = s[j+1]-1;
		}

		/* generate a new extreme base */
		oracle->CalcBase(order,ps[ic]);
		for(i=n;i>0;i--){
			ps[ic][i] = ps[ic][i-1];
		}
		//sf_previous = 0.0;
		//for (j = 1; j <= n ; j++){
		//	sf_new = CalcValue(order,j);      
		//	ps[ic][s[j]] =  sf_new - sf_previous;
		//	sf_previous = sf_new;
		//}

		//number_extreme_point++;

		/******************* (C) ********************/
		xx = xpj = pp = 0.0;

		for (j = 1; j <= n; j++){
			pp += ps[ic][j] * ps[ic][j];
			xx += x[j]*x[j];
			xpj += x[j] * ps[ic][j];
		}
		pss[ic] = pp;

		if ( (xxp - xpj) <= EPS){
			kx++;
			if (kx >= 10) goto END;
		}


		i = ih;
		ss = 1.0;
		while( i != 0){
			if (ss < pss[i]){ ss = pss[i];}
			i = ip[i];
		}
		if (ss < pp) ss = pp;


		xxp = xx;
		if ((xx - xpj) < (ss * EPS) ){ 
			printf("exit xx = xpj \nSize of the final corral = %d\n", k); goto END;
		}

		if (k == n){
			printf("Trying to augment full-dimensional S!\n ");
			goto END;
		}

		/******************* (D) ********************/

		i = ih;
		while ( i != 0){
			flag = 0;
			for (j =  1; j <= n; j++){
				if( ((ps[i][j] - ps[ic][j])*(ps[i][j] - ps[ic][j])) > EPS ) {
					i = ip[i];
					flag = 1;
					break;
				}
			}      
			if (flag == 0) break;
		}
		if (!flag) { printf("The generated point is in S!\n"); goto END; }

		/******************* (E), (F), (G) ********************/
		rr = 0.0;
		i  = ih;
		for ( jk = 1; jk <= k; jk++){
			r[ic][jk] = 1.0;
			for (j = 1; j  <= n; j++){
				r[ic][jk] += ps[i][j]*ps[ic][j];
			}
			for (jj = 1; jj <= jk -1; jj++){
				r[ic][jk] -=  r[i][jj]*r[ic][jj];
			}
			r[ic][jk]  = r[ic][jk]/r[i][jk];
			rr += r[ic][jk]*r[ic][jk];
			i = ip[i];
		}

		r[ic][k+1] = sqrt(1+pp-rr);

		k++;
		ip[it] = ic;
		ib[ic] = it;
		it = ic;
		ic = ip[ic];
		ip[it] = 0;

		w[it] = 0.0;



		/**************   step 2 *********************/
		while(1){ /* 2000 */
			i = ih;
			for (j = 1; j  <= k; j++){
				ubar[j] = 1.0;
				for (jj = 1;jj <= j-1; jj++){
					ubar[j]  -=  ubar[jj]*r[i][jj];
				}
				ubar[j] = ubar[j]/r[i][j];
				i = ip[i];
			}

			usum = 0.0;
			i = it;
			for (j = k; j >= 1; j--){
				u[i] = ubar[j];
				ii = ip[i];
				while (ii != 0){
					u[i] = u[i] - r[ii][j]*u[ii];
					ii = ip[ii];
				}
				u[i] = u[i]/r[i][j];
				usum += u[i];
				i  = ib[i];
			}

			flg = TRUE;
			i = ih;
			while  (i != 0){
				v[i] = u[i]/usum;
				if (v[i] < EPS) flg = FALSE;
				i = ip[i];
			}

			if (flg) {
				i = ih;
				while (i != 0){
					w[i] = v[i];
					i = ip[i];
				}
				break;
			}
			/**************   step 3 *********************/

			/******************* (A), (B) ********************/
			theta = 1.0;
			i = ih;
			while (i != 0){
				if ( (w[i] -v[i]) > EPS)
					theta = (theta <= w[i]/(w[i]- v[i]) )? theta : w[i]/(w[i] -v[i]) ;
				i = ip[i];
			}

			/******************* (C) ********************/
			kk = 0;
			i = ih;
			for (j=1; j <= k; j++){
				w[i] = (1-theta)*w[i] + theta*v[i];
				ipk = ip[i];

				/******************* (D), (E), (F), (G) ********************/

				if( w[i] < EPS ){
					w[i] = 0.0;
					kk++;
					ii = ip[i];
					for  (jj  = j -kk +1; jj <= k -kk; jj++){
						aid = r[ii][jj];
						bid  = r[ii][jj +1];
						cid = sqrt (aid*aid + bid*bid);
						ix = ii;
						while (ix != 0){
							rid = r[ix][jj];
							r[ix][jj]  =  (aid * rid + bid * r[ix][jj+1])/ cid;
							r[ix][jj+1]  =  (-bid * rid + aid * r[ix][jj+1])/ cid;
							ix  = ip[ix];
						}
						ii = ip[ii];
					}

					if ( i == ih){
						ih = ip[i];
						ib[ih] = 0;
					}
					else if (i == it){
						it = ib[i];
						ip[it] =  0;
					}
					else{
						ip[ib[i]] = ip[i];
						ib[ip[i]] = ib[i];
					}
					ip[i] = ic;
					ib[i] = 0;
					ic = i;
				}
				i = ipk;
			}
			k = k - kk;

		}  

	}     

	END:;
	
	string minimizer = "";
	double* y = new double[n];
	for(i=0;i<n;i++){
		y[i] = x[i+1];
		minimizer+=(y[i]< (1.0/(2*n))?"1":"0");
	}
	//SetResult(y,minimizer,iteration);
	SetResult(y,minimizer,iteration);
	delete []y;

	delete[] ip;
	delete[] ib;
	delete[] s ;
	delete[] xz;
	delete[] u ;
	delete[] ubar;
	delete[] v ;
	delete[] w  ;
	delete[] pss;

	for(i=0;i<n+2;i++){
		delete[] r[i];
		delete[]ps[i];
	}
	delete[]r;
	delete[]ps;

	delete[]x;
	delete[]order;

	return Result();
}






void 
swap (double* A,int i,int j)
     //double *A;
     //int i;
     //int j;
{
  double temp;
  
  temp = A[i];
  A[i]  = A[j];
  A[j] = temp;
}


int 
partition(double* A,int left,int right)
     //double *A;
     //int left;
     //int right; 
{
  int i = left-1;    /* left to right pointer */
  int j = right;     /* right to left pointer */
  
  for(;;) {
    while (A[++i] < A[right]);   /*  find element on left to swap */
    while (A[right] < A[--j])    /* look for element on right to swap, but don't run off end */
      if (j == left)     
	break;

    if (i >= j)
      break;    /* pointers cross */
    swap(A, i, j);
  }
  swap(A, i, right);  /*  swap partition  element */ 
  return i;  
}

void
quicksort(double* A,int left,int right)
     //double *A;
     //int left;
     //int right;
{
  int q;
  
  if (right  > left) {
    q = partition (A, left, right);
    quicksort(A, left, q-1);
    quicksort(A, q+1, right);
  }
}


/*******************************************************************************/

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

int 
partition2(double* A,int* s,int left,int right)
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


void
FW::quicksort2(double* A,int* s,int left,int right)
     //double *A;
     //int *s; /* permutation array */
     //int left;
     //int right;
{
  int q;
  
  if (right  > left) {
    q = partition2 (A, s, left, right);
    quicksort2(A, s, left, q-1);
    quicksort2(A, s,  q+1, right);
  }
}
