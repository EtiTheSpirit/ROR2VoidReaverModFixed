using BepInEx.Logging;
using IL.RoR2;
using ROR2VoidReaverModFixed.XanCode;

namespace XanVoidReaverEdit {
	internal static class Log {
		internal static ManualLogSource _logSource;

		internal static void Init(ManualLogSource logSource) {
			_logSource = logSource;
		}

		private static string ToString(object o) {
			return o?.ToString() ?? "null";
		}

		/// <summary>
		/// Does nothing if <see cref="Configuration.TraceLogging"/> is <see langword="false"/>. If it is <see langword="true"/>, it will write to debug level, prefixing the
		/// message with [TRACE]: before writing it.
		/// </summary>
		/// <param name="data"></param>
		internal static void LogTrace(object data) {
			if (!Configuration.TraceLogging) return;
			string result = "[TRACE]: ";
			result += (data?.ToString() ?? "null");
			_logSource.LogDebug(result);
		}
		internal static void LogDebug(object data) => _logSource.LogDebug(ToString(data));
		internal static void LogError(object data) => _logSource.LogError(ToString(data));
		internal static void LogFatal(object data) => _logSource.LogFatal(ToString(data));
		internal static void LogInfo(object data) => _logSource.LogInfo(ToString(data));
		internal static void LogMessage(object data) => _logSource.LogMessage(ToString(data));
		internal static void LogWarning(object data) => _logSource.LogWarning(ToString(data));
	}
}