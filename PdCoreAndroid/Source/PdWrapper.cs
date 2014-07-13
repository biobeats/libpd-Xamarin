using System;
using Android.Content;
using PdCore.Android.IO;
using LibPDBinding;
using Java.IO;
using System.Threading;

namespace PdCore.Android
{
	public class PDWrapper {
		private static String TAG = "PDWrapper";
		private Context activity;
		private int SAMPLE_RATE = 22050;
		private int patch = 0;
		private File patchDirectory;
		public int bufferSize = 50;

		public PDWrapper(Context activity, File patchDirectory) {
			this.activity = activity;
			this.patchDirectory = patchDirectory;
			serviceConnection = new PdServiceConnection(
				(binder) => {
					pdService = ((PdServiceBinder) binder).GetPdService();
					new Thread(() => startAudio()).Start();
				}, null);
		}

		/* the reference to the actual launched PdService */
		public PdService pdService = null;
		private PdServiceConnection serviceConnection;

		public void initPdService() {
			var demoServiceIntent = new Intent ("com.biobeats.PdService");
			activity.BindService (demoServiceIntent, serviceConnection, Bind.AutoCreate);
		}

		private void startAudio() {
			if (pdService == null)
				return;
			AudioParameters.init(activity);
			// clamp it

			/*int sRate = Math.min(AudioParameters.suggestSampleRate(),
                SAMPLE_RATE);
        	Log.i(TAG, "actual sample rate: " + sRate);*/

			int nOut = Math.Min(AudioParameters.suggestOutputChannels(), 1);
			System.Console.WriteLine(TAG + " output channels: " + nOut);
			if (nOut == 0) {
				System.Console.WriteLine(TAG + " audio output not available; exiting");
			}

			if (patch == 0) {
				try {
					pdService.initAudio(SAMPLE_RATE, 0, nOut, bufferSize);
					File patchFile = new File(patchDirectory, "audio_engine.pd");
					LibPD.AddToSearchPath(patchDirectory.AbsolutePath);
					patch = LibPD.OpenPatch(patchFile.AbsolutePath);

				} catch (IOException e) {
					System.Console.WriteLine(TAG + " " + e.Message);
				}
				try {
					Thread.Sleep(1000);
				} catch (ThreadInterruptedException) {
				}
			}
			pdService.startAudio();
		}

		public void cleanup() {
			/* make sure to release all resources */
			stopAudio();
			if (patch != 0) {
				LibPD.ClosePatch(patch);
				patch = 0;
			}
			LibPD.Release();
			try {
				activity.UnbindService(serviceConnection);
			} catch (IllegalArgumentException) {
				pdService = null;
			}
		}

		private void stopAudio() {
			if (pdService == null)
				return;
			/* consider ramping down the volume here to avoid clicks */
			pdService.stopAudio();
		}
	}
}

