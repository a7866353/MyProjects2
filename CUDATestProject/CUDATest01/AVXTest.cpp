#include "Common.h"
#include <immintrin.h>

EXTERN_C{

	// Calculate 8 float in one time.
	DLLTEST_API float __stdcall Sumfloat_AVX_4loop(const float *inArr, int cntbuf)
	{
		float s = 0;
		int i;
		int nBlockWidth = 8 * 4;
		int cntBlock = cntbuf / nBlockWidth;
		int cntRem = cntbuf % nBlockWidth;
		
		__m256 yfsSum = _mm256_setzero_ps();
		__m256 yfsSum1 = _mm256_setzero_ps();
		__m256 yfsSum2 = _mm256_setzero_ps();
		__m256 yfsSum3 = _mm256_setzero_ps();

		__m256 yfsLoad;
		__m256 yfsLoad1;
		__m256 yfsLoad2;
		__m256 yfsLoad3;

		const float *p = inArr;
		const float *q;

		for (i = 0; i < cntBlock; i++)
		{
			yfsLoad = _mm256_loadu_ps(p);
			yfsLoad1 = _mm256_loadu_ps(p + 8);
			yfsLoad2 = _mm256_loadu_ps(p + 16);
			yfsLoad3 = _mm256_loadu_ps(p + 24);

			yfsSum = _mm256_add_ps(yfsSum, yfsLoad);
			yfsSum1 = _mm256_add_ps(yfsSum1, yfsLoad1);
			yfsSum2 = _mm256_add_ps(yfsSum2, yfsLoad2);
			yfsSum3 = _mm256_add_ps(yfsSum3, yfsLoad3);

			p += nBlockWidth;
		}

		yfsSum = _mm256_add_ps(yfsSum, yfsSum1);
		yfsSum2 = _mm256_add_ps(yfsSum2, yfsSum3);
		yfsSum = _mm256_add_ps(yfsSum, yfsSum2);
		q = (const float*)&yfsSum;
		s = q[0] + q[1] + q[2] + q[3] + q[4] + q[5] + q[6] + q[7];

		for (i = 0; i < cntRem; i++)
		{
			s += p[i];
			p++;
		}

		return s;
	}
	DLLTEST_API float __stdcall Sumfloat_AVX(const float *inArr, int cntbuf)
	{
		float s = 0;
		int i;
		int nBlockWidth = 8;
		int cntBlock = cntbuf / nBlockWidth;
		int cntRem = cntbuf % nBlockWidth;

		__m256 yfsSum = _mm256_setzero_ps();
		__m256 yfsLoad;
		const float *p = inArr;
		const float *q;

		for (i = 0; i < cntBlock; i++)
		{
			yfsLoad = _mm256_loadu_ps(p);
			yfsSum = _mm256_add_ps(yfsSum, yfsLoad);
			p += nBlockWidth;
		}

		q = (const float*)&yfsSum;
		s = q[0] + q[1] + q[2] + q[3] + q[4] + q[5] + q[6] + q[7];

		for (i = 0; i < cntRem; i++)
		{
			s += p[i];
			p++;
		}

		return s;
	}

	DLLTEST_API float __stdcall Sumfloat(const float *inArr, int cntbuf)
	{
		float sum = 0;
		for (int i = 0; i < cntbuf; i++)
			sum += inArr[i];
		return sum;
	}

}