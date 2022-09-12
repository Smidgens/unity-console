// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	internal struct CommandRequest
	{
		public string keyword;
		public CommandType type;
		public object[] args;
		public (string, object)[] optionalArgs;
	}
}