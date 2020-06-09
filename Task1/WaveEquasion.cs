using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exp = MathNet.Symbolics.SymbolicExpression;
using MathNet.Symbolics;
using MathNet.Numerics;

namespace AppliedComputerScienceTasks
{
    class WaveEquasion
    {
        //Значения взяты наобум
        //Текстовое представление волнового уравнения
        private string equasionStringForm = "u\"_dt = a^2*y\"_dx";
        //Начальное условие sigma1
        private string sigma1 = "1.1*x+0.1";
        //Граничные условия
        private string phi1 = "3.1*t+0.4";
        private string phi2 = "2.1*t+0.21";
        //Параметр a
        private double paramA = 1.2;
        //Рассматриваемый участок плоскости
        private double lDistance = 2;
        private double lTime = 2;
        //Смещение по X
        private double h = 2.0/60;
        //Смещение по T
        private double tau = 2.0/60;
        //Количество шагов по X и T
        private int stepsX = 60;
        private int stepsT = 60;

        //Система линейных алгебраических уравнений - задающая каждый узел сетки
        SLAE solution;
        //Матрица с приближенными значениями функции в узлах
        NumericMatrix solutionsField;

        
        //Конструкторы класса
       public WaveEquasion(){}

        public WaveEquasion(string uZeroX, string uZeroT, double paramA,
            double LDistance, double LTime, int stepsX, int stepsT)
        {
            this.sigma1 = uZeroX;
            this.phi1 = uZeroT;
            this.paramA = paramA;
            this.stepsX = stepsX;
            this.stepsT = stepsT;
            this.lDistance = LDistance;
            this.lTime = LTime;
            this.h = lDistance / stepsX;
            this.tau = lTime / stepsT;
        }

        //Методы для вычисления значений функций начальных условии и граничных условий
        private double sigma1Fun(double x)
        {
            Dictionary<string, FloatingPoint> xPackage = new Dictionary<string, FloatingPoint>();
            xPackage.Add("x", x);
            return Exp.Parse(sigma1).Evaluate(xPackage).RealValue;
        }

        private double phi1Fun(double t)
        {
            Dictionary<string, FloatingPoint> tPackage = new Dictionary<string, FloatingPoint>();
            tPackage.Add("t", t);
            return Exp.Parse(phi1).Evaluate(tPackage).RealValue;
        }

        private double phi2Fun(double t)
        {
            Dictionary<string, FloatingPoint> tPackage = new Dictionary<string, FloatingPoint>();
            tPackage.Add("t", t);
            return Exp.Parse(phi2).Evaluate(tPackage).RealValue;
        }

        //Решение поставленной краевой задачи методом конечных разностей
        public NumericMatrix FiniteDifferencesMethod()
        {
            solutionsField = new NumericMatrix(stepsT, stepsX);
            solutionsField.Fill(0);
            NumericMatrix slaeCoeffs = new NumericMatrix(stepsT * stepsX, stepsT * stepsX + 1);
            int currentEquasion = 0;

            //Получить известные u (заданные начальными и граничными условиями) 
            //и заполнить их коэффициенты un : 0,0,...,1([n]),0...=sigma(n*h) в итоговой
            //СЛАУ
            //Начальное условие u=phi1(x) - левая вертикаль сетки
            for (int k = 1; k < stepsT; k++)
            {
                solutionsField.SetElement(k, 0, phi1Fun((double)k * tau));
                
                currentEquasion = k * stepsX;
                slaeCoeffs.SetElement(currentEquasion, currentEquasion,
                    1);
                slaeCoeffs.SetElement(currentEquasion + stepsX - 1, stepsT * stepsX,
                    solutionsField.GetElement(k, 0));
            }

            //Начальное условие u=phi2(x) - правая вертикаль сетки
            for (int k = 1; k < stepsT; k++)
            {
                solutionsField.SetElement(k, stepsT-1, phi2Fun((double)k * h));
                currentEquasion = k * stepsX;
                slaeCoeffs.SetElement(currentEquasion + stepsX - 1, currentEquasion + stepsX - 1, 
                    1);
                slaeCoeffs.SetElement(currentEquasion + stepsX - 1, stepsT * stepsX,
                    solutionsField.GetElement(k, stepsT - 1));
            }

            //Начальное условие u=sigma1(x) - нижняя горизонталь сетки
            for (int k = 0; k < stepsX; k++)
            {
                solutionsField.SetElement(0, k, sigma1Fun((double)k * h));
                currentEquasion = k % stepsX;
                slaeCoeffs.SetElement(currentEquasion, currentEquasion,
                    1);
                slaeCoeffs.SetElement(currentEquasion, stepsT * stepsX,
                    solutionsField.GetElement(0, k));
            }
            //Console.WriteLine(solutionsField.ToString());
            //Явная схема - преобразована в линейное уравнение с известными коэффициентами
            //(2a^2/h^2-2/tau^2) * u[k,j]+
            //+ (1/tau^2) * u[k-1,j]+
            //+ (1/tau^2) * u[k+1,j]-
            //- (a^2/h^2) * u[k,j+1]-
            //- (a^2/h^2) * u[k,j-1]=0
            // 0 + 0
            // + + +
            // 0 + 0
            //Заполнить коэффициенты для внутренней сетки (t = tDistance не трогаем) по явной схеме
            for (int k = 1; k < stepsT-1; k++)
            {
                
                for(int j = 1; j < stepsX-1; j++)
                {
                    currentEquasion = k * stepsX + j;
                    slaeCoeffs.SetElement(currentEquasion, stepsX * (k) + j , 2 * Math.Pow(paramA, 2) / Math.Pow(h, 2)
                        - 2 / Math.Pow(tau, 2));
                    slaeCoeffs.SetElement(currentEquasion, stepsX * (k-1) + j, 1 / Math.Pow(tau, 2));
                    slaeCoeffs.SetElement(currentEquasion, stepsX * (k + 1) + j, 1 / Math.Pow(tau, 2));
                    slaeCoeffs.SetElement(currentEquasion, stepsX * (k) + (j+1), -1 *  Math.Pow(paramA, 2)
                        / Math.Pow(h, 2));
                    slaeCoeffs.SetElement(currentEquasion, stepsX * (k) + (j - 1), -1 * Math.Pow(paramA, 2)
                       / Math.Pow(h, 2));

                }
            }

            //Console.WriteLine(solutionsField.ToString());
            //Неявная схема - преобразована в линейное уравнение с известными коэффициентами
            //(1/tau^2 + 2a^2/h^2)*u[k+1,j] -
            //- (2/tau^2) * u[k,j] +
            //+ (1/tau^2) * u[k-1,j] -
            //- (a^2/h^2) * u[k+1, j+1] -
            //- (a^2/h^2) * u[k+1, j-1] = 0
            // + + +
            // 0 + 0
            // 0 + 0
            //Заполнить коэффициенты на stepsT*t по неявной схеме
            int kT = stepsT - 1;
            for (int j = 1; j < stepsX-1; j++)
            {

                currentEquasion = kT * stepsX + j;
                slaeCoeffs.SetElement(currentEquasion, stepsX * (kT) + j, 1 / Math.Pow(tau, 2)
                    + 2 * Math.Pow(paramA, 2) / Math.Pow(h, 2));
                slaeCoeffs.SetElement(currentEquasion, stepsX * (kT-1) + j, (-2) / Math.Pow(tau, 2));
                slaeCoeffs.SetElement(currentEquasion, stepsX * (kT - 2) + j, 1 / Math.Pow(tau, 2));
                slaeCoeffs.SetElement(currentEquasion, stepsX * (kT-1) + (j + 1), -1 * Math.Pow(paramA, 2)
                    / Math.Pow(h, 2));
                slaeCoeffs.SetElement(currentEquasion, stepsX * (kT) + (j - 1), -1 * Math.Pow(paramA, 2)
                   / Math.Pow(h, 2));

            }


            solution = new SLAE(slaeCoeffs);
            //Console.WriteLine(solution.ToString());
            solution.BuildSolution();
            if (!solution.isGeneralSolutionExists())
            {
                List<double> tempList = solution.GetSingleSolution();
                //Printer.PrintList(tempList, "TEMP_LIST");
                int i = 0;
                foreach(double sol in tempList)
                {
                    //Console.WriteLine((i / stepsX) + " : " + (i % stepsX));
                    solutionsField.SetElement(i / stepsX, i % stepsX, sol);
                    i++;
                }
                return solutionsField ;
            }
            else
            {
                return solutionsField;
            }
        }
    }
}
