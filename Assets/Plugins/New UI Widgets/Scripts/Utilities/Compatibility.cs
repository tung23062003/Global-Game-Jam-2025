﻿namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Networking;
	using UnityEngine.UI;
#if UNITY_EDITOR
	using UnityEditor;
#endif

	/// <summary>
	/// Compatibility functions.
	/// </summary>
	public static class Compatibility
	{
#if UNITY_EDITOR
		[DomainReloadExclude]
		static readonly char[] ScriptingSymbolSeparator = new char[] { ';' };

		/// <summary>
		/// Get scripting define symbols.
		/// </summary>
		/// <param name="targetGroup">Target group.</param>
		/// <returns>Scripting define symbols.</returns>
		public static HashSet<string> GetScriptingDefineSymbols(BuildTargetGroup targetGroup)
		{
			#if UNITY_2023_1_OR_NEWER
			var build_target = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
			var symbols = PlayerSettings.GetScriptingDefineSymbols(build_target);
			#else
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
			#endif

			var result = new HashSet<string>();
			foreach (var symbol in symbols.Split(ScriptingSymbolSeparator))
			{
				if (symbol.Length > 0)
				{
					result.Add(symbol);
				}
			}

			return result;
		}

		/// <summary>
		/// Set scripting define symbols.
		/// </summary>
		/// <param name="targetGroup">Target group.</param>
		/// <param name="symbols">Symbols.</param>
		public static void SetScriptingDefineSymbols(BuildTargetGroup targetGroup, HashSet<string> symbols)
		{
			var arr = new string[symbols.Count];
			symbols.CopyTo(arr);
			var str = string.Join(ScriptingSymbolSeparator[0].ToString(), arr);

			#if UNITY_2023_1_OR_NEWER
			var build_target = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
			PlayerSettings.SetScriptingDefineSymbols(build_target, str);
			#else
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, str);
			#endif
		}

		/// <summary>
		/// Upgrade the specified target.
		/// </summary>
		/// <typeparam name="T">Target type.</typeparam>
		/// <param name="target">Target.</param>
		public static void Upgrade<T>(T target)
			where T : Component, IUpgradeable
		{
			target.Upgrade();

			MarkDirty(target);
		}

		/// <summary>
		/// Mark specified component as dirty.
		/// </summary>
		/// <typeparam name="T">Type of the component.</typeparam>
		/// <param name="target">Component.</param>
		public static void MarkDirty<T>(T target)
			where T : Component
		{
#if UNITY_2018_3_OR_NEWER
			if (PrefabUtility.IsPartOfAnyPrefab(target))
#endif
			{
				PrefabUtility.RecordPrefabInstancePropertyModifications(target);
			}

			if (IsPrefab(target))
			{
				EditorUtility.SetDirty(target);
			}
		}

		/// <summary>
		/// Remove text previously added to force recompile.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <returns>True if text removed; otherwise false</returns>
		[Obsolete("Replaced with RemoveForceRecompileByGUID().")]
		public static bool RemoveForceRecompileByLabel(string label)
		{
			var path = Utilities.GetAssetPath(label);
			if (path == null)
			{
				return false;
			}

			if (Directory.Exists(path))
			{
				RemoveForceRecompileFolder(path);
			}
			else
			{
				RemoveForceRecompileFile(path);
			}

			AssetDatabase.Refresh();

			return true;
		}

		static void RemoveForceRecompileFolder(string path)
		{
			var dir = new DirectoryInfo(path);
			var files = dir.GetFiles("*.cs", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				RemoveForceRecompileFile(file.FullName);
			}
		}

		static void RemoveForceRecompileFile(string path)
		{
			using var _ = ListPool<string>.Get(out var lines);
			lines.AddRange(File.ReadAllLines(path));
			var prefix = "// Force script reload at ";

			if (lines[lines.Count - 1].StartsWith(prefix, StringComparison.InvariantCulture))
			{
				lines.RemoveAt(lines.Count - 1);

				File.WriteAllText(path, string.Join("\r\n", lines.ToArray()));
				File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
			}
		}

		/// <summary>
		/// Use this function to create a Prefab Asset at the given path from the given GameObject including any children in the Scene without modifying the input objects.
		/// </summary>
		/// <param name="assetPath">The path to save the Prefab at.</param>
		/// <param name="instanceRoot">The GameObject to save as a Prefab.</param>
		/// <returns>Prefab instance.</returns>
		public static GameObject CreatePrefab(string assetPath, GameObject instanceRoot)
		{
#if UNITY_2018_3_OR_NEWER
			var prefab = PrefabUtility.SaveAsPrefabAsset(instanceRoot, assetPath, out var success);
			if (!success)
			{
				Debug.LogError("Prefab was not saved: " + instanceRoot.name);
			}

			return prefab;
#else
			return PrefabUtility.CreatePrefab(assetPath, instanceRoot);
#endif
		}

		static void ForceRecompileFolder(string path)
		{
			var dir = new DirectoryInfo(path);
			var files = dir.GetFiles("*.cs", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				ForceRecompileFile(file.FullName);
			}
		}

		/// <summary>
		/// Force recompile by changing specified script content.
		/// </summary>
		/// <param name="path">Path to file.</param>
		static void ForceRecompileFile(string path)
		{
			using var _ = ListPool<string>.Get(out var lines);
			lines.AddRange(File.ReadAllLines(path));
			var prefix = "// Force script reload at ";

			if (lines[lines.Count - 1].StartsWith(prefix, StringComparison.InvariantCulture))
			{
				lines[lines.Count - 1] = prefix + DateTime.Now.ToString();
			}
			else
			{
				lines.Add(prefix + DateTime.Now.ToString());
			}

			File.WriteAllText(path, string.Join("\r\n", lines.ToArray()));
			File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
		}

		/// <summary>
		/// Remove text previously added to force recompile.
		/// </summary>
		/// <param name="guids">Scripts GUIDs.</param>
		public static void RemoveForceRecompileByGUID(string[] guids)
		{
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				if (string.IsNullOrEmpty(path))
				{
					Debug.LogWarning("Cannot find path by GUID: " + guids[0]);
					continue;
				}

				if (Directory.Exists(path))
				{
					RemoveForceRecompileFolder(path);
				}
				else
				{
					RemoveForceRecompileFile(path);
				}
			}

			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Force recompile by changing scripts content.
		/// </summary>
		/// <param name="guids">Scripts GUIDs.</param>
		public static void ForceRecompileByGUID(string[] guids)
		{
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				if (string.IsNullOrEmpty(path))
				{
					Debug.LogWarning("Cannot find path by GUID: " + guids[0]);
					continue;
				}

				if (Directory.Exists(path))
				{
					ForceRecompileFolder(path);
				}
				else
				{
					ForceRecompileFile(path);
				}
			}

			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Force recompile by changing scripts content.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <returns>True if recompilation done; otherwise false</returns>
		[Obsolete("Replaced with ForceRecompileByGUID().")]
		public static bool ForceRecompileByLabel(string label)
		{
			var path = Utilities.GetAssetPath(label);
			if (path == null)
			{
				return false;
			}

			if (Directory.Exists(path))
			{
				ForceRecompileFolder(path);
			}
			else
			{
				ForceRecompileFile(path);
			}

			AssetDatabase.Refresh();

			return true;
		}

		/// <summary>
		/// Get TextMeshPro version.
		/// </summary>
		/// <returns>TextMeshPro version.</returns>
		[Obsolete]
		public static string GetTMProVersion()
		{
			Dictionary<string, string> guid2version = new Dictionary<string, string>()
			{
				{ "496f2e385b0c62542b5c739ccfafd8da", "V1Script" }, // first assetstore version
				{ "89f0137620f6af44b9ba852b4190e64e", "V2DLL" }, // unity assetstore version
				{ "f4688fdb7df04437aeb418b961361dc5", "V3Package" }, // unity package version
			};

			foreach (var gv in guid2version)
			{
				var path = AssetDatabase.GUIDToAssetPath(gv.Key);
				if (!string.IsNullOrEmpty(path))
				{
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1
					if (gv.Value == "V1Script")
					{
						return gv.Value + "NoInput";
					}
#endif
					return gv.Value;
				}
			}

			return null;
		}

		/// <summary>
		/// Imports package at packagePath into the current project.
		/// If interactive is true, an import package dialog will be opened, else all assets in the package will be imported into the current project.
		/// </summary>
		/// <param name="packagePath">Package path.</param>
		/// <param name="interactive">Show import package dialog.</param>
		public static void ImportPackage(string packagePath, bool interactive = false)
		{
			AssetDatabase.ImportPackage(packagePath, interactive);
		}

		/// <summary>
		/// Set ScrollRect viewport.
		/// </summary>
		/// <param name="scrollRect">ScrollRect.</param>
		/// <param name="viewport">Viewport.</param>
		public static void SetViewport(ScrollRect scrollRect, RectTransform viewport)
		{
			scrollRect.viewport = viewport;
		}

		/// <summary>
		/// Save scene if user wants to.
		/// </summary>
		/// <returns>True if user saved scene; otherwise false.</returns>
		public static bool SceneSave()
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			return UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
#else
			return EditorApplication.SaveCurrentSceneIfUserWantsTo();
#endif
		}

		/// <summary>
		/// Save scene with the specified path.
		/// </summary>
		/// <param name="path">Path.</param>
		public static void SceneSave(string path)
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), path);
#else
			EditorApplication.SaveScene(path);
#endif
		}

		/// <summary>
		/// Create new empty scene.
		/// </summary>
		public static void SceneNew()
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#else
			EditorApplication.NewScene();

			var camera = GameObject.Find("Main Camera");
			if (camera != null)
			{
				UnityEngine.Object.DestroyImmediate(camera);
			}
#endif
		}

		/// <summary>
		/// Returns the first asset object of type T at given path assetPath.
		/// </summary>
		/// <returns>The asset at path.</returns>
		/// <param name="path">Path.</param>
		/// <typeparam name="T">Asset type.</typeparam>
		public static T LoadAssetAtPath<T>(string path)
			where T : UnityEngine.Object
		{
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			return AssetDatabase.LoadAssetAtPath<T>(path);
#else
			return Resources.LoadAssetAtPath<T>(path);
#endif
		}

		/// <summary>
		/// Create EventSystem.
		/// </summary>
		public static void CreateEventSystem()
		{
			var eventSystemGO = new GameObject("EventSystem");
			eventSystemGO.AddComponent<EventSystem>();
			eventSystemGO.AddComponent<StandaloneInputModule>();
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			eventSystemGO.AddComponent<TouchInputModule>();
#endif

			Undo.RegisterCreatedObjectUndo(eventSystemGO, "Create " + eventSystemGO.name);
		}
#endif

#if UNITY_2018_3_OR_NEWER
		/// <summary>
		/// Check if request ended with any error.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <returns>true if request has error; otherwise false.</returns>
		public static bool IsError(UnityWebRequest request)
		{
#if UNITY_2020_2_OR_NEWER
			return (request.result == UnityWebRequest.Result.ConnectionError)
				|| (request.result == UnityWebRequest.Result.DataProcessingError)
			|| (request.result == UnityWebRequest.Result.ProtocolError);
#else
			return request.isNetworkError || request.isHttpError;
#endif
		}
#endif

		/// <summary>
		/// Clones the object original and returns the clone.
		/// </summary>
		/// <typeparam name="T">Type of the original object.</typeparam>
		/// <param name="original">An existing object that you want to make a copy of.</param>
		/// <returns>The instantiated clone.</returns>
		public static T Instantiate<T>(T original)
			where T : UnityEngine.Object
		{
			return UnityEngine.Object.Instantiate(original);
		}

		/// <summary>
		/// Clones the object original and returns the clone.
		/// </summary>
		/// <typeparam name="T">Type of the original object.</typeparam>
		/// <param name="original">An existing object that you want to make a copy of.</param>
		/// <param name="parent">Parent.</param>
		/// <returns>The instantiated clone.</returns>
		public static T Instantiate<T>(T original, Transform parent)
			where T : Component
		{
			return UnityEngine.Object.Instantiate(original, parent);
		}

		/// <summary>
		/// Set layout group child size settings.
		/// </summary>
		/// <param name="lg">Layout group.</param>
		/// <param name="width">Control width.</param>
		/// <param name="height">Control height.</param>
#if !UNITY_2017_1_OR_NEWER
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "width", Justification = "Reviewed.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "lg", Justification = "Reviewed.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "height", Justification = "Reviewed.")]
#endif
		public static void SetLayoutChildControlsSize(HorizontalOrVerticalLayoutGroup lg, bool width, bool height)
		{
#if UNITY_2017_1_OR_NEWER
			lg.childControlWidth = width;
			lg.childControlHeight = height;
#endif
		}

		/// <summary>
		/// Get layout group childControlWidth setting.
		/// </summary>
		/// <param name="lg">Layout group.</param>
		/// <returns>childControlWidth value.</returns>
		public static bool GetLayoutChildControlWidth(HorizontalOrVerticalLayoutGroup lg)
		{
#if UNITY_2017_1_OR_NEWER
			return lg.childControlWidth;
#else
			return true;
#endif
		}

		/// <summary>
		/// Get layout group childControlHeight setting.
		/// </summary>
		/// <param name="lg">Layout group.</param>
		/// <returns>childControlHeight value.</returns>
		public static bool GetLayoutChildControlHeight(HorizontalOrVerticalLayoutGroup lg)
		{
#if UNITY_2017_1_OR_NEWER
			return lg.childControlHeight;
#else
			return true;
#endif
		}

		/// <summary>
		/// Gets the cursor mode.
		/// </summary>
		/// <returns>The cursor mode.</returns>
		[Obsolete("No more used.")]
		public static CursorMode GetCursorMode()
		{
#if UNITY_WEBGL
			return CursorMode.ForceSoftware;
#else
			return CursorMode.Auto;
#endif
		}

		/// <summary>
		/// Gets the components.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="components">Components.</param>
		/// <typeparam name="T">Component type.</typeparam>
		[Obsolete("No more used.")]
		public static void GetComponents<T>(GameObject source, List<T> components)
			where T : class
		{
			source.GetComponents(components);
		}

		/// <summary>
		/// Gets the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="source">Source.</param>
		/// <typeparam name="T">Component type.</typeparam>
		[Obsolete("No more used.")]
		public static T GetComponent<T>(Component source)
			where T : class
		{
			source.TryGetComponent<T>(out var component);
			return component;
		}

		/// <summary>
		/// Gets the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="source">Source.</param>
		/// <typeparam name="T">Component type.</typeparam>
		[Obsolete("No more used.")]
		public static T GetComponent<T>(GameObject source)
			where T : class
		{
			source.TryGetComponent<T>(out var component);
			return component;
		}

		/// <summary>
		/// Disable and enable gameobject back to apply shader variables changes.
		/// </summary>
		/// <param name="go">Target gameobject.</param>
		public static void ToggleGameObject(GameObject go)
		{
			go.SetActive(false);
			go.SetActive(true);
		}

		/// <summary>
		/// Disable and enable gameobject back to apply shader variables changes.
		/// </summary>
		/// <param name="component">Target component.</param>
		public static void ToggleGameObject(Component component)
		{
			ToggleGameObject(component.gameObject);
		}

		/// <summary>
		/// Returns all components of Type T in the target GameObject or any of its children.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="target">Target gameobject or component.</param>
		/// <param name="includeInactive">Should Components on inactive GameObjects be included in the found set? includeInactive decides which children of the GameObject will be searched. The GameObject that you call GetComponentsInChildren on is always searched regardless.</param>
		/// <param name="results">A list of all found components matching the specified type.</param>
		[Obsolete("No more used.")]
		public static void GetComponentsInChildren<T>(Component target, bool includeInactive, List<T> results)
			where T : class
		{
			target.GetComponentsInChildren(includeInactive, results);
		}

		/// <summary>
		/// Returns the component of Type T in the GameObject or any of its children using depth first search.
		/// </summary>
		/// <typeparam name="T">Component type.</typeparam>
		/// <param name="component">Component.</param>
		/// <param name="includeInactive">Should Components on inactive GameObjects be included in the found set? includeInactive decides which children of the GameObject will be searched. The GameObject that you call GetComponentsInChildren on is always searched regardless.</param>
		/// <returns>A component of the matching type, if found.</returns>
		[Obsolete("No more used.")]
		public static T GetComponentInChildren<T>(Component component, bool includeInactive)
			where T : Component
		{
#if UNITY_2017_1_OR_NEWER
			return component.GetComponentInChildren<T>(includeInactive);
#else
			var children = component.GetComponentsInChildren<T>(includeInactive);
			return (children.Length > 0) ? children[0] : null;
#endif
		}

		/// <summary>
		/// Get root game objects at scene.
		/// </summary>
		/// <param name="output">Root game objects.</param>
		public static void GetRootGameObjects(List<GameObject> output)
		{
			for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
			{
				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
				if (!scene.IsValid() || !scene.isLoaded)
				{
					continue;
				}

				output.AddRange(scene.GetRootGameObjects());
			}
		}

		/// <summary>
		/// Check is component is part of the prefab.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if component is part of the prefab; otherwise false.</returns>
		public static bool IsPrefab(Component component)
		{
#if UNITY_EDITOR
#if UNITY_2018_2_OR_NEWER
			if ((PrefabUtility.GetCorrespondingObjectFromSource(component) == null)
				&& (PrefabUtility.GetPrefabInstanceHandle(component) != null))
			{
				return true;
			}
#else
			if ((PrefabUtility.GetPrefabParent(component.gameObject) == null)
				&& (PrefabUtility.GetPrefabObject(component.gameObject) != null))
			{
				return true;
			}
#endif
#endif

			return false;
		}

		/// <summary>
		/// Find first object of the specified type.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="includeInactive">Include inactive.</param>
		/// <returns>First object.</returns>
		public static T FindFirstObjectOfType<T>(bool includeInactive = true)
			 where T : UnityEngine.Object
		{
#if UNITY_2023_1_OR_NEWER
			return UnityEngine.Object.FindFirstObjectByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
#elif UNITY_2020_3_OR_NEWER
			return UnityEngine.Object.FindObjectOfType<T>(includeInactive);
#else
			return UnityEngine.Object.FindObjectOfType<T>();
#endif
		}

#if CSHARP_7_3_OR_NEWER
		/// <summary>
		/// Get empty array.
		/// </summary>
		/// <typeparam name="T">Type of array.</typeparam>
		/// <returns>Empty array.</returns>
		public static T[] EmptyArray<T>() => Array.Empty<T>();
#else
		/// <summary>
		/// Empty array class.
		/// </summary>
		/// <typeparam name="T">Type of array.</typeparam>
		static class Empty<T>
		{
			/// <summary>
			/// Value.
			/// </summary>
			public static readonly T[] Value;

			static Empty() => Value = new T[0];
		}

		/// <summary>
		/// Get empty array.
		/// </summary>
		/// <typeparam name="T">Type of array.</typeparam>
		/// <returns>Empty array.</returns>
		public static T[] EmptyArray<T>() => Empty<T>.Value;
#endif
	}
}