using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUDATestProject
{
    class SigmoidTest : Float1DTest
    {
        static private int _testDataLength = 8192;
        static SigmoidTest()
        {
            _sampleTestData = new double[_testDataLength];
            _sampleResultData = new double[_testDataLength];

            for(int i=0;i<_testDataLength;i++)
            {
                _sampleTestData[i] = ((double)i / (_testDataLength - 1))*2-1;
                _sampleResultData[i] = 1.0 / (1.0 + Math.Exp(-4.9 * _sampleTestData[i]));
            }
        }
        public SigmoidTest()
        {
            _testName = "Sigmoid" + "C#";
            _testCount = 1000;

            _testData = new float[_testDataLength];
            _resultData = new float[_testDataLength];

            for (int i = 0; i < _testDataLength; i++)
            {
                _testData[i] = (float)_sampleTestData[i];
            }
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

        protected override void Calculate()
        {
            Calculate(_testData, _resultData);
        }
    }

    class SigmoidTest_DLL : SigmoidTest
    {
        public SigmoidTest_DLL()
        {
            _testName = "Sigmoid" + "DLL";

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
            _testName = "Sigmoid" + "AVX";
        }

        override protected void Calculate(float[] dataArr, float[] outArr)
        {
            AVXTools.AVXSigmoid(dataArr, outArr);
        }
    }

}
