﻿namespace UIWidgets.Examples
{
	using UIWidgets;
	using UIWidgets.Extensions;
	using UnityEngine;

	/// <summary>
	/// Copy data from Combobox to Autocomplete.
	/// </summary>
	[RequireComponent(typeof(Autocomplete))]
	[RequireComponent(typeof(Combobox))]
	[System.Obsolete]
	public class ComboboxAutocompleteCopyData : MonoBehaviour, IUpdatable
	{
		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void OnEnable()
		{
			Updater.Add(this);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDisable()
		{
			Updater.Remove(this);
		}

		/// <summary>
		/// Copy data and destroy this component.
		/// </summary>
		public void RunUpdate()
		{
			if (TryGetComponent<Combobox>(out var target) && TryGetComponent<Autocomplete>(out var source))
			{
				target.ListView.DataSource = source.DataSource.ToObservableList();
			}

			Destroy(this);
		}
	}
}