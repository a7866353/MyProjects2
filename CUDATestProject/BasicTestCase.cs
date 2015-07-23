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

    abstract class Float1DTest : BasicTestCase
    {
        protected int _testCount = 1000;
        protected float[] _testData;
        protected float[] _resultData;
        static protected double[] _sampleTestData;
        static protected double[] _sampleResultData;

        private double _errorSum;
        private double[] _errorArr;

        private TimeSpan _duration;
        public override void RunTest()
        {
            DateTime time1 = DateTime.Now;
            for (int i = 0; i < _testCount; i++)
            {
                Calculate();
            }
            _duration = DateTime.Now - time1;
            _errorSum = CalculateError();
            System.Console.WriteLine("Result "
                + _testName + ": "
                + _duration.TotalMilliseconds + "ms, "
                + _errorSum);
        }
        abstract protected void Calculate();

        protected double CalculateError()
        {
            double error = 0;
            _errorArr = new double[_sampleResultData.Length];
            for (int i = 0; i < _sampleResultData.Length; i++)
            {
                _errorArr[i] = Math.Abs(_sampleResultData[i] - _resultData[i]);
                error += _errorArr[i];
            }
            error /= _sampleResultData.Length;
            return error;
        }
    }

}
