using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUDATestProject
{
    class FloatSumTest : Float1DTest
    {
        static private int _testDataLength = 1024*4;
        static FloatSumTest()
        {
            _sampleResultData = new double[1];
            _sampleTestData = new double[_testDataLength];

            double sum = 0;
            for (int i = 0; i < _sampleTestData.Length; i++)
            {
                _sampleTestData[i] = i;
            }

            for (int i = 0; i < _sampleTestData.Length; i++)
            {
                sum += _sampleTestData[i];
            }
            _sampleResultData[0] = sum;
        }

        public FloatSumTest()
        {
            _testName = "FloatSum" + "C#";
            _testCount = 10000;
            _description = "[FloatSum] " +
                "DataLength:" + _testDataLength +
                ", TestCount:" + _testCount +
                "\r\n";

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
    class FloatSumTest_AVXPack : FloatSumTest
    {
        public FloatSumTest_AVXPack()
        {
            _testName = "FloatSum " + "AVXPack";
        }

        protected override float Calculate(float[] dataArr)
        {
            return AVXTools.AVXPackSum(dataArr);
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
    class FloatSumTest_CUDA : FloatSumTest
    {
        public FloatSumTest_CUDA()
        {
            _testName = "FloatSum" + "CUDA";
        }

        protected override float Calculate(float[] dataArr)
        {
            return AVXTools.CUDASum(dataArr);
        }
    }
    class FloatSumTest_CUDA2 : FloatSumTest
    {
        private IntPtr _inPtr;
        private IntPtr _outPtr;

        private float[] _outArr;
        public FloatSumTest_CUDA2()
        {
            _testName = "FloatSum" + "CUDA_V2" ;
            int dataSize = _sampleTestData.Length * sizeof(float);
            _inPtr = CUDATools.Malloc(dataSize);
            _outPtr = CUDATools.Malloc(dataSize);
            _outArr = new float[1];
        }

        ~FloatSumTest_CUDA2()
        {
            CUDATools.Free(_inPtr);
            CUDATools.Free(_outPtr);
        }
        protected override float Calculate(float[] dataArr)
        {
            CUDATools.SetValue(_inPtr, dataArr);
            CUDAKernelTools.SumFloat(_inPtr, _outPtr, dataArr.Length);
            CUDATools.GetValue(_outPtr, _outArr);
            return _outArr[0];
        }
    }
    class FloatSumTest_CUDA3 : FloatSumTest
    {
        private IntPtr _inPtr;
        private IntPtr _outPtr;
        private bool _isFirst;

        private float[] _outArr;
        public FloatSumTest_CUDA3()
        {
            _testName = "FloatSum" + "CUDA_V3";
            int dataSize = _sampleTestData.Length * sizeof(float);
            _inPtr = CUDATools.Malloc(dataSize);
            _outPtr = CUDATools.Malloc(dataSize);
            _outArr = new float[1];
            _isFirst = true;
        }

        ~FloatSumTest_CUDA3()
        {
            CUDATools.Free(_inPtr);
            CUDATools.Free(_outPtr);
        }
        protected override float Calculate(float[] dataArr)
        {
            if (_isFirst == true)
            {
                CUDATools.SetValue(_inPtr, dataArr);
            }
            CUDAKernelTools.SumFloat(_inPtr, _outPtr, dataArr.Length);
            if (_isFirst == true)
            {
                _isFirst = false;
                CUDATools.GetValue(_outPtr, _outArr);
            }
            return _outArr[0];
        }
    }
}
