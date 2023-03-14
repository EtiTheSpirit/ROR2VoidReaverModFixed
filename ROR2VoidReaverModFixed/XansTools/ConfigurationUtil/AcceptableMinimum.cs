using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidReaverMod.XansTools.ConfigurationUtil {

	/// <summary>
	/// A non-generic version of <see cref="AcceptableMinimum"/> mostly for reflection.
	/// </summary>
	[Obsolete("This does not work as intended at this time (is it a bug with BepInEx?)")]
	public abstract class AcceptableMinimum : AcceptableValueBase {
		protected AcceptableMinimum(Type valueType) : base(valueType) { }
	}

	/// <summary>
	/// Written by Xan. This is a custom range rule that only enforces a minimum.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Obsolete("This does not work as intended at this time (is it a bug with BepInEx?)")]
	public class AcceptableMinimum<T> : AcceptableMinimum where T : IComparable {

		private readonly T _minimum;

		private readonly T _nonEqualMin;

		private readonly bool _equalityIsValid;

		private readonly string _desc;

		/// <summary>
		/// Constructs a new value that limits what is acceptable with a minimum, but no maximum.
		/// </summary>
		/// <param name="min">The minimum possible value.</param>
		/// <param name="allowEquality">If true, the value can be <c>&#x2265;</c> the minimum. If false, the value must be <c>&gt;</c>.</param>
		/// <param name="minIfNotEqual">Only used if <paramref name="allowEquality"/> is true. This is the replacement minimum to use in <see cref="Clamp"/></param>
		public AcceptableMinimum(T min = default, bool allowEquality = true, T minIfNotEqual = default) : base(typeof(T)) {
			_minimum = min;
			_equalityIsValid = allowEquality;
			if (allowEquality) {
				_desc = $"The value must be greater than or equal to {min}.";
			} else {
				_desc = $"The value must be greater than, but not equal to, {min}.";
			}

			if (!allowEquality) {
				if (minIfNotEqual.IsLessOrEqual(min)) {
					throw new ArgumentException($"Equality is not allowed, but {nameof(minIfNotEqual)} is either less than or equal to the minimum value, which violates this rule!");
				}
			}
			_nonEqualMin = minIfNotEqual;
		}

		public override object Clamp(object value) {
			if (value is T comparable) return Clamp(comparable);
			throw new ArgumentException($"Parameter {nameof(value)} was not the correct type. Expecting {typeof(T).FullName}, got {value?.GetType()?.FullName ?? "null"}");
		}

		public T Clamp(T value) {
			if (!IsValid(value)) return _equalityIsValid ? _minimum : _nonEqualMin;
			return value;
		}

		public override bool IsValid(object value) {
			if (value is T comparable) return IsValid(comparable);
			throw new ArgumentException($"Parameter {nameof(value)} was not the correct type. Expecting {typeof(T).FullName}, got {value?.GetType()?.FullName ?? "null"}");
		}

		public bool IsValid(T value) {
			if (_equalityIsValid) {
				return _minimum.IsLessOrEqual(value);
			} else {
				return _minimum.IsLessThan(value);
			}
		}

		public override string ToDescriptionString() => _desc;
	}
}
