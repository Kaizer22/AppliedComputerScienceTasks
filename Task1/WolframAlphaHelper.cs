using System;
using System.Collections.Generic;
using Wolfram.Alpha;
using Wolfram.Alpha.Models;


namespace AppliedComputerScienceTasks
{
    class WolframAlphaHelper
    {
        public const string API_KEY = "MY_API_KEY";
        
        private  WolframAlphaService wolfram;
        private  WolframAlphaRequest request;
        private  WolframAlphaResult  result;

        public WolframAlphaHelper()
        {
            wolfram = new WolframAlphaService(API_KEY);
        }

        public string Integrate(string function, string from, string to)
        {
            string res = "";
            string req = "integrate " + function + "dx " + "from " + from + " to " + to;

            request = new WolframAlphaRequest(req)
            {
                Formats = new List<string>
                {
                    Format.Plaintext,
                    Format.MathematicaOutput
                }
            };
            result = wolfram.Compute(request).GetAwaiter().GetResult();
            Console.WriteLine("Вычисляю: " + req);
            if (result.QueryResult.Error != null)
            {
                Console.WriteLine(result.QueryResult.Error.Message);
            }

            if (result != null)
            {
                if (result.QueryResult.Pods != null)
                {
                    foreach (var pod in result.QueryResult.Pods)
                    {
                        if (pod.SubPods != null)
                        {
                            foreach (var subpod in pod.SubPods)
                            {
                                if (pod.Title.Contains("Definite"))
                                    res = subpod.Plaintext.Split('=')[1];//.Replace(" ", "*");
                            }
                        }
                        else
                            res = "SUBPODS ERROR";
                    }
                }
                else
                    Console.WriteLine("NULL PODS");
            }
            else
                res = "UNKNOWN ERROR";
            
            Console.WriteLine("Результат: " + res);
            return res;
        }     
    }
}
