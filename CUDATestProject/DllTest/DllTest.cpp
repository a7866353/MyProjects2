// DllTest.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "DllTest.h"


// 这是导出变量的一个示例
DLLTEST_API int nDllTest=0;

// 这是导出函数的一个示例。
DLLTEST_API int fnDllTest(void)
{
	return 42;
}

// 这是已导出类的构造函数。
// 有关类定义的信息，请参阅 DllTest.h
CDllTest::CDllTest()
{
	return;
}

EXTERN_C{
	DLLTEST_API void __stdcall TestCuda_Add(int *in1, int *in2, int *output, int size)
	{
		for (int i = 0; i < size; i++)
		{
			output[i] = in1[i] + in2[i];
		}
		/*
		// Add vectors in parallel.
		cudaError_t cudaStatus = addWithCuda(output, in1, in2, size);
		if (cudaStatus != cudaSuccess) {
		fprintf(stderr, "addWithCuda failed!");
		return;
		}
		// cudaDeviceReset must be called before exiting in order for profiling and
		// tracing tools such as Nsight and Visual Profiler to show complete traces.
		cudaStatus = cudaDeviceReset();
		if (cudaStatus != cudaSuccess) {
		fprintf(stderr, "cudaDeviceReset failed!");
		return;
		}
		*/
	}
}