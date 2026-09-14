// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;

	/// <summary>
	/// Reference to bound console handler
	/// </summary>
	public readonly struct CommandHandle : IEquatable<CommandHandle>
	{
		public static readonly CommandHandle Empty = default;

		public bool IsValid => _valid == 1;

		public override bool Equals(object obj)
		{
			if (obj is not CommandHandle handle)
			{
				return false;
			}
			return handle._key == _key;
		}

		public override int GetHashCode() => _key.GetHashCode();
		public bool Equals(CommandHandle other) => _key == other._key;
		public static bool operator ==(CommandHandle l, CommandHandle r) => l.Equals(r);
		public static bool operator !=(CommandHandle l, CommandHandle r) => !(l == r);

		internal CommandHandle(uint v)
		{
			_key = v;
			_valid = 1;
		}

		private readonly uint _key;
		private readonly byte _valid;
	}
}