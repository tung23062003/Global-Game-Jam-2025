﻿namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Multiple connector.
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("UI/New UI Widgets/Connectors/Multiple Connector")]
	public class MultipleConnector : ConnectorBase
	{
		/// <summary>
		/// Connector lines.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("connectorLines")]
		protected List<ConnectorLine> ConnectorLines;

		/// <summary>
		/// The lines.
		/// </summary>
		protected ObservableList<ConnectorLine> lines;

		/// <summary>
		/// Gets or sets the lines.
		/// </summary>
		/// <value>The lines.</value>
		[DataBindField]
		public ObservableList<ConnectorLine> Lines
		{
			get
			{
				if (lines == null)
				{
					lines = (ConnectorLines != null) ? new ObservableList<ConnectorLine>(ConnectorLines) : new ObservableList<ConnectorLine>();
					lines.OnChangeMono.AddListener(LinesChanged);
				}

				return lines;
			}

			set
			{
				lines?.OnChangeMono.RemoveListener(LinesChanged);

				lines = value;

				lines?.OnChangeMono.AddListener(LinesChanged);

				LinesChanged();
			}
		}

		/// <summary>
		/// The listeners.
		/// </summary>
		protected List<TransformListener> listeners = new List<TransformListener>();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (lines == null)
			{
				lines = (ConnectorLines != null) ? new ObservableList<ConnectorLine>(ConnectorLines) : new ObservableList<ConnectorLine>();
				lines.OnChangeMono.AddListener(LinesChanged);
			}

			LinesChanged();
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			LinesChanged();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			if (lines != null)
			{
				lines.OnChangeMono.RemoveListener(LinesChanged);
				lines = null;
			}

			RemoveListeners();
			base.OnDestroy();
		}

		/// <summary>
		/// Called when lines changed.
		/// </summary>
		protected virtual void LinesChanged()
		{
			RemoveListeners();
			AddListeners();
			SetVerticesDirty();
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				if (listeners[i] != null)
				{
					listeners[i].OnTransformChanged.RemoveListener(SetVerticesDirty);
				}
			}

			listeners.Clear();
		}

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			for (int i = 0; i < Lines.Count; i++)
			{
				var target = Lines[i].Target;
				if (target != null)
				{
					var listener = Utilities.RequireComponent<TransformListener>(target);
					listener.OnTransformChanged.AddListener(SetVerticesDirty);

					listeners.Add(listener);
				}
			}
		}

		/// <summary>
		/// Fill the vertex buffer data.
		/// </summary>
		/// <param name="vh">VertexHelper.</param>
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			int index = 0;
			vh.Clear();
			foreach (var line in Lines)
			{
				index += AddLine(rectTransform, line, vh, index);
			}
		}
	}
}