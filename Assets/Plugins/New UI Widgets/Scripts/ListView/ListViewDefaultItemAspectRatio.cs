namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// AspectRatio DefaultItem of the ListView.
	/// </summary>
	[RequireComponent(typeof(ListViewBase))]
	public class ListViewDefaultItemAspectRatio : MonoBehaviourInitiable
	{
		/// <summary>
		/// Aspect ratio.
		/// </summary>
		[SerializeField]
		[Tooltip("AspectRatio = Width / Height")]
		public float AspectRatio = 1f;

		/// <summary>
		/// ListView.
		/// </summary>
		protected ListViewBase ListView;

		/// <summary>
		/// Resize listener.
		/// </summary>
		protected ResizeListener ResizeListener;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TryGetComponent(out ListView);
			var viewport = ListView.GetScrollRect().viewport;
			ResizeListener = Utilities.RequireComponent<ResizeListener>(viewport);
			ResizeListener.OnResizeNextFrame.AddListener(Apply);

			ListView.Init();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (ResizeListener != null)
			{
				ResizeListener.OnResizeNextFrame.RemoveListener(Apply);
			}
		}

		/// <summary>
		/// Apply AspectRatio.
		/// </summary>
		public virtual void Apply()
		{
			if (ListView.GetItemsCount() == 0)
			{
				return;
			}

			var instance = ListView.GetInstance(ListView.DisplayedIndexFirst);
			var size = instance.RectTransform.rect.size;
			if (ListView.IsHorizontal())
			{
				size.x = size.y * AspectRatio;
			}
			else
			{
				size.y = size.x / AspectRatio;
			}

			ListView.ChangeDefaultItemSize(size);
		}
	}
}