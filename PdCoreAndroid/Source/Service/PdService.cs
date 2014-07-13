using System;
using Android.App;
using Android.Util;
using System.Threading;
using Android.Media;
using Android.OS;
using Android.Content;
using LibPDBinding;
using PdCore.Android.IO;
using Java.Util.Zip;
using Java.IO;


namespace PdCore.Android
{
	[Service]
	[IntentFilter(new String[]{"com.biobeats.PdService"})]
	public class PdService : Service
	{
		private static string TAG = "PdService";
		private static bool abstractionsInstalled = false;
		private volatile int sampleRate = 0;
		private volatile int inputChannels = 0;
		private volatile int outputChannels = 0;
		private volatile float bufferSizeMillis = 0.0f;
		private PdServiceBinder binder;

		public PdService ()
		{

		}

		#region Getter
		public float getBufferSizeMillis() {
			return bufferSizeMillis;
		}

		public int getInputChannels() {
			return inputChannels;
		}

		public int getOutputChannels() {
			return outputChannels;
		}

		public int getSampleRate() {
			return sampleRate;
		}
		#endregion

		public void initAudio(int srate, int nic, int noc, float millis) {
			//fgManager.stopForeground();

			if (srate < 0 || nic < 0 || noc < 0)
				throw new Exception();

			if (millis < 0) {
				millis = 50.0f;  // conservative choice
			}
			int tpb = (int) (0.001f * millis * srate / LibPD.BlockSize) + 1;
			LibPD.ReInit ();
			PdAudio.initAudio (srate, nic, noc, tpb, true);
			//LibPD.OpenAudio(srate, nic, noc);

			sampleRate = srate;
			inputChannels = nic;
			outputChannels = noc;
			bufferSizeMillis = millis;
		}

		public void startAudio() {
			PdAudio.startAudio(this);
		}

		public void startAudio(Intent intent, int icon, String title, String description) {
			//fgManager.startForeground(intent, icon, title, description);
			PdAudio.startAudio(this);
		}

		public void stopAudio() {
			PdAudio.stopAudio();
			//fgManager.stopForeground();
		}

		public bool isRunning() {
			return PdAudio.isRunning();
		}

		public void release() {
			stopAudio();
			PdAudio.release();
			LibPD.Release();
		}

		public override IBinder OnBind (Intent intent)
		{
			binder = new PdServiceBinder (this);
			return binder;
		}

		public bool OnUnbind(Intent intent) {
			release();
			return false;
		}

		public void OnCreate() {
			base.OnCreate ();
			AudioParameters.init(this);
			if (!abstractionsInstalled) {
				try {
					string unpackDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)+"/lib";

					ZipInputStream zipIs = new ZipInputStream(Assets.Open("extra_abs.zip")); 
					ZipEntry ze = null;

					while ((ze = zipIs.NextEntry) != null) {

						FileOutputStream fout = new FileOutputStream(unpackDirectory +"/"+ ze.Name);

						byte[] buffer = new byte[1024];
						int length = 0;

						while ((length = zipIs.Read(buffer))>0) {
							fout.Write(buffer, 0, length);
						}
						zipIs.CloseEntry();
						fout.Close();
					}
					zipIs.Close();
					abstractionsInstalled = true;
					LibPD.AddToSearchPath(unpackDirectory);
					LibPD.AddToSearchPath("/data/data/" + ApplicationContext.PackageName + "/lib");
				} catch (IOException e) {
					System.Console.WriteLine(TAG + " unable to unpack abstractions:" + e.Message);
				}
			}
		}

		public void OnDestroy() {
			base.OnDestroy ();
			release();
		}
	}
}

