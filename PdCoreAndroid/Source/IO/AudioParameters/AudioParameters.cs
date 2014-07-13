using System;
using Android.Content;
using Android.OS;

namespace PdCore.Android.IO
{
	public class AudioParameters
	{
		private static String TAG = "AudioParameters";
		private static AudioParametersImpl impl = null;

		public static void init(Context context) {
			if (impl != null) return;
			int sdkVersion = ((int)Build.VERSION.SdkInt);
			if (sdkVersion > 16 && context != null) {
				impl = JellyBeanMR10OpenSLParameters.getParameters(context);
			} else if (sdkVersion > 16) {
				Console.WriteLine("AudioParameters Initializing audio parameters with null context on Android 4.2 or later.");
				impl = new BasicOpenSLParameters(64, 64);
			} else if (sdkVersion == 16) {
				impl = JellyBeanOpenSLParameters.getParameters();
			} else if (sdkVersion > 8) {
				impl = new BasicOpenSLParameters(64, 64);
			} else {
				impl = new JavaAudioParameters();
			}
		}

		public static bool supportsLowLatency() {
			init(null);
			return impl.supportsLowLatency();
		}

		public static int suggestSampleRate() {
			init(null);
			return impl.suggestSampleRate();
		}

		public static int suggestInputChannels() {
			init(null);
			return impl.suggestInputChannels();
		}

		public static int suggestOutputChannels() {
			init(null);
			return impl.suggestOutputChannels();
		}

		public static int suggestInputBufferSize(int sampleRate) {
			init(null);
			return impl.suggestInputBufferSize(sampleRate);
		}

		public static int suggestOutputBufferSize(int sampleRate) {
			init(null);
			return impl.suggestOutputBufferSize(sampleRate);
		}

		public static bool checkParameters(int srate, int nin, int nout) {
			return checkInputParameters(srate, nin) && checkOutputParameters(srate, nout);
		}

		public static bool checkInputParameters(int srate, int nin) {
			init(null);
			return impl.checkInputParameters(srate, nin);
		}

		public static bool checkOutputParameters(int srate, int nout) {
			init(null);
			return impl.checkOutputParameters(srate, nout);
		}
	}
}

