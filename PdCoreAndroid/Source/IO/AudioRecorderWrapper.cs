using System;
using Android.Media;
using System.Threading;
using System.Collections.Concurrent;
using Java.Util.Concurrent;
using Android.OS;

namespace PdCore.Android.IO
{
	public class AudioRecordWrapper
	{

		private static Encoding ENCODING = Encoding.Pcm16bit;
		private AudioRecord rec;
		private int bufSizeShorts;
		private BlockingCollection<short[]> queue = new BlockingCollection<short[]>();
		private Thread inputThread = null;

		public AudioRecordWrapper(int sampleRate, int inChannels, int bufferSizePerChannel) {
			bufSizeShorts = inChannels * bufferSizePerChannel;
			int bufSizeBytes = 2 * bufSizeShorts;
			int recSizeBytes = 2 * bufSizeBytes;
			int minRecSizeBytes = AudioRecord.GetMinBufferSize(sampleRate, VersionedAudioFormat.FormatEclair.getInFormat(inChannels), ENCODING);
			if (minRecSizeBytes <= 0) {
				throw new IOException("bad AudioRecord parameters; sr: " + sampleRate + ", ch: " + inChannels + ", bufSize: " + bufferSizePerChannel);
			}
			while (recSizeBytes < minRecSizeBytes) recSizeBytes += bufSizeBytes;

			rec = new AudioRecord(AudioSource.Mic, sampleRate, VersionedAudioFormat.FormatEclair.getInFormat(inChannels), ENCODING, recSizeBytes);
			if (rec != null && rec.State != State.Initialized) {
				rec.Release();
				throw new IOException("unable to initialize AudioRecord instance for sr: " + sampleRate + ", ch: " + inChannels + ", bufSize: " + bufferSizePerChannel);
			}
		}

		public void Start() {
			inputThread = new Thread (() => {
				Process.SetThreadPriority(global::Android.OS.ThreadPriority.Audio);
				rec.StartRecording();
				short[] buf = new short[bufSizeShorts];
				short[] auxBuf = new short[bufSizeShorts];
				try {
					while (true) {
						int nRead = 0;
						while (nRead < bufSizeShorts) {
							nRead += rec.Read(buf, nRead, bufSizeShorts - nRead);
						}
						if (nRead < bufSizeShorts) break;

						queue.Add(buf);
						short[] tmp = buf;
						buf = auxBuf;
						auxBuf = tmp;
					}
				} catch (ThreadInterruptedException) {
					rec.Stop();
				}
			});
			inputThread.Start();
		}

		public void Stop() {
			if (inputThread == null) return;
			inputThread.Interrupt();
			try {
				inputThread.Join();
			} catch (ThreadInterruptedException) {

				//Thread.CurrentThread().interrupt();  // Preserve interrupt flag for caller.
			}
			inputThread = null;
		}

		public void Release() {
			Stop();
			rec.Release();
			queue.Dispose();
		}

		public bool Poll(out short[] val) {
			return queue.TryTake (out val);
		}

		public short[] Take() {
			return queue.Take ();
		}
	}
}

