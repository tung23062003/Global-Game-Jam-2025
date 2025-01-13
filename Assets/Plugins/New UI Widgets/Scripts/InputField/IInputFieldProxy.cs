﻿// <auto-generated/>
// Auto-generated added to suppress names errors.

namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// IInputFieldProxy.
	/// </summary>
	public interface IInputFieldProxy
	{
		/// <summary>
		/// The current value of the input field.
		/// </summary>
		/// <value>The text.</value>
		string text
		{
			get;
			set;
		}

		/// <summary>
		/// Is InputField has rich text support?
		/// </summary>
		bool richText
		{
			get;
		}

		/// <summary>
		/// Determines whether InputField instance is null.
		/// </summary>
		/// <returns><c>true</c> if InputField instance is null; otherwise, <c>false</c>.</returns>
		bool IsNull();

		/// <summary>
		/// Gets the gameobject.
		/// </summary>
		/// <value>The gameobject.</value>
		GameObject gameObject
		{
			get;
		}

		/// <summary>
		/// The Unity Event to call when edit.
		/// </summary>
		/// <value>The OnValueChange.</value>
		UnityEvent<string> onValueChanged
		{
			get;
		}

		/// <summary>
		/// The Unity Event to call when editing has ended.
		/// </summary>
		/// <value>The OnEndEdit.</value>
		UnityEvent<string> onEndEdit
		{
			get;
		}

		/// <summary>
		/// Current InputField caret position (also selection tail).
		/// </summary>
		/// <value>The caret position.</value>
		int caretPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Is the InputField eligible for interaction (excludes canvas groups).
		/// </summary>
		/// <value><c>true</c> if interactable; otherwise, <c>false</c>.</value>
		bool interactable
		{
			get;
			set;
		}

		/// <summary>
		/// Text component.
		/// </summary>
		Graphic textComponent
		{
			get;
			set;
		}

		/// <summary>
		/// Placeholder.
		/// </summary>
		Graphic placeholder
		{
			get;
			set;
		}

		/// <summary>
		/// If the proxy was canceled and will revert back to the original text.
		/// </summary>
		bool wasCanceled
		{
			get;
		}

		/// <summary>
		/// Determines whether this lineType is LineType.MultiLineNewline.
		/// </summary>
		/// <returns><c>true</c> if lineType is LineType.MultiLineNewline; otherwise, <c>false</c>.</returns>
		bool IsMultiLineNewline();

		/// <summary>
		/// Function to activate the InputField to begin processing Events.
		/// </summary>
		void Activate();

		/// <summary>
		/// Deactivate InputField.
		/// </summary>
		void Deactivate();

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		/// <summary>
		/// Move caret to end.
		/// </summary>
		void MoveToEnd();
#endif

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		void Focus();

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		void Select();

		/// <summary>
		/// Set text without raising onValueChanged event.
		/// </summary>
		/// <param name="text">Text.</param>
		void SetTextWithoutNotify(string text);
	}
}