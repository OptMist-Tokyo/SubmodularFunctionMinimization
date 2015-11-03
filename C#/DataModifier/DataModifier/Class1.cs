using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DataModifier
{
    class Class1
    {
        public static void Main()
        {
            new Class1().Do();
        }

        private void Do()
        {
            string folder = @"C:\Users\onigiri\Dropbox\MasterThesis\result";
            string output = @"C:\Users\onigiri\Dropbox\MasterThesis\ModifiedData";
            Modify(folder, output);
            folder += "2";
            output += "2";
            Modify(folder, output);
        }

        private static void Modify(string folder, string output)
        {
            var files = System.IO.Directory.GetFiles(folder, "*", System.IO.SearchOption.AllDirectories);
            foreach (var item in files)
            {
                var sr = new StreamReader(item);
                var sw = new StreamWriter(output + item.Substring(folder.Length));

                var line = sr.ReadLine();
                line = line.Replace("ExecutionTime", "Execution Time (s)");
                line = line.Replace("Execurtion Time", "Execution Time (s)");
                line = line.Replace("Oracle Call", "Oracle Calls");
                line = line.Replace("BaseCall", "Oracle Calls");
                sw.WriteLine(line);
                int N = -1;
                int cnt = 0;
                int kinds = line.Split('\t').Length;
                var sum = new double[kinds];
                while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                {
                    var tmp = line.Split('\t');

                    if (tmp[0] != N.ToString())
                    {
                        if (N != -1)
                        {
                            WriteData(sw, cnt, kinds, sum, N);
                        }
                        N = int.Parse(tmp[0]);
                        cnt = 0;
                        sum = new double[kinds];
                    }

                    for (int i = 0; i < kinds; i++)
                    {
                        try
                        {
                            sum[i] += double.Parse(tmp[i]);
                        }
                        catch
                        { }
                    }
                    cnt++;
                }

                WriteData(sw, cnt, kinds, sum, N);


                sr.Close();
                sw.Close();
            }
        }

        private static void WriteData(StreamWriter sw, int cnt, int kinds, double[] sum,int n)
        {
            if (n<15)
            {
                return;
            }
            for (int i = 0; i < kinds; i++)
            {
                if (i != 0)
                {
                    sw.Write('\t');
                }
                sw.Write(sum[i] / cnt);
            }
            sw.WriteLine();
        }




    }
}
