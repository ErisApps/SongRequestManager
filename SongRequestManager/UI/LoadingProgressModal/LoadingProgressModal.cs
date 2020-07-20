using System;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;

namespace SongRequestManager.UI.LoadingProgressModal
{
	public class LoadingProgressModal : NotifiableSingleton<LoadingProgressModal>
	{
		private Progress<double> _progress;
		private Action? _onCancel;

		[UIComponent("modal")]
		private ModalView _modal;

		[UIAction("cancel-click")]
		private void CancelClick()
		{
			HideDialog();
			_onCancel?.Invoke();
			_onCancel = null;
		}

		[UIValue("progress")]
		internal string ProgressText { get; set; } = "";

		public void ShowDialog(GameObject hostGameObject, Progress<double> progress, Action onCancel = null)
		{
			BSMLParser.instance.Parse(
				BeatSaberMarkupLanguage.Utilities.GetResourceContent(
					Assembly.GetExecutingAssembly(),
					"SongRequestManager.UI.LoadingProgressModal.LoadingProgressModal.bsml"),
				hostGameObject,
				this);

			UpdateProgressText(0);

			_progress = progress;
			_progress.ProgressChanged += OnProgressChanged;

			_onCancel = onCancel;

			_modal.Show(true);
		}

		public void HideDialog()
		{
			_progress.ProgressChanged -= OnProgressChanged;
			_modal.Hide(true);
		}

		private void OnProgressChanged(object sender, double e)
		{
			var progress = (int) Math.Round(e * 100);
			if (progress >= 100)
			{
				HideDialog();
			}

			UpdateProgressText(progress);
		}

		private void UpdateProgressText(int progress)
		{
			ProgressText = $"{progress}%";
			NotifyPropertyChanged(nameof(ProgressText));
		}
	}
}