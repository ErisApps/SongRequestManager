using System;
using IPA.Config.Data;
using IPA.Config.Stores;

namespace SongRequestManager.Settings.Converters
{
	internal class EnumStringConfigConverter<TEnum> : ValueConverter<TEnum> where TEnum : struct
	{
		public override TEnum FromValue(Value value, object parent) => value is Text text && Enum.TryParse(text.Value, out TEnum @enum)
			? @enum
			: throw new ArgumentException($"Value is not of type {typeof(TEnum).Name}", nameof(value));

		public override Value ToValue(TEnum obj, object parent) => Value.Text(obj.ToString());
	}
}