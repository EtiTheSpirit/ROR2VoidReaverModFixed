#nullable enable
using UnityEngine;

namespace ROR2VoidReaverModFixed.XanCode {

	/// <summary>
	/// Some unity utilities oriented towards its weird handling of <see cref="GameObject"/> and <see langword="null"/>.
	/// </summary>
	public static class Coercion {

		/// <summary>
		/// Implements null coalescense into Unity objects.<para/>
		/// This is required because <see cref="Object"/> declares <see cref="Object.operator =="/> such that it returns <see langword="true"/> for objects scheduled
		/// for destruction in the engine. Consequently, this means that <see cref="object.ReferenceEquals"/> will return <see langword="false"/> when compared to null
		/// while <c>==</c> will return <see langword="true"/>. This wreaks havoc on operators like <c>??</c> because they use reference equality instead of overloads.<br/>
		/// <br/>
		/// This function implements <c>??</c> specifically for <see cref="Object"/> in a shorthand term via an extension method.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		#pragma warning disable IDE0029
		public static T? Or<T>(this T? left, T? right) where T : Object => left == null ? right : left;

	}
}
