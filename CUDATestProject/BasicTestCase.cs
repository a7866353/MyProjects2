using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUDATestProject
{
    abstract class BasicTestCase
    {
        protected string _testName = "C#";
        abstract public void RunTest();
        abstract public void RunTestMP();
    }
}
