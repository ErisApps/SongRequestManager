using System;
using System.Data;
using IPA.Config.Data;
using IPA.Config.Stores;

namespace SongRequestManager.Settings.Converters
{
	internal sealed class DateTimeConfigConverter : ValueConverter<DateTime>
	{
		public override DateTime FromValue(Value value, object parent)
		{
			if (!(value is Text text))
			{
				throw new ArgumentException("Value is not of type Text", nameof(value));
			}

			if (DateTime.TryParse(text.Value, out var dateTime))
			{
				return dateTime;
			}

			throw new NoNullAllowedException($"Parsing failed, {text.Value}");
		}

		public override Value ToValue(DateTime obj, object parent) => Value.Text(obj.ToString("O"));
	}
}