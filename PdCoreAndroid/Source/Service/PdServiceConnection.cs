using System;
using Android.Content;
using Android.App;
using Android.OS;

namespace PdCore.Android
{
	public delegate void OnServiceConnectedDelegate(PdServiceBinder binder);
	public delegate void OnServiceDisconnectedDelegate();

	public class PdServiceConnection : Java.Lang.Object, IServiceConnection
	{
		PdServiceBinder binder;
		OnServiceConnectedDelegate connectedDelegate;
		OnServiceDisconnectedDelegate disconnectedDelegate;

		public PdServiceBinder Binder {
			get {
				return binder;
			}
		}

		public PdServiceConnection(OnServiceConnectedDelegate connectedDelegate, OnServiceDisconnectedDelegate disconnectedDelegate) {
			this.connectedDelegate = connectedDelegate;
			this.disconnectedDelegate = disconnectedDelegate;
		}

		public void OnServiceConnected (ComponentName name, IBinder service)
		{
			var demoServiceBinder = service as PdServiceBinder;
			if (demoServiceBinder != null) {
				this.binder = demoServiceBinder;

				if (connectedDelegate != null)
					connectedDelegate (this.binder);
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			if (disconnectedDelegate != null)
				disconnectedDelegate ();
		}
	}
}

