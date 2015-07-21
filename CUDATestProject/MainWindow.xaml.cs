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
            int testSize = 1024*100;
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


            TestDwtHaar();
            /*
            DLlTools.Add(in1, in2, output);
            DLlTools.AddDll(in1, in2, output);
            DLlTools.AddDllMp(in1, in2, output);
            */
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

}
