#include "Common.h"
#include "cuda_runtime.h"
#include "device_launch_parameters.h"




EXTERN_C{
	struct NEATLink
	{
		int toNeuron;
		int fromNeuron;
		float weight;
	};

	struct NEATNetworkParm
	{
		int linkCount;
		NEATLink *link;

		int neuronCount;
		int outputIndex;
		float *preActivation;
		float *postActivation;

		int inputCount;
		float *input;

		int outputCount;
		float *output;

		int activationCycles;
	};

	DLLTEST_API void __stdcall NEATNetwork_Work(NEATNetworkParm param)
	{
		// runTest(input, output, size);
	}

}

