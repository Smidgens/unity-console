// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;

	public interface IConsoleWidget : IOrder
	{
		//public int Order { get; }
		public int Placement { get; }
		public bool Enabled { get; }
		public float GetWidth(float height);
		public void OnWidgetGUI(in Rect pos);
	}
}