using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedComputerScienceTasks
{
    class LinearProgrammingProblem
    {
        public int varNum { private set; get; }
        public int ineqNum { private set;  get; }

        public NumericMatrix limitations { private set; get; }
        public string mainFunction { private set;  get; }

        

        public void InitProblem()
        {
            Console.WriteLine("Введите количество неизвестных:");
            varNum = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите количество неравенств:");
            ineqNum = Convert.ToInt32(Console.ReadLine());

            limitations = new NumericMatrix(ineqNum, varNum + 1);
            Console.WriteLine("Заполните матрицу для " + ineqNum + " ограничений" +
                "вида: a1x1+a2x2+...anxn <= M.");
            limitations.ConsoleInput();

            Console.WriteLine("Введите функцию для МАКСИМИЗАЦИИ:");
            mainFunction = Console.ReadLine();
        }
    }
}
