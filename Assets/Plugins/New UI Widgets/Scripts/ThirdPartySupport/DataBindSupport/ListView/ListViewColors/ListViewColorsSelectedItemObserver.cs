#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItem of an ListViewColors.
	/// </summary>
	public class ListViewColorsSelectedItemObserver : ComponentDataObserver<UIWidgets.ListViewColors, UnityEngine.Color>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewColors target)
		{

			target.OnSelectObject.AddListener(OnSelectObjectListViewColors);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewColors);
		}

		/// <inheritdoc />
		protected override UnityEngine.Color GetValue(UIWidgets.ListViewColors target)
		{
			return target.SelectedItem;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewColors target)
		{

			target.OnSelectObject.RemoveListener(OnSelectObjectListViewColors);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectListViewColors);
		}


		void OnSelectObjectListViewColors(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectListViewColors(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

	}
}
#endif