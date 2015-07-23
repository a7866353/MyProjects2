using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUDATestProject
{
    class FloatSumTest : Float1DTest
    {
        static private int _testDataLength = 8192;
        static FloatSumTest()
        {
            _sampleResultData = new double[_testDataLength];
            _sampleTestData = new double[_testDataLength];


            for (int i = 0; i < _sampleTestData.Length; i++)
            {
                _sampleTestData[i] = i;
            }
        }

        public FloatSumTest()
        {
            _testName = "C#";
            _testCount = 100000;

            _testData = new float[_testDataLength];
            _resultData = new float[_testDataLength];

            for (int i = 0; i < _testData.Length; i++)
            {
                _testData[i] = (float)_sampleTestData[i];
            }

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


        protected override void Calculate()
        {
            Calculate(_testData);
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
