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

            // for Load DLL
            DLlTools.Add(in1, in2, output);


            FloatSumTest();
            // SigmoidTest();
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
            BasicTestCase[] sumTestArr = new BasicTestCase[]
            {
                new FloatSumTest(),
                new FloatSumTest_AVX(),
                new FloatSumTest_AVX4Loop(),
                new FloatSumTest_DLL(),
            };

            ResultTestBlock.Text = "";
            foreach (BasicTestCase test in sumTestArr)
            {
                test.RunTest();
                ResultTestBlock.Text += test.Reslut;

            }
            ResultTestBlock.Text += "\r\n=================\r\n\r\n";

            foreach (BasicTestCase test in sumTestArr)
            {
                test.RunTestMP();
                ResultTestBlock.Text += test.Reslut;
            }

        }
        private void SigmoidTest()
        {
            BasicTestCase[] sumTestArr = new BasicTestCase[]
            {
                new SigmoidTest_DLL(),
                new SigmoidTest(),
                new SigmoidTest_AVX(),
                new SigmoidTest_DLL(),
            };

            foreach (BasicTestCase test in sumTestArr)
            {
                test.RunTest();
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

        [DllImport("CUDATest01.dll", EntryPoint = "ActivationSigmoid_Dll", CallingConvention = CallingConvention.StdCall)]
        private static extern float ActivationSigmoid_Dll(IntPtr inArr, IntPtr outArr, int size);
        public static void DllSigmoid(float[] inArr, float[] outArr)
        {
            IntPtr i1 = Marshal.UnsafeAddrOfPinnedArrayElement(inArr, 0);
            IntPtr o1 = Marshal.UnsafeAddrOfPinnedArrayElement(outArr, 0);
            ActivationSigmoid_Dll(i1, o1, inArr.Length);
        }


        [DllImport("CUDATest01.dll", EntryPoint = "ActivationSigmoid_AVX", CallingConvention = CallingConvention.StdCall)]
        private static extern float ActivationSigmoid_AVX(IntPtr inArr, IntPtr outArr, int size);
        public static void AVXSigmoid(float[] inArr, float[] outArr)
        {
            IntPtr i1 = Marshal.UnsafeAddrOfPinnedArrayElement(inArr, 0);
            IntPtr o1 = Marshal.UnsafeAddrOfPinnedArrayElement(outArr, 0);
            ActivationSigmoid_AVX(i1, o1, inArr.Length);
        }
    }



    class NEATLink
    {
        public int FromNeuron;
        public int ToNeuron;
        public double Weight;
    }
    class NEATNetwork
    {
        public NEATLink[] Links;

        public int OutputIndex;
        public int InputCount;
        public int OutputCount;

        public int ActivationCycles = 4;

        public double[] PostActivation;
        public double[] PreActivation;

        public double[] Compute(double[] input)
        {
            var result = new double[OutputCount];

            // clear from previous
            Fill(PreActivation, 0.0);
            Fill(PostActivation, 0.0);
            PostActivation[0] = 1.0;

            // copy input
            for (int i = 0; i < InputCount; i++)
            {
                PostActivation[i + 1] = input[i];
            }

            // iterate through the network activationCycles times
            for (int i = 0; i < ActivationCycles; ++i)
            {
                InternalCompute();
            }

            // copy output
            for (int i = 0; i < OutputCount; i++)
            {
                result[i] = PostActivation[OutputCount + i];
            }

            return result;
        }
        private void InternalCompute()
        {
            foreach (NEATLink t in Links)
            {
                PreActivation[t.ToNeuron] += PostActivation[t.FromNeuron] * t.Weight;
            }

            for (int j = OutputIndex; j < PreActivation.Length; j++)
            {
                PostActivation[j] = PreActivation[j];
                PostActivation[j] = ActivationSigmoid(PostActivation[j]);
                PreActivation[j] = 0.0F;
            }
        }
        private double ActivationSigmoid(double v)
        {
            return 1.0 / (1.0 + Math.Exp(-4.9 * v));
        }

        private void Fill(double[] dataArr, double value)
        {
            for (int i = 0; i < dataArr.Length; i++)
                dataArr[i] = value;
        }
    }
    class NetworkFloatDllTools
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Link
        {
            public int toNeuron;
            public int fromNeuron;
            public float weight;
        };
        [StructLayout(LayoutKind.Sequential)]
        struct OutputBufferCtrl
        {
            IntPtr pBuf;
            IntPtr pBufStart;
            int length;
        };
        [StructLayout(LayoutKind.Sequential)]
        struct NEATNetworkParm
        {
            public int linkCount;
            public IntPtr link;

            public int neuronCount;
            public int outputIndex;
            public IntPtr preActivation;
            public IntPtr postActivation;

            public int outBufferCtrlCount;
            public IntPtr outBufferCtrl;


            public int inputCount;
            public IntPtr input;

            public int outputCount;
            public IntPtr output;

            public int activationCycles;
        };

        [DllImport("CUDATest01.dll", EntryPoint = "DllTools_NEATNetwork_AVX", CallingConvention = CallingConvention.StdCall)]
        private static extern void DllTools_NEATNetwork_AVX(NEATNetworkParm param);

        private NEATNetworkParm parm;
        private Link[] _linkArr;
        private OutputBufferCtrl[] _outBufferCtrlArr;
        private double[] _outputBuffer;
        public NetworkFloatDllTools(NEATNetwork network)
        {
            parm = new NEATNetworkParm();

            parm.linkCount = network.Links.Length;
            _linkArr = new Link[parm.linkCount];
            int index = 0;
            foreach (NEATLink l in network.Links)
            {
                _linkArr[index].fromNeuron = l.FromNeuron;
                _linkArr[index].toNeuron = l.ToNeuron;
                _linkArr[index].weight = (float)l.Weight;
                index++;
            }
            parm.link = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Link)) * parm.linkCount);
            IntPtr pAddr = parm.link;
            for (int i = 0; i < _linkArr.Length; i++)
            {
                Marshal.StructureToPtr(_linkArr[i], pAddr, false);
                pAddr += Marshal.SizeOf(typeof(Link));
            }

            parm.neuronCount = network.PostActivation.Length;
            parm.outputIndex = network.OutputIndex;
            parm.preActivation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * parm.neuronCount);
            parm.postActivation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * parm.neuronCount);

            parm.outBufferCtrlCount = network.PreActivation.Length - network.OutputIndex;
            _outBufferCtrlArr = new OutputBufferCtrl[parm.outBufferCtrlCount];
            parm.outBufferCtrl = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(OutputBufferCtrl)) * parm.outBufferCtrlCount);
            IntPtr pAddr2 = parm.outBufferCtrl;
            for (int i = 0; i < _outBufferCtrlArr.Length; i++)
            {
                Marshal.StructureToPtr(_outBufferCtrlArr[i], pAddr2, false);
                pAddr += Marshal.SizeOf(typeof(OutputBufferCtrl));
            }

            parm.inputCount = network.InputCount;
            parm.input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.inputCount);

            parm.outputCount = network.OutputCount;
            _outputBuffer = new double[network.OutputCount];
            parm.output = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.outputCount);

            parm.activationCycles = network.ActivationCycles;

        }
        ~NetworkFloatDllTools()
        {
            Marshal.FreeHGlobal(parm.preActivation);
            Marshal.FreeHGlobal(parm.postActivation);
            Marshal.FreeHGlobal(parm.input);
            Marshal.FreeHGlobal(parm.output);
            Marshal.FreeHGlobal(parm.link);
        }

        public double[] Compute(double[] input)
        {
            // parm.link = Marshal.UnsafeAddrOfPinnedArrayElement(_linkArr, 0);
            // parm.input = Marshal.UnsafeAddrOfPinnedArrayElement(_inputBuffer, 0);
            // parm.output = _outputBuffer, 0);

            Marshal.Copy(input, 0, parm.input, input.Length);

            DllTools_NEATNetwork_AVX(parm);

            Marshal.Copy(parm.output, _outputBuffer, 0, _outputBuffer.Length);
            return _outputBuffer;
        }

        public int InputCount
        {
            get { return parm.inputCount; }
        }

        public int OutputCount
        {
            get { return parm.outputCount; }
        }
    }



}
