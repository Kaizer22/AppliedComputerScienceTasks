using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedComputerScienceTasks
{
    class ZeroDetException : Exception
    {
        public ZeroDetException(string message)
            : base(message)
        {
 
        }
    }
}
