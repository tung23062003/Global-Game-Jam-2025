﻿namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Tabs.
	/// </summary>
	public class Tabs : TabsBase, IStylable<StyleTabs>, IUpgradeable
	{
		/// <summary>
		/// Information of the Tab button.
		/// </summary>
		[Serializable]
		public class TabButtonInfo
		{
			[SerializeField]
			[FormerlySerializedAs("Owner")]
			Tabs owner;

			[SerializeField]
			[FormerlySerializedAs("DefaultButton")]
			Button defaultButton;

			/// <summary>
			/// Default button.
			/// </summary>
			public Button DefaultButton => defaultButton;

			[SerializeField]
			[FormerlySerializedAs("ActiveButton")]
			Button activeButton;

			/// <summary>
			/// Active button.
			/// </summary>
			public Button ActiveButton => activeButton;

			/// <summary>
			/// Default component.
			/// </summary>
			public TabButtonComponentBase DefaultComponent
			{
				get;
				protected set;
			}

			/// <summary>
			/// Active component.
			/// </summary>
			public TabButtonComponentBase ActiveComponent
			{
				get;
				protected set;
			}

			[SerializeField]
			[FormerlySerializedAs("Tab")]
			Tab tab;

			/// <summary>
			/// Initializes a new instance of the <see cref="TabButtonInfo"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="defaultButton">Default button.</param>
			/// <param name="activeButton">Active button.</param>
			public TabButtonInfo(Tabs owner, Button defaultButton, Button activeButton)
			{
				this.owner = owner;

				this.defaultButton = defaultButton;
				this.defaultButton.transform.SetParent(this.owner.Container, false);
				if (this.defaultButton.TryGetComponent<TabButtonComponentBase>(out var default_btn))
				{
					DefaultComponent = default_btn;
				}

				this.activeButton = activeButton;
				this.activeButton.transform.SetParent(this.owner.Container, false);

				if (this.activeButton.TryGetComponent<TabButtonComponentBase>(out var active))
				{
					ActiveComponent = active;
				}

				this.defaultButton.onClick.AddListener(Click);
				DefaultComponent.OnSelectEvent.AddListener(Select);
			}

			/// <summary>
			/// Process the select event.
			/// </summary>
			protected void Select()
			{
				if (owner.ImmediateSelect)
				{
					owner.ProcessButtonClick(tab);
				}
			}

			/// <summary>
			/// Process the click event.
			/// </summary>
			protected void Click() => owner.ProcessButtonClick(tab);

			/// <summary>
			/// Set the tab.
			/// </summary>
			/// <param name="tab">Tab.</param>
			public void SetTab(Tab tab)
			{
				this.tab = tab;

				SetData();
			}

			/// <summary>
			/// Set buttons data.
			/// </summary>
			public void SetData()
			{
				DefaultComponent.SetButtonData(tab);
				ActiveComponent.SetButtonData(tab);
			}

			/// <summary>
			/// Remove buttons callback.
			/// </summary>
			protected void RemoveCallback()
			{
				if (defaultButton != null)
				{
					defaultButton.onClick.RemoveListener(Click);
				}
			}

			/// <summary>
			/// Enable buttons interactions.
			/// </summary>
			public void EnableInteractable()
			{
				defaultButton.interactable = true;
				activeButton.interactable = true;
			}

			/// <summary>
			/// Disable buttons interactions
			/// </summary>
			public void DisableInteractable()
			{
				defaultButton.interactable = false;
				activeButton.interactable = false;
			}

			/// <summary>
			/// Toggle to the default state.
			/// </summary>
			public void Default()
			{
				defaultButton.gameObject.SetActive(true);
				activeButton.gameObject.SetActive(false);
			}

			/// <summary>
			/// Toggle to the active state.
			/// </summary>
			public void Active()
			{
				defaultButton.gameObject.SetActive(false);
				activeButton.gameObject.SetActive(true);

				if (owner.EventSystemSelectActiveHeader && activeButton.gameObject.activeInHierarchy && (EventSystem.current != null))
				{
					if (EventSystem.current.alreadySelecting)
					{
						Updater.RunOnce(activeButton, SetSelected);
					}
					else
					{
						SetSelected();
					}
				}
			}

			void SetSelected()
			{
				EventSystem.current.SetSelectedGameObject(activeButton.gameObject);
			}

			/// <summary>
			/// Set the style.
			/// </summary>
			/// <param name="style">Style.</param>
			public void SetStyle(StyleTabs style)
			{
				style.DefaultButton.ApplyTo(defaultButton.gameObject);
				style.ActiveButton.ApplyTo(activeButton.gameObject);
			}

			/// <summary>
			/// Destroy buttons.
			/// </summary>
			public void Destroy()
			{
				Updater.RemoveRunOnce(SetSelected);

				RemoveCallback();

				if (defaultButton != null)
				{
					UnityEngine.Object.Destroy(defaultButton.gameObject);
				}

				if (activeButton != null)
				{
					UnityEngine.Object.Destroy(activeButton.gameObject);
				}

				owner = null;
			}
		}

		/// <summary>
		/// Container for the tabs buttons.
		/// </summary>
		[SerializeField]
		public Transform Container;

		/// <summary>
		/// The default tab button.
		/// </summary>
		[SerializeField]
		public Button DefaultTabButton;

		/// <summary>
		/// The active tab button.
		/// </summary>
		[SerializeField]
		public Button ActiveTabButton;

		[SerializeField]
		Tab[] tabObjects = Compatibility.EmptyArray<Tab>();

		/// <summary>
		/// Gets or sets the tab objects.
		/// </summary>
		/// <value>The tab objects.</value>
		public Tab[] TabObjects
		{
			get => tabObjects;

			set
			{
				tabObjects = value;
				UpdateButtons();
			}
		}

		/// <summary>
		/// The name of the default tab.
		/// </summary>
		[SerializeField]
		[Tooltip("Tab name which will be active by default, if not specified will be opened first Tab.")]
		public string DefaultTabName = string.Empty;

		/// <summary>
		/// If true does not deactivate hidden tabs.
		/// </summary>
		[SerializeField]
		[Tooltip("If true does not deactivate hidden tabs.")]
		public bool KeepTabsActive = false;

		/// <summary>
		/// Toggle tab on EventSystem select event.
		/// </summary>
		[SerializeField]
		[Tooltip("Toggle tab on EventSystem select event.")]
		public bool ImmediateSelect = false;

		/// <summary>
		/// OnTabSelect event.
		/// </summary>
		[SerializeField]
		public TabSelectEvent OnTabSelect = new TabSelectEvent();

		/// <summary>
		/// Gets or sets the selected tab.
		/// </summary>
		/// <value>The selected tab.</value>
		public Tab SelectedTab
		{
			get;
			protected set;
		}

		/// <inheritdoc/>
		public override int SelectedTabIndex => Array.IndexOf(TabObjects, SelectedTab);

		/// <inheritdoc/>
		protected override int TabsCount => TabObjects.Length;

		/// <summary>
		/// Buttons.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TabButtonInfo> Buttons = new List<TabButtonInfo>();

		/// <summary>
		/// Make active header selected by EventsSystem.
		/// </summary>
		[SerializeField]
		[Tooltip("Select active header by EventsSystem.")]
		public bool EventSystemSelectActiveHeader = true;

		/// <summary>
		/// Check is tab can be selected.
		/// </summary>
		public Func<Tab, bool> CanSelectTab = AllowSelect;

		/// <summary>
		/// Default function for the CanSelectTab.
		/// </summary>
		[DomainReloadExclude]
		public static Func<Tab, bool> AllowSelect = x => true;

		/// <summary>
		/// Init this instance only once.
		/// </summary>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (Container == null)
			{
				throw new InvalidOperationException("Container is null. Set object of type GameObject to Container.");
			}

			if (DefaultTabButton == null)
			{
				throw new InvalidOperationException("DefaultTabButton is null. Set object of type GameObject to DefaultTabButton.");
			}

			if (ActiveTabButton == null)
			{
				throw new InvalidOperationException("ActiveTabButton is null. Set object of type GameObject to ActiveTabButton.");
			}

			DefaultTabButton.gameObject.SetActive(false);
			ActiveTabButton.gameObject.SetActive(false);

			UpdateButtons();
		}

		bool localeSubscription;

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			if (!localeSubscription)
			{
				Init();

				localeSubscription = true;
				Localization.OnLocaleChanged += LocaleChanged;
				LocaleChanged();
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Localization.OnLocaleChanged -= LocaleChanged;

			for (int i = 0; i < Buttons.Count; i++)
			{
				Buttons[i].Destroy();
			}

			Buttons.Clear();
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		protected virtual void LocaleChanged()
		{
			UpdateButtonsData();
		}

		/// <summary>
		/// Update buttons data.
		/// </summary>
		protected virtual void UpdateButtonsData()
		{
			for (int i = 0; i < Buttons.Count; i++)
			{
				Buttons[i].SetData();
			}
		}

		/// <summary>
		/// Process tab button click.
		/// </summary>
		/// <param name="tab">Tab.</param>
		protected virtual void ProcessButtonClick(Tab tab)
		{
			if (CanSelectTab(tab))
			{
				SelectTab(tab);
			}
		}

		/// <summary>
		/// Updates the buttons.
		/// </summary>
		protected virtual void UpdateButtons()
		{
			CreateButtons();

			if (tabObjects.Length == 0)
			{
				return;
			}

			if (!string.IsNullOrEmpty(DefaultTabName))
			{
				var index = Name2Index(DefaultTabName);
				if (index != -1)
				{
					SelectTab(index);
				}
				else
				{
					Debug.LogWarning(string.Format("Tab with specified DefaultTabName \"{0}\" not found. Opened the first Tab.", DefaultTabName), this);
					SelectTab(0);
				}
			}
			else
			{
				SelectTab(0);
			}
		}

		/// <summary>
		/// Is exists tab with specified name.
		/// </summary>
		/// <param name="tabName">Tab name.</param>
		/// <returns>true if exists tab with specified name; otherwise, false.</returns>
		protected virtual bool IsExistsTabName(string tabName)
		{
			return Name2Index(tabName) != -1;
		}

		/// <summary>
		/// Get index for the specified Tab name.
		/// </summary>
		/// <param name="name">Tab name.</param>
		/// <returns>Index.</returns>
		protected int Name2Index(string name)
		{
			for (int i = 0; i < tabObjects.Length; i++)
			{
				if (tabObjects[i].Name == name)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Selects the tab.
		/// </summary>
		/// <param name="tabName">Tab name.</param>
		public virtual void SelectTab(string tabName)
		{
			var index = Name2Index(tabName);
			if (index == -1)
			{
				throw new ArgumentException(string.Format("Tab with name \"{0}\" not found.", tabName));
			}

			SelectTab(index);
		}

		/// <summary>
		/// Selects the tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void SelectTab(Tab tab)
		{
			var index = Array.IndexOf(tabObjects, tab);
			if (index == -1)
			{
				throw new ArgumentException(string.Format("Tab with name \"{0}\" not found.", tab.Name));
			}

			SelectTab(index);
		}

		/// <inheritdoc/>
		public override void SelectTab(int index)
		{
			if ((index < 0) || (index >= tabObjects.Length))
			{
				throw new ArgumentException(string.Format("Invalid tab index \"{0}\"; should be in range 0..{1}", index.ToString(), (tabObjects.Length - 1).ToString()));
			}

			if (KeepTabsActive)
			{
				tabObjects[index].TabObject.transform.SetAsLastSibling();
			}
			else
			{
				foreach (var tab in tabObjects)
				{
					DeactivateTab(tab);
				}

				tabObjects[index].TabObject.SetActive(true);
			}

			foreach (var button in Buttons)
			{
				DefaultState(button);
			}

			Buttons[index].Active();

			SelectedTab = tabObjects[index];
			OnTabSelect.Invoke(index);
		}

		/// <inheritdoc/>
		protected override bool CanSelectTabByIndex(int index) => CanSelectTab(TabObjects[index]);

		/// <summary>
		/// Deactivate tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		protected virtual void DeactivateTab(Tab tab) => tab.TabObject.SetActive(false);

		/// <summary>
		/// Activate button.
		/// </summary>
		/// <param name="button">Button.</param>
		protected virtual void DefaultState(TabButtonInfo button) => button.Default();

		/// <summary>
		/// Enable button interactions.
		/// </summary>
		/// <param name="button">Button.</param>
		protected virtual void EnableInteractable(TabButtonInfo button) => button.EnableInteractable();

		/// <summary>
		/// Creates the buttons.
		/// </summary>
		protected virtual void CreateButtons()
		{
			foreach (var button in Buttons)
			{
				EnableInteractable(button);
			}

			if (tabObjects.Length > Buttons.Count)
			{
				for (int i = Buttons.Count; i < tabObjects.Length; i++)
				{
					var defaultButton = Compatibility.Instantiate(DefaultTabButton);

					var activeButton = Compatibility.Instantiate(ActiveTabButton);

					Buttons.Add(new TabButtonInfo(this, defaultButton, activeButton));
				}
			}

			// delete existing buttons if necessary
			if (tabObjects.Length < Buttons.Count)
			{
				for (int i = Buttons.Count - 1; i > tabObjects.Length - 1; i--)
				{
					Buttons[i].Destroy();

					Buttons.RemoveAt(i);
				}
			}

			for (int i = 0; i < Buttons.Count; i++)
			{
				SetButtonName(Buttons[i], i);
			}
		}

		/// <summary>
		/// Sets the name of the button.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="index">Index.</param>
		protected virtual void SetButtonName(TabButtonInfo button, int index) => button.SetTab(TabObjects[index]);

		/// <summary>
		/// Disable the tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void DisableTab(Tab tab)
		{
			var i = Array.IndexOf(TabObjects, tab);
			if (i != -1)
			{
				Buttons[i].DisableInteractable();
			}
		}

		/// <summary>
		/// Enable the tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void EnableTab(Tab tab)
		{
			var i = Array.IndexOf(TabObjects, tab);
			if (i != -1)
			{
				Buttons[i].EnableInteractable();
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
			if (DefaultTabButton != null)
			{
				var default_info = Utilities.RequireComponent<TabButtonComponentBase>(DefaultTabButton);
				default_info.Upgrade();
				if (default_info.NameAdapter == null)
				{
					Utilities.RequireComponent(default_info.GetComponentInChildren<Text>(true), ref default_info.NameAdapter);
				}
			}

			if (ActiveTabButton != null)
			{
				var active_info = Utilities.RequireComponent<TabButtonComponentBase>(ActiveTabButton);
				active_info.Upgrade();
				if (active_info.NameAdapter == null)
				{
					Utilities.RequireComponent(active_info.GetComponentInChildren<Text>(true), ref active_info.NameAdapter);
				}
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(StyleTabs styleTyped, Style style)
		{
			if (DefaultTabButton != null)
			{
				styleTyped.DefaultButton.ApplyTo(DefaultTabButton.gameObject);
			}

			if (ActiveTabButton != null)
			{
				styleTyped.ActiveButton.ApplyTo(ActiveTabButton.gameObject);
			}

			for (int i = 0; i < Buttons.Count; i++)
			{
				Buttons[i].SetStyle(styleTyped);
			}

			for (var i = 0; i < tabObjects.Length; i++)
			{
				var tab = tabObjects[i];
				if (tab.TabObject != null)
				{
					styleTyped.ContentBackground.ApplyTo(tab.TabObject.GetComponent<Image>());
					style.ApplyForChildren(tab.TabObject);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(StyleTabs styleTyped, Style style)
		{
			if (DefaultTabButton != null)
			{
				styleTyped.DefaultButton.GetFrom(DefaultTabButton.gameObject);
			}

			if (ActiveTabButton != null)
			{
				styleTyped.ActiveButton.GetFrom(ActiveTabButton.gameObject);
			}

			for (var i = 0; i < tabObjects.Length; i++)
			{
				var tab = tabObjects[i];
				if (tab.TabObject != null)
				{
					styleTyped.ContentBackground.GetFrom(tab.TabObject.GetComponent<Image>());
					style.GetFromChildren(tab.TabObject);
				}
			}

			return true;
		}

		#endregion
	}
}