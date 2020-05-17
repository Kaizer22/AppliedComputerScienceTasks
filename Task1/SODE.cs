using System;
using System.Collections.Generic;
using System.Linq;
using Expr = MathNet.Symbolics.SymbolicExpression;
using MathNet.Symbolics;

namespace AppliedComputerScienceTasks
{
    class SODE
    {
        /// ----------- Задание 7 --------------------
        /// System of ordinary differencial equation  
        /// Поиск приближенного решения системы обыкновенных дифференциальных уравнений методом Рунге-Кутта 4-ого порядка
        /// Для парсинга алгебраических выражений использована библиотека MathNet.Symbolic
        /// Построение графиков осуществляется с помощью библиотеки ZedGraph
        /// Полезные ссылки: https://jenyay.net/Programming/ZedGraph
        /// ------------------------------------------
        /// 

        #region variables

        //Кол-во уравнений в системе
        private int eqNumber;
        //ОДУ, составляющие систему
        private List<ODE> equasions;
        //Точное решение для каждой переменной y1, y2, ... yn, если известно
        private Dictionary<string, Expr> accurateSolution;

        //Кол-во переменных в системе
        private int varNumber;
        //Наименования переменных системы
        private List<string> variables;

        //Список точек,значений приближенного решения системы
        private List<Dictionary<string, FloatingPoint>> functionsSolutionsApproximations;
        //Список точек,значений точного решения системы
        private List<Dictionary<string, FloatingPoint>> accurateSolutionsNumbers;
        //Локальные ошибки
        private List<Dictionary<string, FloatingPoint>> localErrors;
        //Глобальные ошибки
        private Dictionary<string, FloatingPoint> globalError;

        //Длина интервала приближения, количество шагов приближения, длина шага
        private double interval;
        private int stepsNumber;
        private double step;
        //Начальная точка приближения
        private Dictionary<string, FloatingPoint> startVector;

        //Вспомогательные значаения К1-4
        private double[] k_1, k_2, k_3, k_4;
        #endregion variables


        //Конструктор по умолчанию
        public SODE()
        {
            variables = new List<string>();
            equasions = new List<ODE>();
            startVector = new Dictionary<string, FloatingPoint>();
            functionsSolutionsApproximations = new List<Dictionary<string, FloatingPoint>>();
            accurateSolutionsNumbers = new List<Dictionary<string, FloatingPoint>>();
            accurateSolution = new Dictionary<string, Expr>();
            localErrors = new List<Dictionary<string, FloatingPoint>>();
            globalError = new Dictionary<string, FloatingPoint>();
        }

        //Инициализация СОДУ с клавиатуры
        public void InitSODE()
        {
            Console.WriteLine("Введите количество обыкновенных дифференциальных уравнений в системе:");
            eqNumber = Convert.ToInt32(Console.ReadLine());

            int buf;
            for (int i = 0; i < eqNumber; i++)
            {
                Console.WriteLine("Введите ОДУ строго в виде yk'=f(x, y1, y2 ... yn), " +
                    "где K принадлежит 1, 2, ... n. n  - количество уравнений в системе:");
                equasions.Add(new ODE(Console.ReadLine()));
                buf = equasions.Last().GetVariables().Count();
                if (buf > varNumber)
                {
                    varNumber = buf;
                    //variables.Clear();
                    foreach (Expr e in equasions.Last().GetVariables())
                        if (!variables.Contains(e.ToString()))
                            variables.Add(e.ToString());
                }
            }

            Console.WriteLine("Введите начальный вектор [x, y1, y2 ... yn]:");
            for (int i = 0; i < varNumber; i++)
            {
                Console.WriteLine(variables[i]);
                startVector.Add(variables[i], Convert.ToDouble(Console.ReadLine()));
            }

            k_1 = new double[varNumber - 1];
            k_2 = new double[varNumber - 1];
            k_3 = new double[varNumber - 1];
            k_4 = new double[varNumber - 1];

            Console.WriteLine("Введите длину интервала приближения (по x):");
            interval = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Введите количество шагов приближения (по x):");
            stepsNumber = Convert.ToInt32(Console.ReadLine());

            step = interval / stepsNumber;
        }

        #region calculations
        //Получение точек приближенного решения системы ОДУ методом Рунге-Кутты 4-го порядка на заданном промежутке с заданным шагом
        public List<Dictionary<string, FloatingPoint>> GetFunctionsSolutionApproximation()
        {
            functionsSolutionsApproximations.Clear();

            Dictionary<string, FloatingPoint> buf_vector = new Dictionary<string, FloatingPoint>(startVector);
            functionsSolutionsApproximations.Add(new Dictionary<string, FloatingPoint>(buf_vector));

            for (int i = 0; i < stepsNumber; i++)
            {
                for (int j = 0; j < varNumber - 1; j++)
                    k_1[j] = equasions[j].F(buf_vector) * step;

                buf_vector["x"] = buf_vector["x"].RealValue + step / 2;
                for (int l = 0; l < varNumber - 1; l++)
                    buf_vector["y" + (l + 1)] = buf_vector["y" + (l + 1)].RealValue + k_1[l] / 2;

                for (int j = 0; j < varNumber - 1; j++)
                    k_2[j] = equasions[j].F(buf_vector) * step;

                for (int l = 0; l < varNumber - 1; l++)
                {
                    buf_vector["y" + (l + 1)] = buf_vector["y" + (l + 1)].RealValue - k_1[l] / 2;
                    buf_vector["y" + (l + 1)] = buf_vector["y" + (l + 1)].RealValue + k_2[l] / 2;
                }

                for (int j = 0; j < varNumber - 1; j++)
                    k_3[j] = equasions[j].F(buf_vector) * step;

                buf_vector["x"] = buf_vector["x"].RealValue + step / 2;
                for (int l = 0; l < varNumber - 1; l++)
                {
                    buf_vector["y" + (l + 1)] = buf_vector["y" + (l + 1)].RealValue - k_2[l] / 2;
                    buf_vector["y" + (l + 1)] = buf_vector["y" + (l + 1)].RealValue + k_3[l];
                }

                for (int j = 0; j < varNumber - 1; j++)
                    k_4[j] = equasions[j].F(buf_vector) * step;

                for (int l = 0; l < varNumber - 1; l++)
                {
                    buf_vector["y" + (l + 1)] = buf_vector["y" + (l + 1)].RealValue - k_3[l];
                }
                double bufdelta;
                for (int var = 0; var < varNumber - 1; var++)
                {
                    bufdelta = (1.0 / 6.0) * (k_1[var] + 2 * k_2[var] + 2 * k_3[var] + k_4[var]);
                    buf_vector["y" + (var + 1)] = buf_vector["y" + (var + 1)].RealValue + bufdelta;
                }
                functionsSolutionsApproximations.Add(new Dictionary<string, FloatingPoint>(buf_vector));
            }
            return functionsSolutionsApproximations;
        }

        //Получение точек приближенного решения системы ОДУ методом Рунге-Кутты 4-го порядка на заданном промежутке с заданным шагом
        public List<Dictionary<string, FloatingPoint>> GetFunctionsAccurateSolution()
        {
            string pb_answers = "Yeah_yeah_yes_y_Y_Д_ДА_да_Да_дA_Yes_Ага_ага_конечно_Конечно";
            string answer = "NO";
            double x_i = startVector["x"].RealValue;
            Dictionary<string, FloatingPoint> vector_package = new Dictionary<string, FloatingPoint>();


            Console.WriteLine("Вы знаете точное решение системы ОДУ?");
            answer = Console.ReadLine();
            if (pb_answers.Contains(answer))
            {
                accurateSolutionsNumbers.Clear();
                foreach (Expr var in variables)
                {
                    if (!var.ToString().Equals("x"))
                    {
                        Console.WriteLine("Введите функцию вида {0} = f(x) - точное значение переменной {0}" +
                    "(с учетом начальных условий - конкретные значения C1, ... Cn):", var);
                        accurateSolution.Add(var.ToString(), Expr.Parse(Console.ReadLine().Split('=')[1]));
                    }

                }

                vector_package.Add("x", x_i);
                foreach (string var in accurateSolution.Keys)
                    vector_package.Add(var, accurateSolution[var].Evaluate(vector_package).RealValue);

                accurateSolutionsNumbers.Add(new Dictionary<string, FloatingPoint>(vector_package));
                vector_package.Clear();
                for (int i = 0; i < stepsNumber; i++)
                {
                    x_i += step;
                    vector_package.Add("x", x_i);
                    foreach (string var in accurateSolution.Keys)
                        vector_package.Add(var, accurateSolution[var].Evaluate(vector_package).RealValue);

                    accurateSolutionsNumbers.Add(new Dictionary<string, FloatingPoint>(vector_package));
                    vector_package.Clear();
                }
                GetErrors();
            }
            else
                return null;
            return accurateSolutionsNumbers;
        }

        //Вычисление локальных и глобальных ошибок, при наличии точного решения
        public void GetErrors()
        {
            foreach (string var in variables)
                if (!var.Equals("x"))
                    globalError.Add(var, 0.0);

            double buf_error;
            for (int i = 0; i < stepsNumber; i++)
            {
                localErrors.Add(new Dictionary<string, FloatingPoint>());
                foreach (string var in functionsSolutionsApproximations[i].Keys)
                {
                    if (!var.Equals("x"))
                    {
                        buf_error = Math.Abs(functionsSolutionsApproximations[i][var].RealValue -
                        accurateSolutionsNumbers[i][var].RealValue);
                        globalError[var] = globalError[var].RealValue + buf_error;
                        localErrors.Last().Add(var, buf_error);
                    }
                }
            }
        }

        #endregion calcualtions

        #region extra
        public Object[] GetParams()
        {
            return new Object[5] { interval, stepsNumber, step, eqNumber, startVector};
        }

        public string GetFunction(int number)
        {
            //Console.WriteLine(equasions[number].ToString().Split('=')[1]);
            return equasions[number].ToString().Split('=')[1];
        }

        public List<string> GetVariables()
        {
            return variables;
        }

        public override string ToString()
        {
            string format = "[";
            int i = 0 ;
            foreach(Expr v in variables)
            {
                format += "{" + i +"}---";
                i++;
            }
            format += "]";

            string result = Printer.PrintListNamedVectors(functionsSolutionsApproximations, "Приближенные точки решения:");
            result += "\n" + Printer.PrintListNamedVectors(accurateSolutionsNumbers, "Точки точного решения:");
            result += "\n" + Printer.PrintListNamedVectors(localErrors, "Локальные ошибки:");
            result += "\n" + Printer.PrintDictionary(globalError, "Глобальная ошибка по каждой переменной:")+ "\n";

            return result;
        }
        #endregion extra
    }
}
