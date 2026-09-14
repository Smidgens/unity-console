// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using global::Unity.Properties;
	using UnityEngine;

	[Serializable]
	public struct ConsoleLogItem
	{
		public static readonly ConsoleLogItem Empty = new ("", default);

		[CreateProperty] internal string Text => text;
		[CreateProperty] internal string Timestamp { get; }
		[CreateProperty] internal Color LogColor => ConsoleConstants.LOG_COLORS[(int)type + 2];

		internal readonly bool Filter(in ConsoleLogFilter filter)
		{
			return (filter.flags & flags) != 0;
		}

		public readonly string text;

		/// <summary>
		/// Error, warning etc
		/// </summary>
		public readonly ELogType type;

		/// <summary>
		/// Creation time
		/// </summary>
		public readonly DateTime timestamp;

		/// <summary>
		/// Category bitmask, can be used for filtering
		/// </summary>
		public readonly long flags;

		public ConsoleLogItem(string t, DateTime date, ELogType type = 0, long flags = 0)
		{
			this.type = type;
			text = t;
			timestamp = date;
			this.flags = flags;
			Timestamp = date.ToLogTime();
		}
	}
}

namespace Smidgenomics.Unity.Console
{
	[System.Serializable]
	internal struct ConsoleLogFilter
	{
		public readonly bool IsSet()
		{
			return flags != ~0;
		}

		public static readonly ConsoleLogFilter Default = new()
		{
			flags = ~0
		};
		public long flags;
	}
}

namespace Smidgenomics.Unity.Console
{
	internal struct CommandRequest
	{
		public string keyword;
		public ECommandType type;
		public object[] args;
		public (string, object)[] optionalArgs;
	}
}

namespace Smidgenomics.Unity.Console
{
	using System.Reflection;

	internal struct ConsoleCallableInfo
	{
		public string keyword;
		public string description;
		public MemberInfo member;
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections;
	using UnityEngine;

	[System.Serializable]
	public struct ConsoleLogBitFlags
	{
		/// <summary>
		/// Max number of custom flags
		/// </summary>
		public const int MAX = 63;

		public static ConsoleLogBitFlags New()
		{
			return new ConsoleLogBitFlags
			{
				_names = Array.Empty<string>()
			};
		}

		public string[] names => _names;
		[SerializeField] internal string[] _names;
	}
}

namespace Smidgenomics.Unity.Console
{
	using System;

	[Serializable]
	internal struct ConsoleSettings
	{
		public static readonly ConsoleSettings Default = new ConsoleSettings
		{
			builtInCommands = EDefaultConsoleCommand.All,
		};

		[ToggleEnum]
		public EDefaultConsoleCommand builtInCommands;

	}
}