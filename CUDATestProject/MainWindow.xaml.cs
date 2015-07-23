using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CUDATestProject
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int testSize = 4096;
            int[] in1 = new int[testSize];
            int[] in2 = new int[testSize];
            int[] output = new int[testSize];
            for( int i=0;i<testSize;i++)
            {
                in1[i] = i;
                in2[i] = i;
                output[i] = 0;
            }

            DLlTools.Add(in1, in2, output);


            // TestDwtHaar();
            /*
            DLlTools.Add(in1, in2, output);
            DLlTools.AddDll(in1, in2, output);
            DLlTools.AddDllMp(in1, in2, output);
            */


            FloatSumTest();
        }

        private void TestDwtHaar()
        {
            int length = 4096;
            float[] inData = new float[length];
            float[] outData = new float[length];

            for(int i=0;i<length;i++)
            {
                inData[i] = (float)Math.Sin(2*Math.PI * i/length);
            }
            DLlTools.DwtHaar1D(inData, outData);
        }

        private void FloatSumTest()
        {
            FloatSumTest[] sumTestArr = new FloatSumTest[]
            {
                new FloatSumTest(),
                new FloatSumTest_AVX(),
                new FloatSumTest_AVX4Loop(),
                new FloatSumTest_DLL(),
                new FloatSumTest(),
            };

            foreach (FloatSumTest test in sumTestArr)
            {
                test.RunTest();
            }

            foreach (FloatSumTest test in sumTestArr)
            {
                test.RunTestMP();
            }

        }
    }


    class DLlTools
    {
        static TimeSpan durationCuda;
        static TimeSpan durationDll;
        static TimeSpan durationDllMP;


        [DllImport("CUDATest01.dll", EntryPoint = "TestCuda_Add", CallingConvention = CallingConvention.StdCall)]
        private static extern void TestCuda_Add(IntPtr in1, IntPtr in2, IntPtr output, int size);

        public static void Add(int[] in1, int[] in2, int[] output)
        {
            IntPtr i1 = Marshal.UnsafeAddrOfPinnedArrayElement(in1, 0);
            IntPtr i2 = Marshal.UnsafeAddrOfPinnedArrayElement(in2, 0);
            IntPtr out1 = Marshal.UnsafeAddrOfPinnedArrayElement(output, 0);

            DateTime time1 = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                TestCuda_Add(i1, i2, out1, output.Length);
            }
            durationCuda = DateTime.Now - time1;
        }

        [DllImport("CUDATest01.dll", EntryPoint = "TestDll_Add", CallingConvention = CallingConvention.StdCall)]
        private static extern void TestDll_Add(IntPtr in1, IntPtr in2, IntPtr output, int size);
        public static void AddDll(int[] in1, int[] in2, int[] output)
        {
            IntPtr i1 = Marshal.UnsafeAddrOfPinnedArrayElement(in1, 0);
            IntPtr i2 = Marshal.UnsafeAddrOfPinnedArrayElement(in2, 0);
            IntPtr out1 = Marshal.UnsafeAddrOfPinnedArrayElement(output, 0);

            DateTime time1 = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                TestDll_Add(i1, i2, out1, output.Length);
            }
            durationDll = DateTime.Now - time1;
        }

        [DllImport("CUDATest01.dll", EntryPoint = "TestDllMP_Add", CallingConvention = CallingConvention.StdCall)]
        private static extern void TestDllMP_Add(IntPtr in1, IntPtr in2, IntPtr output, int size);
        public static void AddDllMp(int[] in1, int[] in2, int[] output)
        {
            IntPtr i1 = Marshal.UnsafeAddrOfPinnedArrayElement(in1, 0);
            IntPtr i2 = Marshal.UnsafeAddrOfPinnedArrayElement(in2, 0);
            IntPtr out1 = Marshal.UnsafeAddrOfPinnedArrayElement(output, 0);

            DateTime time1 = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                TestDllMP_Add(i1, i2, out1, output.Length);
            }
            durationDllMP = DateTime.Now - time1;
        }

        [DllImport("CUDATest01.dll", EntryPoint = "DwtHaar1D_Work", CallingConvention = CallingConvention.StdCall)]
        private static extern void DwtHaar1D_Work(IntPtr in1, IntPtr output, int size);
        public static void DwtHaar1D(float[] in1, float[] output)
        {
            IntPtr i1 = Marshal.UnsafeAddrOfPinnedArrayElement(in1, 0);
            IntPtr out1 = Marshal.UnsafeAddrOfPinnedArrayElement(output, 0);

            DateTime time1 = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                DwtHaar1D_Work(i1, out1, output.Length);
            }
            durationCuda = DateTime.Now - time1;
        }

    }

    class AVXTools
    {
        [DllImport("CUDATest01.dll", EntryPoint = "Sumfloat_AVX_4loop", CallingConvention = CallingConvention.StdCall)]
        private static extern float Sumfloat_AVX_4loop(IntPtr in1, int size);
        public static float AVXSum4Loop(float[] arr)
        {
            IntPtr i1 = Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0);
            float sum = Sumfloat_AVX_4loop(i1, arr.Length);
            return sum;
        }

        [DllImport("CUDATest01.dll", EntryPoint = "Sumfloat_AVX", CallingConvention = CallingConvention.StdCall)]
        private static extern float Sumfloat_AVX(IntPtr in1, int size);
        public static float AVXSum(float[] arr)
        {
            IntPtr i1 = Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0);
            float sum = Sumfloat_AVX(i1, arr.Length);
            return sum;
        }

        [DllImport("CUDATest01.dll", EntryPoint = "Sumfloat", CallingConvention = CallingConvention.StdCall)]
        private static extern float Sumfloat(IntPtr in1, int size);
        public static float DllSum(float[] arr)
        {
            IntPtr i1 = Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0);
            float sum = Sumfloat(i1, arr.Length);
            return sum;
        }

    }

    class FloatSumTest
    {
        private int _testDataLength = 8192;
        private int _testCount = 100000;

        protected float[] _testData;
        protected string _testName = "C#";

        public FloatSumTest()
        {
            _testData = new float[_testDataLength];
            for (int i = 0; i < _testData.Length; i++)
            {
                _testData[i] = i;
            }

        }
        public void RunTest()
        {
            DateTime time1;
            TimeSpan duration;
            float result=0;

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
        public void RunTestMP()
        {
            DateTime time1;
            TimeSpan duration;
            float result = 0;

            time1 = DateTime.Now;
            Parallel.For(0, _testCount, i=>
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
