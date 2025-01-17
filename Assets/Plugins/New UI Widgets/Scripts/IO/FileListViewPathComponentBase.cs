﻿namespace UIWidgets
{
	using System.IO;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for FileListViewPathComponent.
	/// </summary>
	[RequireComponent(typeof(Image))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/filelistview.html")]
	public class FileListViewPathComponentBase : ComponentPool<FileListViewPathComponentBase>, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// Pointer button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton PointerButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Path to displayed directory.
		/// </summary>
		public string FullName
		{
			get;
			protected set;
		}

		/// <summary>
		/// Parent FileListViewPath.
		/// </summary>
		[HideInInspector]
		public FileListViewPath Owner;

		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		protected Graphic Background;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			if (TryGetComponent<EasyLayoutNS.EasyLayout>(out var layout))
			{
				layout.ChildrenWidth = EasyLayoutNS.ChildrenSize.SetPreferred;
			}

			var mask = Utilities.RequireComponent<Mask>(this);
			mask.showMaskGraphic = true;

			if (Background == null)
			{
				TryGetComponent(out Background);
			}
		}

		/// <summary>
		/// Set interactable state.
		/// </summary>
		/// <param name="disabledColor">Disabled color.</param>
		public virtual void SetInteractableState(Color disabledColor)
		{
			Init();

			if (Background != null)
			{
				Background.canvasRenderer.SetColor(disabledColor);
			}
		}

		/// <summary>
		/// Set path.
		/// </summary>
		/// <param name="path">Path.</param>
		public virtual void SetPath(string path)
		{
			FullName = path;
			var dir = Path.GetFileName(path);
			NameAdapter.text = !string.IsNullOrEmpty(dir) ? dir : path;
		}

		/// <summary>
		/// OnPointerDown event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerUp event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerClick event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (!Owner.IsInteractable())
			{
				return;
			}

			if (eventData.button == PointerButton)
			{
				Owner.Open(FullName);
			}
		}

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			foreach (var c in Cache)
			{
				c.SetStyle(styleBackground, styleText, style);
			}

			if (TryGetComponent<Image>(out var bg))
			{
				styleBackground.ApplyTo(bg);
			}

			if (NameAdapter != null)
			{
				styleText.ApplyTo(NameAdapter.gameObject);
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void GetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				styleBackground.GetFrom(bg);
			}

			if (NameAdapter != null)
			{
				styleText.GetFrom(NameAdapter.gameObject);
			}
		}
	}
}