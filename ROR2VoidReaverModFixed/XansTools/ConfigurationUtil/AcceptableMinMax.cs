using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VoidReaverMod.XansTools.ConfigurationUtil {

	/// <summary>
	/// Not to be confused with <see cref="AcceptableValueRange{T}"/>, this class validates a user-defined min and max value for a setting, where that setting
	/// is represented as a Vector2 (x is the minimum, y is the maximum). This ensures that the range is actually valid.
	/// </summary>
	[Obsolete("This does not work as intended at this time (is it a bug with BepInEx?)")]
	public class AcceptableUserDefinedMinMax : AcceptableValueBase {

		private readonly Vector2 _minLimits;
		private readonly Vector2 _maxLimits;

		private readonly bool _enforceMinLimit;
		private readonly bool _enforceMaxLimit;

		private readonly string _desc;

		public AcceptableUserDefinedMinMax(Vector2? minLimit = null, Vector2? maxLimit = null) : base(typeof(Vector2)) {
			// This is going to be confusing as shit because it gets meta
			// This class is for when a user setting is a Vector2, where that setting defines some min/max for a value, like bullet accuracy or something.
			// The parameters here are the same, but for the user defined value
			// minLimit is the valid range of numbers for the minimum of the setting (like, "the minimum you type into the setting must be within the range of [x, y]")
			// maxLimit is the valid range of numbers for the maximum of the setting, same idea as above

			// This creates a sort of layered hellhole of these Vector2 limits, and the terminology here in this ctor might get confusing.
			// This block of code here makes sure that the limits of the limits (confusing, right?) are actually possible to achieve.

			string desc = "Declare a [min, max] range. The minimum must be less than the maximum.";
			bool hadMinForGrammar = false;
			if (minLimit != null) {
				Vector2 min = minLimit.Value;
				if (min.x > min.y) throw new ArgumentException("Limits for the minimum value of the user input have been defined, but this range's minimum is larger than its maximum.");

				_enforceMinLimit = true;
				_minLimits = min;
				desc += $" Additionally, the minimum must be within the range of [{min.x}, {min.y}]";
				hadMinForGrammar = true;
			}
			if (maxLimit != null) {
				Vector2 max = maxLimit.Value;
				if (max.x > max.y) throw new ArgumentException("Limits for the maximum value of the user input have been defined, but this range's minimum is larger than its maximum.");

				_enforceMaxLimit = true;
				_maxLimits = max;
				if (hadMinForGrammar) {
					desc += ", and";
				} else {
					desc += " Additionally,";
				}
				desc += $" the maximum must be within the range of [{max.x}, {max.y}]";
			}
			if (minLimit != null && maxLimit != null) {
				if (minLimit.Value.x > maxLimit.Value.y) {
					// This catches a limit like [50, ??] [??, 10]
					// In the above case, the lowest possible value for the user-defined minimum is bigger than the largest possible value for the user defined maximum.
					// This effectively forces what the user types in to always be invalid (like [min=50, max=10]), which is not good.
					throw new ArgumentException("The limit for what the user can enter as a minimum value enforces that the legal values are always larger than what the user can input as a maximum, forcing users to always input an invalid range where the minimum is higher than the maximum.");
				}
			}

			_desc = desc;
		}

		private static bool CheckRange(float value, Vector2 range) {
			return value >= range.x && value <= range.y; 
		}

		private static float ClampInRange(float value, Vector2 range) => Mathf.Clamp(value, range.x, range.y);

		public override object Clamp(object value) {
			if (value is Vector2 vec2) return Clamp(vec2);
			throw new ArgumentException($"Parameter {nameof(value)} was not the correct type. Expecting {typeof(Vector2).FullName}, got {value?.GetType()?.FullName ?? "null"}");
		}

		public Vector2 Clamp(Vector2 value) {
			float newMin = value.x;
			float newMax = value.y;
			if (_enforceMinLimit) {
				newMin = ClampInRange(newMin, _minLimits);
			}
			if (_enforceMaxLimit) {
				newMax = ClampInRange(newMax, _maxLimits);
			}
			if (newMin > newMax) {
				newMax = newMin;
			}
			return new Vector2(newMin, newMax);
		}

		public override bool IsValid(object value) {
			if (value is Vector2 vec2) return IsValid(vec2);
			throw new ArgumentException($"Parameter {nameof(value)} was not the correct type. Expecting {typeof(Vector2).FullName}, got {value?.GetType()?.FullName ?? "null"}");
		}

		public bool IsValid(Vector2 value) {
			float newMin = value.x;
			float newMax = value.y;
			if (_enforceMinLimit) {
				if (!CheckRange(newMin, _minLimits)) return false;
			}
			if (_enforceMaxLimit) {
				if (!CheckRange(newMax, _maxLimits)) return false;
			}
			if (newMin > newMax) return false;
			return true;
		}
		public override string ToDescriptionString() => _desc;
	}
}
