namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Safe area.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/mobile/safe-area.html")]
	public class SafeArea : UIBehaviour, IUpdatable
	{
		readonly struct ScreenState
		{
			public readonly Rect SafeArea;

			public readonly Resolution Resolution;

			public readonly ScreenOrientation Orientation;

			private ScreenState(Rect safeArea, Resolution resolution, ScreenOrientation orientation)
			{
				SafeArea = safeArea;

				Resolution = resolution;
				Orientation = orientation;
			}

			public static ScreenState Current => new ScreenState(Screen.safeArea, Screen.currentResolution, Screen.orientation);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj) => (obj is ScreenState state) && Equals(state);

			bool SameResolution(Resolution resolution) => Resolution.width == resolution.width && Resolution.height == resolution.height;

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(ScreenState other) => SafeArea == other.SafeArea && SameResolution(other.Resolution) && Orientation == other.Orientation;

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode() => SafeArea.GetHashCode() ^ Resolution.width ^ Resolution.height ^ (int)Orientation;

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">Left instance.</param>
			/// <param name="b">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(ScreenState a, ScreenState b) => a.Equals(b);

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">Left instance.</param>
			/// <param name="b">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(ScreenState a, ScreenState b) => !a.Equals(b);
		}

		/// <summary>
		/// Borders.
		/// </summary>
		public readonly struct Borders
		{
			/// <summary>
			/// Top border.
			/// </summary>
			public readonly Rect Top;

			/// <summary>
			/// Bottom border.
			/// </summary>
			public readonly Rect Bottom;

			/// <summary>
			/// Left border.
			/// </summary>
			public readonly Rect Left;

			/// <summary>
			/// Right border.
			/// </summary>
			public readonly Rect Right;

			/// <summary>
			/// Initializes a new instance of the <see cref="Borders"/> struct.
			/// </summary>
			/// <param name="top">Top.</param>
			/// <param name="bottom">Bottom.</param>
			/// <param name="left">Left.</param>
			/// <param name="right">Right.</param>
			public Borders(Canvas canvas, Rect safeArea)
			{
				var scale = canvas.scaleFactor;
				var size = (canvas.transform as RectTransform).rect.size;
				Top = new Rect(0f, 0f, size.x / scale, Mathf.Round(safeArea.yMin / scale));
				Bottom = new Rect(0f, Mathf.Round(safeArea.yMax / scale), size.x / scale, Mathf.Round((size.y - safeArea.yMax) / scale));

				Left = new Rect(0f, 0f, Mathf.Round(safeArea.xMin / scale), size.y / scale);
				Right = new Rect(Mathf.Round(safeArea.xMax / scale), 0f, Mathf.Round(size.x - safeArea.xMax / scale), size.y / scale);
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj) => (obj is Borders state) && Equals(state);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(Borders other) => Top == other.Top && Bottom == other.Bottom && Left == other.Left && Right == other.Right;

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode() => Top.GetHashCode() ^ Bottom.GetHashCode() ^ Left.GetHashCode() ^ Right.GetHashCode();

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">Left instance.</param>
			/// <param name="b">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(Borders a, Borders b) => a.Equals(b);

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">Left instance.</param>
			/// <param name="b">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(Borders a, Borders b) => !a.Equals(b);
		}

		/// <summary>
		/// Borders event.
		/// </summary>
		[Serializable]
		public class BordersEvent : UnityEvent<Borders>
		{
		}

		RectTransform rectTransform;

		Canvas canvas;

		ScreenState lastState;

		/// <summary>
		/// On screen change event.
		/// </summary>
		[SerializeField]
		public BordersEvent OnScreenChange = new BordersEvent();

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			TryGetComponent(out rectTransform);
			canvas = GetComponentInParent<Canvas>();

			lastState = ScreenState.Current;
			ChangeScreenState();
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			Updater.Add(this);
			base.OnEnable();
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			Updater.Remove(this);
			base.OnDisable();
		}

		/// <summary>
		/// Run update.
		/// </summary>
		public void RunUpdate()
		{
			CheckScreenState();
		}

		/// <summary>
		/// Process the resize event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			CheckScreenState();
		}

		void CheckScreenState()
		{
			var current = ScreenState.Current;
			if (current != lastState && canvas != null)
			{
				lastState = current;
				Updater.RunOnceNextFrame(this, ChangeScreenState);
			}
		}

		void ChangeScreenState()
		{
			var scale = canvas.scaleFactor;
			var area = lastState.SafeArea;
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Mathf.Round(area.xMin / scale), Mathf.Round(area.width / scale));
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, Mathf.Round(area.yMin / scale), Mathf.Round(area.height / scale));

			var borders = new Borders(canvas, area);
			OnScreenChange.Invoke(borders);
		}
	}
}