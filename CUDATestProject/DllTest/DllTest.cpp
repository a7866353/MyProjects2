// DllTest.cpp : ���� DLL Ӧ�ó���ĵ���������
//

#include "stdafx.h"
#include "DllTest.h"


// ���ǵ���������һ��ʾ��
DLLTEST_API int nDllTest=0;

// ���ǵ���������һ��ʾ����
DLLTEST_API int fnDllTest(void)
{
	return 42;
}

// �����ѵ�����Ĺ��캯����
// �й��ඨ�����Ϣ������� DllTest.h
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