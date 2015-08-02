using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CUDATestProject
{
    class CUDATools
    {
        [DllImport("CUDATest01.dll", EntryPoint = "CUDA_Malloc", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr Malloc(int size);

        [DllImport("CUDATest01.dll", EntryPoint = "CUDA_Free", CallingConvention = CallingConvention.StdCall)]
        public static extern void Free(IntPtr cudaAddr);

        [DllImport("CUDATest01.dll", EntryPoint = "CUDA_SetValue", CallingConvention = CallingConvention.StdCall)]
        private static extern void SetValue(IntPtr cudaAddr, IntPtr dataArr, int size);
        public static void SetValue(IntPtr cudaAddr, float[] dataArr)
        {
            IntPtr dataPtr = Marshal.UnsafeAddrOfPinnedArrayElement(dataArr, 0);
            SetValue(cudaAddr, dataPtr, sizeof(float) * dataArr.Length);
        }

        [DllImport("CUDATest01.dll", EntryPoint = "CUDA_GetValue", CallingConvention = CallingConvention.StdCall)]
        private static extern void GetValue(IntPtr cudaAddr, IntPtr dataArr, int size);
        public static void GetValue(IntPtr cudaAddr, float[] dataArr)
        {
            IntPtr dataPtr = Marshal.UnsafeAddrOfPinnedArrayElement(dataArr, 0);
            GetValue(cudaAddr, dataPtr, sizeof(float) * dataArr.Length);
        }
    }

    class CUDAKernelTools
    {
        [DllImport("CUDATest01.dll", EntryPoint = "CUDA_SumFloat", CallingConvention = CallingConvention.StdCall)]
        public static extern void SumFloat(IntPtr cudaInData, IntPtr cudaOutData, int dataCount);

    }
}
