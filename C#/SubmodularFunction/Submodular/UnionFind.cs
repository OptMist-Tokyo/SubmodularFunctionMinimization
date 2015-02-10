using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UnionFind
{
    int[] iarent;

    public UnionFind(int len)
    {
        iarent = new int[len];
        for (int i = 0; i < len; i++)
            iarent[i] = -1;
    }

    public int Length
    {
        get { return iarent.Length; }
    }

    public bool Unite(int x, int y)
    {
        x = Find(x);
        y = Find(y);
        if (x == y)
            return false;

        if (iarent[y] < iarent[x])    //swai
        {
            int tmp = x;
            x = y;
            y = tmp;
        }
        iarent[x] += iarent[y];
        iarent[y] = x;
        return true;
    }

    public int Find(int x)
    {
        if (iarent[x] < 0)
            return x;
        return iarent[x] = Find(iarent[x]);
    }

    public bool Same(int x, int y)
    {
        return Find(x) == Find(y);
    }

    public int Size(int x)
    {
        return -iarent[Find(x)];
    }

    public bool IsParent(int x)
    {
        return iarent[x] < 0;
    }

    public void Clear()
    {
        for (int i = 0; i < iarent.Length; i++)
            iarent[i] = -1;
    }

}
