// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using global::Unity.Properties;
	using UnityEngine;
	using UnityEngine.UIElements;

	[UxmlElement("ToolbarButton", libraryPath = "Console")]
	[Icon("UIToolkit/Icons/Button.png")]
	internal sealed partial class ConsoleToolbarButton : Button
	{
		[UxmlAttribute]
		[CreateProperty]
		public Rect UV
		{
			get
			{
				var ico = GetIconImage();
				return ico?.uv ?? default;
			}
			set
			{
				var ico = GetIconImage();
				if (ico != null)
				{
					ico.uv = value;
				}
			}
		}

		public ConsoleToolbarButton()
		{
			AddToClassList("sm-console__toolbar__button");
			AddToClassList("sm-console__toolbar__item");
			text = "";
		}

		private Image _icon;

		private Image GetIconImage()
		{
			if (_icon == null)
			{
				_icon = this.Q<Image>();
			}
			return _icon;
		}
		
	}
}