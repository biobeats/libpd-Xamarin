using System;
using Android.OS;
using Java.Lang;
using LibPDBinding;
using Android.Content;

namespace PdCore.Android.IO
{
	public class PdAudio
	{
		private static AudioWrapper audioWrapper = null;

		public static void initAudio(int sampleRate, int inChannels, int outChannels, int ticksPerBuffer, bool restart) {
			if (isRunning() && !restart) return;
			stopAudio();

			if (LibPD.OpenAudio(inChannels, outChannels, sampleRate) != 0) {
				throw new IOException("unable to open Pd audio: " + sampleRate + ", " + inChannels + ", " + outChannels);
			}

			//if (!LibPD.ImplementsAudio ()) {
				if (!AudioParameters.checkParameters (sampleRate, inChannels, outChannels) || ticksPerBuffer <= 0) {
					throw new IOException("bad Java audio parameters: " + sampleRate + ", " + inChannels + ", " + outChannels + ", " + ticksPerBuffer);
				}

				int bufferSizePerChannel = ticksPerBuffer * LibPD.BlockSize;

				audioWrapper = new AudioWrapper(sampleRate, inChannels, outChannels, bufferSizePerChannel, (inBuf, outBuf) => {

					Array.Clear(outBuf, 0, outBuf.Length);

					int err = LibPD.Process(ticksPerBuffer, inBuf, outBuf);
					/*PdBase.pollMidiQueue();
					PdBase.pollPdMessageQueue();*/

					return err;
				});
			//}
		}

		public static void startAudio(Context context) {
			LibPD.ComputeAudio(true);
			audioWrapper.start(context);
			/*if (LibPD.ImplementsAudio()) {
				//LibPD.StartAudio();
			} else {
				if (audioWrapper == null) {
					throw new IllegalStateException("audio not initialized");
				}

			}*/
		}

		public static void stopAudio() {
			/*if (LibPD.ImplementsAudio()) {

			} else {

			}*/
			if (!isRunning()) return;
			audioWrapper.Stop();
		}

		public static bool isRunning() {
			return audioWrapper != null && audioWrapper.IsRunning();
			/*if (LibPD.ImplementsAudio()) {
				return true;
			} else {

			}*/
		}

		public static void release() {
			stopAudio();
			/*if (LibPD.ImplementsAudio()) {

			} else {
				if (audioWrapper == null) return;
				audioWrapper.Release();
				audioWrapper = null;
			}*/
		}

	}
}

