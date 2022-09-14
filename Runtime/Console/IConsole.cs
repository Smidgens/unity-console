// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Reflection;

	public interface IConsole
	{
		public IConsoleLog Log { get; }

		public void Exec(string input);

		public CommandHandle Add
		(
			string name,
			MemberInfo p,
			object ctx,
			string description = ""
		);
		public void Remove(in CommandHandle cmd);
	}
}