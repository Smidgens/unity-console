// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using UnityEngine.Events;

	public abstract class ConsoleWidget : MonoBehaviour, IConsoleWidget
	{
		public bool Enabled => isActiveAndEnabled;
		public int Order => _order;
		public int Placement => (int)_placement;
		public virtual float GetWidth(float height) => height;
		public abstract void OnWidgetGUI(in Rect r);
		[SerializeField] private int _order = 0;
		[SerializeField] private TPlacement _placement = TPlacement.Right;

		private enum TPlacement
		{
			Left = -1,
			//Center = 0,
			Right = 1
		}

		private void OnEnable() { }

	}

	[AddComponentMenu(Config.AddComponentMenu.TOOLBAR_BUTTON)]
	internal class ConsoleButton : ConsoleWidget
	{
		public override float GetWidth(float height)
		{
			return height;
		}

		public override void OnWidgetGUI(in Rect r)
		{
			CGUI.Color(r, _colors.background);
			var ir = r;

			var pressed = Button(r);
			_icon.Draw(ir);
			if (pressed)
			{
				_onClick.Invoke();
			}
		}

		[Expand]
		[SerializeField]
		private Colors _colors = new Colors
		{
			background = Color.clear,
			hover = Color.white * 0.07f
		};

		[SerializeField] private AtlasSprite _icon = AtlasSprite.fill;
		[SerializeField] private UnityEvent _onClick = default;

		[System.Serializable]
		private struct Colors
		{
			public Color background, hover;
		}

		private void HoverArea(in Rect pos)
		{
			if (!pos.Contains(Event.current.mousePosition)) { return; }
			CGUI.Color(pos, _colors.hover);
		}

		private bool Button(in Rect pos, bool enabled = true)
		{
			var te = GUI.enabled;

			GUI.enabled = enabled;
			var pressed = GUI.Button(pos, GUIContent.none, GUIStyle.none);
			GUI.enabled = te;

			if (enabled)
			{
				HoverArea(pos);
			}
			
			return pressed;
		}

	}
}
