#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the Value of an SpinnerVector3.
	/// </summary>
	public class SpinnerVector3ValueObserver : ComponentDataObserver<UIWidgets.SpinnerVector3, UnityEngine.Vector3>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.SpinnerVector3 target)
		{

			target.OnValueChanged.AddListener(OnValueChangedSpinnerVector3);
		}

		/// <inheritdoc />
		protected override UnityEngine.Vector3 GetValue(UIWidgets.SpinnerVector3 target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.SpinnerVector3 target)
		{

			target.OnValueChanged.RemoveListener(OnValueChangedSpinnerVector3);
		}


		void OnValueChangedSpinnerVector3(UnityEngine.Vector3 arg0)
		{
			OnTargetValueChanged();
		}

	}
}
#endif