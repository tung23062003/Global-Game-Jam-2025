namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Color picker RGB palette.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/colorpicker.html")]
	public class ColorPickerRGBPalette : ColorPickerBlock
	{
		[SerializeField]
		Image palette;

		RectTransform paletteRect;

		DragListener dragListener;

		ClickListener clickListener;

		/// <summary>
		/// Gets or sets the palette.
		/// </summary>
		/// <value>The palette.</value>
		public Image Palette
		{
			get => palette;

			set => SetPalette(value);
		}

		[SerializeField]
		Shader paletteShader;

		/// <summary>
		/// Gets or sets the shader to display gradient in palette.
		/// </summary>
		/// <value>The palette shader.</value>
		public Shader PaletteShader
		{
			get => paletteShader;

			set
			{
				paletteShader = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		RectTransform paletteCursor;

		/// <summary>
		/// Gets or sets the palette cursor.
		/// </summary>
		/// <value>The palette cursor.</value>
		public RectTransform PaletteCursor
		{
			get => paletteCursor;

			set
			{
				paletteCursor = value;
				if (paletteCursor != null)
				{
					UpdateView();
				}
			}
		}

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
		/// Gets or sets the slider background.
		/// </summary>
		/// <value>The slider background.</value>
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
		Shader sliderShader;

		/// <summary>
		/// Gets or sets the shader to display gradient for slider background.
		/// </summary>
		/// <value>The slider shader.</value>
		public Shader SliderShader
		{
			get => sliderShader;

			set
			{
				sliderShader = value;
				UpdateMaterial();
			}
		}

		ColorPickerInputMode inputMode;

		/// <summary>
		/// Gets or sets the input mode.
		/// </summary>
		/// <value>The input mode.</value>
		public ColorPickerInputMode InputMode
		{
			get => inputMode;

			set => inputMode = value;
		}

		ColorPickerPaletteMode paletteMode;

		/// <summary>
		/// Gets or sets the palette mode.
		/// </summary>
		/// <value>The palette mode.</value>
		public ColorPickerPaletteMode PaletteMode
		{
			get => paletteMode;

			set => SetPaletteMode(value);
		}

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// OnChangeRGB event.
		/// </summary>
		public ColorRGBChangedEvent OnChangeRGB = new ColorRGBChangedEvent();

		/// <summary>
		/// OnChangeHSV event.
		/// </summary>
		public ColorHSVChangedEvent OnChangeHSV = new ColorHSVChangedEvent();

		/// <summary>
		/// OnChangeAlpha event.
		/// </summary>
		public ColorAlphaChangedEvent OnChangeAlpha = new ColorAlphaChangedEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Palette = palette;
			Slider = slider;
			SliderBackground = sliderBackground;
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			UpdateMaterial();
		}

		/// <summary>
		/// Sets the palette.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetPalette(Image value)
		{
			if (dragListener != null)
			{
				dragListener.OnDragEvent.RemoveListener(OnDrag);
			}

			palette = value;
			if (palette != null)
			{
				paletteRect = palette.transform as RectTransform;

				dragListener = Utilities.RequireComponent<DragListener>(palette);
				dragListener.OnDragEvent.AddListener(OnDrag);

				clickListener = Utilities.RequireComponent<ClickListener>(palette);
				clickListener.ClickEvent.AddListener(OnDrag);

				UpdateMaterial();
			}
			else
			{
				paletteRect = null;
			}
		}

		/// <summary>
		/// Sets the palette mode.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetPaletteMode(ColorPickerPaletteMode value)
		{
			paletteMode = value;
			var is_active = paletteMode == ColorPickerPaletteMode.Red
				|| paletteMode == ColorPickerPaletteMode.Green
				|| paletteMode == ColorPickerPaletteMode.Blue;
			gameObject.SetActive(is_active);
			if (is_active)
			{
				UpdateView();
			}
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
		/// Values the changed.
		/// </summary>
		protected virtual void ValueChanged()
		{
			if (!IsInteractable() || inUpdateMode)
			{
				return;
			}

			currentColor = GetColor();

			OnChangeRGB.Invoke(currentColor);
		}

		/// <summary>
		/// Current color.
		/// </summary>
		protected Color32 currentColor;

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(Color32 color)
		{
			currentColor = color;
			UpdateView();
		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(ColorHSV color)
		{
			currentColor = color;
			UpdateView();
		}

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData)
		{
			return (eventData.button == DragButton) && IsInteractable();
		}

		/// <summary>
		/// When drag is occurring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			Vector2 size = paletteRect.rect.size;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(paletteRect, eventData.position, eventData.pressEventCamera, out var cur_pos);

			cur_pos.x = Mathf.Clamp(cur_pos.x, 0, size.x);
			cur_pos.y = Mathf.Clamp(cur_pos.y, -size.y, 0);
			paletteCursor.localPosition = cur_pos;

			ValueChanged();
		}

		/// <summary>
		/// Gets the color.
		/// </summary>
		/// <returns>The color.</returns>
		protected Color32 GetColor()
		{
			var coordinates = GetCursorCoords();

			var s = (byte)slider.value;
			var x = (byte)Mathf.RoundToInt(coordinates.x);
			var y = (byte)Mathf.RoundToInt(coordinates.y);

			return paletteMode switch
			{
				ColorPickerPaletteMode.Red => new Color32(s, y, x, currentColor.a),
				ColorPickerPaletteMode.Green => new Color32(y, s, x, currentColor.a),
				ColorPickerPaletteMode.Blue => new Color32(x, y, s, currentColor.a),
				_ => currentColor,
			};
		}

		/// <summary>
		/// Gets the cursor coords.
		/// </summary>
		/// <returns>The cursor coords.</returns>
		protected Vector2 GetCursorCoords()
		{
			var coords = paletteCursor.localPosition;
			var size = paletteRect.rect.size;

			var x = (coords.x / size.x) * 255;
			var y = ((coords.y / size.y) + 1) * 255;

			return new Vector2(x, y);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			UpdateViewReal();

			Compatibility.ToggleGameObject(Palette);
		}

		/// <summary>
		/// Updates the view real.
		/// </summary>
		protected virtual void UpdateViewReal()
		{
			inUpdateMode = true;

			// set slider value
			if (slider != null)
			{
				slider.value = GetSliderValue();
			}

			// set slider colors
			if (sliderBackground != null)
			{
				var colors = GetSliderColors();

				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Bottom, colors[0]);
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Top, colors[1]);
				sliderBackground.SetMaterialDirty();
			}

			// set palette drag position
			if ((paletteCursor != null) && (palette != null) && (paletteRect != null))
			{
				var coords = GetPaletteCoords();
				var size = paletteRect.rect.size;

				paletteCursor.localPosition = new Vector3((coords.x / 255f) * size.x, -(1 - (coords.y / 255f)) * size.y, 0);
			}

			// set palette colors
			if (palette != null)
			{
				var colors = GetPaletteColors();

				palette.material.SetColor(ColorPicker.ShaderIDs.Left, colors[0]);
				palette.material.SetColor(ColorPicker.ShaderIDs.Right, colors[1]);
				palette.material.SetColor(ColorPicker.ShaderIDs.Bottom, colors[2]);
				palette.material.SetColor(ColorPicker.ShaderIDs.Top, colors[3]);
				palette.SetMaterialDirty();
			}

			inUpdateMode = false;
		}

		/// <summary>
		/// Gets the slider value.
		/// </summary>
		/// <returns>The slider value.</returns>
		protected int GetSliderValue()
		{
			return paletteMode switch
			{
				ColorPickerPaletteMode.Red => currentColor.r,
				ColorPickerPaletteMode.Green => currentColor.g,
				ColorPickerPaletteMode.Blue => currentColor.b,
				_ => 0,
			};
		}

		/// <summary>
		/// Gets the slider colors.
		/// </summary>
		/// <returns>The slider colors.</returns>
		protected Color32[] GetSliderColors()
		{
			return paletteMode switch
			{
				ColorPickerPaletteMode.Red => new Color32[]
				{
					new Color32(0, currentColor.g, currentColor.b, 255),
					new Color32(255, currentColor.g, currentColor.b, 255),
				},
				ColorPickerPaletteMode.Green => new Color32[]
				{
					new Color32(currentColor.r, 0, currentColor.b, 255),
					new Color32(currentColor.r, 255, currentColor.b, 255),
				},
				ColorPickerPaletteMode.Blue => new Color32[]
				{
					new Color32(currentColor.r, currentColor.g, 0, 255),
					new Color32(currentColor.r, currentColor.g, 255, 255),
				},
				_ => new Color32[]
				{
					new Color32(0, 0, 0, 255),
					new Color32(255, 255, 255, 255),
				},
			};
		}

		/// <summary>
		/// Gets the palette coords.
		/// </summary>
		/// <returns>The palette coords.</returns>
		protected Vector2 GetPaletteCoords()
		{
			return paletteMode switch
			{
				ColorPickerPaletteMode.Red => new Vector2(currentColor.b, currentColor.g),
				ColorPickerPaletteMode.Green => new Vector2(currentColor.b, currentColor.r),
				ColorPickerPaletteMode.Blue => new Vector2(currentColor.r, currentColor.g),
				_ => new Vector2(0, 0),
			};
		}

		/// <summary>
		/// Gets the palette colors.
		/// </summary>
		/// <returns>The palette colors.</returns>
		protected Color[] GetPaletteColors()
		{
			return paletteMode switch
			{
				ColorPickerPaletteMode.Red => new Color[]
				{
					new Color(currentColor.r / 255f / 2f, 0f, 0f, 1f),
					new Color(currentColor.r / 255f / 2f, 0f, 1f, 1f),
					new Color(currentColor.r / 255f / 2f, 0f, 0f, 1f),
					new Color(currentColor.r / 255f / 2f, 1f, 0f, 1f),
				},
				ColorPickerPaletteMode.Green => new Color[]
				{
					new Color(0f, currentColor.g / 255f / 2f, 0f, 1f),
					new Color(0f, currentColor.g / 255f / 2f, 1f, 1f),
					new Color(0f, currentColor.g / 255f / 2f, 0f, 1f),
					new Color(1f, currentColor.g / 255f / 2f, 0f, 1f),
				},
				ColorPickerPaletteMode.Blue => new Color[]
				{
					new Color(0f, 0f, currentColor.b / 255f / 2f, 1f),
					new Color(1f, 0f, currentColor.b / 255f / 2f, 1f),
					new Color(0f, 0f, currentColor.b / 255f / 2f, 1f),
					new Color(0f, 1f, currentColor.b / 255f / 2f, 1f),
				},
				_ => new Color[]
				{
					new Color(0f, 0f, 0f, 1f),
					new Color(1f, 1f, 1f, 1f),
					new Color(0f, 0f, 0f, 1f),
					new Color(1f, 1f, 1f, 1f),
				},
			};
		}

		/// <summary>
		/// Updates the material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			if ((paletteShader != null) && (palette != null))
			{
				palette.material = new Material(paletteShader);
			}

			if ((sliderShader != null) && (sliderBackground != null))
			{
				sliderBackground.material = new Material(sliderShader);
			}

			UpdateViewReal();
		}

		/// <inheritdoc/>
		public override void SetTargetOwner()
		{
			UIThemes.Utilities.SetTargetOwner(typeof(Color), palette, nameof(Graphic.color), this);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), sliderBackground, nameof(Graphic.color), this);
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnDestroy()
		{
			dragListener = null;
			slider = null;
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void SetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if (palette.transform.parent.TryGetComponent<Image>(out var border))
			{
				styleColorPicker.PaletteBorder.ApplyTo(border);
			}

			if ((paletteCursor != null) && paletteCursor.TryGetComponent<Image>(out var cursor))
			{
				styleColorPicker.PaletteCursor.ApplyTo(cursor);
			}

			if ((slider != null) && (slider.handleRect != null) && slider.handleRect.TryGetComponent<Image>(out var img))
			{
				var handle_style = UtilitiesUI.IsHorizontal(slider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.ApplyTo(img);
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void GetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if (palette.transform.parent.TryGetComponent<Image>(out var border))
			{
				styleColorPicker.PaletteBorder.GetFrom(border);
			}

			if ((paletteCursor != null) && paletteCursor.TryGetComponent<Image>(out var cursor))
			{
				styleColorPicker.PaletteCursor.GetFrom(cursor);
			}

			if ((slider != null) && (slider.handleRect != null) && slider.handleRect.TryGetComponent<Image>(out var img))
			{
				var handle_style = UtilitiesUI.IsHorizontal(slider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.GetFrom(img);
			}
		}
	}
}