using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUDATestProject
{
    class FloatSumTest : BasicTestCase
    {
        private int _testDataLength = 8192;
        private int _testCount = 100000;

        protected float[] _testData;

        public FloatSumTest()
        {
            _testName = "C#";
            _testData = new float[_testDataLength];
            for (int i = 0; i < _testData.Length; i++)
            {
                _testData[i] = i;
            }

        }
        override public void RunTest()
        {
            DateTime time1;
            TimeSpan duration;
            float result = 0;

            time1 = DateTime.Now;
            for (int i = 0; i < _testCount; i++)
            {
                result = Calculate(_testData);
            }
            duration = DateTime.Now - time1;

            System.Console.WriteLine("Result "
                + _testName + ": "
                + duration.TotalMilliseconds + "ms, "
                + result);
        }
        override public void RunTestMP()
        {
            DateTime time1;
            TimeSpan duration;
            float result = 0;

            time1 = DateTime.Now;
            Parallel.For(0, _testCount, i =>
            {
                Calculate(_testData);
            });
            duration = DateTime.Now - time1;

            System.Console.WriteLine("MP Result "
                + _testName + ": "
                + duration.TotalMilliseconds + "ms, "
                + result);

        }

        virtual protected float Calculate(float[] dataArr)
        {
            float sum = 0;
            for (int i = 0; i < dataArr.Length; i++)
                sum += dataArr[i];

            return sum;
        }

    }
    class FloatSumTest_AVX : FloatSumTest
    {
        public FloatSumTest_AVX()
        {
            _testName = "AVX";
        }

        protected override float Calculate(float[] dataArr)
        {
            return AVXTools.AVXSum(dataArr);
        }
    }
    class FloatSumTest_AVX4Loop : FloatSumTest
    {
        public FloatSumTest_AVX4Loop()
        {
            _testName = "AVX4Loop";
        }

        protected override float Calculate(float[] dataArr)
        {
            return AVXTools.AVXSum4Loop(dataArr);
        }
    }

    class FloatSumTest_DLL : FloatSumTest
    {
        public FloatSumTest_DLL()
        {
            _testName = "DLL";
        }

        protected override float Calculate(float[] dataArr)
        {
            return AVXTools.DllSum(dataArr);
        }
    }
}
