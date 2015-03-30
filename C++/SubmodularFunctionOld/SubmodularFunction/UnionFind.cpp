#include "Library.h"

using namespace OnigiriLibrary;


UnionFind::UnionFind(int len)
{
	UnionFind::len =len;
	parent = new int[len];
	for (int i = 0; i < len; i++)
		parent[i] = -1;
}

UnionFind::~UnionFind(){
	delete[] parent;
}

int UnionFind::Find(int x)
{
	if (parent[x] < 0)
		return x;
	return parent[x] = Find(parent[x]);
}

bool UnionFind::Unite(int x, int y)
{
	x = Find(x);
	y = Find(y);
	if (x == y)
		return false;

	if (parent[y] < parent[x])    //swap
	{
		int tmp = x;
		x = y;
		y = tmp;
	}
	parent[x] += parent[y];
	parent[y] = x;
	return true;
}

bool UnionFind::Same(int x, int y)
{
	return Find(x) == Find(y);
}

void UnionFind::Clear()
{
	for (int i = 0; i < len; i++)
		parent[i] = -1;
}
