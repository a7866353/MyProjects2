#include "Common.h"
#include <math.h>
#include <immintrin.h>

EXTERN_C{
	static inline float ActivationSigmoid(float v)
	{
		return 1.0f / (1.0f + exp(-4.9f * v));
	}
	
	DLLTEST_API void __stdcall ActivationSigmoid_Dll(const float *inArr, float *outArr, int cntbuf)
	{
		for (int i = 0; i < cntbuf; i++)
			outArr[i] = ActivationSigmoid(inArr[i]);
	}
	DLLTEST_API void __stdcall ActivationSigmoid_AVX(const float *inArr, float *outArr, int cntbuf)
	{
#define D_DATA_BLOCK 8
		int cntBlock = cntbuf / D_DATA_BLOCK;
		int cntRem = cntbuf % D_DATA_BLOCK;

		const float *pIn = inArr;
		float *pOut = outArr;
		const float f49[] = { -4.9f, -4.9f, -4.9f, -4.9f, -4.9f, -4.9f, -4.9f, -4.9f };
		const float f10[] = { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };

		__m256 p0Data = _mm256_loadu_ps(f49);
		__m256 p1Data = _mm256_loadu_ps(f10);

		for (int i = 0; i < cntBlock; i++)
		{
			// -4.9f * v
			__m256 inData = _mm256_loadu_ps(pIn);
			__m256 d1 = _mm256_mul_ps(inData, p0Data);

			// exp(-4.9f * v)
			float *p1= (float *)&d1;
			for (int j = 0; j < D_DATA_BLOCK; j++)
			{
				p1[j] = exp(p1[j]);
			}

			// 
			d1 = _mm256_add_ps(d1, p1Data);
			d1 = _mm256_div_ps(p1Data, d1);

			for (int j = 0; j < D_DATA_BLOCK; j++)
			{
				*pOut = p1[j];
				pOut++;
			}
		}

		for (int i = 0; i < cntRem; i++)
		{
			*pOut = ActivationSigmoid(*pIn);
			pOut++;
			pIn++;
		}		
	}
}