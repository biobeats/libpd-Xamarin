using System;
using Android.Content;
using Android.Content.PM;
using Android.Media;

namespace PdCore.Android.IO
{
	public class JellyBeanMR10OpenSLParameters : JellyBeanOpenSLParameters
	{
		private int sampleRate;

		public JellyBeanMR10OpenSLParameters(int sampleRate, int inputBufferSize, int outputBufferSize, int nativeBufferSize, bool lowLatency) : base(inputBufferSize, outputBufferSize, nativeBufferSize, lowLatency) {
			this.sampleRate = sampleRate;
		}

		public override int suggestSampleRate() { return sampleRate; }

		public static JellyBeanMR10OpenSLParameters getParameters(Context context) {

			PackageManager pm = context.PackageManager;
			bool lowLatency = pm.HasSystemFeature(PackageManager.FeatureAudioLowLatency);
			AudioManager am = (AudioManager) context.GetSystemService(Context.AudioService);
			int sr = 44100;
			int bs = 64;
			try {
				int.TryParse(am.GetProperty(AudioManager.PropertyOutputSampleRate), out sr);
				int.TryParse(am.GetProperty(AudioManager.PropertyOutputFramesPerBuffer), out bs);
				Console.WriteLine(AudioParametersImpl.TAG + " sample rate: " + sr + ", buffer size: " + bs);
			} catch (Exception e) {
				Console.WriteLine(AudioParametersImpl.TAG +  " Missing or malformed audio property: " + e.Message);
			}
			return new JellyBeanMR10OpenSLParameters(sr, 64, 64, bs, lowLatency);
		}
	}
}

