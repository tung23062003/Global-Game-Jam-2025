#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the IsInteractable of a ScrollBlockBase depending on the System.Func<System.Boolean> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ScrollBlockBase IsInteractable Setter")]
	public class ScrollBlockBaseIsInteractableSetter : ComponentSingleSetter<UIWidgets.ScrollBlockBase, System.Func<System.Boolean>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ScrollBlockBase target, System.Func<System.Boolean> value)
		{
			target.IsInteractable = value;
		}
	}
}
#endif