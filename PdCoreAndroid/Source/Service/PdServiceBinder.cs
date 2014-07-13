using System;
using Android.OS;

namespace PdCore.Android
{
	public class PdServiceBinder : Binder
	{
		PdService service;

		public PdServiceBinder (PdService service)
		{
			this.service = service;
		}

		public PdService GetPdService ()
		{
			return service;
		}
	}
}

