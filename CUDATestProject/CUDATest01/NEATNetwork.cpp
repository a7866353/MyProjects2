#include "Common.h"
#include <immintrin.h>
#include <math.h>

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
			pIn += D_DATA_BLOCK;
		}

		for (int i = 0; i < cntRem; i++)
		{
			*pOut = ActivationSigmoid(*pIn);
			pOut++;
			pIn++;
		}		
	}
	DLLTEST_API void __stdcall ActivationSigmoid_AVXPack(const float *inArr, float *outArr, int cntbuf)
	{
#define D_DATA_BLOCK 8
		int cntBlock = cntbuf / D_DATA_BLOCK;
		int cntRem = cntbuf % D_DATA_BLOCK;

		const float *pIn = inArr;
		float *pOut = outArr;
		__declspec(align(16)) const float f49[] = { -4.9f, -4.9f, -4.9f, -4.9f, -4.9f, -4.9f, -4.9f, -4.9f };
		__declspec(align(16)) const float f10[] = { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };

		__m256 p0Data = _mm256_load_ps(f49);
		__m256 p1Data = _mm256_load_ps(f10);

		for (int i = 0; i < cntBlock; i++)
		{
			// -4.9f * v
			__m256 inData = _mm256_loadu_ps(pIn);
			__m256 d1 = _mm256_mul_ps(inData, p0Data);

			// exp(-4.9f * v)
			float *p1 = (float *)&d1;
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
			pIn += D_DATA_BLOCK;
		}

		for (int i = 0; i < cntRem; i++)
		{
			*pOut = ActivationSigmoid(*pIn);
			pOut++;
			pIn++;
		}
	}

	struct NEATLink
	{
		int toNeuron;
		int fromNeuron;
		float weight;
	};
	struct OutputBufferCtrl
	{
		float *pBuf;
		float *pBufStart;
		int length;
	};
	struct NEATNetworkAVXParm
	{
		int linkCount;
		NEATLink *link;

		int neuronCount;
		int outputIndex;
		float *preActivation;
		float *postActivation;

		int outBufferCtrlCount;
		OutputBufferCtrl *outBufferCtrl;

		int inputCount;
		double *input;

		int outputCount;
		double *output;

		int activationCycles;
	};

	

#define BLOCK_LENGTH (8)
	static float Sumfloat_AVX_4loop(const float *inArr, int cntbuf)
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


	static void InternalCompute(NEATNetworkAVXParm *parm)
	{
		NEATLink *pLink, *pLinkBlockStart;

		float *outputArr;
		int outputArrStride = parm->neuronCount - parm->outputIndex;
		float **outputPtrArr;

		OutputBufferCtrl *pOutBufferCtrl;


		float postActArr[BLOCK_LENGTH];
		float weightArr[BLOCK_LENGTH];
		float preArr[BLOCK_LENGTH];

		// Init output buffer control
		for (int i = 0; i < parm->outBufferCtrlCount; i++)
		{
			OutputBufferCtrl *pCtrl = &parm->outBufferCtrl[i];
			pCtrl->pBuf = pCtrl->pBufStart;
			pCtrl->length = 0;
		}

		pLinkBlockStart = pLink = &parm->link[0];
		int cntRem = parm->linkCount;
		int cntBlockLen;

		// update neuron input sum
		while (cntRem>0)
		{
			cntBlockLen = parm->linkCount>BLOCK_LENGTH ? BLOCK_LENGTH : parm->linkCount;
			pLink = pLinkBlockStart;
			for (int j = 0; j < cntBlockLen; j++)
			{
				postActArr[j] = parm->postActivation[pLink->fromNeuron];
				weightArr[j] = pLink->weight;
				pLink++;
			}
			__m256 post = _mm256_loadu_ps(postActArr);
			__m256 weight = _mm256_loadu_ps(weightArr);
			__m256 pre = _mm256_mul_ps(post, weight);
			_mm256_storeu_ps(preArr, pre);

			pLink = pLinkBlockStart;
			for (int j = 0; j < cntBlockLen; j++)
			{
				OutputBufferCtrl *pCtrl = &parm->outBufferCtrl[pLink->toNeuron];
				*(pCtrl->pBuf) = preArr[j];
				pCtrl->pBuf++;
				pCtrl->length++;
			}
			pLinkBlockStart += cntBlockLen;

			cntRem -= cntBlockLen;
		}

		// Calculate output sum
		for (int i = 0; i < parm->outBufferCtrlCount; i++)
		{
			OutputBufferCtrl *pCtrl = &parm->outBufferCtrl[i];
			float result = Sumfloat_AVX_4loop(pCtrl->pBufStart, pCtrl->length);
			parm->preActivation[parm->outputIndex + i] = result;
		}

		// update neuron output
		for (int i = parm->outputIndex; i < parm->neuronCount; i++)
		{
			parm->postActivation[i] = parm->preActivation[i];
			parm->postActivation[i] = ActivationSigmoid(parm->postActivation[i]);
			parm->preActivation[i] = 0.0;
		}
	}

	static void Compute(NEATNetworkAVXParm *parm)
	{
		// clear
		for (int i = 0; i < parm->neuronCount; i++)
		{
			parm->preActivation[i] = 0.0;
			parm->postActivation[i] = 0.0;
		}
		parm->postActivation[0] = 1.0;

		// copy input
		for (int i = 0; i < parm->inputCount; i++)
		{
			parm->postActivation[i + 1] = (float)parm->input[i];
		}

		// iterate through the network activationCycles times
		for (int i = 0; i < parm->activationCycles; i++)
		{
			InternalCompute(parm);
		}

		// copy output
		for (int i = 0; i < parm->outputCount; i++)
		{
			parm->output[i] = parm->postActivation[parm->outputIndex + i];
		}
	}

	DLLTEST_API void __stdcall DllTools_NEATNetwork_AVX(NEATNetworkAVXParm param)
	{
		Compute(&param);
	}
}