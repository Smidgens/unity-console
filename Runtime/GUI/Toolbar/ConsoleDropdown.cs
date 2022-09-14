// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using UnityEngine.Events;

#if SMIDGENOMICS_DEV
	[AddComponentMenu(Config.AddComponentMenu.TOOLBAR_DROPDOWN)]
#else
	[AddComponentMenu("")]
#endif
	internal class ConsoleDropdown : ConsoleWidget
	{
		public override float GetWidth(float height)
		{
			return height;
		}

		public override void OnWidgetGUI(in Rect r)
		{
			CGUI.Color(r, _colors.background);
			var ir = r;
			_icon.Draw(ir);
		}

		[Expand]
		[SerializeField]
		private Colors _colors = new Colors
		{
			background = Color.clear,
			hover = Color.white * 0.07f
		};

		[System.Serializable]
		private struct DropdownOption
		{
			public string label;
			public AtlasIcon icon;
			public UnityEvent onSelect;
		}

		[SerializeField] private AtlasIcon _icon = AtlasIcon.fill;

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
