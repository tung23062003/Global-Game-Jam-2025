﻿namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Base class for Slide Scale.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/scale.html")]
	[DataBindSupport]
	public abstract class SliderScaleBase : MonoBehaviourConditional, IStylable
	{
		[SerializeField]
		Scale scale;

		/// <summary>
		/// Scale.
		/// </summary>
		public Scale Scale
		{
			get => scale;

			set
			{
				if (scale != null)
				{
					scale.Clear();
				}

				scale = value;
				UpdateScale();
			}
		}

		[SerializeField]
		string format = string.Empty;

		/// <summary>
		/// Value format.
		/// </summary>
		[DataBindField]
		public string Format
		{
			get => format;

			set
			{
				if (format != value)
				{
					format = value;
					UpdateScale();
				}
			}
		}

		Func<float, string> formatter;

		/// <summary>
		/// Custom value formatter.
		/// </summary>
		[DataBindField]
		public Func<float, string> Formatter
		{
			get => formatter;

			set
			{
				if (formatter != value)
				{
					formatter = value;
					UpdateScale();
				}
			}
		}

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
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

		Func<float, Scale.MarkData> value2MarkDataDelegate;

		/// <summary>
		/// Delegate for the Value2MarkData method.
		/// </summary>
		protected Func<float, Scale.MarkData> Value2MarkDataDelegate
		{
			get
			{
				value2MarkDataDelegate ??= Value2MarkData;

				return value2MarkDataDelegate;
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			AddListeners();

			UpdateScale();
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			Scale.OnChange += UpdateScale;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			Scale.OnChange -= UpdateScale;
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}

		/// <summary>
		/// Update scale.
		/// </summary>
		public abstract void UpdateScale();

		/// <summary>
		/// Convert value to label.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Label.</returns>
		protected virtual string Value2Label(float value)
		{
			if (Formatter != null)
			{
				return Formatter(value);
			}

			return value.ToString(Format);
		}

		/// <summary>
		/// Convert value to mark data.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Mark data.</returns>
		protected virtual Scale.MarkData Value2MarkData(float value)
		{
			var a = Value2Anchor(value);
			return new Scale.MarkData(value, anchorMin: a, anchorMax: a, value2label: Value2Label);
		}

		/// <summary>
		/// Convert value to anchor.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Anchor.</returns>
		protected abstract Vector2 Value2Anchor(float value);

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			Scale.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			Scale.GetStyle(style);

			return true;
		}
		#endregion
	}
}