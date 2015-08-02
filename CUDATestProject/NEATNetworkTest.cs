using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CUDATestProject
{
    class NEATLink
    {
        public int FromNeuron;
        public int ToNeuron;
        public double Weight;
    }
    class NEATNetwork
    {
        public NEATLink[] Links;

        public int OutputIndex;
        public int InputCount;
        public int OutputCount;

        public int ActivationCycles = 4;

        public double[] PostActivation;
        public double[] PreActivation;

        public double[] Compute(double[] input)
        {
            var result = new double[OutputCount];

            // clear from previous
            Fill(PreActivation, 0.0);
            Fill(PostActivation, 0.0);
            PostActivation[0] = 1.0;

            // copy input
            for (int i = 0; i < InputCount; i++)
            {
                PostActivation[i + 1] = input[i];
            }

            // iterate through the network activationCycles times
            for (int i = 0; i < ActivationCycles; ++i)
            {
                InternalCompute();
            }

            // copy output
            for (int i = 0; i < OutputCount; i++)
            {
                result[i] = PostActivation[OutputCount + i];
            }

            return result;
        }
        private void InternalCompute()
        {
            foreach (NEATLink t in Links)
            {
                PreActivation[t.ToNeuron] += PostActivation[t.FromNeuron] * t.Weight;
            }

            for (int j = OutputIndex; j < PreActivation.Length; j++)
            {
                PostActivation[j] = PreActivation[j];
                PostActivation[j] = ActivationSigmoid(PostActivation[j]);
                PreActivation[j] = 0.0F;
            }
        }
        private double ActivationSigmoid(double v)
        {
            return 1.0 / (1.0 + Math.Exp(-4.9 * v));
        }

        private void Fill(double[] dataArr, double value)
        {
            for (int i = 0; i < dataArr.Length; i++)
                dataArr[i] = value;
        }
    }

    class NEATNetworkTest : Float1DTest
    {
        static private int _inputCount = 128 * 5;
        static private int _outputCount = 4;
        static protected NEATNetwork _net;

        static NEATNetworkTest()
        {
            // Create Network
            _net = new NEATNetwork();
            _net.InputCount = _inputCount;
            _net.OutputCount = _outputCount;
            _net.ActivationCycles = 4;
            _net.OutputIndex = _net.InputCount + 1;

            _net.Links = new NEATLink[(int)(_net.InputCount * _net.OutputCount * 1.5)];
            Random rand = new Random();
            for (int i = 0; i < _net.Links.Length; i++)
            {
                _net.Links[i] = new NEATLink()
                {
                    FromNeuron = rand.Next(_net.Links.Length),
                    ToNeuron = rand.Next(_net.Links.Length),
                    Weight = (rand.NextDouble() - 0.5) * 0.02,
                };
            }
            _net.PreActivation = new double[_net.Links.Length];
            _net.PostActivation = new double[_net.Links.Length];


            // Create sampleData;
            _sampleTestData = new double[_inputCount];
            _sampleResultData = new double[_outputCount];

            for (int i = 0; i < _inputCount; i++)
            {
                _sampleTestData[i] = rand.NextDouble() * 2 - 1;
            }

            _sampleResultData = _net.Compute(_sampleTestData);
        }


        public NEATNetworkTest()
        {
            _testName = "NEATNetwork " + "C#";
            _testCount = 1000;
            _description = "[NEATNetwork]"
                + "In:" + _inputCount
                + ", Out:" + _outputCount
                + ", Link: " + _net.Links.Length
                + "\r\n";

            _testData = new float[_inputCount];
            _resultData = new float[_outputCount];

            for (int i = 0; i < _inputCount; i++)
            {
                _testData[i] = (float)_sampleTestData[i];
            }



        }

        public override void RunTestMP()
        {
            return;
        }
        virtual protected void Calculate(float[] dataArr, float[] outArr)
        {
            double[] outData = _net.Compute(_sampleTestData);
            for (int i = 0; i < outData.Length; i++)
                outArr[i] = (float)outData[i];
        }

        protected override void Calculate()
        {
            Calculate(_testData, _resultData);
        }
    }

    class NetworkFloatDllTools
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Link
        {
            public int toNeuron;
            public int fromNeuron;
            public float weight;
        };
        [StructLayout(LayoutKind.Sequential)]
        struct OutputBufferCtrl
        {
            public IntPtr pBuf;
            public IntPtr pBufStart;
            public int length;
        };
        [StructLayout(LayoutKind.Sequential)]
        struct NEATNetworkParm
        {
            public int linkCount;
            public IntPtr link;

            public int neuronCount;
            public int outputIndex;
            public IntPtr preActivation;
            public IntPtr postActivation;

            public int outBufferCtrlCount;
            public IntPtr outBufferCtrl;


            public int inputCount;
            public IntPtr input;

            public int outputCount;
            public IntPtr output;

            public int activationCycles;
        };

        [DllImport("CUDATest01.dll", EntryPoint = "DllTools_NEATNetwork_AVX", CallingConvention = CallingConvention.StdCall)]
        private static extern void DllTools_NEATNetwork_AVX(NEATNetworkParm param);

        private NEATNetworkParm parm;
        private Link[] _linkArr;
        private double[] _outputBuffer;
        private IntPtr _outBuffer;
        public NetworkFloatDllTools(NEATNetwork network)
        {
            parm = new NEATNetworkParm();

            parm.linkCount = network.Links.Length;
            _linkArr = new Link[parm.linkCount];
            int index = 0;
            foreach (NEATLink l in network.Links)
            {
                _linkArr[index].fromNeuron = l.FromNeuron;
                _linkArr[index].toNeuron = l.ToNeuron;
                _linkArr[index].weight = (float)l.Weight;
                index++;
            }
            parm.link = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Link)) * parm.linkCount);
            IntPtr pAddr = parm.link;
            for (int i = 0; i < _linkArr.Length; i++)
            {
                Marshal.StructureToPtr(_linkArr[i], pAddr, false);
                pAddr += Marshal.SizeOf(typeof(Link));
            }

            parm.neuronCount = network.PostActivation.Length;
            parm.outputIndex = network.OutputIndex;
            parm.preActivation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * parm.neuronCount);
            parm.postActivation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * parm.neuronCount);

            parm.outBufferCtrlCount = network.PreActivation.Length;
            parm.outBufferCtrl = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(OutputBufferCtrl)) * parm.outBufferCtrlCount);
            int outBufferLineLength = sizeof(float) * network.PreActivation.Length;
            _outBuffer = Marshal.AllocHGlobal(outBufferLineLength * parm.outBufferCtrlCount);
            IntPtr pAddr2 = parm.outBufferCtrl;
            IntPtr pOutAddr = _outBuffer;
            OutputBufferCtrl bufferCtrl = new OutputBufferCtrl();
            bufferCtrl.length = 0;
            bufferCtrl.pBuf = IntPtr.Zero;
            for (int i = 0; i < parm.outBufferCtrlCount; i++)
            {
                bufferCtrl.pBufStart = pOutAddr;
                Marshal.StructureToPtr(bufferCtrl, pAddr2, false);
                pAddr2 += Marshal.SizeOf(typeof(OutputBufferCtrl));
                pOutAddr += outBufferLineLength;
            }

            parm.inputCount = network.InputCount;
            parm.input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.inputCount);

            parm.outputCount = network.OutputCount;
            _outputBuffer = new double[network.OutputCount];
            parm.output = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.outputCount);

            parm.activationCycles = network.ActivationCycles;

        }
        ~NetworkFloatDllTools()
        {
            Marshal.FreeHGlobal(parm.preActivation);
            Marshal.FreeHGlobal(parm.postActivation);
            Marshal.FreeHGlobal(parm.input);
            Marshal.FreeHGlobal(parm.output);
            Marshal.FreeHGlobal(parm.link);
            Marshal.FreeHGlobal(_outBuffer);
        }

        public double[] Compute(double[] input)
        {
            // parm.link = Marshal.UnsafeAddrOfPinnedArrayElement(_linkArr, 0);
            // parm.input = Marshal.UnsafeAddrOfPinnedArrayElement(_inputBuffer, 0);
            // parm.output = _outputBuffer, 0);

            Marshal.Copy(input, 0, parm.input, input.Length);

            DllTools_NEATNetwork_AVX(parm);

            Marshal.Copy(parm.output, _outputBuffer, 0, _outputBuffer.Length);
            return _outputBuffer;
        }

        public int InputCount
        {
            get { return parm.inputCount; }
        }

        public int OutputCount
        {
            get { return parm.outputCount; }
        }
    }


    class NEATNetworkTest_DLL : NEATNetworkTest
    {
        NetworkFloatDllTools _network;
        public NEATNetworkTest_DLL()
        {
            _testName = "NEATNetwork " + "DLL";
            _network = new NetworkFloatDllTools(_net);
        }

        protected override void Calculate(float[] dataArr, float[] outArr)
        {
            double[] outData = _network.Compute(_sampleTestData);
            for (int i = 0; i < outData.Length; i++)
                outArr[i] = (float)outData[i];

        }
    }

}
