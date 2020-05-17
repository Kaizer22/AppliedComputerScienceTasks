using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedComputerScienceTasks
{
    class SLAE
    {
        // Матрица коэффициентов при иксах и расширенная матрица СЛАУ
        NumericMatrix extended_matrix;
        NumericMatrix matrix;

        // Матрица, описывающее общее решение системы, в случае, когда решений бесконечно много
        // Необходима для удобного получения частных решений из общего
        NumericMatrix general_solution;

        // Количество неизвестных и количестов уравнений, составляющих СЛАУ
        int n_x;
        int n_equasions;

        // Ранги расширенной матрицы и матрицы коэффициентов
        int extended_rank;
        int rank;

        // Строковое представление решения для вывода на экран
        private string solution = "NONE";

        private List<double> single_solution;

        public SLAE(){ }
        

        //Конструктор СЛАУ по расширенной матрице (добавлено для Задания 5)
        //Расширенная матрица и матрица коэффициентов сразу же приводятся к треугольному виду; получаем их ранги
        public SLAE(NumericMatrix extended_matrix_of_SLAE)
        {
            extended_matrix = extended_matrix_of_SLAE;
            n_equasions = extended_matrix_of_SLAE.Height();
            n_x = extended_matrix_of_SLAE.Width() - 1;

            matrix = new NumericMatrix(n_equasions, n_x);
            matrix = extended_matrix.GetArea(0, 0, n_x - 1, n_equasions - 1);

            extended_rank = NumericMatrix.GetRank(extended_matrix);
            rank = NumericMatrix.GetRank(matrix);

            single_solution = new List<double>();
            general_solution = new NumericMatrix(n_x, n_x + 1);

        }

        //Инициализация СЛАУ с клавиатуры
        //Расширенная матрица и матрица коэффициентов сразу же приводятся к треугольному виду; получаем их ранги
        public void ConsoleInput()
        {
            Console.WriteLine("Введите количество неизвестных:");
            n_x = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите количество уравнений:");
            n_equasions = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Заполните расширенную матрицу:");
            extended_matrix = new NumericMatrix(n_equasions, n_x + 1);
            extended_matrix.ConsoleInput();
            Console.WriteLine(extended_matrix.ToString());

            matrix = new NumericMatrix(n_equasions, n_x);
            matrix = extended_matrix.GetArea(0,0, n_x-1, n_equasions-1);

            extended_rank = NumericMatrix.GetRank(extended_matrix);
            rank = NumericMatrix.GetRank(matrix);

            single_solution = new List<double>();
            general_solution = new NumericMatrix(n_x, n_x + 1);

        }

        //Метод, осуществлющий построение решения на основе сведений о рангах расширенной матрицы и матрицы коэффициентов
        public void BuildSolution()
        {
            if (rank != extended_rank)
                solution = "Эта СЛАУ не имеет решений";
            else
            {
                if (rank == n_x)
                    BuildSingleSolution();
                else
                    BuildGeneralSolution();
            }
        }

        //Метод, осуществляющий построение общего решения системы
        //Прежде всего, происходит выбор базисных переменных, стоящих "на ступеньках", остальные переменные считаются свободными
        //Далее базисные переменные выражаются через свободные; происходит заполнение матрицы коэффициентов - general_solution
        //Общее решение представляется в строковом виде
        private void BuildGeneralSolution()
        {
            bool[] is_basis = new bool[n_x];
            int[] pos = new int[n_equasions];
            solution = "Данная СЛАУ имеет бесконечное кол-во частных решений, общее же решение: \n " +
                "(";
            for (int i = 0  ; i < n_equasions ; i++)
                for (int j = 0; j < n_x; j++)
                    if (extended_matrix.GetElement(i, j) != 0)
                    {
                        is_basis[j] = true;
                        pos[i] = j; 
                        break;
                    }

            int var_num = rank-1; 
            for (int i = rank - 1; i >= 0; i--)
            {
                for (int j = n_x - 1; j >= 0; j--)
                {
                    if (is_basis[j] && j <= var_num && extended_matrix.GetElement(i, j) != 0)
                    {
                        for (int h = 0; h < n_x; h++)
                        {
                            if (h != j)
                            {
                                if (is_basis[h] && h > var_num)
                                    general_solution.SumRows(j, h, -1 * extended_matrix.GetElement(i, h));
                                else
                                {
                                    general_solution.SetElement(j, h,
                                    general_solution.GetElement(j, h) - (extended_matrix.GetElement(i, h)) / extended_matrix.GetElement(i, j));
                                }
                                
                            }

                        }
                        general_solution.SetElement(j, n_x, general_solution.GetElement(j, n_x) + (extended_matrix.GetElement(i, n_x)) 
                            / extended_matrix.GetElement(i, j));
                        var_num--;
                        Console.WriteLine(general_solution.ToString());
                        break;
                    }
                }
            }

            for (int i = 0; i < n_x; i++)
            {
                if (is_basis[i])
                {
                    solution += "x" + (i + 1) + " = ";
                    for (int j = 0; j < n_x; j++)
                    {
                        if (general_solution.GetElement(i, j) != 0)
                            solution += "x" + (j + 1) + "*(" + general_solution.GetElement(i, j) + ") + ";
                    }
                    if (general_solution.GetElement(i, n_x) != 0)
                        solution += general_solution.GetElement(i, n_x) + " , ";
                    else
                    {
                        solution = solution.Remove(solution.Length - 3);
                        solution += " , ";
                    }
                }
                else
                {
                    solution += "x" + (i + 1) + " , ";

                }
                
            }
            solution = solution.Remove(solution.Length - 3);
            solution += ")";

        }


        //Метод, осуществляющий нахождение единственного решения системы
        //Происходит последовательное вычисление неизвестных
        //Затем решение представляется в виде строки
        private void BuildSingleSolution()
        {
            int k = n_x-1;
            double left_part;
            single_solution.AddRange(new double[n_x]);
          
            
            for (int i = rank-1; i >=0; i--, k--)
            {
                left_part = 0;
                for (int j = n_x-1; j >=0 ; j--)
                    if (j != k)
                        left_part += matrix.GetElement(i, j) * single_solution[j];// Чтобы не заполнять List<> значениями 0 по умолчанию
                                                                                                                         // Ранее использовался double[]
                
                single_solution[i] = (extended_matrix.GetElement(i, n_x ) - left_part)/extended_matrix.GetElement(i, k);
            }
            solution = "Решение данной системы единственно и равно : \n";
            for (int i = 0; i < n_x; i++)
                solution += "x" + (i+1) + " = " + single_solution[i] + ", ";
            solution = solution.Remove(solution.Length - 2);
            
        }

        public List<double> GetSingleSolution()
        {
            return single_solution;
        }

        

        public override string ToString()
        {
            return extended_matrix.ToString() + "\n"
                + matrix.ToString() + "\n"
                + extended_rank + "\n"
                + rank + "\n"
                + solution;
        }
    }
}
