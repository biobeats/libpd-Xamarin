using System;
using PdCore.Android.IO;

namespace PdCore.Android.IO
{
	public class BasicOpenSLParameters : AudioParametersImpl
	{
		private int inputBufferSize;
		private int outputBufferSize;

		public BasicOpenSLParameters(int inputBufferSize, int outputBufferSize) {
			this.inputBufferSize = inputBufferSize;
			this.outputBufferSize = outputBufferSize;
		}

		public override bool supportsLowLatency() { return false; }
		public override int suggestSampleRate() { return 44100; }
		public override int suggestInputChannels() { return 1; }
		public override int suggestOutputChannels() { return 2; }
		public override int suggestInputBufferSize(int sampleRate) { return inputBufferSize; }
		public override int suggestOutputBufferSize(int sampleRate) { return outputBufferSize; }

		public override bool checkInputParameters(int srate, int nin) {
			return srate > 0 && nin >= 0 && nin <= 2;
		}

		public override bool checkOutputParameters(int srate, int nout) {
			return srate > 0 && nout >= 0 && nout <= 2;
		}
	}
}

