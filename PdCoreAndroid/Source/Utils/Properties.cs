using System;
using Android.OS;

namespace PdCore.Android.Utils
{
	public class Properties
	{
		public static int VersionSDK {
			get {
				return ((int)Build.VERSION.SdkInt);
			}
		}

		public static bool HasEclair {
			get {
				return VersionSDK >= 5;
			}
		}
	}
}

