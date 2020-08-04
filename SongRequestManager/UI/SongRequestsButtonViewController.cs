using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Notify;
using IPA.Config.Stores.Attributes;
using SongRequestManager.Converters;
using UnityEngine;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.UI
{
	[NotifyPropertyChanges]
	public class SongRequestsButtonViewController : MonoBehaviour, INotifiableHost
	{
		[UIValue("glowy-color")]
		public string GlowColor { get; set; } = "#ff0d72";

		[UIValue("interactable")]
		public bool Interactable { get; set; } = true;

		[UIComponent("srm-button")]
		private Transform _srmButtonTransform;

		private SongRequestsFlowController? _songRequestsFlowController;

		[UIAction("button-click")]
		internal void OpenRequestsView()
		{
			if (_songRequestsFlowController == null)
			{
				_songRequestsFlowController = BeatSaberUI.CreateFlowCoordinator<SongRequestsFlowController>();
			}

			Resources.FindObjectsOfTypeAll<LevelSelectionFlowCoordinator>()
				.First()
				.PresentFlowCoordinator(_songRequestsFlowController);
		}

		private void Awake()
		{
			Init();
		}

		public void Init()
		{
			var standardLevel = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().First();
			BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "SongRequestManager.UI.SongRequestsButton.bsml"),
				standardLevel.transform.Find("LevelDetail").gameObject, this);
			_srmButtonTransform.localScale *= 0.7f; //no scale property in bsml as of now so manually scaling it

			SongRequestManager.Instance.SongQueueService.RequestQueue.CollectionChanged += OnRequestQueueChanged;
			UpdateSrmButtonColor();
		}

		private void OnDisable()
		{
			Logger.Log("Oh noes, the SRM button got disabled!!!");
		}
		
		private void OnDestroy()
		{
			Logger.Log("Oh noes, the SRM button got destroyed!!!");
			if (SongRequestManager.Instance && SongRequestManager.Instance.SongQueueService != null)
			{
				SongRequestManager.Instance.SongQueueService.RequestQueue.CollectionChanged -= OnRequestQueueChanged;
			}
		}

		private void OnRequestQueueChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Reset:
					UpdateSrmButtonColor();
					break;
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Move:
				default:
					break;
			}
		}

		private void UpdateSrmButtonColor()
		{
			if (SongRequestManager.Instance == null)
			{
				Logger.Log("SRM Instance null");
			}
			else if (SongRequestManager.Instance.SongQueueService == null)
			{
				Logger.Log("SongQueueService instance null");
			}
			else if (SongRequestManager.Instance.SongQueueService.RequestQueue == null)
			{
				Logger.Log("RequestQueue instance null");
			}
			
			GlowColor = ButtonColorValueConverter.Convert(SongRequestManager.Instance.SongQueueService.QueuedRequestCount > 0);
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(GlowColor)));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}