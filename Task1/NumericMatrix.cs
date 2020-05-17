using System;
using System.Collections.Generic;

namespace AppliedComputerScienceTasks
{
    class NumericMatrix : Matrix<double>
    {
       

        //Конструктор квадратной матрицы
        public NumericMatrix(int matrix_size) : base(matrix_size) { }
        //Конструктор прямоугольной матрицы
        public NumericMatrix( int height, int width) : base(height, width) { }

        //Инициализация значений матрицы с клавиатуры
        public void ConsoleInput()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.Write("[" + i + "]" + "[" + j + "]    ");
                    elements[i, j] = Convert.ToDouble(Console.ReadLine());
                }
            }
        }

        //Возвращает матрицу, лежащую в прямоугольнике с диагональю x1y1-x2y2
        public new NumericMatrix GetArea(int x1, int y1, int x2, int y2)
        {
            NumericMatrix result = new NumericMatrix(y2 - y1 + 1, x2 - x1 + 1);

            for (int i = y1; i <= y2; i++)
                for (int j = x1; j <= x2; j++)
                    result.SetElement(i - y1, j - x1, elements[i, j]);

            return result;
        }

        public  NumericMatrix ConcatHorizontally(NumericMatrix first, NumericMatrix second)
        {
            int newWidth = first.width + second.width;
            NumericMatrix concatenatedMatrix = new NumericMatrix(first.height, newWidth);

            for(int i = 0; i < first.height; i++)
            {
                for(int j = 0; j < newWidth; j++)
                {
                    if (j < first.width)
                    {
                        concatenatedMatrix.SetElement(i, j, first.GetElement(i,j));
                    }
                    else
                    {
                        concatenatedMatrix.SetElement(i, j, second.GetElement(i, j - first.width));
                    }
                    
                }
            }

            return concatenatedMatrix;
        }

        public NumericMatrix ConcatHorizontally(NumericMatrix first, List<double> second)
        {
            int newWidth = first.width + 1;
            NumericMatrix concatenatedMatrix = new NumericMatrix(first.height, newWidth);

            for (int i = 0; i < first.height; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    if (j < first.width)
                    {
                        concatenatedMatrix.SetElement(i, j, first.GetElement(i, j));
                    }
                    else
                    {
                        concatenatedMatrix.SetElement(i, j, second[i]);
                    }

                }
            }
            return concatenatedMatrix;
        }


        //Умножение матрицы на число
        public void Multiple(double k)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    elements[i, j] = elements[i, j] * k;
                }
            }
        }

        //Метод возвращает матрицу - результат перемножения matrix2 с данной
        public NumericMatrix Multiple(NumericMatrix matrix2)
        {
            if (matrix2.size == size)
            {
                NumericMatrix result = new NumericMatrix(size);
                double buf = 0;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < width; k++)
                        {
                            buf += this.elements[i, k] * matrix2.GetElement(k, j);

                        }
                        result.SetElement(i, j, buf);
                        buf = 0;
                    }
                }

                return result;
            }
            else
            {
                Console.WriteLine("ERROR using NumericMatrix.Multiply(NumericMatrix matrix)");
                return null;
            }
        }

        
       

        //Метод, возвращающий определитель переданной матрицы
        public static double GetDeterminant(NumericMatrix matrix)
        {
            int size = matrix.size;
            if (size == 1) return matrix.GetElement(0,0);

            else if (size == 2)
                return matrix.GetElement(0,0) * matrix.GetElement(1,1) - matrix.GetElement(0, 1) * matrix.GetElement(1, 0);
      
            else
            {
                double det = 0;
                for (int j = 0; j < size; j++)
                    {
                        det += Math.Pow(-1, 0 + j) * matrix.GetElement(0, j) * GetDeterminant(GetMinorMatrixOfElement(matrix, 0, j));
                    }
                return det;
            }
        }

        //Метод, осуществляющий сложение данной матрицы с переданной в метод, в случае если размерность матриц совпадает
        public void Plus(NumericMatrix matrix2)
        {
            if (matrix2.width == width && matrix2.height == height)
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        elements[i, j] += matrix2.GetElement(i, j);
                    }
                }
            }
            else Console.WriteLine("ERROR using NumericMatrix.Plus(NumericMatrix matrix)");
            
        }

        

       //Метод, преобразующий матрицу к треугольному виду и возвращающий ее ранг
        public static int GetRank(NumericMatrix matrix)
        {
            NumericMatrix.ToTriangular(matrix);
            int rank = 0;
            bool is_empty = true;
            for(int i = 0; i < matrix.height; i++)
            {
                for(int j = 0; j < matrix.width; j++)
                {
                    if (matrix.GetElement(i, j) != 0)
                    {
                        is_empty = false;
                        break;
                    }

                }
                if (!is_empty) rank++;
                else break;
                is_empty = true;
                
            }
            return rank;
        }

        //Метод, преобразующий матрицу к треугольному виду путем последовательного вычитания строк
        public static void ToTriangular(NumericMatrix matrix)
        {
            int j = 0;
            int i = 0;
            int h;
            double coeff = 0;
           
            for (; i < matrix.height-1; i++)
            {
                if  (matrix.GetElement(i, j) != 0) matrix.DevideRow(i, matrix.GetElement(i, j));
                for (int k = i + 1; k < matrix.height; k++)
                {
                    if ( matrix.GetElement(i, j) == 0)
                    {
                        h = j;
                        while (matrix.GetElement(i, h) == 0 && h < matrix.width)
                            h++;
                        if (h != matrix.width)
                            coeff = -(matrix.GetElement(k, j) / matrix.GetElement(i, h));

                    }
                    else {
                        coeff = -(matrix.GetElement(k, j) / matrix.GetElement(i, j));
                    }
                   
                    matrix.SumRows(k, i, coeff);
                    //Console.WriteLine(matrix.ToString());
                }
                j++;
                
            }
            if (matrix.GetElement(i, j) != 0) matrix.DevideRow(i, matrix.GetElement(i, j));
            //CheckRowsEquality(matrix);
            matrix.SetType(Type.TRIANGULAR);
        }
        
        private static void CheckRowsEquality(NumericMatrix matrix)
        {
            //double coeff = 
            for (int i = 0; i < matrix.height; i++)
            {
                for (int k = i; k < matrix.height; k++)
                {
                    for (int j = 0; j < matrix.width; j++) { }
          
                

                }
            }
            
        }
        
        //Метод, возвращающий обратную матрицу к переданной в метод матрице
       public static NumericMatrix GetInverseMatrix(NumericMatrix matrix)
       {
            int size = matrix.size;
            NumericMatrix result = new NumericMatrix(size);

            try {
                double det = GetDeterminant(matrix);

                if (det == 0)  throw new ZeroDetException("Исходная матрица вырожденная");
           

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        result.SetElement(j, i, Math.Pow(-1, i + j) * GetDeterminant(GetMinorMatrixOfElement(matrix, i, j)));
                    }
                }
                result.Multiple(1 / det);
                
            }
            catch (ZeroDetException ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
            return result;
        }


        //Метод, возвращающий матрицу минора указанного элемента заданной матрицы matrix
        public static NumericMatrix GetMinorMatrixOfElement(NumericMatrix matrix, int i, int j)
        {
            int size = matrix.size;
            NumericMatrix result = new NumericMatrix(size - 1 );
            int x = 0, y = 0;
            for (int k = 0; k < size; k++)
            {
                if (k != i)
                {
                    for (int t = 0; t < size; t++)
                    {
                        
                        if (t != j)
                        {
                            result.SetElement(y, x, matrix.GetElement(k, t));
                            x++;
                        }
                    }
                    y++;
                    x = 0;
                }
                
            }
            Console.WriteLine(result.ToString());
            return result;

        }

       

       
        // Метод, производящий элементарное преобразование матрицы - сложение строк с умножением на коэффициент
        public void SumRows(int row_number_to, int row_number_which, double coeff = 1)
        {
            for (int j = 0; j < width; j++)
                elements[row_number_to,j] += elements[row_number_which,j] * coeff;
            
        }

        //Деление строки на заданное число
        public void DevideRow(int row_number, double num)
        {
            for (int j = 0; j < width; j++)
                elements[row_number, j] /= num;
            
        }


        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < height; i++)
            {
                result += "|";

                for (int j = 0; j < width; j++)
                    result += string.Format("{0,9}", Math.Round(elements[i, j], 2));  
              
                result += "|";
                result +="\n";
            }

            return result;
        } 
    }
}
