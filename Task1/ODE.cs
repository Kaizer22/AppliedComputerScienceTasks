using MathNet.Symbolics;
using System;
using System.Collections.Generic;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace AppliedComputerScienceTasks
{
    /// ----------- Задание 6 --------------------
    /// Ordinary differencial equation  
    /// Поиск приближенного решения обыкновенных дифференциальных уравнений вида y'=f(x,y) методом Рунге-Кутта
    /// Для парсинга алгебраических выражений использована библиотека MathNet.Symbolic
    /// Построение графиков осуществляется с помощью библиотеки ZedGraph
    /// Полезные ссылки: https://jenyay.net/Programming/ZedGraph
    /// (Класс переделан для задания 7)
    /// ------------------------------------------
    class ODE
    {
        #region variables
        //Обыкновенное дифференциальное уравнение
        private string differencialEquasion;
        //Преобразованная правая часть - f(x,y)
        private Expr function;

        #endregion variables

        //Конструктор по умолчаию 
        public ODE(string diffEq){
            differencialEquasion = diffEq;
            function = Expr.Parse(differencialEquasion.Split('=')[1]);
        }
        //Возвращает наименования переменных в данном ОДУ
        public IEnumerable<Expr> GetVariables() => function.CollectVariables();

        #region functions
        //Раcчет значения правой части ОДУ f(x, y1, y2 ... yn)
        public double F(Dictionary<string, FloatingPoint> vars) {
            FloatingPoint res = function.Evaluate(vars);
            return res.RealValue;
        }
        #endregion functions
        public override string ToString() {
            //Console.WriteLine(differencialEquasion);
            differencialEquasion.Replace("Real", "");
            return differencialEquasion;
        }
    }
}
