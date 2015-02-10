using MathParserTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmodularDataSet
{
    class Checker
    {

        internal static bool ParseAndCheckOverflow(int n, DataCreatorForm.CheckParseClass data, ValueForm valueForm, string text, out int value)
        {
            value = 0;
            try 
            {
                var parser = new MathParser();
                string replaced = "(" + n.ToString() + ")";
                var formula = valueForm.Value.Replace("#n", replaced).Replace("#N", replaced);
                var tmpValue = parser.Parse(formula);
                if (tmpValue < int.MinValue || tmpValue > int.MaxValue)
                {
                    throw new ArgumentException("Values must be in int range.");
                }//if
                value= (int)Math.Floor(tmpValue+ Parameters.Eps);
            }
            catch
            {
                AppendErrorMessage(data, text);
                return false;
            }
            return true;
        }

        internal static bool ParseAndCheckOverflow(int n, DataCreatorForm.CheckParseClass data, ValueForm valueForm, string text, out double value)
        {
            value = 0;
            try
            {
                var parser = new MathParser();
                string replaced = "(" + n.ToString() + ")";
                var formula = valueForm.Value.Replace("#n", replaced).Replace("#N", replaced);
                var tmpValue = parser.Parse(formula);
                if (double.IsInfinity( tmpValue )||double.IsNaN(tmpValue))
                {
                    throw new ArgumentException("Values must be in double range.");
                }//if
            }
            catch
            {
                AppendErrorMessage(data, text);
                return false;
            }
            return true;
        }


        public static bool ParseAndCheckOverflow(int n, SubmodularDataSet.DataCreatorForm.CheckParseClass data, RangeForm rangeForm, string text, out int min, out int max)
        {
            min = max = 0;
            try
            {
                var parser = new MathParser();
                string replaced = "(" + n.ToString() + ")";
                var minFormula = rangeForm.Min.Replace("#n", replaced).Replace("#N", replaced);
                var maxFormula = rangeForm.Max.Replace("#n", replaced).Replace("#N", replaced);
                var minValue = parser.Parse(minFormula);
                var maxValue = parser.Parse(maxFormula);
                if (minValue < int.MinValue || minValue > int.MaxValue
                    || maxValue < int.MinValue || maxValue > int.MaxValue)
                {
                    throw new ArgumentException("Values must be in int range.");
                }//if
                min = (int)Math.Floor(minValue+ Parameters.Eps);
                max = (int)Math.Floor(maxValue+ Parameters.Eps);
                Creator.GetRangeInt(min, max);
            }
            catch
            {
                AppendErrorMessage(data, text);
                return false;
            }
            return true;
        }

        public static bool ParseAndCheckOverflow(int n, SubmodularDataSet.DataCreatorForm.CheckParseClass data, RangeForm rangeForm, string text, out double min, out double max)
        {
            min = max = 0;
            try
            {
                var parser = new MathParser();
                string replaced = "(" + n.ToString() + ")";
                var minFormula = rangeForm.Min.Replace("#n", replaced).Replace("#N", replaced);
                var maxFormula = rangeForm.Max.Replace("#n", replaced).Replace("#N", replaced);
                min= parser.Parse(minFormula);
                max = parser.Parse(maxFormula);
                if (double.IsInfinity(min)||double.IsNaN(min)||double.IsInfinity(max)||double.IsNaN(max))
                {
                    throw new ArgumentException("Values must be in double range.");
                }//if
                Creator.GetRangeDouble(min, max);
            }
            catch
            {
                AppendErrorMessage(data, text);
                return false;
            }
            return true;
        }


        internal static bool ParseProbAndCheck(int n, DataCreatorForm.CheckParseClass data, ValueForm valueForm, string text, out double prob)
        {
            prob= 0;
            try
            {
                var parser = new MathParser();
                string replaced = "(" + n.ToString() + ")";
                var formula = valueForm.Value.Replace("#n", replaced).Replace("#N", replaced);
                prob = parser.Parse(formula);
                if (prob < 0)
                {
                    throw new ArgumentException("The probability must be nonnegative.");
                }//if
                if (prob>1)
                {
                    prob = 1;
                }//if
            }
            catch
            {
                AppendErrorMessage(data, text);
                return false;
            }
            return true;
        }

        //internal static bool ParseProbAndCheck(int n, DataCreatorForm.CheckParseClass data, RangeForm rangeForm, string text, out double probMin,out double probMax)
        //{
        //    probMin = 0;
        //    probMax = 0;
        //    try
        //    {
        //        var parser = new MathParser();
        //        string replaced = "(" + n.ToString() + ")";
        //        var minFormula = rangeForm.Min.Replace("#n", replaced).Replace("#N", replaced);
        //        var maxFormula = rangeForm.Max.Replace("#n", replaced).Replace("#N", replaced);
        //        probMin = parser.Parse(minFormula);
        //        probMax = parser.Parse(maxFormula);
        //        if (probMin < 0 || probMax > 1)
        //        {
        //            throw new ArgumentException("The probability must be in [0..1]");
        //        }//if
        //        double range = checked(probMax - probMin);
        //        if (range < 0)
        //        {
        //            throw new ArgumentException("The range must be non-negative.");
        //        }//if
        //    }
        //    catch
        //    {
        //        AppendErrorMessage(data, text);
        //        return false;
        //    }
        //    return true;
        //}

        private static void AppendErrorMessage(DataCreatorForm.CheckParseClass data, string text)
        {
            data.dataCreator.LoopOfN = false;
            data.dataCreator.ErrorMessage += "A " + text + " value is incorrent.\r\n";
            data.dataCreatorAll.ErrorMessage += "A " + text + " value of " + data.name + " is incorrent.\r\n";
        }



    }
}
