using System;
using System.Collections.Generic;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace AppliedComputerScienceTasks
{
    /// ----------- Задание 8 --------------------
    /// Numeric Methods
    /// Поиск решения системы обыкновенных дифференциальных уравнений методами:
    /// последовательных приближений, прогноза и коррекции, Адамса
    /// Для парсинга алгебраических выражений использована библиотека MathNet.Symbolic
    /// Построение графиков осуществляется с помощью библиотеки ZedGraph
    /// Для вычисления неопределенных интегралов используется WolframAlpha API
    /// Полезные ссылки: 
    /// https://jenyay.net/Programming/ZedGraph
    /// https://github.com/xjose97x/WolframAlpha
    /// ------------------------------------------
    /// 
    class NumericSolver
    {
        #region Variables
        //Количество уравнений в системе, кол-во шагов приближения на заданом отрезке, величина шага
        private int equasionCount;
        private int stepsNumber;
        private double step;

        //Система ОДУ
        private SODE diffEq;
        //Вспомогательный класс для работы с WolframAlpha API
        private WolframAlphaHelper integrator;
        //Текущие значения функций-приближений и их значения на прошлом шаге
        private List<string> currentYApproxima;
        private List<string> previousYApproxima;
        //Именованный вектор начальных значений
        private Dictionary<string, FloatingPoint> startValues;
        //Количество приближений
        private const int SUCCESSIVE_APPROXIMATIONS_STEPS = 2;
        //Количество повторений коррекции на каждом шаге
        private const int CORRECTIONS_NUMBER = 100;
        //Точки приближения функций-решений полученные методом Рунге-Кутты 4-го порядка
        private List<Dictionary<string, FloatingPoint>> rungeKuttaVectors;
        #endregion 

        //Конструктор по умолчанию
        public NumericSolver()
        {
            integrator = new WolframAlphaHelper();
            currentYApproxima = new List<string>();
            previousYApproxima = new List<string>();
        }

        //Инициализация экземпляра класса NumericSolver 
        public void InitNumericSolver()
        {
            diffEq = new SODE();
            Console.WriteLine("Внимание! Используйте только функции с берущимся интегралом.");
            diffEq.InitSODE();

            Object[] param = diffEq.GetParams();
            stepsNumber = (int)param[1];
            step = (double)param[2];
            equasionCount = (int)param[3];
            startValues = (Dictionary<string, FloatingPoint>)param[4];

        }
        #region Successive Approximations
        //Вычисление функции-приближения для переменной с заданным номером
        private void CalculateYApproxima(int yNumber)
        {

            string function = diffEq.GetFunction(yNumber);
            string res;
            for (int i = 0; i < equasionCount; i++)
            {
                function = function.Replace("y" + (i + 1), "(" + previousYApproxima[i] + ")");

            }
            res = integrator.Integrate(function, "0", "x");
            currentYApproxima[yNumber] = startValues["y" + (yNumber + 1)].RealValue + "+" + res;
        }

        //Вычисление функций-приближений методом последовательных приближений
        public List<string> SuccessiveApproxiamtions()
        {
            string func;
            string res;
            for (int k = 0; k < equasionCount; k++)
            {
                func = diffEq.GetFunction(k);
                for (int l = 0; l < equasionCount; l++)
                    func = func.Replace("y" + (l + 1), "(" + startValues["y" + (l + 1)].RealValue + ")");
                res = integrator.Integrate(func, "0", "x");
                previousYApproxima.Add(startValues["y" + (k + 1)].RealValue + "+" + res);
                currentYApproxima.Add("");
            }
            for (int i = 1; i < SUCCESSIVE_APPROXIMATIONS_STEPS; i++)
            {
                for (int j = 0; j < currentYApproxima.Count; j++)
                    CalculateYApproxima(j);
                previousYApproxima = new List<string>(currentYApproxima);
                Printer.PrintList(currentYApproxima, "APPROXIMA " + (i + 1));
            }
            InitComputableApproximation();
            return currentYApproxima;
        }

        //Приведение функций к виду, при котором доступно их вычисление средствами библиотеки MathNet
        private void InitComputableApproximation()
        {
            Console.WriteLine("Перепишите полученный результат (функции приближения) еще раз в форме для вычисления " +
                "(без пробелов, со знаком умножения -*-, со скобками, где это необходимо (коэффициенты с минусом))");
            for (int i = 0; i < equasionCount; i++) {
                Console.WriteLine("Внимательно перепишите " + currentYApproxima[i] + " в указанной форме");
                currentYApproxima[i] = Console.ReadLine();
            }
        }

        //Получение точек функций-приближений на заданном пользователем отрезке
        public List<Dictionary<string, FloatingPoint>> GetPoints(){
            List<Dictionary<string, FloatingPoint>> namedVector = new List<Dictionary<string, FloatingPoint>>();
            Dictionary<string, FloatingPoint> vectorPackage = new Dictionary<string, FloatingPoint>();
            double x_i = startValues["x"].RealValue;
            for (int i = 0; i < stepsNumber; i++)
            {
                x_i += step;
                vectorPackage.Add("x", x_i);
                for(int j = 0; j < equasionCount; j++)
                    vectorPackage.Add("y" + (j+1), 
                        Expr.Parse(currentYApproxima[j]).Evaluate(vectorPackage).RealValue);
                
                namedVector.Add(new Dictionary<string, FloatingPoint>(vectorPackage));
                vectorPackage.Clear();
            }
            return namedVector;
        }
        #endregion

        #region Forecast And Correction
        //Получение точек приближения функций-решений методом прогноза и коррекции
        public List<Dictionary<string, FloatingPoint>> ForecastAndCorrection()
        {
            List<Dictionary<string, FloatingPoint>> namedVector = new List<Dictionary<string, FloatingPoint>>();
            Dictionary<string, FloatingPoint> vectorPackage = new Dictionary<string, FloatingPoint>();
            List<string> variables = diffEq.GetVariables();
            variables.Remove("x");
            double y_n3, y_n2, y_n1, y_n, x_i = startValues["x"].RealValue;
            double forecast, correction;
            for (int i = 0; i < stepsNumber; i++)
            {
                x_i += step;
                vectorPackage.Add("x", x_i);
                foreach (string varName in variables)
                {
                    Console.WriteLine(varName);
                    switch (i)
                    {
                        case 0:
                            y_n3 = rungeKuttaVectors[i][varName].RealValue;
                            y_n2 = rungeKuttaVectors[i + 1][varName].RealValue;
                            y_n1 = rungeKuttaVectors[i + 2][varName].RealValue;
                            y_n = rungeKuttaVectors[i + 3][varName].RealValue;
                            break;
                        case 1:
                            y_n3 = rungeKuttaVectors[i][varName].RealValue;
                            y_n2 = rungeKuttaVectors[i + 1][varName].RealValue;
                            y_n1 = rungeKuttaVectors[i + 2][varName].RealValue;
                            y_n = namedVector[i - 1][varName].RealValue;
                            break;
                        case 2:
                            y_n3 = rungeKuttaVectors[i][varName].RealValue;
                            y_n2 = rungeKuttaVectors[i + 1][varName].RealValue;
                            y_n1 = namedVector[i - 2][varName].RealValue;
                            y_n = namedVector[i - 1][varName].RealValue;
                            break;
                        case 3:
                            y_n3 = rungeKuttaVectors[i][varName].RealValue;
                            y_n2 = namedVector[i - 3][varName].RealValue;
                            y_n1 = namedVector[i - 2][varName].RealValue;
                            y_n = namedVector[i - 1][varName].RealValue;
                            break;
                        default:
                            y_n3 = namedVector[i - 4][varName].RealValue;
                            y_n2 = namedVector[i - 3][varName].RealValue;
                            y_n1 = namedVector[i - 2][varName].RealValue;
                            y_n = namedVector[i - 1][varName].RealValue;
                            break;
                    }
                    forecast =
                        y_n3 + (4.0 / 3.0) * step * (2 * y_n - y_n1 + 2 * y_n2);
                    correction =
                        y_n1 + (1.0 / 3.0) * step * (forecast + 4 * y_n + y_n1);
                    vectorPackage.Add(varName, correction);

                }
                namedVector.Add(new Dictionary<string, FloatingPoint>(vectorPackage));
                vectorPackage.Clear();
            }
            return namedVector;
        }
        #endregion

        #region Adams
        //Получение точек приближения функций-решений методом Адамса
        public List<Dictionary<string, FloatingPoint>> Adams()
        {
            List<Dictionary<string, FloatingPoint>> namedVector = new List<Dictionary<string, FloatingPoint>>();
            Dictionary<string, FloatingPoint> vectorPackage = new Dictionary<string, FloatingPoint>();
            List<string> variables = diffEq.GetVariables();

            double y_n3, y_n2, y_n1, y_n, x_i = startValues["x"].RealValue;
            double y_i;

            for (int i = 0; i < stepsNumber; i++)
            {
                x_i += step;
                vectorPackage.Add("x", x_i);
                foreach (string varName in variables)
                {
                    Console.WriteLine(varName);
                    switch (i)
                    {
                        case 0:
                            y_n3 = rungeKuttaVectors[i][varName].RealValue;
                            y_n2 = rungeKuttaVectors[i + 1][varName].RealValue;
                            y_n1 = rungeKuttaVectors[i + 2][varName].RealValue;
                            y_n = rungeKuttaVectors[i + 3][varName].RealValue;
                            break;
                        case 1:
                            y_n3 = rungeKuttaVectors[i][varName].RealValue;
                            y_n2 = rungeKuttaVectors[i + 1][varName].RealValue;
                            y_n1 = rungeKuttaVectors[i + 2][varName].RealValue;
                            y_n = namedVector[i - 1][varName].RealValue;
                            break;
                        case 2:
                            y_n3 = rungeKuttaVectors[i][varName].RealValue;
                            y_n2 = rungeKuttaVectors[i + 1][varName].RealValue;
                            y_n1 = namedVector[i - 2][varName].RealValue;
                            y_n = namedVector[i - 1][varName].RealValue;
                            break;
                        case 3:
                            y_n3 = rungeKuttaVectors[i][varName].RealValue;
                            y_n2 = namedVector[i - 3][varName].RealValue;
                            y_n1 = namedVector[i - 2][varName].RealValue;
                            y_n = namedVector[i - 1][varName].RealValue;
                            break;
                        default:
                            y_n3 = namedVector[i - 4][varName].RealValue;
                            y_n2 = namedVector[i - 3][varName].RealValue;
                            y_n1 = namedVector[i - 2][varName].RealValue;
                            y_n = namedVector[i - 1][varName].RealValue;
                            break;
                    }
                    y_i = y_n + (step/24)*(55*y_n - 59*y_n1+37*y_n2 - 9*y_n3);
                    vectorPackage.Add(varName, y_i);
                }
                namedVector.Add(new Dictionary<string, FloatingPoint>(vectorPackage));
                vectorPackage.Clear();
            }
            return namedVector;
        }
        #endregion


        #region extra
        //Метод возвращает именованные вектора - точки приближения, полученные методом Рунге-Кутты 4-го порядка
        public List<Dictionary<string, FloatingPoint>> GetRungeKuttaPoints()
        {
            rungeKuttaVectors = diffEq.GetFunctionsSolutionApproximation();
            return rungeKuttaVectors;
        }

        #endregion
    }
}
