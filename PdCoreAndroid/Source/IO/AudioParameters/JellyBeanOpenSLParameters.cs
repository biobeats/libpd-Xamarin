using System;
using Android.OS;

namespace PdCore.Android.IO
{
	public class JellyBeanOpenSLParameters : BasicOpenSLParameters
	{
		private int nativeBufferSize;
		private bool lowLatency;

		public JellyBeanOpenSLParameters(int inputBufferSize, int outputBufferSize, int nativeBufferSize, bool lowLatency) : base(inputBufferSize, outputBufferSize) {

			this.nativeBufferSize = nativeBufferSize;
			this.lowLatency = lowLatency;
		}

		public static JellyBeanOpenSLParameters getParameters() {
			bool lowLatency = Build.Model.Equals("Galaxy Nexus");
			return new JellyBeanOpenSLParameters(64, 64, lowLatency ? 384 : 64, lowLatency);  // 384 is the magic number for GN + JB (Android 4.1).
		}

		public override int suggestOutputBufferSize(int sampleRate) {
			return (sampleRate == suggestSampleRate()) ? nativeBufferSize : base.suggestOutputBufferSize(sampleRate);
		}

		public override bool supportsLowLatency() {
			return lowLatency;
		}
	}
}

