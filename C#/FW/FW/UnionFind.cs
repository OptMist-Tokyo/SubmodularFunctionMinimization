using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onigiri.FW
{

    /// <summary>
    /// varified by SRM472 DIV1 Medium TwoSidedCards
    /// 0 : サイズを指定して初期化
    /// -1 で初期化。
    /// 初期化に O(len)
    /// 関数は平均 A^-1 (len)
    /// </summary>
    public class UnionFind
    {
        int[] parent;    //UnionFind の本体

        /// <summary>
        /// -1 で初期化
        /// O( n )
        /// </summary>
        /// <param name="len">要素の数</param>
        public UnionFind(int len)
        {
            parent = new int[len];
            for (int i = 0; i < len; i++)
                parent[i] = -1;
        }//Constractor

        /// <summary>
        /// 要素数の取得
        /// </summary>
        public int Length
        {
            get { return parent.Length; }
        }//Length

        /// <summary>
        /// 2 つの要素を繋げる
        /// O( A(n) )
        /// </summary>
        /// <param name="x">繋げられる要素</param>
        /// <param name="y">繋げる要素</param>
        /// <returns>繋げることができたかどうか</returns>
        public bool Unite(int x, int y)
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
        }//Unite

        /// <summary>
        /// 同値類の要素の親を探す
        /// O( A(n) )
        /// </summary>
        /// <param name="x">親を探す要素</param>
        /// <returns>同じ</returns>
        public int Find(int x)
        {
            if (parent[x] < 0)
                return x;
            return parent[x] = Find(parent[x]);
        }//Find

        /// <summary>
        /// 同値類かどうか
        /// O( A(n) )
        /// </summary>
        /// <param name="x">比べられる要素</param>
        /// <param name="y">比べる要素</param>
        /// <returns>true : 同値類</returns>
        public bool Same(int x, int y)
        {
            return Find(x) == Find(y);
        }//Same

        /// <summary>
        /// 同値類の数を数える
        /// O( A(n) )
        /// </summary>
        /// <param name="x">同値類を数える要素</param>
        /// <returns>同値類の数</returns>
        public int Size(int x)
        {
            return -parent[Find(x)];
        }//Size

        /// <summary>
        /// UnionFind の親（根）になっているかの判定
        /// O( 1 )
        /// </summary>
        /// <param name="x">判定するインデックス</param>
        /// <returns>true : 親</returns>
        public bool IsParent(int x)
        {
            return parent[x] < 0;
        }//IsPaarent

        public void Clear()
        {
            for (int i = 0; i < parent.Length; i++)
                parent[i] = -1;
        }


        public UnionFind Copy()
        {
            var res = new UnionFind(parent.Length);
            res.parent = (int[])parent.Clone();
            return res;
        }

    }//UnonFind


}
