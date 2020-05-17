using MathNet.Symbolics;
using System;
using System.Collections.Generic;

namespace AppliedComputerScienceTasks
{
    static class Printer
    {
        public static string PrintList(List<double> values, string TAG, bool borders = true, string format = "val")
        {
            string result = "";
            Console.WriteLine(TAG);
            if (borders) Console.WriteLine("------------------");
            foreach (double value in values)
                Console.WriteLine(value);
            if (borders) Console.WriteLine("------------------");

            return result;
        }

        public static string PrintList(List<string> values, string TAG, bool borders = true, string format = "val")
        {
            string result = "";
            Console.WriteLine(TAG);
            if (borders) Console.WriteLine("------------------");
            foreach (string value in values)
                Console.WriteLine(value);
            if (borders) Console.WriteLine("------------------");

            return result;
        }

        public static string PrintListVectors(List<List<double>> values, string TAG, bool borders = true, 
            string singleStringFormat = "[{0},{1}]",
            int valuePerStringNumber = 2, string borderTemplate = "------------------\n")
        {
            string result = "";
            double[] valuesPerString = new double[valuePerStringNumber];
            result += TAG + "\n";
            object[] stringValues = new object[valuePerStringNumber];
            if (borders) result += borderTemplate;
            foreach (List<double> value in values) {
                value.ToArray().CopyTo(stringValues, 0);
                result += string.Format(singleStringFormat,stringValues) + "\n";
            } 
            if (borders) result += borderTemplate;
            return result;
        }

        public static string PrintListNamedVectors(List<Dictionary<string, FloatingPoint>> values, string TAG, bool borders = true,
           string borderTemplate = "------------------\n")
        {
            string result = "";
            result += TAG + "\n";
            if (borders) result += borderTemplate;
            foreach (Dictionary<string, FloatingPoint> value in values)
            {
                result += "(";
                foreach (string key in value.Keys)
                {
                    result += key + " : ";
                }
                result += ") - ";
                result += "[";
                foreach (FloatingPoint num in value.Values)
                {
                    result += num.RealValue + " : ";
                }
                result += "]" + "\n";
            }
            if (borders) result += borderTemplate;
            return result;
        }


        public static string PrintDictionary(Dictionary<string, FloatingPoint> dictionary, string TAG,
            string borders = "--------------------\n")
        {
            string result = "";
            result += TAG + "\n";
            result += borders;
            foreach (string key in dictionary.Keys)
            {
                result += "{" + key + "} : [" + dictionary[key].RealValue + "] | "; 
            }
            result += "\n" + borders;
            return result;
        }
    }
}
