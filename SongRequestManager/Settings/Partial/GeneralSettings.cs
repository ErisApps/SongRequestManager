﻿namespace SongRequestManager.Settings.Partial
{
	internal class GeneralSettings
	{
		public virtual string Prefix { get; set; } = "!";
		public virtual bool QueueOpen { get; set; } = true;
		public virtual int MaxQueueSize { get; set; } = 50;
	}
}