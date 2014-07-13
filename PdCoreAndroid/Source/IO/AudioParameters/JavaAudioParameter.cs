using System;
using Android.Media;


namespace PdCore.Android.IO
{
	public class JavaAudioParameters : AudioParametersImpl {

		private static Encoding ENCODING = Encoding.Pcm16bit;
		private static int COMMON_RATE = 8000; // 8kHz seems to work everywhere, including the simulator
		private static int MAX_CHANNELS = 8;
		private int sampleRate;
		private int inputChannels;
		private int outputChannels;

		public JavaAudioParameters() {
			int oc = 0;
			for (int n = 1; n < MAX_CHANNELS; n++) {
				if (checkOutputParameters(COMMON_RATE, n)) oc = n;
			}
			outputChannels = oc;
			int ic = 0;
			for (int n = 0; n < MAX_CHANNELS; n++) {
				if (checkInputParameters(COMMON_RATE, n)) ic = n;
			}
			inputChannels = ic;
			int sr = COMMON_RATE;
			foreach (int n in new int[] {11025, 16000, 22050, 32000, 44100}) {
				if (checkInputParameters(n, inputChannels) && checkOutputParameters(n, outputChannels))  sr = n;
			}
			sampleRate = sr;
		}
			
		public override bool supportsLowLatency() { return false; }
		public override int suggestSampleRate() { return sampleRate; }
		public override int suggestInputChannels() { return inputChannels; }
		public override int suggestOutputChannels() { return outputChannels; }
		public override int suggestInputBufferSize(int sampleRate) { return -1; }
		public override int suggestOutputBufferSize(int sampleRate) { return -1; }



		public override bool checkInputParameters(int srate, int nin) {
			try {
				return nin == 0 || AudioRecord.GetMinBufferSize(srate, VersionedAudioFormat.FormatEclair.getInFormat(nin), ENCODING) > 0;
			} catch (Exception) {
				return false;
			}
		}

		public override bool checkOutputParameters(int srate, int nout) {
			try {
				return AudioTrack.GetMinBufferSize(srate, VersionedAudioFormat.FormatEclair.getOutFormat(nout), ENCODING) > 0;
			} catch (Exception) {
				return false;
			}
		}
	}
}

