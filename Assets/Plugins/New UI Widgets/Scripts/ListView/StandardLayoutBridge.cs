﻿namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Layout bridge to Horizontal or Vertical Layout Group.
	/// </summary>
	public class StandardLayoutBridge : ILayoutBridge
	{
		readonly bool isHorizontal;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is horizontal.
		/// </summary>
		/// <value><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</value>
		public bool IsHorizontal
		{
			get => isHorizontal;

			set
			{
				if (isHorizontal != value)
				{
					throw new NotSupportedException("HorizontalLayoutGroup Or VerticalLayoutGroup direction cannot be change in runtime.");
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UIWidgets.StandardLayoutBridge"/> update content size fitter.
		/// </summary>
		/// <value><c>true</c> if update content size fitter; otherwise, <c>false</c>.</value>
		public bool UpdateContentSizeFitter
		{
			get;
			set;
		}

		readonly HorizontalOrVerticalLayoutGroup Layout;

		readonly RectTransform DefaultItem;

		readonly LayoutElement FirstFiller;

		readonly LayoutElement LastFiller;

		readonly ContentSizeFitter fitter;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.StandardLayoutBridge"/> class.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="defaultItem">Default item.</param>
		/// <param name="updateContentSizeFitter">Update ContentSizeFitter on direction change.</param>
		public StandardLayoutBridge(HorizontalOrVerticalLayoutGroup layout, RectTransform defaultItem, bool updateContentSizeFitter = true)
		{
			LayoutUtilities.UpdateLayout(layout);

			Layout = layout;
			DefaultItem = defaultItem;
			UpdateContentSizeFitter = updateContentSizeFitter;

			isHorizontal = layout is HorizontalLayoutGroup;

			var firstFillerGO = new GameObject("FirstFiller");
			var firstFillerTransform = Utilities.RequireComponent<RectTransform>(firstFillerGO);
			firstFillerTransform.SetParent(Layout.transform, false);
			firstFillerTransform.localScale = Vector3.one;
			FirstFiller = firstFillerGO.AddComponent<LayoutElement>();

			var lastFillerGO = new GameObject("LastFiller");
			var lastFillerTransform = Utilities.RequireComponent<RectTransform>(lastFillerGO);
			lastFillerTransform.SetParent(Layout.transform, false);
			lastFillerTransform.localScale = Vector3.one;
			LastFiller = lastFillerGO.AddComponent<LayoutElement>();

			var size = GetItemSize();
			if (IsHorizontal)
			{
				firstFillerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				lastFillerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}
			else
			{
				firstFillerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				lastFillerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
			}

			Layout.TryGetComponent(out fitter);
		}

		bool isUpdating;

		/// <summary>
		/// Updates the layout.
		/// </summary>
		public void UpdateLayout()
		{
			if (isUpdating)
			{
				return;
			}

			isUpdating = true;

			LayoutRebuilder.ForceRebuildLayoutImmediate(Layout.transform as RectTransform);

			if (fitter != null)
			{
				fitter.SetLayoutHorizontal();
				fitter.SetLayoutVertical();
			}

			isUpdating = false;
		}

		/// <summary>
		/// Sets the filler.
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="last">Last.</param>
		public void SetFiller(float first, float last)
		{
			if (FirstFiller != null)
			{
				if (first == 0f)
				{
					FirstFiller.gameObject.SetActive(false);
				}
				else
				{
					FirstFiller.gameObject.SetActive(true);
					FirstFiller.transform.SetAsFirstSibling();
					if (IsHorizontal)
					{
						FirstFiller.preferredWidth = first - Layout.spacing;
						(FirstFiller.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, first - Layout.spacing);
					}
					else
					{
						FirstFiller.preferredHeight = first - Layout.spacing;
						(FirstFiller.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, first - Layout.spacing);
					}
				}
			}

			if (LastFiller != null)
			{
				if (last == 0f)
				{
					LastFiller.gameObject.SetActive(false);
				}
				else
				{
					LastFiller.gameObject.SetActive(true);

					if (IsHorizontal)
					{
						LastFiller.preferredWidth = last - Layout.spacing;
						(LastFiller.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, last - Layout.spacing);
					}
					else
					{
						LastFiller.preferredHeight = last - Layout.spacing;
						(LastFiller.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, last - Layout.spacing);
					}

					LastFiller.transform.SetAsLastSibling();
				}
			}
		}

		/// <summary>
		/// Gets the size of the item.
		/// </summary>
		/// <returns>The item size.</returns>
		public Vector2 GetItemSize() => new LayoutElementData(DefaultItem).PreferredSize;

		/// <summary>
		/// Gets the top or left margin.
		/// </summary>
		/// <returns>The margin.</returns>
		public float GetMargin() => IsHorizontal ? Layout.padding.left : Layout.padding.top;

		/// <summary>
		/// Reset margin.
		/// </summary>
		public void ResetMargin()
		{
			var padding = Layout.padding;
			if (IsHorizontal)
			{
				padding.left = 0;
				padding.right = 0;
			}
			else
			{
				padding.top = 0;
				padding.bottom = 0;
			}

			Layout.padding = padding;
		}

		/// <summary>
		/// Return MarginX or MarginY depend of direction.
		/// </summary>
		/// <returns>The full margin.</returns>
		public float GetFullMargin() => IsHorizontal ? GetFullMarginX() : GetFullMarginY();

		/// <summary>
		/// Gets sum of left and right margin.
		/// </summary>
		/// <returns>The margin.</returns>
		public float GetFullMarginX() => Layout.padding.left + Layout.padding.right;

		/// <summary>
		/// Gets sum of top and bottom margin.
		/// </summary>
		/// <returns>The full margin.</returns>
		public float GetFullMarginY() => Layout.padding.top + Layout.padding.bottom;

		/// <summary>
		/// Gets the size of the margin.
		/// </summary>
		/// <returns>The margin size.</returns>
		public Vector4 GetMarginSize() => new Vector4(Layout.padding.left, Layout.padding.right, Layout.padding.top, Layout.padding.bottom);

		/// <summary>
		/// Gets the spacing.
		/// </summary>
		/// <returns>The spacing.</returns>
		public float GetSpacing() => Layout.spacing;

		/// <summary>
		/// Gets the horizontal spacing.
		/// </summary>
		/// <returns>The spacing.</returns>
		public float GetSpacingX() => IsHorizontal ? Layout.spacing : 0;

		/// <summary>
		/// Gets the vertical spacing.
		/// </summary>
		/// <returns>The spacing.</returns>
		public float GetSpacingY() => IsHorizontal ? 0 : Layout.spacing;

		/// <summary>
		/// Get columns constraint.
		/// </summary>
		/// <param name="columns">Columns.</param>
		/// <returns>Columns constraint.</returns>
		public int ColumnsConstraint(int columns) => IsHorizontal ? columns : 1;

		/// <summary>
		/// Get rows constraint.
		/// </summary>
		/// <param name="rows">Rows.</param>
		/// <returns>Rows constraint.</returns>
		public int RowsConstraint(int rows) => IsHorizontal ? 1 : rows;

		/// <summary>
		/// Get blocks constraint.
		/// </summary>
		/// <param name="blocks">Blocks.</param>
		/// <returns>Blocks constraint.</returns>
		public int BlocksConstraint(int blocks) => blocks;
	}
}