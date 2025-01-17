﻿namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// ColorPickerRange.
	/// Allow to select colors in range between specified colors.
	/// </summary>
	[DataBindSupport]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/colorpicker-range.html")]
	public class ColorPickerRange : ColorPickerBlock, IStylable
	{
		[SerializeField]
		Slider slider;

		/// <summary>
		/// Gets or sets the slider.
		/// </summary>
		/// <value>The slider.</value>
		public Slider Slider
		{
			get => slider;

			set => SetSlider(value);
		}

		[SerializeField]
		Image sliderBackground;

		/// <summary>
		/// Gets or sets the Blue slider background.
		/// </summary>
		/// <value>The Blue slider background.</value>
		public Image SliderBackground
		{
			get => sliderBackground;

			set
			{
				sliderBackground = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("defaultShader")]
		Shader defaultShaderHorizontal;

		/// <summary>
		/// Gets or sets the default shader to display gradients for horizontal slider background.
		/// </summary>
		/// <value>The default shader.</value>
		[Obsolete("Use DefaultShaderHorizontal and DefaultShaderVertical instead.")]
		public Shader DefaultShader
		{
			get => DefaultShaderHorizontal;

			set => DefaultShaderHorizontal = value;
		}

		/// <summary>
		/// Gets or sets the default shader to display gradients for horizontal slider background.
		/// </summary>
		/// <value>The default shader.</value>
		public Shader DefaultShaderHorizontal
		{
			get => defaultShaderHorizontal;

			set
			{
				defaultShaderHorizontal = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Shader defaultShaderVertical;

		/// <summary>
		/// Gets or sets the default shader to display gradients for vertical slider background.
		/// </summary>
		/// <value>The default shader.</value>
		public Shader DefaultShaderVertical
		{
			get
			{
				return defaultShaderVertical;
			}

			set
			{
				defaultShaderVertical = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Color colorLeft = Color.black;

		/// <summary>
		/// Gets or sets the color on left side (or bottom side).
		/// </summary>
		/// <value>The color.</value>
		public Color ColorLeft
		{
			get => colorLeft;

			set
			{
				colorLeft = value;
				UpdateView();
				ValueChanged();
			}
		}

		[SerializeField]
		Color colorRight = Color.white;

		/// <summary>
		/// Gets or sets the color on right side (or top side).
		/// </summary>
		/// <value>The color right.</value>
		public Color ColorRight
		{
			get => colorRight;

			set
			{
				colorRight = value;
				UpdateView();
				ValueChanged();
			}
		}

		[SerializeField]
		Color color = Color.white;

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		[DataBindField]
		public Color Color
		{
			get => color;

			set
			{
				inUpdateMode = true;

				SetColor(value);
				UpdateView();
				OnChange.Invoke(color);

				inUpdateMode = false;
			}
		}

		/// <summary>
		/// OnChangeRGB event.
		/// </summary>
		[SerializeField]
		[DataBindEvent(nameof(Color))]
		public ColorRGBChangedEvent OnChange = new ColorRGBChangedEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Slider = slider;
			Slider.minValue = 0;
			Slider.maxValue = 255;
			Slider.wholeNumbers = true;
			UpdateMaterial();
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			UpdateMaterial();
		}

		/// <summary>
		/// Sets the slider.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetSlider(Slider value)
		{
			if (slider != null)
			{
				slider.onValueChanged.RemoveListener(SliderValueChanged);
			}

			slider = value;
			if (slider != null)
			{
				slider.onValueChanged.AddListener(SliderValueChanged);
			}
		}

		void SliderValueChanged(float value)
		{
			ValueChanged();
		}

		/// <summary>
		/// If in update mode?
		/// </summary>
		protected bool inUpdateMode;

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetColor(Color value)
		{
			using var _ = ListPool<float>.Get(out var diffs);

			if ((ColorRight.r - ColorLeft.r) != 0f)
			{
				diffs.Add(Mathf.Abs((value.r - ColorLeft.r) / (ColorRight.r - ColorLeft.r)));
			}

			if ((ColorRight.g - ColorLeft.g) != 0f)
			{
				diffs.Add(Mathf.Abs((value.g - ColorLeft.g) / (ColorRight.g - ColorLeft.g)));
			}

			if ((ColorRight.b - ColorLeft.b) != 0f)
			{
				diffs.Add(Mathf.Abs((value.b - ColorLeft.b) / (ColorRight.b - ColorLeft.b)));
			}

			if ((ColorRight.a - ColorLeft.a) != 0f)
			{
				diffs.Add(Mathf.Abs((value.a - ColorLeft.a) / (ColorRight.a - ColorLeft.a)));
			}

			var t = diffs.Count == 0 ? 1f : UtilitiesCollections.Sum(diffs) / diffs.Count;
			color = Color.Lerp(ColorLeft, ColorRight, t);
			Slider.value = t * (Slider.maxValue - Slider.minValue);
		}

		/// <summary>
		/// Values the changed.
		/// </summary>
		protected virtual void ValueChanged()
		{
			if (!IsInteractable() || inUpdateMode)
			{
				return;
			}

			color = Color.Lerp(ColorLeft, ColorRight, Slider.value / (Slider.maxValue - Slider.minValue));
			OnChange.Invoke(color);
		}

		/// <summary>
		/// Updates the material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			var shader = UtilitiesUI.IsHorizontal(slider) ? defaultShaderHorizontal : DefaultShaderVertical;
			if (shader == null)
			{
				return;
			}

			sliderBackground.material = new Material(shader);

			UpdateViewReal();
		}

		/// <summary>
		/// Should be called if slider direction was changed.
		/// </summary>
		public virtual void DirectionChanged()
		{
			UpdateMaterial();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			UpdateViewReal();

			Compatibility.ToggleGameObject(slider);
		}

		/// <summary>
		/// Updates the view real.
		/// </summary>
		protected virtual void UpdateViewReal()
		{
			inUpdateMode = true;

			SetColor(color);

			if (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft)
			{
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Left, ColorLeft);
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Right, ColorRight);
				sliderBackground.SetMaterialDirty();
			}
			else
			{
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Bottom, ColorLeft);
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Top, ColorRight);
				sliderBackground.SetMaterialDirty();
			}

			inUpdateMode = false;
		}

		/// <inheritdoc/>
		public override void SetTargetOwner()
		{
			UIThemes.Utilities.SetTargetOwner(typeof(Color), sliderBackground, nameof(Graphic.color), this);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (slider != null)
			{
				var colorpicker_style = UtilitiesUI.IsHorizontal(slider)
					? style.ColorPickerRangeHorizontal
					: style.ColorPickerRangeVertical;

				if (TryGetComponent<Image>(out var bg))
				{
					colorpicker_style.Background.ApplyTo(bg);
				}

				if ((slider.handleRect != null) && slider.handleRect.TryGetComponent<Image>(out var img))
				{
					colorpicker_style.Handle.ApplyTo(img);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (slider != null)
			{
				var colorpicker_style = UtilitiesUI.IsHorizontal(slider)
					? style.ColorPickerRangeHorizontal
					: style.ColorPickerRangeVertical;

				if (TryGetComponent<Image>(out var bg))
				{
					colorpicker_style.Background.GetFrom(bg);
				}

				if ((slider.handleRect != null) && slider.handleRect.TryGetComponent<Image>(out var img))
				{
					colorpicker_style.Handle.GetFrom(img);
				}
			}

			return true;
		}
		#endregion
	}
}