﻿namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Resize listener.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/listeners/resize.html")]
	public class ResizeListener : UIBehaviour, IUpdatable
	{
		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>The RectTransform.</value>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// The OnResize event.
		/// </summary>
		public UnityEvent OnResize = new UnityEvent();

		/// <summary>
		/// The OnResize event raised at next frame.
		/// </summary>
		public UnityEvent OnResizeNextFrame = new UnityEvent();

		Rect oldRect;

		bool raiseNextFrame;

		/// <summary>
		/// Handle RectTransform dimensions change event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			if (!IsActive())
			{
				return;
			}

			var newRect = RectTransform.rect;
			var equals = Mathf.Approximately(newRect.x, oldRect.x)
				&& Mathf.Approximately(newRect.y, oldRect.y)
				&& Mathf.Approximately(newRect.width, oldRect.width)
				&& Mathf.Approximately(newRect.height, oldRect.height);
			if (equals)
			{
				return;
			}

			oldRect = newRect;
			OnResize.Invoke();

			if (!raiseNextFrame)
			{
				raiseNextFrame = true;
				Updater.RunOnceNextFrame(this);
			}
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			Updater.RemoveRunOnceNextFrame(this);
		}

		/// <summary>
		/// Process enable event.
		/// </summary>
		protected override void OnEnable()
		{
			OnRectTransformDimensionsChange();
		}

		/// <summary>
		/// Run update.
		/// </summary>
		public virtual void RunUpdate()
		{
			OnResizeNextFrame.Invoke();
			raiseNextFrame = false;
		}
	}
}