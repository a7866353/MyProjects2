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

        abstract public string Reslut
        {
            get;
        }

        abstract public string Description
        {
            get;
        }
    }

    abstract class Float1DTest : BasicTestCase
    {
        protected long _testCount = 1000;
        protected float[] _testData;
        protected float[] _resultData;
        static protected double[] _sampleTestData;
        static protected double[] _sampleResultData;

        private double _errorSum;
        private double[] _errorArr;

        protected string _result = "";
        protected string _description = "";
        private TimeSpan _duration;
        public override void RunTest()
        {
            DateTime time1 = DateTime.Now;
            for (long i = 0; i < _testCount; i++)
            {
                Calculate();
            }
            _duration = DateTime.Now - time1;
            _errorSum = CalculateError();
            _result = "Result "
                + _testName + ": "
                + _duration.TotalMilliseconds + "ms, "
                + _errorSum + "\r\n";
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

        public override void RunTestMP()
        {
            DateTime time1;
            TimeSpan duration;

            time1 = DateTime.Now;
            Parallel.For(0, _testCount, i =>
            {
                Calculate();
            });
            duration = DateTime.Now - time1;
            _errorSum = CalculateError();

            _result = "MP Result "
                + _testName + ": "
                + duration.TotalMilliseconds + "ms, "
                + _errorSum + "\r\n";

        }

        public override string Reslut
        {
            get { return _result; }
        }

        public override string Description
        {
            get { return _description; }
        }
    }

}
