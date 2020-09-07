using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace SongRequestManager.Models
{
	public class User
	{
		/// <remarks>
		///	Please use this static Create method instead, the public ctor only exists for the BSIPA config
		/// </remarks>
		public static User Create(string id, Platform platform, string displayName)
		{	return new User
			{
				Id = id,
				Platform = platform,
				DisplayName = displayName
			};
		}

		[NonNullable]
		public virtual string Id { get; set; } = null!;

		[NonNullable]
		[UseConverter(typeof(EnumConverter<Platform>))]
		public virtual Platform Platform { get; set; }

		[NonNullable]
		public virtual string DisplayName { get; set; } = null!;
	}
}