using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace SongRequestManager.Models
{
	public class User
	{
		public User()
		{
		}

		public User(string id, Platform platform, string username)
		{
			Id = id;
			Platform = platform;
			Username = username;
		}


		[NonNullable]
		public virtual string Id { get; set; }

		[NonNullable]
		[UseConverter(typeof(EnumConverter<Platform>))]
		public virtual Platform Platform { get; set; }

		[NonNullable]
		public virtual string Username { get; set; }
	}
}