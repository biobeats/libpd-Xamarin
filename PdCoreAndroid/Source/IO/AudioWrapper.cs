using System;
using Android.Media;
using System.Threading;

using Android.OS;
using Android.Content;


public delegate int ProcessDelegate(short[] inBuffer, short[] outBuffer);

namespace PdCore.Android.IO
{
	public class AudioWrapper
	{
		private static String TAG = "AudioWrapper";
		private static Encoding ENCODING = Encoding.Pcm16bit;
		private AudioRecordWrapper rec;
		private AudioTrack track;
		public short[] outBuf;
		int inputSizeShorts;
		int bufSizeShorts;
		volatile bool isStarted;
		private Thread audioThread = null;

		public AudioWrapper(int sampleRate, int inChannels, int outChannels, int bufferSizePerChannel, ProcessDelegate processDel) {
			rec = (inChannels == 0) ? null : new AudioRecordWrapper(sampleRate, inChannels, bufferSizePerChannel);
			inputSizeShorts = inChannels * bufferSizePerChannel;
			bufSizeShorts = outChannels * bufferSizePerChannel;
			outBuf = new short[bufSizeShorts];
			int bufSizeBytes = 2 * bufSizeShorts;
			int trackSizeBytes = 2 * bufSizeBytes;
			int minTrackSizeBytes = AudioTrack.GetMinBufferSize(sampleRate, VersionedAudioFormat.FormatEclair.getOutFormat(outChannels), ENCODING);
			if (minTrackSizeBytes <= 0) {
				throw new IOException("bad AudioTrack parameters; sr: " + sampleRate +", ch: " + outChannels + ", bufSize: " + trackSizeBytes);
			}
			while (trackSizeBytes < minTrackSizeBytes) trackSizeBytes += bufSizeBytes;
			track = new AudioTrack(Stream.Music, sampleRate, VersionedAudioFormat.FormatCupcake.getOutFormat(outChannels), ENCODING, trackSizeBytes, AudioTrackMode.Stream);
			if (track.State != AudioTrackState.Initialized) {
				track.Release();
				throw new IOException("unable to initialize AudioTrack instance for sr: " + sampleRate +", ch: " + outChannels + ", bufSize: " + trackSizeBytes);
			}
			this.processDelegate = processDel;
			isStarted = false;
		}

		protected ProcessDelegate processDelegate; 

		public void start(Context context) {
			avoidClickHack(context);
			audioThread = new Thread (() => {
				Process.SetThreadPriority(global::Android.OS.ThreadPriority.Audio);
				if (rec != null)
					rec.Start ();
				track.Play ();
				short[] inBuf;
				try {
					inBuf = (rec != null) ? rec.Take () : new short[inputSizeShorts];
				} catch (ThreadInterruptedException) {
					return;
				}
				try {
					short[] newBuf;
					while (isStarted) {
						if (processDelegate (inBuf, outBuf) != 0)
							break;
						track.Write (outBuf, 0, bufSizeShorts);
						if (rec != null) {

							if (rec.Poll (out newBuf)) {
								inBuf = newBuf;
							} else {
								Console.WriteLine (TAG + " no input buffer available");
							}
						}
					}
					if (rec != null)
						while(rec.Poll(out newBuf));
				} catch (ThreadInterruptedException) {

				}
				if (rec != null)
					rec.Stop ();
				track.Stop ();
			});
			isStarted = true;
			audioThread.Start();
		}

		public void Stop() {
			if (audioThread == null) return;
			if (isStarted) {
				isStarted = false;
				audioThread.Interrupt ();
				try {
					audioThread.Join ();
				} catch (ThreadInterruptedException) {
					//Thread.currentThread().interrupt();  // Preserve interrupt flag for caller.
				}
			}
			audioThread = null;
		}

		public void Release() {
			Stop();
			track.Release();
			if (rec != null) rec.Release();
		}

		public bool IsRunning() {
			return audioThread != null && audioThread.ThreadState == ThreadState.Running && isStarted;
		}

		void avoidClickHack (Context context)
		{
			/*try {

				MediaPlayer mp = MediaPlayer.Create(context, Resource.Raw.silence);
				mp.Start();
				Thread.Sleep(10);
				mp.Stop();
				mp.Release();
			} catch (Exception e) {
				Console.WriteLine(TAG + " " + e.Message);
			}*/

		}
	}
}

