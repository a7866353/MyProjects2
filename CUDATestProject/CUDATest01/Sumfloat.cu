#include "Common.h"
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

bool  getLevels(unsigned int len, unsigned int *levels);

__global__ void
Sumfloat_Kernel(const float *id, float *od, const int size)
{
	extern __shared__ float tmp[];
	for (int i = 1; i < blockDim.x; i <<= 1){
		if (threadIdx.x % (i << 1) == i){
			tmp[threadIdx.x - i] += tmp[threadIdx.x];
		}
		__syncthreads();
	}

	if (threadIdx.x == 0)    od[0] = tmp[0];
}




EXTERN_C{
	DLLTEST_API float __stdcall Sumfloat_CUDA(const float *inArr, int cntbuf)
	{
		float s = 0.0;
		float odata[1];
		float *d_idata = NULL;
		float *d_odata = NULL;
		unsigned int dlevels_left = 0;
		unsigned int dlevels_step = 0;

		getLevels(cntbuf, &dlevels_left);
		const unsigned int smem_size = sizeof(float)* cntbuf;
		cudaMalloc((void **)&d_idata, smem_size);
		cudaMalloc((void **)&d_odata, smem_size);
		// copy input data to device
		cudaMemcpy(d_idata, inArr, smem_size, cudaMemcpyHostToDevice);


		unsigned int num_threads_total_left = cntbuf / 2;

		dim3  block_size;
		dim3  grid_size;

		if (dlevels_left <= 10)
		{
			// decomposition can be performed at once
			block_size.x = num_threads_total_left;
		}
		else
		{
			// 512 threads per block
			grid_size.x = (num_threads_total_left / 512);
			block_size.x = 512;

			// 512 threads corresponds to 10 decomposition steps
			dlevels_step = 10;
			dlevels_left -= 10;

		}
		while (0 != num_threads_total_left)
		{
			// double the number of threads as bytes
			unsigned int mem_shared = (2 * block_size.x) * sizeof(float);

			// run kernel
			Sumfloat_Kernel << <grid_size, block_size, mem_shared >> >(d_idata, d_odata, cntbuf);
			// Copy approx_final to appropriate location

			// update level variables
			if (dlevels_left < 10)
			{
				// approx_final = d_odata;
			}

			// more global steps necessary
			dlevels_step = (dlevels_left > 10) ? dlevels_left - 10 : dlevels_left;
			dlevels_left -= 10;

			// after each step only half the threads are used any longer
			// therefore after 10 steps 2^10 less threads
			num_threads_total_left = num_threads_total_left >> 10;

			// update block and grid size
			grid_size.x = (num_threads_total_left / 512)
				+ (0 != (num_threads_total_left % 512)) ? 1 : 0;

			if (grid_size.x <= 1)
			{
				block_size.x = num_threads_total_left;
			}


		}
		cudaMemcpy(odata, d_odata, sizeof(float)* 1,
			cudaMemcpyDeviceToHost);

		return odata[0];
	}


}

