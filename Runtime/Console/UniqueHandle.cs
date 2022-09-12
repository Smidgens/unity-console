// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	internal struct KeyGen
	{
		public static CommandHandle Empty => default;
		public CommandHandle Next() => new CommandHandle(++_current);
		private uint _current;
	}

	/// <summary>
	/// Reference to bound console handler
	/// </summary>
	public struct CommandHandle : IEquatable<CommandHandle>
	{
		public bool IsValid => _valid == 1;

		public override bool Equals(object obj)
		{
			if (!(obj is CommandHandle)) { return false; }
			var o = (CommandHandle)obj;
			return o._value == _value;
		}

		public override int GetHashCode() => _value.GetHashCode();
		public bool Equals(CommandHandle other) => _value == other._value;
		public static bool operator ==(CommandHandle l, CommandHandle r) => l.Equals(r);
		public static bool operator !=(CommandHandle l, CommandHandle r) => !(l == r);

		internal CommandHandle(uint v)
		{
			_value = v;
			_valid = 1;
		}

		private uint _value;
		private byte _valid;
	}
}