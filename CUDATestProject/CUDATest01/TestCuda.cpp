#include "Common.h"

#include "cuda_runtime.h"
#include "device_launch_parameters.h"



extern cudaError_t addWithCuda(int *c, const int *a, const int *b, unsigned int size);
EXTERN_C{
	DLLTEST_API void __stdcall TestCuda_Add(int *in1, int *in2, int *output, int size)
	{
		
		// Add vectors in parallel.
		cudaError_t cudaStatus = addWithCuda(output, in1, in2, size);
		if (cudaStatus != cudaSuccess) {
			fprintf(stderr, "addWithCuda failed!");
			return;
		}
		// cudaDeviceReset must be called before exiting in order for profiling and
		// tracing tools such as Nsight and Visual Profiler to show complete traces.
#if 0
		cudaStatus = cudaDeviceReset();
		if (cudaStatus != cudaSuccess) {
			fprintf(stderr, "cudaDeviceReset failed!");
			return;
		}
#endif
		
	}

	DLLTEST_API void __stdcall TestDll_Add(int *in1, int *in2, int *output, int size)
	{

		for (int i = 0; i < size; i++)
		{
			output[i] = in1[i] + in2[i];
		}
	}

	DLLTEST_API void __stdcall TestDllMP_Add(int *in1, int *in2, int *output, int size)
	{
#pragma omp parallel for
		for (int i = 0; i < size; i++)
		{
			output[i] = in1[i] + in2[i];
		}
	}
}