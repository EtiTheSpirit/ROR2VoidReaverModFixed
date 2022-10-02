using System;

namespace ROR2VoidReaverModFixed.XanCode.ConfigurationUtil {

	/// <summary>
	/// Some extension methods for <see cref="IComparable"/> that are intended for use when the instance is a primitive numeric type.
	/// </summary>
	public static class ComparableExtensions {

		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="this"/> is less than <paramref name="other"/>.
		/// </summary>
		/// <param name="this"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool IsLessThan(this IComparable @this, IComparable other) {
			int compPos = @this.CompareTo(other);
			return compPos < 0;
		}

		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="this"/> is greater than <paramref name="other"/>.
		/// </summary>
		/// <param name="this"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool IsGreaterThan(this IComparable @this, IComparable other) => !IsLessOrEqual(@this, other);

		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="this"/> is less than or equal to <paramref name="other"/>.
		/// </summary>
		/// <param name="this"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool IsLessOrEqual(this IComparable @this, IComparable other) {
			int compPos = @this.CompareTo(other);
			return compPos <= 0;
		}

		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="this"/> is greater than or equal to <paramref name="other"/>.
		/// </summary>
		/// <param name="this"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool IsGreaterOrEqual(this IComparable @this, IComparable other) => !IsLessThan(@this, other);

	}
}
