// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	public static partial class IConsole_
	{
		public static void LogMessage(this IConsole console, string msg, long category = 0)
		{
			console.Log.Append(msg, ELogType.Normal, category);
		}
		
		public static void LogWarning(this IConsole console, string msg, long category = 0)
		{
			console.Log.Append(msg, ELogType.Warning, category);
		}
		
		public static void LogError(this IConsole console, string msg, long category = 0)
		{
			console.Log.Append(msg, ELogType.Error, category);
		}
		
		public static void LogInfo(this IConsole console, string msg, long category = 0)
		{
			console.Log.Append(msg, ELogType.Info, category);
		}
		
		public static void LogSuccess(this IConsole console, string msg, long category = 0)
		{
			console.Log.Append(msg, ELogType.Success, category);
		}
	}
}