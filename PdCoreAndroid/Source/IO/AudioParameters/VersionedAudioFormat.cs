using System;
using Android.Media;
using PdCore.Android.Utils;


namespace PdCore.Android.IO
{
	public class VersionedAudioFormat
	{
		private VersionedAudioFormat() {}

		public static class FormatEclair {
			public static ChannelIn getInFormat(int inChannels) {
				switch (inChannels) {
				case 1: return ChannelIn.Mono;
				case 2: return ChannelIn.Stereo;
				default: throw new IllegalArgumentException("illegal number of input channels: " + inChannels);
				}
			}

			public static ChannelOut getOutFormat(int outChannels) {
				switch (outChannels) {
				case 1: return ChannelOut.Mono;
				case 2: return ChannelOut.Stereo;
				case 4: return ChannelOut.Quad;
				case 6: return ChannelOut.FivePointOne;
				case 8: return ChannelOut.SevenPointOne;
				default: throw new IllegalArgumentException("illegal number of output channels: " + outChannels);
				}
			}
		}

		public class FormatCupcake {

			public static ChannelConfiguration getInFormat(int inChannels) {

				switch (inChannels) {
				case 1: return ChannelConfiguration.Mono;
				case 2: return ChannelConfiguration.Stereo;
				default: throw new IllegalArgumentException("illegal number of input channels: " + inChannels);
				}
			}

			public static ChannelConfiguration getOutFormat(int outChannels) {
				switch (outChannels) {
				case 1: return ChannelConfiguration.Mono;
				case 2: return ChannelConfiguration.Stereo;
				default: throw new IllegalArgumentException("illegal number of output channels: " + outChannels);
				}
			}
		}

		public static ChannelConfiguration getInFormat(int inChannels) {
			//return Properties.HasEclair ? FormatEclair.getInFormat(inChannels) : FormatCupcake.getInFormat(inChannels); // crucial: lazy class loading
			return FormatCupcake.getInFormat (inChannels);
		}

		public static ChannelConfiguration getOutFormat(int outChannels) {
			//return Properties.HasEclair ? FormatEclair.getOutFormat(outChannels) : FormatCupcake.getOutFormat(outChannels);
			return FormatCupcake.getOutFormat (outChannels);
		}
	}

}

