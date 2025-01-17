﻿namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Utilities.
	/// </summary>
	public static class Utilities
	{
		/// <summary>
		/// Check if instance (interface) is null.
		/// </summary>
		/// <typeparam name="T">Type of instance.</typeparam>
		/// <param name="instance">Instance.</param>
		/// <returns>true if instance is null; otherwise false.</returns>
		public static bool IsNull<T>(T instance)
		{
			if (typeof(T).IsValueType)
			{
				return false;
			}

			return instance is UnityEngine.Object unity_obj
				? unity_obj == null
				: instance == null;
		}

		/// <summary>
		/// Check if instance id belong to the specified target.
		/// </summary>
		/// <typeparam name="T">Type of object.</typeparam>
		/// <param name="id">Object ID.</param>
		/// <param name="target">Object.</param>
		/// <returns>true if instance id belong to the specified target; otherwise false.</returns>
		public static bool IsSame<T>(InstanceID id, T target)
			where T : UnityEngine.Object
		{
			var b_null = target == null;
			if (b_null)
			{
				return false;
			}

			return id == new InstanceID(target);
		}

		/// <summary>
		/// Check if objects are the same instance.
		/// </summary>
		/// <typeparam name="T">Type of object.</typeparam>
		/// <param name="a">First object.</param>
		/// <param name="b">Second object.</param>
		/// <returns>true if objects are the same instance; otherwise false.</returns>
		public static bool IsSame<T>(T a, T b)
			where T : UnityEngine.Object
		{
			var a_null = a == null;
			var b_null = b == null;
			if (a_null && b_null)
			{
				return true;
			}

			if (a_null != b_null)
			{
				return false;
			}

			return a.GetInstanceID() == b.GetInstanceID();
		}

		/// <summary>
		/// Get path to gameobject.
		/// </summary>
		/// <param name="go">GameObject.</param>
		/// <returns>Path.</returns>
		public static string GameObjectPath(GameObject go)
		{
			var parent = go.transform.parent;
			return parent == null ? go.name : string.Format("{0}/{1}", GameObjectPath(parent.gameObject), go.name);
		}

		/// <summary>
		/// Get hierarchy depth of transform.
		/// </summary>
		/// <param name="transform">Transform.</param>
		/// <returns>Depth.</returns>
		public static int GetDepth(Transform transform)
		{
			int depth = 0;

			while (transform.parent != null)
			{
				transform = transform.parent;
				depth += 1;
			}

			return depth;
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <typeparam name="T">Component type.</typeparam>
		/// <param name="obj">Object.</param>
		/// <returns>Component.</returns>
		[Obsolete("Renamed to RequireComponent().")]
		public static T GetOrAddComponent<T>(Component obj)
			where T : Component
		{
			return RequireComponent<T>(obj);
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <typeparam name="T">Component type.</typeparam>
		/// <param name="obj">Object.</param>
		/// <returns>Component.</returns>
		public static T RequireComponent<T>(Component obj)
			where T : Component
		{
			if (!obj.TryGetComponent<T>(out var component))
			{
				component = obj.gameObject.AddComponent<T>();
			}

			return component;
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <typeparam name="T">Component type.</typeparam>
		/// <param name="obj">Object.</param>
		/// <returns>Component.</returns>
		[Obsolete("Renamed to RequireComponent().")]
		public static T GetOrAddComponent<T>(GameObject obj)
			where T : Component
		{
			return RequireComponent<T>(obj);
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <typeparam name="T">Component type.</typeparam>
		/// <param name="obj">Object.</param>
		/// <returns>Component.</returns>
		public static T RequireComponent<T>(GameObject obj)
			where T : Component
		{
			if (!obj.TryGetComponent<T>(out var component))
			{
				component = obj.AddComponent<T>();
			}

			return component;
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <param name="source">Source component.</param>
		/// <param name="target">Target component.</param>
		/// <typeparam name="T">Component type.</typeparam>
		[Obsolete("Renamed to RequireComponent().")]
		public static void GetOrAddComponent<T>(Component source, ref T target)
			where T : Component
		{
			RequireComponent(source, ref target);
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <param name="source">Source component.</param>
		/// <param name="target">Target component.</param>
		/// <typeparam name="T">Component type.</typeparam>
		public static void RequireComponent<T>(Component source, ref T target)
			where T : Component
		{
			if ((target != null) || (source == null))
			{
				return;
			}

			target = RequireComponent<T>(source);

#if UNITY_EDITOR
			Compatibility.MarkDirty(source);
			Compatibility.MarkDirty(target);
#endif
		}

		/// <summary>
		/// Fix the instantiated object (in some cases object have wrong position, rotation and scale).
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="instance">Instance.</param>
		public static void FixInstantiated(Component source, Component instance)
		{
			FixInstantiated(source.gameObject, instance.gameObject);
		}

		/// <summary>
		/// Fix the instantiated object (in some cases object have wrong position, rotation and scale).
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="instance">Instance.</param>
		public static void FixInstantiated(GameObject source, GameObject instance)
		{
			var defaultRectTransform = source.transform as RectTransform;
			if (defaultRectTransform == null)
			{
				return;
			}

			var rectTransform = instance.transform as RectTransform;

			FixInstantiated(defaultRectTransform, rectTransform);
		}

		/// <summary>
		/// Fix the instantiated object (in some cases object have wrong position, rotation and scale).
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="instance">Instance.</param>
		public static void FixInstantiated(RectTransform source, RectTransform instance)
		{
			if (instance.localPosition != source.localPosition)
			{
				instance.localPosition = source.localPosition;
			}

			if (instance.localRotation != source.localRotation)
			{
				instance.localRotation = source.localRotation;
			}

			if (instance.localScale != source.localScale)
			{
				instance.localScale = source.localScale;
			}

			if (instance.anchoredPosition != source.anchoredPosition)
			{
				instance.anchoredPosition = source.anchoredPosition;
			}

			if (instance.sizeDelta != source.sizeDelta)
			{
				instance.sizeDelta = source.sizeDelta;
			}
		}

		/// <summary>
		/// Default handle for the property changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event arguments.</param>
		public static void DefaultPropertyHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
		}

		/// <summary>
		/// Default handle for the changed event.
		/// </summary>
		public static void DefaultHandler()
		{
		}

		#region Obsolete misc

		/// <summary>
		/// Updates the layout.
		/// </summary>
		/// <param name="layout">Layout.</param>
		[Obsolete("Use LayoutUtilities.UpdateLayout() instead.")]
		public static void UpdateLayout(LayoutGroup layout)
		{
			LayoutUtilities.UpdateLayout(layout);
		}

		/// <summary>
		/// Is two float values is nearly equal?
		/// </summary>
		/// <param name="a">First value.</param>
		/// <param name="b">Second value.</param>
		/// <param name="epsilon">Epsilon.</param>
		/// <returns>true if two float values is nearly equal; otherwise false.</returns>
		[Obsolete("Replaced with Mathf.Approximately(a, b)")]
		public static bool NearlyEqual(float a, float b, float epsilon)
		{
			return Mathf.Approximately(a, b);
		}
		#endregion

		#region Obsolete UI

		/// <summary>
		/// Get graphic component from TextAdapter.
		/// </summary>
		/// <param name="adapter">Adapter.</param>
		/// <returns>Graphic component.</returns>
		public static Graphic GetGraphic(TextAdapter adapter)
		{
			return UtilitiesUI.GetGraphic(adapter);
		}

		/// <summary>
		/// Finds the canvas.
		/// </summary>
		/// <returns>The canvas.</returns>
		/// <param name="currentObject">Current object.</param>
		[Obsolete("Replaced with UtilitiesUI.FindCanvas()")]
		public static Transform FindCanvas(Transform currentObject)
		{
			return UtilitiesUI.FindCanvas(currentObject);
		}

		/// <summary>
		/// Finds the topmost canvas.
		/// </summary>
		/// <returns>The canvas.</returns>
		/// <param name="currentObject">Current object.</param>
		[Obsolete("Replaced with UtilitiesUI.FindTopmostCanvas()")]
		public static Transform FindTopmostCanvas(Transform currentObject)
		{
			return UtilitiesUI.FindTopmostCanvas(currentObject);
		}

		/// <summary>
		/// Calculates the drag position.
		/// </summary>
		/// <returns>The drag position.</returns>
		/// <param name="screenPosition">Screen position.</param>
		/// <param name="canvas">Canvas.</param>
		/// <param name="canvasRect">Canvas RectTransform.</param>
		[Obsolete("Replaced with UtilitiesUI.CalculateDragPosition()")]
		public static Vector3 CalculateDragPosition(Vector3 screenPosition, Canvas canvas, RectTransform canvasRect)
		{
			return UtilitiesUI.CalculateDragPosition(screenPosition, canvas, canvasRect);
		}

		/// <summary>
		/// Determines if slider is horizontal.
		/// </summary>
		/// <returns><c>true</c> if slider is horizontal; otherwise, <c>false</c>.</returns>
		/// <param name="slider">Slider.</param>
		[Obsolete("Replaced with UtilitiesUI.IsHorizontal()")]
		public static bool IsHorizontal(Slider slider)
		{
			return UtilitiesUI.IsHorizontal(slider);
		}

		/// <summary>
		/// Determines if scrollbar is horizontal.
		/// </summary>
		/// <returns><c>true</c> if scrollbar is horizontal; otherwise, <c>false</c>.</returns>
		/// <param name="scrollbar">Scrollbar.</param>
		[Obsolete("Replaced with UtilitiesUI.IsHorizontal()")]
		public static bool IsHorizontal(Scrollbar scrollbar)
		{
			return UtilitiesUI.IsHorizontal(scrollbar);
		}
		#endregion

		#region Obsolete RectTransform

		/// <summary>
		/// Get top left corner anchored position of the specified RectTransform.
		/// </summary>
		/// <param name="target">RectTransform.</param>
		/// <returns>Top left corner anchored position.</returns>
		[Obsolete("Replaced with UtilitiesRectTransform.GetTopLeftCorner()")]
		public static Vector2 GetTopLeftCorner(RectTransform target)
		{
			return UtilitiesRectTransform.GetTopLeftCorner(target);
		}

		/// <summary>
		/// Set top left corner position of the specified RectTransform.
		/// </summary>
		/// <param name="target">RectTransform.</param>
		/// <param name="position">Top left corner position.</param>
		[Obsolete("Replaced with UtilitiesRectTransform.SetTopLeftCorner()")]
		public static void SetTopLeftCorner(RectTransform target, Vector2 position)
		{
			UtilitiesRectTransform.SetTopLeftCorner(target, position);
		}

		/// <summary>
		/// Get top left corner global position of the specified RectTransform.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Top left corner global position.</returns>
		[Obsolete("Replaced with UtilitiesRectTransform.GetTopLeftCornerGlobalPosition()")]
		public static Vector2 GetTopLeftCornerGlobalPosition(RectTransform target)
		{
			return UtilitiesRectTransform.GetTopLeftCornerGlobalPosition(target, null);
		}

		/// <summary>
		/// Get top right corner global position of the specified RectTransform.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Top right corner global position.</returns>
		[Obsolete("Replaced with UtilitiesRectTransform.GetTopRightCornerGlobalPosition()")]
		public static Vector3 GetTopRightCornerGlobalPosition(RectTransform target)
		{
			return UtilitiesRectTransform.GetTopRightCornerGlobalPosition(target);
		}

		/// <summary>
		/// Copy RectTransform values.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="target">Target.</param>
		[Obsolete("Replaced with UtilitiesRectTransform.CopyValues()")]
		public static void CopyRectTransformValues(RectTransform source, RectTransform target)
		{
			UtilitiesRectTransform.CopyValues(source, target);
		}
		#endregion

		#region Obsolete Input

		/// <summary>
		/// Is pointer over screen?
		/// </summary>
		/// <returns>true if pointer over screen; otherwise false.</returns>
		[Obsolete("Replaced with CompatibilityInput.IsPointerOverScreen.")]
		public static bool IsPointerOverScreen()
		{
			return CompatibilityInput.IsPointerOverScreen;
		}
		#endregion

		#region Obsolete Time

		/// <summary>
		/// Function to get time to use with animations.
		/// Can be replaced with custom function.
		/// </summary>
		[Obsolete("Replaced with UtilitiesTime.GetTime().")]
		[DomainReloadExclude]
		public static readonly Func<bool, float> GetTime = UtilitiesTime.DefaultGetTime;

		/// <summary>
		/// Function to get unscaled time to use by widgets.
		/// Can be replaced with custom function.
		/// </summary>
		[Obsolete("Replaced with UtilitiesTime.GetTime(true)")]
		[DomainReloadExclude]
		public static readonly Func<float> GetUnscaledTime = UtilitiesTime.DefaultGetUnscaledTime;

		/// <summary>
		/// Function to get delta time to use with animations.
		/// Can be replaced with custom function.
		/// </summary>
		[Obsolete("Replaced with UtilitiesTime.GetDeltaTime().")]
		[DomainReloadExclude]
		public static readonly Func<bool, float> GetDeltaTime = UtilitiesTime.DefaultGetDeltaTime;

		/// <summary>
		/// Default GetTime function from the default Time class.
		/// </summary>
		/// <param name="unscaledTime">Return unscaled time.</param>
		/// <returns>Time.</returns>
		[Obsolete("Replaced with UtilitiesTime.DefaultGetTime().")]
		public static float DefaultGetTime(bool unscaledTime)
		{
			return UtilitiesTime.DefaultGetTime(unscaledTime);
		}

		/// <summary>
		/// Default GetUnscaledTime function.
		/// </summary>
		/// <returns>Default Time.unscaledTime.</returns>
		[Obsolete("Replaced with UtilitiesTime.DefaultGetUnscaledTime().")]
		public static float DefaultGetUnscaledTime()
		{
			return UtilitiesTime.DefaultGetUnscaledTime();
		}

		/// <summary>
		/// Default GetDeltaTime function from the default Time class.
		/// </summary>
		/// <param name="unscaledTime">Return unscaled delta time.</param>
		/// <returns>Delta Time.</returns>
		[Obsolete("Replaced with UtilitiesTime.DefaultGetDeltaTime().")]
		public static float DefaultGetDeltaTime(bool unscaledTime)
		{
			return UtilitiesTime.DefaultGetDeltaTime(unscaledTime);
		}

		/// <summary>
		/// Suspends the coroutine execution for the given amount of seconds using unscaled time.
		/// </summary>
		/// <param name="seconds">Delay in seconds.</param>
		/// <returns>Yield instruction to wait.</returns>
		[Obsolete("Replaced with UtilitiesTime.Wait(seconds, true).")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		public static IEnumerator WaitForSecondsUnscaled(float seconds)
		{
			return UtilitiesTime.Wait(seconds, true);
		}

		/// <summary>
		/// Check how much time takes to run specified action.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="log">Text to add to log.</param>
		[Obsolete("Replaced with UtilitiesTime.CheckRunTime().")]
		public static void CheckRunTime(Action action, string log = "")
		{
			UtilitiesTime.CheckRunTime(action, log);
		}
		#endregion

		#region Obsolete color

		/// <summary>
		/// Convert specified color to RGB hex.
		/// </summary>
		/// <returns>RGB hex.</returns>
		/// <param name="c">Color.</param>
		[Obsolete("Replaced with UtilitiesColor.RGB2Hex().")]
		public static string RGB2Hex(Color32 c)
		{
			return UtilitiesColor.RGB2Hex(c);
		}

		/// <summary>
		/// Convert specified color to RGBA hex.
		/// </summary>
		/// <returns>RGBA hex.</returns>
		/// <param name="c">Color.</param>
		[Obsolete("Replaced with UtilitiesColor.RGBA2Hex().")]
		public static string RGBA2Hex(Color32 c)
		{
			return UtilitiesColor.RGBA2Hex(c);
		}

		/// <summary>
		/// Converts the string representation of a number to its Byte equivalent. A return value indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <returns><c>true</c> if hex was converted successfully; otherwise, <c>false</c>.</returns>
		/// <param name="hex">A string containing a number to convert.</param>
		/// <param name="result">When this method returns, contains the 8-bit unsigned integer value equivalent to the number contained in s if the conversion succeeded, or zero if the conversion failed. The conversion fails if the s parameter is null or String.Empty, is not of the correct format, or represents a number less than MinValue or greater than MaxValue. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
		[Obsolete("Replaced with UtilitiesColor.TryParseHex().")]
		public static bool TryParseHex(string hex, out byte result)
		{
			return UtilitiesColor.TryParseHex(hex, out result);
		}

		/// <summary>
		/// Converts the string representation of a color to its Color equivalent. A return value indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <returns><c>true</c> if hex was converted successfully; otherwise, <c>false</c>.</returns>
		/// <param name="hex">A string containing a color to convert.</param>
		/// <param name="result">When this method returns, contains the color value equivalent to the color contained in hex if the conversion succeeded, or Color.black if the conversion failed. The conversion fails if the hex parameter is null or String.Empty, is not of the correct format. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
		[Obsolete("Use ColorUtility.TryParseHtmlString().")]
		public static bool TryHexToRGBA(string hex, out Color32 result)
		{
			return UtilitiesColor.TryHexToRGBA(hex, out result);
		}
		#endregion

		#region Obsolete Editor
#if UNITY_EDITOR
		/// <summary>
		/// Get friendly name of the specified type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>Friendly name.</returns>
		[Obsolete("Replaced with UtilitiesEditor.GetFriendlyTypeName().")]
		public static string GetFriendlyTypeName(Type type)
		{
			return UtilitiesEditor.GetFriendlyTypeName(type);
		}

		/// <summary>
		/// Get type by full name.
		/// </summary>
		/// <param name="typename">Type name.</param>
		/// <returns>Type.</returns>
		[Obsolete("Replaced with UtilitiesEditor.GetType().")]
		public static Type GetType(string typename)
		{
			return UtilitiesEditor.GetType(typename);
		}

		/// <summary>
		/// Create gameobject.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		/// <param name="undo">Support editor undo.</param>
		/// <returns>Created gameobject.</returns>
		[Obsolete("Replaced with Widgets.CreateGameObject().")]
		public static GameObject CreateGameObject(GameObject prefab, bool undo = true)
		{
			return null;
		}

		/// <summary>
		/// Returns the asset object of type T with the specified GUID.
		/// </summary>
		/// <param name="guid">GUID.</param>
		/// <returns>Asset with the specified GUID.</returns>
		/// <typeparam name="T">Asset type.</typeparam>
		[Obsolete("Replaced with UtilitiesEditor.LoadAssetWithGUID<T>().")]
		public static T LoadAssetWithGUID<T>(string guid)
			where T : UnityEngine.Object
		{
			return UtilitiesEditor.LoadAssetWithGUID<T>(guid);
		}

		/// <summary>
		/// Find assets by specified search.
		/// </summary>
		/// <typeparam name="T">Assets type.</typeparam>
		/// <param name="search">Search string.</param>
		/// <returns>Assets list.</returns>
		[Obsolete("Replaced with UtilitiesEditor.GetAssets<T>().")]
		public static List<T> GetAssets<T>(string search)
			where T : UnityEngine.Object
		{
			return UtilitiesEditor.GetAssets<T>(search);
		}

		/// <summary>
		/// Get asset by label.
		/// </summary>
		/// <typeparam name="T">Asset type.</typeparam>
		/// <param name="label">Asset label.</param>
		/// <returns>Asset.</returns>
		[Obsolete("Replaced with UtilitiesEditor.GetAsset<T>().")]
		public static T GetAsset<T>(string label)
			where T : UnityEngine.Object
		{
			return UtilitiesEditor.GetAsset<T>(label);
		}

		/// <summary>
		/// Get asset path by label.
		/// </summary>
		/// <param name="label">Asset label.</param>
		/// <returns>Asset path.</returns>
		[Obsolete("Replaced with UtilitiesEditor.GetAssetPath().")]
		public static string GetAssetPath(string label)
		{
			return UtilitiesEditor.GetAssetPath(label);
		}

		/// <summary>
		/// Get prefab by label.
		/// </summary>
		/// <param name="label">Prefab label.</param>
		/// <returns>Prefab.</returns>
		[Obsolete("Replaced with UtilitiesEditor.GetPrefab().")]
		public static GameObject GetPrefab(string label)
		{
			return UtilitiesEditor.GetPrefab(label);
		}

		/// <summary>
		/// Get generated prefab by label.
		/// </summary>
		/// <param name="label">Prefab label.</param>
		/// <returns>Prefab.</returns>
		[Obsolete("Replaced with UtilitiesEditor.GetGeneratedPrefab().")]
		public static GameObject GetGeneratedPrefab(string label)
		{
			return UtilitiesEditor.GetGeneratedPrefab(label);
		}

		/// <summary>
		/// Set prefabs label.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		[Obsolete("Replaced with UtilitiesEditor.SetPrefabLabel().")]
		public static void SetPrefabLabel(UnityEngine.Object prefab)
		{
			UtilitiesEditor.SetPrefabLabel(prefab);
		}

		/// <summary>
		/// Create widget template from asset specified by label.
		/// </summary>
		/// <param name="templateLabel">Template label.</param>
		/// <returns>Widget template.</returns>
		[Obsolete("Replaced with Widgets.CreateTemplateFromAsset().")]
		public static GameObject CreateWidgetTemplateFromAsset(string templateLabel)
		{
			return null;
		}

		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <param name="widget">Widget name.</param>
		/// <param name="applyStyle">Apply style to created widget.</param>
		/// <param name="converter">Converter for the created widget (mostly used to replace Unity Text with TMPro Text).</param>
		/// <returns>Created GameObject.</returns>
		[Obsolete("Replaced with Widgets.CreateFromAsset().")]
		public static GameObject CreateWidgetFromAsset(string widget, bool applyStyle = true, Action<GameObject> converter = null)
		{
			return null;
		}

		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <param name="prefab">Widget name.</param>
		/// <param name="applyStyle">Apply style to created widget.</param>
		/// <param name="converter">Converter for the created widget (mostly used to replace Unity Text with TMPro Text).</param>
		/// <returns>Created GameObject.</returns>
		[Obsolete("Replaced with Widgets.CreateFromPrefab().")]
		public static GameObject CreateWidgetFromPrefab(GameObject prefab, bool applyStyle = true, Action<GameObject> converter = null)
		{
			return null;
		}

		/// <summary>
		/// Replace Close button callback on Cancel instead of the Hide for the Dialog components in the specified GameObject.
		/// </summary>
		/// <param name="go">GameObject.</param>
		[Obsolete("Replaced with Widgets.FixDialogCloseButton().")]
		public static void FixDialogCloseButton(GameObject go)
		{
		}

		/// <summary>
		/// Gets the canvas transform.
		/// </summary>
		/// <returns>The canvas transform.</returns>
		[Obsolete("Replaced with Widgets.GetCanvasTransform().")]
		public static Transform GetCanvasTransform()
		{
			return null;
		}

		/// <summary>
		/// Serialize object with BinaryFormatter to base64 string.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <returns>Serialized string.</returns>
		[Obsolete("Replaced with UtilitiesEditor.Serialize().")]
		public static string Serialize(object obj)
		{
			return UtilitiesEditor.Serialize(obj);
		}

		/// <summary>
		/// De-serialize object with BinaryFormatter from base64 string.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		/// <param name="encoded">Serialized string.</param>
		/// <returns>De-serialized object.</returns>
		[Obsolete("Replaced with UtilitiesEditor.Deserialize<T>().")]
		public static T Deserialize<T>(string encoded)
		{
			return UtilitiesEditor.Deserialize<T>(encoded);
		}
#endif
		#endregion

		#region Obsolete Collections

		/// <summary>
		/// Create list.
		/// </summary>
		/// <typeparam name="T">Type of the item.</typeparam>
		/// <param name="count">Items count.</param>
		/// <param name="create">Function to create item.</param>
		/// <returns>List.</returns>
		[Obsolete("Replaced with UtilitiesCollections.CreateList<T>().")]
		public static ObservableList<T> CreateList<T>(int count, Func<int, T> create)
		{
			return UtilitiesCollections.CreateList(count, create);
		}

		/// <summary>
		/// Retrieves all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <typeparam name="T">Item type.</typeparam>
		/// <param name="source">Items.</param>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the elements to search for.</param>
		/// <returns>A List{T} containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty List{T}.</returns>
		[Obsolete("Replaced with UtilitiesCollections.FindAll<T>().")]
		public static ObservableList<T> FindAll<T>(List<T> source, Func<T, bool> match)
		{
			return UtilitiesCollections.FindAll(source, match);
		}

		/// <summary>
		/// Get sum of the list.
		/// </summary>
		/// <param name="source">List to sum.</param>
		/// <returns>Sum.</returns>
		[Obsolete("Replaced with UtilitiesCollections.Sum<T>().")]
		public static float Sum(List<float> source)
		{
			return UtilitiesCollections.Sum(source);
		}

		/// <summary>
		/// Check is input array not empty and all values are null.
		/// </summary>
		/// <typeparam name="T">Type of value.</typeparam>
		/// <param name="arr">Input array.</param>
		/// <returns>true if input array not empty and all values are null; otherwise false.</returns>
		[Obsolete("Replaced with UtilitiesCollections.AllNull<T>().")]
		public static bool AllNull<T>(T[] arr)
			where T : class
		{
			return UtilitiesCollections.AllNull(arr);
		}

		/// <summary>
		/// Prints the specified list to log.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="list">List.</param>
		[Obsolete("Replaced with UtilitiesCollections.Log<T>().")]
		public static void Log<T>(List<T> list)
		{
			UtilitiesCollections.Log(list);
		}
		#endregion
	}
}