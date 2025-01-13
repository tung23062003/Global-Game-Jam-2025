#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// Proxy for TMP_InputField.
	/// </summary>
	public class InputFieldTMProProxy : IInputFieldProxy
	{
		/// <summary>
		/// The InputField.
		/// </summary>
		readonly TMP_InputField inputField;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.InputFieldTMProProxy"/> class.
		/// </summary>
		/// <param name="input">Input.</param>
		public InputFieldTMProProxy(TMP_InputField input)
		{
			inputField = input;
		}

#region IInputFieldProxy implementation

		/// <summary>
		/// The current value of the input field.
		/// </summary>
		/// <value>The text.</value>
		public string text
		{
			get => inputField.text;

			set
			{
				if (inputField.text == value)
				{
					return;
				}

				inputField.text = value;

				// workaround to properly update layout because TMP_InputField does not call LayoutRebuilder.MarkLayoutForRebuild() when it should be
				if (inputField.IsActive())
				{
					LayoutRebuilder.MarkLayoutForRebuild(inputField.transform as RectTransform);
				}
			}
		}

		/// <summary>
		/// Is InputField has rich text support?
		/// </summary>
		public bool richText => inputField.richText;

		/// <summary>
		/// The Unity Event to call when edit.
		/// </summary>
		/// <value>The OnValueChange.</value>
		public UnityEvent<string> onValueChanged
		{
			get
			{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				return inputField.onValueChanged;
#else
				return inputField.onValueChange;
#endif
			}
		}

		/// <summary>
		/// The Unity Event to call when editing has ended.
		/// </summary>
		/// <value>The OnEndEdit.</value>
		public UnityEvent<string> onEndEdit => inputField.onEndEdit;

		/// <summary>
		/// Current InputField caret position (also selection tail).
		/// </summary>
		/// <value>The caret position.</value>
		public int caretPosition
		{
			get
			{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				return inputField.caretPosition;
#else
				return inputField.text.Length;
#endif
			}

			set
			{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				inputField.caretPosition = value;
#else
				// inputField.ActivateInputField();
#endif
			}
		}

		/// <summary>
		/// Is the InputField eligible for interaction (excludes canvas groups).
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public bool interactable
		{
			get => inputField.interactable;

			set => inputField.interactable = value;
		}

		/// <summary>
		/// Determines whether InputField instance is null.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsNull() => inputField == null;

		/// <summary>
		/// Gets the game object.
		/// </summary>
		/// <value>The game object.</value>
		public GameObject gameObject => (inputField != null) ? inputField.gameObject : null;

		/// <summary>
		/// Text component.
		/// </summary>
		public Graphic textComponent
		{
			get => inputField.textComponent;

			set => inputField.textComponent = value as TMP_Text;
		}

		/// <summary>
		/// Placeholder.
		/// </summary>
		public Graphic placeholder
		{
			get => inputField.placeholder;

			set => inputField.placeholder = value;
		}

		/// <summary>
		/// If the proxy was canceled and will revert back to the original text.
		/// </summary>
		public bool wasCanceled => inputField.wasCanceled;

		/// <summary>
		/// Determines whether this lineType is LineType.MultiLineNewline.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsMultiLineNewline() => inputField.lineType == TMP_InputField.LineType.MultiLineNewline;

		/// <summary>
		/// Function to activate the InputField to begin processing Events.
		/// </summary>
		public void Activate() => inputField.ActivateInputField();

		/// <summary>
		/// Deactivate InputField.
		/// </summary>
		public void Deactivate() => inputField.DeactivateInputField();

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Focus()
		{
			Activate();
			inputField.Select();
		}

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Select()
		{
			Activate();
			inputField.Select();
		}

		/// <summary>
		/// Set text without raising onValueChanged event.
		/// </summary>
		/// <param name="text">Text.</param>
		public void SetTextWithoutNotify(string text) => inputField.SetTextWithoutNotify(text);
		#endregion
	}
}
#endif