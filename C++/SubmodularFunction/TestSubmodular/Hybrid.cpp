#include "stdafx.h"
#include "CppUnitTest.h"
#include "Hybrid.h"
#include <queue>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

Hybrid::Hybrid(){};
Hybrid::~Hybrid(){};

double EPS = 1e-10;

 double RoundingValue(double val0, double val1)
        {
            double sum = val0 + val1;
            double absSum = abs(sum);
            double maxAbsValue = max(abs(val0), abs(val1));
            if (absSum<maxAbsValue*EPS)
            {
                return 0;
            }//if
            return sum;
        }


SFMResult Hybrid::Minimization(SubmodularOracle* oracle){
	SetOracle(oracle);


	int n =oracle->N();
	N = n;
	K = 2*n;
	M = N+K;

	int     g,i,j,k,l,m,s,t,u,v;
	int     ws,wr,wt;
	int     ju,jv,jw; 

	lst = new int*[M];
	y = new double*[M];
	f = new double*[M];
	phi = new double*[N];
	for(i=0;i<M;i++){
		lst[i] = new int[N];
		y[i] = new double[N];
		f[i] = new double[N];
	}
	for(i=0;i<N;i++){
		phi[i] = new double[N];
	}

	int*     d=new int[N];
	int*     pa= new int[N+1];
	int*     q=new int[N];
	int*     w=new int[N];
	int*     psu=new int[N];
	int* psv=new int[N];
	double*  psi=new double[N];
	double*  x=new double[N];
	double* z=new double[N];
	double*  b=new double[N]; 
	double*  lambda=new double[M];
	double  alpha, beta, gamma, delta, epsilon, eta, theta, /*omega,*/ mu, zeta;
	int     count_reduce=0;

	//for reduce
	p  = new int[N+1];
	kappa= new double[N]; 
	a = new double*[M];
	for( i=0;i<M;i++){
		a[i] = new double[N+1];
	}

	for (s=0;s<n;s++){
		lst[0][s]=s;
	}
	oracle->CalcBase(lst[0],y[0]);
	for(i=0;i<N;i++){
		x[i] = y[0][i];
	}

	//f[0][0]=CalcValue(lst[0],1);
	//omega=f[0][0];
	//y[0][0]=f[0][0];
	//x[0]=y[0][0];
	//for (j=1;j<n;j++){
	//	s=lst[0][j];
	//	f[0][j]=CalcValue(lst[0],j+1);
	//	if (f[0][j]<omega){
	//		omega=f[0][j];
	//	}
	//	y[0][s]=f[0][j]-f[0][j-1];
	//	x[s]=y[0][s];
	//}
	gamma=0.0;
	zeta=0.0;
	for (s=0;s<n;s++){
		z[s]=x[s];
		if (x[s]<0){
			gamma-=x[s];
		}
		else {
			zeta+=x[s];
		}
	}
	lambda[0]=1.0;
	k=1;
	if (gamma>zeta){
		gamma=zeta;
	}
	mu=(double)n*n;
	epsilon=1/mu;
	delta=gamma/(mu);
	for (u=0;u<n;u++){
		for (v=0;v<n;v++){
			phi[u][v]=0.0;
		}
	}

	long iteration = 0;
	while (delta>=epsilon){
		iteration++;
		delta/=2;
		/*     printf("delta %f\n",delta); */ 
		for (u=0;u<n;u++){
			for (v=0;v<n;v++){
				if (phi[u][v]>delta){
					phi[u][v]-=delta;
					z[u]-=delta;
					z[v]+=delta;
				}
			}
		}
		for (u=0;u<n;u++){
			d[u]=0;
		}
		while (1){
			/*       for (i=0;i<k;i++){
			printf("%f:   ",lambda[i]);
			for (j=0;j<n;j++){
			printf("%f, ",y[i][j]);
			}
			printf("\n");
			}
			printf("            ");
			for (j=0;j<n;j++){
			printf("%f, ",x[j]);
			}
			printf("\n");
			printf("\n"); */ 
			for (u=0,wt=0;u<n;u++){
				if (RoundingValue(z[u],+delta)<=0){
					pa[u]=-1;
					w[wt]=u;
					wt++;
				}
				else {
					pa[u]=n;
				}
			}
			pa[n]=-1; 
			ws=0;
			wr=0;
			while (ws<wt && pa[n]<0){
				s=w[ws];
				for (t=0;t<n;t++){
					if (pa[t]==n && phi[s][t]==0.0){
						pa[t]=s;
						if (RoundingValue( z[t],-delta)>=0){
							pa[n]=t;
							break;
						}
						w[wt]=t;
						wt++;
					}
				}
				ws++;
			}
			if (pa[n]<0){
				l=n;
				for (u=0;u<n;u++){
					if (pa[u]==n && d[u]<l){
						l=d[u];
					}
				}
				if (l==n){
					break;
				}
				i=0;
				while (i<k){ 
					ju=-1;
					jv=n;
					for (j=0;j<n;j++){
						s=lst[i][j];
						if (d[s]==l-1 && pa[s]<n){
							ju=j;
						}
						if (d[s]==l && pa[s]==n){
							if (jv==n){
								jv=j;
							}
						}       	     
					}
					if (jv<ju){
						break;
					}
					i++;
				}
				if (i==k){
					for (s=0;s<n;s++){
						if (d[s]==l && pa[s]==n){
							d[s]++;
						} 
					}
				}
				else {
					/* printf("Multi-Exchange \n"); */  
					for (j=0;j<n;j++){
						s=lst[i][j];
						lst[k][j]=s;
						f[k][j]=f[i][j];
						y[k][s]=y[i][s];
					}
					j=jv; 
					for (g=jv;g<=ju;g++){
						s=lst[k][g];
						if (pa[s]<n){
							lst[i][j]=s;
							j++;
						}
					}
					jw=j;
					for (g=jv;g<=ju;g++){
						s=lst[k][g];
						if (pa[s]==n){
							lst[i][j]=s;
							j++;
						}
					}
					oracle->CalcBase(lst[i],y[i]);
					//for (g=jv;g<=ju;g++){
					//	f[i][g]=CalcValue(lst[i],g+1);
					//	if (f[i][g]<omega){
					//		omega=f[i][g];
					//	}
					//	s=lst[i][g];
					//	if (g==0){
					//		y[i][s]=f[i][g];
					//	}
					//	else {
					//		y[i][s]=f[i][g]-f[i][g-1];
					//	}
					//}
					for (g=jv;g<jw;g++){
						s=lst[i][g];
						b[s]=y[i][s]-y[k][s];
					}
					for (j=jw;j<=ju;j++){
						t=lst[i][j];
						b[t]=y[k][t]-y[i][t];
					}
					j=jw;
					m=0;
					for (g=jv;g<jw;g++){
						s=lst[i][g];
						while (j<=ju){ 
							t=lst[i][j];
							psu[m]=s;
							psv[m]=t;
							if (b[s]<b[t]){
								psi[m]=b[s];
								b[t]-=b[s];
								m++;
								break;
							}
							else {
								psi[m]=b[t];
								b[s]-=b[t];
								j++;
								m++;
							}                               
						}
					}
					beta=0.0;
					for (j=0;j<m;j++){
						if (psi[j]>beta){
							beta=psi[j];
						}
					}
					eta=lambda[i]*beta;
					if (eta>delta){
						alpha=delta/beta;
						lambda[k]=lambda[i]-alpha;
						lambda[i]=alpha;
						k++;
					}
					else {
						alpha=lambda[i];
					}
					/*	   printf("alpha= %f \n", alpha); */ 
					for (j=0;j<m;j++){
						u=psu[j];
						v=psv[j];
						theta=alpha*psi[j]; 
						/*             printf("%d, %d, %f\n", u, v, psi[j]); */
						x[u]+=theta;
						x[v]-=theta;
						if (phi[u][v]>theta){
							phi[u][v]-=theta;
						}
						else {
							phi[v][u]=theta-phi[u][v];
							phi[u][v]=0.0;
						}
					}
				}
			}         
			else {
				/* printf("Augmentation \n"); */  
				t=pa[n];
				z[t]-=delta;
				s=pa[t];
				while (s>=0){
					phi[s][t]=delta-phi[t][s];
					phi[t][s]=0.0;
					t=s;
					s=pa[t];
				}
				z[t]+=delta;
			}
			if (k>K){
				count_reduce++;
				k=reduce(y,lambda,n,k,q);
				for (i=0;i<k;i++){
					if (q[i]!=i){
						lambda[i]=lambda[q[i]];
						for (s=0;s<n;s++){
							y[i][s]=y[q[i]][s];
							f[i][s]=f[q[i]][s];
							lst[i][s]=lst[q[i]][s];
						}
					}
				}
			}
		}
	}
	gamma=0.0;
	for (s=0;s<n;s++){
		if (x[s]<0){
			gamma+=x[s];
		}
	}


	bool* reachable = new bool[N];
	bool** graph = new bool*[N];
	for(i=0;i<N;i++){
		reachable[i] = false;
		graph[i] = new bool[N];
		for(j=0;j<N;j++){
			graph[i][j] = false;
		}
	}
	for(i=0;i<k;i++){
		for(j=0;j<n-1;j++){
			graph[lst[i][j+1]][lst[i][j]] = true;
		}
	}
	queue<int>que;
	for(i=0;i<N;i++){
		if(RoundingValue(z[i],delta)<=0){
			que.push(i);
			reachable[i] = true;
		}
	}
	while(!que.empty()){
		int deq = que.front();
		que.pop();
		for(i=0;i<N;i++){
			if(graph[deq][i]&&!reachable[i]){
				que.push(i);
				reachable[i] = true;
			}
		}
	}

	if(delta==0){
		bool res = false;
		for(i=0;i<N;i++){
			if(x[i]<0)
				res = true;
		}
		for(i=0;i<N;i++){
			reachable[i] = res;
		}
	}

	string minimizer = "";
		for(i=0;i<N;i++){
			minimizer+=(reachable[i]?"1":"0");
		}
	delete[]reachable;
	for(i=0;i<N;i++){
		delete[] graph[i];
	}
	delete[] graph;

	//SetResult(x,minimizer,iteration);
	SetResult(NULL,minimizer,iteration);


	for(i=0;i<M;i++){
		delete[] lst[i];
		delete[]y[i];
		delete[]f[i];
	}
	for(i=0;i<N;i++){
		delete[]phi[i];
	}
	delete[]     lst;
	delete[]  y;
	delete[]  f;
	delete[]  phi;   

	delete[]     d;
	delete[]     pa;
	delete[]     q;
	delete[]     w;
	delete[]     psu, psv;
	delete[]  psi;
	delete[]  x, z;
	delete[]  b; 
	delete[]  lambda;

	delete[]p;
	delete[]kappa;


	for( i=0;i<M;i++){
		delete[] a[i];
	}
	delete[] a;


	return Result();
}

int Hybrid::reduce(double** pt,double* lambda,int n,int k,int* q)
{
	int    d,g,h,i,j,r,s;
	/*   double a[M][N+1]; */ 
	//double **a;
	double alpha, zeta, theta, gamma;
	//double** matrixalloc();

	/*   printf("Reduce! k=%d --> ", k); */


	//a=(double **)matrixalloc(M,N+1);
	//if (a==NULL) {
	//  printf("Matrix Allocation Fails!");
	//  exit(1);
	//} 



	for (i=0;i<k;i++){
		for (j=0;j<n;j++){
			a[i][j] = pt[i][j];
			//  a[i][j]=*pt;
			//  pt++;
		}
		a[i][n]=1.0;
	}   
	/*   printf("\n");
	for (l=0;l<k;l++){
	printf("%f: ",lambda[l]);  
	for (j=0;j<=n;j++){ 
	printf("%f, ",a[l][j]);  
	}
	printf("\n");
	} */
	for (j=0;j<=n;j++){ 
		p[j]=j;
	}
	for (i=0,j=0;j<=n && i<k;i++){
		/*     printf("**********************\n");
		printf("i=%d\n", i);
		printf("j=%d\n", j); */
		gamma=0.0;
		for (g=j,d=j;g<=n;g++){
			if (a[i][p[g]]>=0.0){
				alpha=a[i][p[g]];
			}
			else {
				alpha=-a[i][p[g]];
			}
			if (alpha>gamma){
				gamma=alpha;
				d=g;
			}
		}
		/*     printf("gamma=%f\n", gamma); */
		if (gamma>0.000001 && j<n){
			/*       printf("gamma=%f\n", gamma); */
			q[j]=i;
			s=p[d]; p[d]=p[j]; p[j]=s;
			for (g=0;g<j;g++){
				for (h=i+1;h<k;h++){
					a[h][p[g]]-=a[i][p[g]]*a[h][p[j]]/a[i][p[j]];
				}
				a[i][p[g]]=0.0;
			}
			for (g=n;g>j;g--){
				for (h=i+1;h<k;h++){
					a[h][p[g]]-=a[i][p[g]]*a[h][p[j]]/a[i][p[j]];
				}
				a[i][p[g]]=0.0;
			}
			j++;
		}
		else {
			theta=lambda[i];
			d=j; 
			for (h=0;h<j;h++){
				kappa[h]=a[i][p[h]]/a[q[h]][p[h]];
				if (a[i][p[h]]!=0.0){
					zeta=-lambda[q[h]]/kappa[h];
					if (zeta>0.0 && zeta<theta){
						theta=zeta;
						d=h;
					}
				}
			}
			if (d<j){
				lambda[q[d]]=0.0;
				q[d]=i;
				kappa[d]=-1.0;
				for (g=0;g<d;g++){
					for (h=i+1;h<k;h++){
						a[h][p[g]]-=a[i][p[g]]*a[h][p[d]]/a[i][p[d]];
					}
				}
				for (g=n;g>d;g--){
					for (h=i+1;h<k;h++){
						a[h][p[g]]-=a[i][p[g]]*a[h][p[d]]/a[i][p[d]];
					}
				}
			}
			else {
				lambda[i]=0.0;
			}
			for (h=0;h<j;h++){
				lambda[q[h]]+=theta*kappa[h]; 
			}
		}
		/*     printf("\n");
		for (l=0;l<k;l++){
		printf("%f: ",lambda[l]);  
		for (s=0;s<=n;s++){ 
		printf("%f, ",a[l][s]);  
		}
		printf("\n");
		} */
	} 
	r=j;
	for (h=0;h<r;h++){
		while (q[h]<r && q[h]!=h){
			d=q[h];
			q[h]=q[d];
			q[d]=d;
		}
	}
	//for (i=0;i<M;i++){
	//  free(a[i]); 
	//}
	//free(a); 
	/*   printf("r=%d \n", r); */
	return(r);
}
