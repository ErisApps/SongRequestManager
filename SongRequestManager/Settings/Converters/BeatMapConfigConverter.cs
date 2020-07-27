using System;
using System.Data;
using BeatSaverSharp;
using IPA.Config.Data;
using IPA.Config.Stores;
using Newtonsoft.Json;

namespace SongRequestManager.Settings.Converters
{
	internal sealed class BeatMapConfigConverter : ValueConverter<Beatmap>
	{
		public override Beatmap FromValue(Value value, object parent)
		{
			if (value is Text text)
			{
				var beatmap = JsonConvert.DeserializeObject<Beatmap>(text.Value);
				if (beatmap == null)
				{
					throw new NoNullAllowedException($"Beatmap may not be null, {text.Value}");
				}

				return beatmap;
			}

			throw new ArgumentException("Value is not of type Text", nameof(value));
		}

		public override Value ToValue(Beatmap obj, object parent) => Value.Text(JsonConvert.SerializeObject(obj));
	}
}