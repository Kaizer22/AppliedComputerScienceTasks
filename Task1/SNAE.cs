using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace AppliedComputerScienceTasks
{
    /// ----------- Задание 5 --------------------
    /// System of non-linear algebraic equations 
    /// Метод Ньютона
    /// Для парсинга алгебраических выражений использована библиотека MathNet.Symbolic
    /// Для нахождения производной использована библиотека MathNet.Numeric
    /// ------------------------------------------
    class SNAE
    {
        //Максимальное допустимое кол-во итераций
        private const int MAX_ITERATIONS = 65536;

        //Кол-во уравнений в системе
        int expressionsNumber;
        //Кол-во неизвестных
        int varNumber;

        //Уравнения
        List<String> expressions;
        //Преобразованные уравнения после парсинга
        List<Expr> symbolicExpressions;

        //Столбец свободных членов, по факту не используется
        List<double> freeTerms;

        //Текущая матрица Якоби (производные всех функции по всем переменным)
        Matrix<Expr> currentJakobiMatrix;

        //Числовые значения матрицы Якоби при текущем приближении
        NumericMatrix currentJakobiValues;

        //Начальное приближение, задается пользователем
        List<double> startApproximation;
        //Столбец текущих значений функций (на данной итерации)
        List<double> currentFunctionValue;

        //Список имен переменных после парсинга
        Expr[] variables;

        //Все найденные решения
        List<List<double>> solutions;
        //Кол-во итераций, за которое было найдено кажде решение
        List<int> iterationsCounter;

        //Погрешность вычисления корней
        double epsilon;

        //Конструктор по умолчанию
        public SNAE()
        {
            expressions = new List<String>();
            startApproximation = new List<double>();
            symbolicExpressions = new List<Expr>();
            freeTerms = new List<double>();
            solutions = new List<List<double>>();
            iterationsCounter = new List<int>();
        }

        //Инициализация СНАУ с клавиатуры
        public void InitSNAE()
        {
            int maxValNumber = 0;
            variables = new Expr[0];
            Console.WriteLine("Введите количество уравнений " +
                "(количество неизвестных должно совпадать с кол-вом уравнений или быть меньше): ");
            expressionsNumber = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < expressionsNumber; i++)
            {
                Console.WriteLine("Введите уравнение номер {0}:", i + 1);
                // expressions.Add(Console.ReadLine().Replace(' ', '\0')); TODO исправить удаление лишних пробелов
                freeTerms.Add(Double.Parse(expressions.Last().Split('=')[1]));
                symbolicExpressions.Add(Expr.Parse(expressions.Last().Split('=')[0] + "-" + freeTerms.Last().ToString()));
                Console.WriteLine(symbolicExpressions.Last().ToString());
                if (symbolicExpressions.Last().CollectVariables().ToArray().Length > maxValNumber)
                {
                    variables = symbolicExpressions.Last().CollectVariables().ToArray();
                    maxValNumber = variables.Length;
                }
            }

            varNumber = maxValNumber;
            if (varNumber > expressionsNumber)
                Console.WriteLine("Ошибка! Количество неизвестных больше количества уравнений!");

            Console.WriteLine("Введите точность нахождения корня эпсилон:");
            epsilon = Convert.ToDouble(Console.ReadLine());

            currentJakobiMatrix = new Matrix<Expr>(expressionsNumber, varNumber);
            currentJakobiValues = new NumericMatrix(expressionsNumber, varNumber);
        }

        //Инициализация нового начального приближения
        private void InitApproximation()
        {
            startApproximation.Clear();
            Console.WriteLine("Введите начальное приближение:");
            for (int i = 0; i < varNumber; i++)
            {
                Console.WriteLine(variables[i].ToString() + " = ", i);
                startApproximation.Add(Convert.ToDouble(Console.ReadLine()));
            }
        }


        //Нахождение матрицы Якоби, дифференцирование функций
        private Matrix<Expr> UpdateJakobi()
        {
            Matrix<Expr> jakobi = new Matrix<Expr>(varNumber);

            for (int i = 0; i < varNumber; i++)
            {
                for (int j = 0; j < varNumber; j++)
                {
                    jakobi.SetElement(i, j, symbolicExpressions[i].Differentiate(variables[j]));
                }
            }
            Console.WriteLine(jakobi.ToString());
            return jakobi;
        }

        //Обновление текущих числовых значений матрицы Якоби
        private void UpdateJakobiValues(Dictionary<string, FloatingPoint> vars)
        { 
            for (int i = 0; i < currentJakobiValues.Height(); i++)
            {
                for (int j = 0; j < currentJakobiValues.Width(); j++)
                    currentJakobiValues.SetElement(i, j, currentJakobiMatrix.GetElement(i, j).Evaluate(vars).RealValue);
            }
        }

        //Обновление текущих значений функций
        private List<double> UpdateFunctionsValue(List<double> values)
        {
            List<double> expressions_values = new List<double>();
            Dictionary<string, FloatingPoint> vars = new Dictionary<string, FloatingPoint>();
            int i = 0;

            foreach (Expr val_name in variables)
            {
                vars.Add(val_name.ToString(), values[i]);
                i++;
            }

            foreach (Expr exp in symbolicExpressions)
                expressions_values.Add(exp.Evaluate(vars).RealValue);

            expressions_values.ToString();

            //Console.WriteLine(expressions_values.ToString());
            return expressions_values;
        }

        //Решение СЛАУ : Wjakobi * deltaX = -f(x)
        //Нахождение столбца deltaX
        private List<double> GetLinearSystemSolution(SLAE deltaXIteration){
            deltaXIteration.BuildSolution();
            return deltaXIteration.GetSingleSolution();
        }

        //Решение СНАУ методом Ньютона
        // Находим матрицу Якоби для данной системы уравнений
        // На каждой итерации обновляем числовые значения этой матрицы при неизвестных равных текущему приближению
        // На каждой итерации обновляем числовые значения функций при неизвестных равных текущему приближению
        // На каждой итерации получаем текущее deltaX
        // На каждой итерации корректируем текущее приближение на deltaX
        // Сравниваем максимальное deltaX и epsilon, выходим из цикла
        private List<double> GetNewtonSolution()
       {
            int iterations = 0;
            List<double> solution = startApproximation;
            List<double> deltaX = new List<double>();
            List<double> buffer;
            Dictionary<string, FloatingPoint> vars = new Dictionary<string, FloatingPoint>();
            SLAE deltaXIteration;
            int u;
            currentJakobiMatrix = UpdateJakobi();

            do
            {
                u = 0;
                foreach(Expr var in variables)
                {
                    vars.Add(var.ToString(), solution[u]);
                    u++;
                }
                UpdateJakobiValues(vars);
                //Console.WriteLine(currentJakobiValues.ToString());
                vars.Clear();

                currentFunctionValue = UpdateFunctionsValue(solution);
                currentFunctionValue = currentFunctionValue.Select(p => -p).ToList();
                //ListPrint(currentFunctionValue, "FUNCTION_VALUES");

                deltaXIteration = new SLAE(currentJakobiValues.ConcatHorizontally(currentJakobiValues, currentFunctionValue));
                deltaX = GetLinearSystemSolution(deltaXIteration);
                //Console.WriteLine(deltaXIteration.ToString());
                //ListPrint(deltaX, "DELTA_X");


                
                buffer = deltaX.Select(p => Math.Abs(p)).ToList();
                //ListPrint(buffer, "BUFFER");

                for(int i = 0; i < varNumber; i++)
                {
                    solution[i] += deltaX[i];
                }
                //ListPrint(solution, "SOLUTION");

                iterations++;
                currentFunctionValue = currentFunctionValue.Select(p => -p).ToList();

                if (iterations >= MAX_ITERATIONS)
                {
                    Console.WriteLine("Ошибка! Достигнут предел по итерациям!");
                    break;
                }

                //Console.WriteLine(buffer.Max());
            } while (buffer.Max() > epsilon);
            //TODO: исправить добавление уникальных решений
            if (!solutions.Contains(solution))
            {
                solutions.Add(new List<double>(solution));
                iterationsCounter.Add(iterations);
            }
     
            return solution;
       }

   
        //Построение решения СНАУ
        //В данном случае - методом Ньютона
        public void GetSolution()
        {
            string pb_answers = "Yeah_yeah_yes_y_Y_Д_ДА_да_Да_дA_Yes_Ага_ага_конечно_Конечно";
            string answer = "NO";
            do
            {
                InitApproximation();
                GetNewtonSolution();
                Console.WriteLine(this.ToString());
                Console.WriteLine("Попробовать другое начальное приближение?");
                answer = Console.ReadLine();
            } while (pb_answers.Contains(answer)); 
        }

        //Представление класса SNAE в виде строки вида:
        //--
        //|*уравнения*
        //--
        //*все найденные до этого решения*
        //*кол-во итераций, за которые эти решения найдены*
        
        public override string ToString()
        {
            string output = "";
            int i = 0;
            int epsilonDigitsCount;
            if (epsilon.ToString().Contains(','))
            {
                epsilonDigitsCount = epsilon.ToString().Split(',')[1].Length;
            }
            else
            {
                epsilonDigitsCount = Int32.Parse(epsilon.ToString().Split('-')[1]);
            }
            
            output += "--\n";


            foreach (string expression in expressions)
            {
                output += "| " + expression + "\n";
            }
            output += "--\n Найденные решения:\n";
            foreach(List<double> solution in solutions)
            {
                output += "X_"+ i +" = (";
                foreach(double answer in solution)
                    output += Math.Round(answer, epsilonDigitsCount + 2).ToString() + " ";
                output += ")\n";
                i++;
            }

            output += "Решения найдены за: ";
            foreach(int iter in iterationsCounter)
            {
                output += iter + " ";
            }

            output += " итераций соответственно.";
            return output ;
        }


        //Вывод List<> в консоль; лучше выделить в отдельный класс Printer
        


    }
}
