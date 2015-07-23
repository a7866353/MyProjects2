using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUDATestProject
{
    class SigmoidTest : BasicTestCase
    {
        private int _testDataLength = 8192;
        private int _testCount = 10000;

        protected float[] _testData;
        protected float[] _resultData;

        public SigmoidTest()
        {
            _testName = "C#";

            _testData = new float[_testDataLength];
            _resultData = new float[_testDataLength];

            for(int i=0;i<_testDataLength;i++)
            {
                _testData[i] = (float)((double)i / (_testDataLength - 1));
            }
        }
        public override void RunTest()
        {
            DateTime time1;
            TimeSpan duration;
            float result = 0;

            time1 = DateTime.Now;
            for (int i = 0; i < _testCount; i++)
            {
                Calculate(_testData, _resultData);
            }
            duration = DateTime.Now - time1;

            System.Console.WriteLine("Result "
                + _testName + ": "
                + duration.TotalMilliseconds + "ms, "
                + result);
        }

        public override void RunTestMP()
        {
            return;
        }
        private float ActivationSigmoid(float v)
        {
            return 1.0f / (1.0f + (float)Math.Exp(-4.9 * v));
        }
        virtual protected void Calculate(float[] dataArr, float[] outArr)
        {
            for (int i = 0; i < dataArr.Length; i++)
                outArr[i] = ActivationSigmoid(dataArr[i]);
        }
    }

    class SigmoidTest_DLL : SigmoidTest
    {
        public SigmoidTest_DLL()
        {
            _testName = "DLL";

        }

        override protected void Calculate(float[] dataArr, float[] outArr)
        {
            AVXTools.DllSigmoid(dataArr, outArr);
        }
    }
    class SigmoidTest_AVX : SigmoidTest
    {
        public SigmoidTest_AVX()
        {
            _testName = "AVX";
        }

        override protected void Calculate(float[] dataArr, float[] outArr)
        {
            AVXTools.AVXSigmoid(dataArr, outArr);
        }
    }

}
