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
            _sampleResultData = new double[1];
            _sampleTestData = new double[_testDataLength];

            double sum = 0;

            for (int i = 0; i < _sampleTestData.Length; i++)
            {
                _sampleTestData[i] = i;
                sum += i;
            }
            _sampleResultData[0] = sum;
        }

        public FloatSumTest()
        {
            _testName = "FloatSum" + "C#";
            _testCount = 100000;

            _testData = new float[_testDataLength];
            _resultData = new float[_sampleResultData.Length];

            for (int i = 0; i < _testData.Length; i++)
            {
                _testData[i] = (float)_sampleTestData[i];
            }

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
            _resultData[0] = Calculate(_testData);
        }
    }
    class FloatSumTest_AVX : FloatSumTest
    {
        public FloatSumTest_AVX()
        {
            _testName = "FloatSum" + "AVX";
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
            _testName = "FloatSum" + "AVX4Loop";
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
            _testName = "FloatSum" + "DLL";
        }

        protected override float Calculate(float[] dataArr)
        {
            return AVXTools.DllSum(dataArr);
        }
    }
}
