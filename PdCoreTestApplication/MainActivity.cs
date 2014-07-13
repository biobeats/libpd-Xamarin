using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using LibPDBinding;
using PdCore.Android;
using System.IO;


namespace PdCoreTestApplication
{

	[Activity (Label = "PdCoreTestApplication", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		PDWrapper pdWrapper;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button start = FindViewById<Button> (Resource.Id.startButton);
			Button stop = FindViewById<Button> (Resource.Id.stopButton);

			copyFileToExternal ();

			Java.IO.File s = new Java.IO.File( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath.ToString ());
			pdWrapper = new PDWrapper (this, s);


			LibPD.Print += ((string text) => Console.WriteLine("LibpdPrint " + text));
			LibPD.Bang += ((string text) => Console.WriteLine("LibpdPrint " + text));
			LibPD.Float += ((string text, float value) => Console.WriteLine("LibpdPrint " + text + " " + value));
			LibPD.List += ((string text, object[] objects) => Console.WriteLine("LibpdPrint " + text));
			LibPD.Message += ((string text, string msg, object[] args) => Console.WriteLine("LibpdPrint " + text + " " + msg));
			LibPD.Symbol += ((string recv, string sym) => Console.WriteLine("LibpdPrint " + recv + " " + sym));

			start.Click += delegate {
				pdWrapper.initPdService();
			};

			stop.Click += delegate {
				pdWrapper.cleanup();
			};
		}

		protected override void OnStart ()
		{
			base.OnStart ();
		}

		private string copyFileToExternal() {
			Stream file = Assets.Open ("audio_engine.pd");
			string pathToFile = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "audio_engine.pd");
			Stream dest = new FileStream (pathToFile, FileMode.OpenOrCreate);
			Console.WriteLine (pathToFile);
			file.CopyTo (dest);
			dest.Close ();
			return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			pdWrapper.cleanup ();
			pdWrapper = null;
		}
	}
}


