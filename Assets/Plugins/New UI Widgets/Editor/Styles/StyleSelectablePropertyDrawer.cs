﻿#if UNITY_EDITOR
namespace UIWidgets.Styles
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Property drawer for StyleSelectable.
	/// </summary>
	[CustomPropertyDrawer(typeof(StyleSelectable))]
	public class StyleSelectablePropertyDrawer : PropertyDrawer
	{
		/// <summary>
		/// The indent.
		/// </summary>
		protected const float Indent = 16;

		/// <summary>
		/// The height.
		/// </summary>
		protected const float Height = 18;

		/// <summary>
		/// The empty space between properties.
		/// </summary>
		protected const float EmptySpace = 2;

		/// <summary>
		/// The labels.
		/// </summary>
		[DomainReloadExclude]
		protected static Dictionary<string, string> Labels = new Dictionary<string, string>()
		{
			{ "Transition", "HandleMinTransition" },
			{ "Colors", "HandleMinColors" },
			{ "Sprites", "HandleMinSprites" },
			{ "Animation", "HandleMinAnimation" },
		};

		/// <summary>
		/// Is opened?
		/// </summary>
		protected bool IsOpened = false;

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			position = DrawFoldout(position, label);

			if (IsOpened)
			{
				DrawHangleProperty(position, property);
			}

			EditorGUI.EndProperty();
		}

		/// <summary>
		/// Draw image properties.
		/// </summary>
		/// <param name="position">Start position.</param>
		/// <param name="property">Property.</param>
		/// <returns>New position.</returns>
		protected static Rect DrawHangleProperty(Rect position, SerializedProperty property)
		{
			var result = position;

			var transition = property.FindPropertyRelative("Transition");
			result = DrawProperty(result, transition);

			switch ((Selectable.Transition)transition.enumValueIndex)
			{
				case Selectable.Transition.None:
					break;
				case Selectable.Transition.ColorTint:
					result = DrawProperty(result, property.FindPropertyRelative("Colors"));
					break;
				case Selectable.Transition.SpriteSwap:
					result = DrawProperty(result, property.FindPropertyRelative("Sprites"));
					break;
				case Selectable.Transition.Animation:
					result = DrawProperty(result, property.FindPropertyRelative("Animation"));
					break;
				default:
					break;
			}

			return result;
		}

		/// <summary>
		/// Draws the property.
		/// </summary>
		/// <returns>The new position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="field">Field.</param>
		protected static Rect DrawProperty(Rect position, SerializedProperty field)
		{
			var rect = new Rect(position.x, position.y, position.width - Indent, EditorGUI.GetPropertyHeight(field));
			EditorGUI.PropertyField(rect, field, true);

			position.y += rect.height + EmptySpace;

			return position;
		}

		/// <summary>
		/// Draws the foldout.
		/// </summary>
		/// <returns>The new position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="label">Label.</param>
		protected Rect DrawFoldout(Rect position, GUIContent label)
		{
			position.height = Height;
			IsOpened = EditorGUI.Foldout(position, IsOpened, label, true);

			position.x += Indent;
			position.y += Height + EmptySpace;

			return position;
		}

		/// <summary>
		/// Gets the height of the property.
		/// </summary>
		/// <returns>The height of the property.</returns>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var result = 16f;

			if (!IsOpened)
			{
				return result + EmptySpace;
			}

			result += GetHandleHeight(property) + EmptySpace;

			return result + EmptySpace;
		}

		/// <summary>
		/// Gets the height of the property.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <returns>The height of the property.</returns>
		protected static float GetHandleHeight(SerializedProperty property)
		{
			var result = 0f;

			var transition = property.FindPropertyRelative("Transition");
			result += EditorGUI.GetPropertyHeight(transition);

			switch ((Selectable.Transition)transition.enumValueIndex)
			{
				case Selectable.Transition.None:
					break;
				case Selectable.Transition.ColorTint:
					result += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Colors")) + EmptySpace;
					break;
				case Selectable.Transition.SpriteSwap:
					result += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Sprites")) + EmptySpace;
					break;
				case Selectable.Transition.Animation:
					result += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Animation")) + EmptySpace;
					break;
				default:
					break;
			}

			return result;
		}
	}
}
#endif