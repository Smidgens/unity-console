// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Collections.Generic;
	using System.Text;
	using UnityEngine;
	using UnityEngine.UIElements;

	[UxmlElement("Filters", libraryPath = "Console")]
	[Icon("UIToolkit/Icons/Dropdown.png")]
	internal sealed partial class ConsoleFilters : VisualElement
	{
		private const string _CLS_EMPTY_LABEL = "sm-console__toolbar__filter__label--empty";
		private const string _CLS_FILTER = "sm-console__toolbar__filter";
		
		internal ref readonly ConsoleLogFilter Value => ref _value;

		internal delegate void ChangeEvent(in ConsoleLogFilter filter);

		internal event ChangeEvent onChange;

		public void SetConsole(ConsoleAsset console)
		{
			_console = console;
			SetBitOptions(console._logCategories);
		}

		public ConsoleFilters()
		{
			AddToClassList(_CLS_FILTER);
			AddToClassList("sm-console__toolbar__item");
			_filterIcon = new Image();
			_filterLabel = new Label();
			_filterLabel.AddToClassList("sm-console__toolbar__filter__label");
			
			_filterIcon.image = ConsoleResources.GetInstance()._icons;
			SetFilterIcon(false);
			_filterIcon.AddToClassList("sm-console__toolbar__filter__icon");
			Add(_filterIcon);
			Add(_filterLabel);

			SetFilterLabel(GetFilterLabel());

			RegisterCallback<ClickEvent>(evt =>
			{
				// init menu
				var m = GetDropMenu();
				var mRect = contentRect;
				mRect.position = default;
				m.DropDown(worldBound, this, DropdownMenuSizeMode.Auto);
			});
		}

		private ConsoleAsset _console;
		private readonly Label _filterLabel;
		private readonly Image _filterIcon;
		private ConsoleLogFilter _value =  ConsoleLogFilter.Default;
		private readonly List<(string, long)> _options = new();
		private readonly List<(string, int)> _typeOptions = new()
		{
			( "Log", (int)ELogTypeFlags.Log ),
			( "Warning", (int)ELogTypeFlags.Warning ),
			( "Error", (int)ELogTypeFlags.Error ),
		};

		private void SetFilterIcon(bool active)
		{
			var xCoord = active ? 0.75f : 0.5f;
			_filterIcon.uv = new Rect(xCoord, 1-0.25f, 0.25f, 0.25f);
			_filterIcon.MarkDirtyRepaint();
		}

		private GenericDropdownMenu GetDropMenu()
		{
			var m = new GenericDropdownMenu();

			if (_value.IsSet())
			{
				m.AddItem("Reset", false, ResetFilters);
				m.AddSeparator(string.Empty);
			}
			
			foreach (var (l, v) in _typeOptions)
			{
				m.AddItem(l, ((int)_value.types & v) != 0, OnLogTypeOption, v);
			}
			
			m.AddSeparator(string.Empty);

			if (_value.flags != 0)
			{
				m.AddItem("Any", !_value.matchAll, FilterByAny);
				m.AddItem("All", _value.matchAll, FilterByAll);
				m.AddSeparator(string.Empty);
			}
			
			for (int i = 0; i < _options.Count; i++)
			{
				var (l, v) = _options[i];
				m.AddItem(l, (_value.flags & v) != 0, OnLogCategoryOption, v);
			}
			m.contentContainer.AddToClassList("sm-console__dropdown-menu");
			GetGrandparent(m.contentContainer, 3)?.AddToClassList("sm-console__dropdown-menu__root");
			GetGrandparent(m.contentContainer, 4)?.AddToClassList("sm-console__dropdown-menu__container");
			return m;
		}

		private void FilterByAll()
		{
			FilterByAll(true);
		}

		private void FilterByAny()
		{
			FilterByAll(false);
		}
		

		private void FilterByAll(bool matchAll)
		{
			if (_value.matchAll == matchAll)
			{
				return;
			}
			_value.matchAll = matchAll;
			if (_value.flags != 0)
			{
				EmitFilters();
			}
		}

		private void SetFilterLabel(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				_filterLabel.AddToClassList(_CLS_EMPTY_LABEL);
			}
			else
			{
				_filterLabel.RemoveFromClassList(_CLS_EMPTY_LABEL);
			}
			_filterLabel.text = text;
		}

		internal void ResetFilters()
		{
			_value.Reset();
			EmitFilters();
		}

		private static VisualElement GetGrandparent(VisualElement v, int n)
		{
			for (var i = 0; i < n; i++)
			{
				v = v?.parent;
			}
			return v;
		}

		private string GetFilterLabel()
		{
			if (!Value.IsSet())
			{
				return string.Empty;
			}

			var sb = new StringBuilder();
			var n = 0;
			var sep = _value.matchAll ? '&' : '|';

			if (_value.types != 0)
			{
				sb.Append('[');
			}

			if ((_value.types & (int)ELogTypeFlags.Log) != 0)
			{
				sb.Append('l');
			}
			
			if ((_value.types & (int)ELogTypeFlags.Warning) != 0)
			{
				sb.Append("<color=yellow>w</color>");
			}

			if ((_value.types & (int)ELogTypeFlags.Error) != 0)
			{
				sb.Append("<color=red>e</color>");
			}

			if (_value.types != 0)
			{
				sb.Append("] ");
			}
			
			
			for (int i = 0; i < _options.Count; i++)
			{
				var v = _options[i].Item2;
				if ((Value.flags & v) != 0)
				{
					if (n != 0)
					{
						sb.Append(sep);
					}
					sb.Append(_options[i].Item1);
					n++;
				}
			}
			return sb.ToString();
		}

		private void OnLogTypeOption(object v)
		{
			var flag = (int)v;
			if ((_value.types & flag) != 0)
			{
				_value.types &= (~flag);
			}
			else
			{
				_value.types |= flag;
			}
			EmitFilters();
		}
		
		private void OnLogCategoryOption(object v)
		{
			var flag = (long)v;
			if ((_value.flags & flag) != 0)
			{
				_value.flags &= (~flag);
			}
			else
			{
				_value.flags |= flag;
			}
			EmitFilters();
		}

		private void EmitFilters()
		{
			SetFilterIcon(Value.IsSet());
			SetFilterLabel(GetFilterLabel());
			onChange?.Invoke(Value);
		}

		private void SetBitOptions(in ConsoleLogBitFlags flags)
		{
			_options.Clear();
			int i = -1;
			foreach (var l in flags.names)
			{
				i++;
				var bitValue = 1 << i;
				_options.Add((l, bitValue));
			}
		}



	}
}