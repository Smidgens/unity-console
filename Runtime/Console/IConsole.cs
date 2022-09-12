// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Reflection;

	public interface IConsole
	{
		public CommandHandle AddCommand
		(
			string name,
			MemberInfo p,
			object ctx,
			string description = ""
		);
		public void RemoveCommand(in CommandHandle k);
	}
}