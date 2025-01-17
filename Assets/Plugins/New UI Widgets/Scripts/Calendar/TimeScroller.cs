﻿namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Time ScrollBlock widget for 24 hour format.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/datetime-scroller.html")]
	public class TimeScroller : TimeBase
	{
		/// <summary>
		/// Time comparison precision.
		/// </summary>
		protected enum Precision
		{
			/// <summary>
			/// Days.
			/// </summary>
			Days = 0,

			/// <summary>
			/// Hours.
			/// </summary>
			Hours,

			/// <summary>
			/// Minutes.
			/// </summary>
			Minutes,

			/// <summary>
			/// Seconds.
			/// </summary>
			Seconds,

			/// <summary>
			/// Milliseconds.
			/// </summary>
			Milliseconds,

			/// <summary>
			/// Ticks.
			/// </summary>
			Ticks,
		}

		/// <summary>
		/// If enabled any time period changes will not change other time periods.
		/// </summary>
		[SerializeField]
		[Tooltip("If enabled any time period changes will not change other time periods.")]
		public bool IndependentScroll;

		/// <summary>
		/// If enabled any time period changes will not change other time periods.
		/// </summary>
		[SerializeField]
		[Tooltip("If enabled hours changes will not change AM PM value.")]
		public bool IndependentAMPMScroll = true;

		[SerializeField]
		bool hours = true;

		/// <summary>
		/// Display ScrollBlock for the hours.
		/// </summary>
		public bool Hours
		{
			get => hours;

			set
			{
				hours = value;

				if (HoursScrollBlock != null)
				{
					HoursScrollBlock.gameObject.SetActive(hours);
				}
			}
		}

		/// <summary>
		/// ScrollBlock for the hours.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(hours))]
		[FormerlySerializedAs("HoursScroller")]
		[FormerlySerializedAs("HoursScrollBlock")]
		protected ScrollBlockBase hoursScrollBlock;

		/// <summary>
		/// ScrollBlock for the hours.
		/// </summary>
		public ScrollBlockBase HoursScrollBlock => hoursScrollBlock;

		/// <summary>
		/// Step to change hour.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(hours))]
		public int HoursStep = 1;

		/// <summary>
		/// Format to display hours.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(hours))]
		public string HoursFormat = "HH";

		/// <summary>
		/// Format to display hours.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(hours))]
		public string HoursAMPMFormat = "hh";

		[SerializeField]
		bool minutes = true;

		/// <summary>
		/// Display ScrollBlock for the minutes.
		/// </summary>
		public bool Minutes
		{
			get => minutes;

			set
			{
				minutes = value;

				if (MinutesScrollBlock != null)
				{
					MinutesScrollBlock.gameObject.SetActive(minutes);
				}
			}
		}

		/// <summary>
		/// ScrollBlock for the minutes.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(minutes))]
		[FormerlySerializedAs("MinutesScroller")]
		[FormerlySerializedAs("MinutesScrollBlock")]
		protected ScrollBlockBase minutesScrollBlock;

		/// <summary>
		/// ScrollBlock for the minutes.
		/// </summary>
		public ScrollBlockBase MinutesScrollBlock => minutesScrollBlock;

		/// <summary>
		/// Step to change minutes.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(minutes))]
		public int MinutesStep = 1;

		/// <summary>
		/// Format to display minutes.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(minutes))]
		public string MinutesFormat = "mm";

		[SerializeField]
		bool seconds = true;

		/// <summary>
		/// Display ScrollBlock for the seconds.
		/// </summary>
		public bool Seconds
		{
			get => seconds;

			set
			{
				seconds = value;

				if (SecondsScrollBlock != null)
				{
					SecondsScrollBlock.gameObject.SetActive(seconds);
				}
			}
		}

		/// <summary>
		/// ScrollBlock for the seconds.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(seconds))]
		[FormerlySerializedAs("SecondsScroller")]
		[FormerlySerializedAs("SecondsScrollBlock")]
		protected ScrollBlockBase secondsScrollBlock;

		/// <summary>
		/// ScrollBlock for the seconds.
		/// </summary>
		public ScrollBlockBase SecondsScrollBlock => secondsScrollBlock;

		/// <summary>
		/// Step to change seconds.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(seconds))]
		public int SecondsStep = 1;

		/// <summary>
		/// Format to display seconds.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(seconds))]
		public string SecondsFormat = "ss";

		[SerializeField]
		bool ampm = true;

		/// <summary>
		/// Display ScrollBlock for the AM-PM.
		/// </summary>
		public bool AMPM
		{
			get => ampm;

			set
			{
				ampm = value;

				if (AMPMScrollBlock != null)
				{
					AMPMScrollBlock.gameObject.SetActive(ampm);
				}
			}
		}

		/// <summary>
		/// ScrollBlock for the AM-PM.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(ampm))]
		[FormerlySerializedAs("AMPMScroller")]
		[FormerlySerializedAs("AMPMScrollBlock")]
		protected ScrollBlockBase aMPMScrollBlock;

		/// <summary>
		/// ScrollBlock for the AM-PM.
		/// </summary>
		public ScrollBlockBase AMPMScrollBlock => aMPMScrollBlock;

		/// <summary>
		/// Format to display AM-PM.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(ampm))]
		public string AMPMFormat = "tt";

		/// <summary>
		/// Disable multiple values at AM/PM block.
		/// </summary>
		[SerializeField]
		[Tooltip("Disable multiple values at AM/PM block.")]
		protected bool singleAMPM = true;

		/// <summary>
		/// Disable multiple values at AM/PM block.
		/// </summary>
		public bool SingleAMPM
		{
			get => singleAMPM;

			set
			{
				if (singleAMPM != value)
				{
					SetSingleAMPM(value);
				}
			}
		}

		/// <summary>
		/// Set SingleAMPM.
		/// </summary>
		/// <param name="value">Value.</param>
		protected void SetSingleAMPM(bool value)
		{
			singleAMPM = value;

			if (AMPMScrollBlock != null)
			{
				if (value)
				{
					AMPMScrollBlock.AllowDecrease = AllowAMPMDecrease;
					AMPMScrollBlock.AllowIncrease = AllowAMPMIncrease;
				}
				else
				{
					AMPMScrollBlock.AllowDecrease = AllowAlways;
					AMPMScrollBlock.AllowIncrease = AllowAlways;
				}
			}
		}

		/// <summary>
		/// Allow always.
		/// </summary>
		/// <returns>true.</returns>
		protected bool AllowAlways() => true;

		/// <summary>
		/// Allow AM/PM decrease.
		/// </summary>
		/// <returns>true if decrease allowed; otherwise false.</returns>
		protected bool AllowAMPMDecrease() => Time.Hours >= 12;

		/// <summary>
		/// Allow AM/PM increase.
		/// </summary>
		/// <returns>true if increase allowed; otherwise false.</returns>
		protected bool AllowAMPMIncrease() => Time.Hours < 12;

		/// <summary>
		/// Looped clamp the specified time.
		/// </summary>
		/// <param name="value">Time.</param>
		/// <param name="step">Step.</param>
		/// <returns>Clamped time.</returns>
		protected virtual TimeSpan LoopedClamp(TimeSpan value, TimeSpan step)
		{
			value += step;
			if (value.Ticks < 0)
			{
				value += new TimeSpan(-value.Days, 0, 0, 0);
			}

			if (value < timeMin)
			{
				value = timeMax;
			}

			if (value > timeMax)
			{
				value = timeMin;
			}

			return value;
		}

		/// <summary>
		/// Looped increment/decrement for the time.
		/// </summary>
		/// <param name="value">Start time.</param>
		/// <param name="steps">Steps.</param>
		/// <param name="step">Function increment/decrement for the time.</param>
		/// <returns>Resulted time.</returns>
		protected virtual TimeSpan LoopedTime(TimeSpan value, int steps, TimeSpan step)
		{
			for (int i = 0; i < steps; i++)
			{
				value = LoopedClamp(value, step);
			}

			return value;
		}

		/// <summary>
		/// Increment/decrement current date by hours.
		/// </summary>
		/// <param name="steps">Steps.</param>
		/// <returns>Date.</returns>
		protected virtual TimeSpan LoopedHours(int steps)
		{
			return LoopedHours(steps, HoursStep);
		}

		/// <summary>
		/// Is AM time?
		/// </summary>
		/// <param name="time">Time.</param>
		/// <returns>true if AMPM time.</returns>
		protected bool IsAM(TimeSpan time)
		{
			while (time.Ticks < 0)
			{
				time += new TimeSpan(24, 0, 0);
			}

			return time.Hours < 12;
		}

		/// <summary>
		/// Increment/decrement current date by hours.
		/// </summary>
		/// <param name="steps">Steps.</param>
		/// <param name="hoursStep">Step.</param>
		/// <returns>Date.</returns>
		protected virtual TimeSpan LoopedHours(int steps, int hoursStep)
		{
			var value = Time;
			var step = new TimeSpan(hoursStep, 0, 0);
			var increase = steps > 0;
			if (!increase)
			{
				steps = -steps;
				step = -step;
			}

			if (!IndependentScroll)
			{
				return LoopedTime(value, steps, step);
			}

			var is_am = IsAM(value);
			var ampm_fix = AMPM && IndependentAMPMScroll && (hoursStep == 1);

			for (int i = 0; i < steps; i++)
			{
				value += step;

				// AM/PM value should not be changed on hours scroll
				if (ampm_fix && (is_am != IsAM(value)))
				{
					value += new TimeSpan(increase ? 12 : -12, 0, 0);
				}

				value = CopyTimePeriods(value, Precision.Days);

				var new_hours = value.Hours;
				if (TimeEquals(Time, TimeMin, Precision.Days) && (new_hours < TimeMin.Hours))
				{
					new_hours = increase ? TimeMax.Hours : Mathf.Max(TimeMin.Hours, 24 - hoursStep);
				}

				if (TimeEquals(Time, TimeMax, Precision.Days) && (new_hours > TimeMax.Hours))
				{
					new_hours = increase ? TimeMin.Hours : Mathf.Min(TimeMax.Hours, 24 - hoursStep);
				}

				if (value.Hours != new_hours)
				{
					value += new TimeSpan(new_hours - value.Hours, 0, 0);
				}
			}

			return value;
		}

		/// <summary>
		/// Increment/decrement current date by minutes.
		/// </summary>
		/// <param name="steps">Steps.</param>
		/// <returns>Date.</returns>
		protected virtual TimeSpan LoopedMinutes(int steps)
		{
			var value = Time;
			var step = new TimeSpan(0, MinutesStep, 0);
			var increase = steps > 0;
			if (!increase)
			{
				steps = -steps;
				step = -step;
			}

			if (!IndependentScroll)
			{
				return LoopedTime(value, steps, step);
			}

			for (int i = 0; i < steps; i++)
			{
				value += step;
				value = CopyTimePeriods(value, Precision.Hours);

				var new_minutes = value.Minutes;
				if (TimeEquals(Time, TimeMin, Precision.Hours) && (new_minutes < TimeMin.Minutes))
				{
					new_minutes = increase ? TimeMin.Minutes : Mathf.Max(TimeMin.Minutes, 60 - MinutesStep);
				}

				if (TimeEquals(Time, TimeMax, Precision.Hours) && (new_minutes > TimeMax.Minutes))
				{
					new_minutes = increase ? 0 : Mathf.Min(TimeMax.Minutes, 60 - MinutesStep);
				}

				if (value.Minutes != new_minutes)
				{
					value += new TimeSpan(0, new_minutes - value.Minutes, 0);
				}
			}

			return value;
		}

		/// <summary>
		/// Increment/decrement current date by seconds.
		/// </summary>
		/// <param name="steps">Steps.</param>
		/// <returns>Date.</returns>
		protected virtual TimeSpan LoopedSeconds(int steps)
		{
			var value = Time;
			var step = new TimeSpan(0, 0, SecondsStep);
			var increase = steps > 0;
			if (!increase)
			{
				steps = -steps;
				step = -step;
			}

			if (!IndependentScroll)
			{
				return LoopedTime(value, steps, step);
			}

			for (int i = 0; i < steps; i++)
			{
				value += step;
				value = CopyTimePeriods(value, Precision.Minutes);

				var new_seconds = value.Seconds;
				if (TimeEquals(Time, TimeMin, Precision.Minutes) && (new_seconds < TimeMin.Seconds))
				{
					new_seconds = increase ? TimeMin.Seconds : Mathf.Max(TimeMin.Seconds, 60 - SecondsStep);
				}

				if (TimeEquals(Time, TimeMax, Precision.Minutes) && (new_seconds > TimeMax.Seconds))
				{
					new_seconds = increase ? 0 : Mathf.Min(TimeMax.Seconds, 60 - SecondsStep);
				}

				if (value.Seconds != new_seconds)
				{
					value += new TimeSpan(0, 0, new_seconds - value.Seconds);
				}
			}

			return value;
		}

		/// <summary>
		/// Copy time periods from current time to the specified time with precision.
		/// </summary>
		/// <param name="value">Initial time.</param>
		/// <param name="precision">Copy precision.</param>
		/// <returns>Time.</returns>
		protected TimeSpan CopyTimePeriods(TimeSpan value, Precision precision)
		{
			if (value.Ticks < 0)
			{
				value += new TimeSpan(1, 0, 0, 0);
			}

			if (value.Days > 0)
			{
				value -= new TimeSpan(value.Days, 0, 0, 0);
			}

			return precision switch
			{
				Precision.Days => value,
				Precision.Hours => value + new TimeSpan(Time.Days - value.Days, Time.Hours - value.Hours, 0, 0),
				Precision.Minutes => value + new TimeSpan(Time.Days - value.Days, Time.Hours - value.Hours, Time.Minutes - value.Minutes, 0),
				Precision.Seconds => value + new TimeSpan(Time.Days - value.Days, Time.Hours - value.Hours, Time.Minutes - value.Minutes, Time.Seconds - value.Seconds),
				Precision.Milliseconds => value + new TimeSpan(Time.Days - value.Days, Time.Hours - value.Hours, Time.Minutes - value.Minutes, Time.Seconds - value.Seconds, Time.Milliseconds - value.Milliseconds),
				Precision.Ticks => Time,
				_ => throw new NotSupportedException(string.Format("Unknown precision: {0}", EnumHelper<Precision>.ToString(precision))),
			};
		}

		/// <summary>
		/// Compare time with specified precision.
		/// </summary>
		/// <param name="time1">Time 1.</param>
		/// <param name="time2">Time 2.</param>
		/// <param name="precision">Compare precision.</param>
		/// <returns>true if time equals; otherwise false.</returns>
		protected static bool TimeEquals(TimeSpan time1, TimeSpan time2, Precision precision)
		{
			return precision switch
			{
				Precision.Days => time1.Days == time2.Days,
				Precision.Hours => (time1.Days == time2.Days) && (time1.Hours == time2.Hours),
				Precision.Minutes => (time1.Days == time2.Days) && (time1.Hours == time2.Hours) && (time1.Minutes == time2.Minutes),
				Precision.Seconds => (time1.Days == time2.Days) && (time1.Hours == time2.Hours) && (time1.Minutes == time2.Minutes) && (time1.Seconds == time2.Seconds),
				Precision.Milliseconds => (time1.Days == time2.Days) && (time1.Hours == time2.Hours) && (time1.Minutes == time2.Minutes) && (time1.Seconds == time2.Seconds) && (time1.Milliseconds == time2.Milliseconds),
				Precision.Ticks => time1 == time2,
				_ => throw new NotSupportedException(string.Format("Unknown precision: {0}", EnumHelper<Precision>.ToString(precision))),
			};
		}

		/// <summary>
		/// Get value for the hours at specified step.
		/// </summary>
		/// <param name="steps">Step.</param>
		/// <returns>Formatted hours.</returns>
		protected virtual string HoursValue(int steps)
		{
			var time = LoopedHours(steps);
			var date = new DateTime(2019, 1, 2);
			date += time - date.TimeOfDay;

			return date.ToString(AMPM ? HoursAMPMFormat : HoursFormat, Culture);
		}

		/// <summary>
		/// Get value for the minutes at specified step.
		/// </summary>
		/// <param name="steps">Step.</param>
		/// <returns>Formatted minutes.</returns>
		protected virtual string MinutesValue(int steps)
		{
			var time = LoopedMinutes(steps);
			var date = new DateTime(2019, 1, 2);
			date += time - date.TimeOfDay;

			return date.ToString(MinutesFormat, Culture);
		}

		/// <summary>
		/// Get value for the seconds at specified step.
		/// </summary>
		/// <param name="steps">Step.</param>
		/// <returns>Formatted seconds.</returns>
		protected virtual string SecondsValue(int steps)
		{
			var time = LoopedSeconds(steps);
			var date = new DateTime(2019, 1, 2);
			date += time - date.TimeOfDay;

			return date.ToString(SecondsFormat, Culture);
		}

		/// <summary>
		/// Get value for the AM-PM at specified step.
		/// </summary>
		/// <param name="steps">Step.</param>
		/// <returns>Formatted AM-PM.</returns>
		protected virtual string AMPMValue(int steps)
		{
			var original_steps = steps;

			var date = new DateTime(2019, 1, 2);
			var step = new TimeSpan(12, 0, 0);

			if (steps < 0)
			{
				steps = -steps;
				step = -step;
			}

			date += LoopedTime(Time, steps, step) - date.TimeOfDay;

			if (SingleAMPM)
			{
				if ((original_steps < -1) || (original_steps > 1))
				{
					return string.Empty;
				}

				if (date.Hour < 12)
				{
					return (original_steps <= 0) ? date.ToString(AMPMFormat, Culture) : string.Empty;
				}
				else
				{
					return (original_steps >= 0) ? date.ToString(AMPMFormat, Culture) : string.Empty;
				}
			}
			else
			{
				return date.ToString(AMPMFormat, Culture);
			}
		}

		/// <summary>
		/// Increase the current time on one hour.
		/// </summary>
		protected void IncreaseHours()
		{
			Time = LoopedHours(1);
		}

		/// <summary>
		/// Decrease the current time on one hour.
		/// </summary>
		protected void DecreaseHours()
		{
			Time = LoopedHours(-1);
		}

		/// <summary>
		/// Increase the current time on one minute.
		/// </summary>
		protected void IncreaseMinutes()
		{
			Time = LoopedMinutes(1);
		}

		/// <summary>
		/// Decrease the current time on one minute.
		/// </summary>
		protected void DecreaseMinutes()
		{
			Time = LoopedMinutes(-1);
		}

		/// <summary>
		/// Increase the current time on one second.
		/// </summary>
		protected void IncreaseSeconds()
		{
			Time = LoopedSeconds(1);
		}

		/// <summary>
		/// Decrease the current time on one second.
		/// </summary>
		protected void DecreaseSeconds()
		{
			Time = LoopedSeconds(-1);
		}

		/// <summary>
		/// Increase the current time on 12 hours.
		/// </summary>
		protected void IncreaseAMPM()
		{
			Time = LoopedHours(1, 12);
		}

		/// <summary>
		/// Decrease the current time on 12 hours.
		/// </summary>
		protected void DecreaseAMPM()
		{
			Time = LoopedHours(-1, 12);
		}

		/// <summary>
		/// Init the input.
		/// </summary>
		protected override void InitInput()
		{
			if (HoursScrollBlock != null)
			{
				HoursScrollBlock.gameObject.SetActive(Hours);

				HoursScrollBlock.Value = HoursValue;
				HoursScrollBlock.Decrease = DecreaseHours;
				HoursScrollBlock.Increase = IncreaseHours;
				HoursScrollBlock.IsInteractable = IsActive;

				HoursScrollBlock.Init();
			}

			if (MinutesScrollBlock != null)
			{
				MinutesScrollBlock.gameObject.SetActive(Minutes);

				MinutesScrollBlock.Value = MinutesValue;
				MinutesScrollBlock.Decrease = DecreaseMinutes;
				MinutesScrollBlock.Increase = IncreaseMinutes;
				MinutesScrollBlock.IsInteractable = IsActive;

				MinutesScrollBlock.Init();
			}

			if (SecondsScrollBlock != null)
			{
				SecondsScrollBlock.gameObject.SetActive(Seconds);

				SecondsScrollBlock.Value = SecondsValue;
				SecondsScrollBlock.Decrease = DecreaseSeconds;
				SecondsScrollBlock.Increase = IncreaseSeconds;
				SecondsScrollBlock.IsInteractable = IsActive;

				SecondsScrollBlock.Init();
			}

			if (AMPMScrollBlock != null)
			{
				AMPMScrollBlock.gameObject.SetActive(AMPM);

				AMPMScrollBlock.Value = AMPMValue;
				AMPMScrollBlock.Decrease = DecreaseAMPM;
				AMPMScrollBlock.Increase = IncreaseAMPM;
				AMPMScrollBlock.IsInteractable = IsActive;

				SetSingleAMPM(SingleAMPM);

				AMPMScrollBlock.Init();
			}
		}

		/// <summary>
		/// Scroll to the specified time.
		/// </summary>
		/// <param name="time">Time.</param>
		/// <param name="curve">Animation curve.</param>
		public virtual void Scroll(TimeSpan time, AnimationCurve curve = null)
		{
			curve ??= AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

			if (Hours && (time.Hours != 0) && (HoursScrollBlock != null))
			{
				HoursScrollBlock.Scroll(time.Hours, curve);
			}

			if (Minutes && (time.Minutes != 0) && (MinutesScrollBlock != null))
			{
				MinutesScrollBlock.Scroll(time.Minutes, curve);
			}

			if (Seconds && (time.Seconds != 0) && (SecondsScrollBlock != null))
			{
				SecondsScrollBlock.Scroll(time.Seconds, curve);
			}
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		/// <param name="interactableState">Current interactable state.</param>
		protected override void OnInteractableChange(bool interactableState)
		{
		}

		/// <summary>
		/// Add the listeners.
		/// </summary>
		protected override void AddListeners()
		{
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected override void RemoveListeners()
		{
		}

		/// <summary>
		/// Updates the inputs.
		/// </summary>
		public override void UpdateInputs()
		{
			if (HoursScrollBlock != null)
			{
				HoursScrollBlock.UpdateView();
			}

			if (MinutesScrollBlock != null)
			{
				MinutesScrollBlock.UpdateView();
			}

			if (SecondsScrollBlock != null)
			{
				SecondsScrollBlock.UpdateView();
			}

			if (AMPMScrollBlock != null)
			{
				AMPMScrollBlock.UpdateView();
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			if (HoursScrollBlock != null)
			{
				HoursScrollBlock.SetStyle(style);
			}

			if (MinutesScrollBlock != null)
			{
				MinutesScrollBlock.SetStyle(style);
			}

			if (SecondsScrollBlock != null)
			{
				SecondsScrollBlock.SetStyle(style);
			}

			if (AMPMScrollBlock != null)
			{
				AMPMScrollBlock.SetStyle(style);
			}

			style.ScrollBlock.Highlight.ApplyTo(transform.Find("Highlight"));

			if (TryGetComponent<Image>(out var bg))
			{
				style.ScrollBlock.Background.ApplyTo(bg);
			}

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			if (HoursScrollBlock != null)
			{
				HoursScrollBlock.GetStyle(style);
			}

			if (MinutesScrollBlock != null)
			{
				MinutesScrollBlock.GetStyle(style);
			}

			if (SecondsScrollBlock != null)
			{
				SecondsScrollBlock.GetStyle(style);
			}

			if (AMPMScrollBlock != null)
			{
				AMPMScrollBlock.GetStyle(style);
			}

			style.ScrollBlock.Highlight.GetFrom(transform.Find("Highlight"));

			if (TryGetComponent<Image>(out var bg))
			{
				style.ScrollBlock.Background.GetFrom(bg);
			}

			return true;
		}
		#endregion
	}
}