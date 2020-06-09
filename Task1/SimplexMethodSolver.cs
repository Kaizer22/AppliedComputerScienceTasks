using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedComputerScienceTasks
{
    class SimplexMethodSolver
    {
       public static void Solve(LinearProgrammingProblem lpTask)
        {
            NumericMatrix simplexTable = new NumericMatrix(lpTask.ineqNum, lpTask.ineqNum + lpTask.varNum);
            double[] deltaC = new double[lpTask.ineqNum];
            double[] currentResultRow = new double[lpTask.ineqNum + lpTask.varNum + 1];
            double[] AZero = lpTask.limitations.GetRow(lpTask.varNum + 1);


        }



    }
}
