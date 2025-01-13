﻿namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// File with data for ListViewString.
	/// </summary>
	[RequireComponent(typeof(ListViewString))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/collections/listviewstring-datafile.html")]
	public class ListViewStringDataFile : MonoBehaviourInitiable
	{
		[NonSerialized]
		ListViewString listView;

		/// <summary>
		/// ListView.
		/// </summary>
		public ListViewString ListView
		{
			get
			{
				if (listView == null)
				{
					TryGetComponent(out listView);
				}

				return listView;
			}
		}

		[SerializeField]
		TextAsset file;

		/// <summary>
		/// Gets or sets the file with strings for ListView. One string per line.
		/// </summary>
		/// <value>The file.</value>
		public TextAsset File
		{
			get => file;

			set
			{
				file = value;
				if (file != null)
				{
					ListView.DataSource = GetItemsFromFile(file);
					ListView.ScrollToPosition(0);
				}
			}
		}

		/// <summary>
		/// The comments in file start with specified strings.
		/// </summary>
		[SerializeField]
		public List<string> CommentsStartWith = new List<string>() { "#", "//" };

		/// <summary>
		/// Allow only unique strings.
		/// </summary>
		[SerializeField]
		public bool Unique = true;

		/// <summary>
		/// Allow empty strings.
		/// </summary>
		[SerializeField]
		public bool AllowEmptyItems;

		/// <summary>
		/// Create a new list.
		/// </summary>
		[SerializeField]
		public bool CreateNewList;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if ((ListView == null) || (File == null))
			{
				return;
			}

			var items = GetItemsFromFile(File);
			items.Comparison = ListView.DataSource.Comparison;

			if (CreateNewList)
			{
				ListView.DataSource = items;
			}
			else
			{
				using var _ = ListView.DataSource.BeginUpdate();
				ListView.DataSource.Clear();
				ListView.DataSource.AddRange(items);
			}

			ListView.ScrollToPosition(0);
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		/// <param name="sourceFile">Source file.</param>
		/// <returns>Items list.</returns>
		public ObservableList<string> GetItemsFromFile(TextAsset sourceFile)
		{
			var result = new ObservableList<string>();

			if (file == null)
			{
				result.Comparison = ListView.ItemsComparison;
				return result;
			}

			foreach (var str in sourceFile.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
			{
				var item = str.TrimEnd();

				if (Unique && result.Contains(item))
				{
					continue;
				}

				if (!AllowEmptyItems && string.IsNullOrEmpty(item))
				{
					continue;
				}

				if (IsComment(item))
				{
					continue;
				}

				result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// Is comment?
		/// </summary>
		/// <param name="item">Item to check.</param>
		/// <returns>true if item is comment; otherwise false.</returns>
		protected bool IsComment(string item)
		{
			foreach (var comment in CommentsStartWith)
			{
				if (UtilitiesCompare.StartsWith(item, comment))
				{
					return true;
				}
			}

			return false;
		}
	}
}