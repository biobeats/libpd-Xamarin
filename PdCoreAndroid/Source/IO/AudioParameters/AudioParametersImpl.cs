using System;

namespace PdCore.Android.IO
{
	public abstract class AudioParametersImpl
	{
		public static string TAG = "AudioParameters";
		public abstract bool supportsLowLatency();
		public abstract bool checkOutputParameters(int srate, int nout);
		public abstract bool checkInputParameters(int srate, int nin);
		public abstract int suggestOutputBufferSize(int sampleRate);
		public abstract int suggestInputBufferSize(int sampleRate);
		public abstract int suggestOutputChannels();
		public abstract int suggestInputChannels();
		public abstract int suggestSampleRate();
	}
}

