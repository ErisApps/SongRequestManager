using ChatCore.Interfaces;
using ChatCore.Models.Twitch;
using SongRequestManager.Models;

namespace SongRequestManager.Extensions
{
	public static class PlatformExtensions
	{
		internal static Platform GetPlatform(this IChatMessage chatMessage)
		{
			switch (chatMessage)
			{
				case TwitchMessage _:
					return Platform.Twitch;
				default:
					return Platform.Unknown;
			}
		}

		internal static Platform GetPlatform(this IChatUser chatUser)
		{
			switch (chatUser)
			{
				case TwitchUser _:
					return Platform.Twitch;
				default:
					return Platform.Unknown;
			}
		}
	}
}