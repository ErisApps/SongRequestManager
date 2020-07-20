namespace SongRequestManager.Converters
{
	public static class ButtonColorValueConverter
	{
		public static string Convert(bool queueOpen) => queueOpen ? "#22dd76" : "#ff0d72";
	}
}