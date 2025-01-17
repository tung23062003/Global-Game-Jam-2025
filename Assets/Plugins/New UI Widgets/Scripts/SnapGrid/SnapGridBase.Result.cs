﻿namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Base class for the SnapGrid.
	/// </summary>
	public abstract partial class SnapGridBase : UIBehaviourConditional
	{
		/// <summary>
		/// Result.
		/// </summary>
		public readonly struct Result
		{
			/// <summary>
			/// Distance to snap.
			/// </summary>
			public readonly Vector2 Delta;

			/// <summary>
			/// Snapped on X axis.
			/// </summary>
			public readonly bool SnappedX;

			/// <summary>
			/// Snapped on Y axis.
			/// </summary>
			public readonly bool SnappedY;

			/// <summary>
			/// Snapped on both axis.
			/// </summary>
			public bool Snapped => SnappedX && SnappedY;

			/// <summary>
			/// Initializes a new instance of the <see cref="Result"/> struct.
			/// </summary>
			/// <param name="delta">Delta.</param>
			/// <param name="snappedX">Snapped on X axis.</param>
			/// <param name="snappedY">Snapped on Y axis.</param>
			public Result(Vector2 delta, bool snappedX, bool snappedY)
			{
				Delta = delta;
				SnappedX = snappedX;
				SnappedY = snappedY;
			}

			/// <summary>
			/// Converts this instance to the string.
			/// </summary>
			/// <returns>String.</returns>
			public override string ToString()
			{
				return string.Format("SnapGridBase.Result(Delta: {0}; SnappedX: {1}; SnappedY: {2})", Delta.ToString(), SnappedX.ToString(), SnappedY.ToString());
			}
		}
	}
}