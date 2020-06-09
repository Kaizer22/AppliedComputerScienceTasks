using MathNet.Symbolics;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Wolfram.Alpha;
using Wolfram.Alpha.Models;
using System.Web.Services.Description;


namespace AppliedComputerScienceTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Введите номер задания (1/3/4/5/7 - (для 6 и 7 задания)/8/91 - решение волнового уравнения/92/10 - wolframAlphaTest):");
            switch (Convert.ToInt32(Console.ReadLine()))
            {
                case 1:
                    TestMatrix();
                    break;
                case 3:
                    TestGraph();
                    break;
                case 4:
                    TestGauss();
                    break;
                case 5:
                    TestNewton();
                    break;
                case 6:
                    TestRungeKuttaSystem();
                    break;
                case 7:
                    TestRungeKuttaSystem();
                    break;
                case 8:
                    TestNumericMethods();
                    break;
                case 92:
                    TestSimplexMethod();
                    break;
                case 91:
                    TestWaveEquasionSolution();
                    break;
                case 10:
                    WolframTestAsync();
                    Console.WriteLine("---------");
                    Console.ReadLine();
                    break;
            }
            
            
        }

        

        private static void TestGauss()
        {

            SLAE equasions = new SLAE();
            equasions.ConsoleInput();
            equasions.BuildSolution();
            Console.WriteLine(equasions.ToString());

            Console.ReadLine();
        }

        private static void TestGraph()
        {
            //Graph g = new Graph(3);
            Graph g1 = new Graph(6, 11);

            //g.InitGraphInc();
            g1.InitGraphAdj();

            //Console.WriteLine(g.ToString());
            Console.WriteLine(g1.ToString());


            AlgoHelper.DFS(g1);
            AlgoHelper.BFS(g1);

            g1.InitWeights();
            AlgoHelper.Dijkstra(g1,0);

            Console.ReadLine();
        }

        private static void TestMatrix()
        {
            NumericMatrix matrix = new NumericMatrix(2,4);
            matrix.Fill(9);
            Console.WriteLine(matrix.ToString());


            NumericMatrix matrix2 = new NumericMatrix(3);
            matrix2.ConsoleInput();
            Console.WriteLine("Определитель:" + NumericMatrix.GetDeterminant(matrix2));
            Console.WriteLine("Обратная матрица:");
            Console.WriteLine(NumericMatrix.GetInverseMatrix(matrix2).ToString());



            Console.ReadLine();
        }

        private static void TestNewton()
        {
            SNAE one = new SNAE();
            one.InitSNAE();
            one.GetSolution();

            Console.WriteLine(one.ToString());
            
            Console.ReadLine();

        }

        private static void TestRungeKutta()
        {
           // ODE one = new ODE();
            //one.InitODE();
            //List<List<double>> a_Points;
            //List<List<double>> s_Points;
            //a_Points = one.GetFunctionSolutionApproximation();
            //s_Points = one.GetFunctionAccurateSolution();
            //Console.WriteLine(one.ToString());
            //TestGraphic(a_Points, s_Points);
           // Console.ReadLine();
            
        }

        private static void TestRungeKuttaSystem()
        {
            SODE sode = new SODE();
            sode.InitSODE();
            List<Dictionary<string, FloatingPoint>> a_Points = sode.GetFunctionsSolutionApproximation();
            List<Dictionary<string, FloatingPoint>> s_Points = sode.GetFunctionsAccurateSolution();
            Console.WriteLine(sode.ToString());

            DrawGraphics(a_Points, s_Points);
            Console.ReadLine();
        }

        private  static void TestNumericMethods()
        {
            NumericSolver task = new NumericSolver();
            task.InitNumericSolver();
            List<string> res = task.SuccessiveApproxiamtions();
            List<Dictionary<string, FloatingPoint>> points = task.GetPoints();
            List<Dictionary<string, FloatingPoint>> rungeKuttaPoints = task.GetRungeKuttaPoints();
            List<Dictionary<string, FloatingPoint>> forecastAndCorrectionPoints = task.ForecastAndCorrection();
            List<Dictionary<string, FloatingPoint>> adamsPoints = task.Adams();

            Console.WriteLine(Printer.PrintList(res, "ФУНКЦИИ-ПРИБЛИЖЕНИЯ РЕШЕНИЙ СИСТЕМЫ ОДУ"));
            Console.WriteLine(Printer.PrintListNamedVectors(points, "ТОЧКИ ПРИБЛИЖЕНИЯ - Метод последовательных приближений -"));
            Console.WriteLine(Printer.PrintListNamedVectors(forecastAndCorrectionPoints, "ТОЧКИ ПРИБЛИЖЕНИЯ - Метод прогноза и коррекции - "));
            Console.WriteLine(Printer.PrintListNamedVectors(adamsPoints, "ТОЧКИ ПРИБЛИЖЕНИЯ - Метод Адамса - "));
            DrawGraphics(points,rungeKuttaPoints, 
                TAG_1: "Методом последовательных приближений для {0}",
                TAG_2: "Методом Рунге-Кутты 4-го порядка для {0}");
            DrawGraphics(forecastAndCorrectionPoints, rungeKuttaPoints,
                TAG_1: "Методом прогноза и коррекции для {0}",
                TAG_2: "Методом Рунге-Кутты 4-го порядка для {0}");
            DrawGraphics(adamsPoints, rungeKuttaPoints,
                TAG_1: "Методом Адамса для {0}",
                TAG_2: "Методом Рунге-Кутты 4-го порядка для {0}");

            Console.ReadLine();
        }

        private static void TestSimplexMethod()
        {

        }

        private static void TestWaveEquasionSolution()
        {
            WaveEquasion wave = new WaveEquasion();
            Console.WriteLine(wave.FiniteDifferencesMethod().ToString());
            Console.ReadLine();
        }

        private static async System.Threading.Tasks.Task WolframTestAsync()
        {
            WolframAlphaService wolfram = new WolframAlphaService(WolframAlphaHelper.API_KEY);
            WolframAlphaRequest request = new WolframAlphaRequest("integrate(2x + x ^ 2 / 12 + (x ^ 2)) dx from 0 to x")
            {
                Formats = new List<string>
                {
                    Format.Plaintext,
                    Format.MathematicaOutput,
                    Format.MathML,
                    Format.MathematicaCell
                }
            };
            WolframAlphaResult result = await wolfram.Compute(request);

           

            string res = "";

            if (result != null)
            {
                foreach (var pod in result.QueryResult.Pods)
                {
                    if (pod.SubPods != null)
                    {
                        Console.WriteLine(pod.Title + "----");
                        foreach (var subpod in pod.SubPods)
                        {
                            Console.WriteLine(subpod.Plaintext);
                            Console.WriteLine(subpod.Minput);
                            Console.WriteLine(subpod.Title);
                        }
                    }
                    else
                        res = "SUBPODS ERROR";
                }
            }
            else
                res = "UNKNOWN ERROR";

            Console.WriteLine(res);
            Console.ReadLine();
        }

        

        [STAThread]
        static void DrawGraphics(List<Dictionary<string, FloatingPoint>> a_Points, 
            List<Dictionary<string, FloatingPoint>> s_Points = null, 
            string TAG_1 = "Приближение решения для {0}",
            string TAG_2 = "Точное решения для {0}")
        {
            Application.EnableVisualStyles();
            Application.Run(new ODEGraphics(a_Points, s_Points, TAG_1, TAG_2)); 
        }
    }
}
