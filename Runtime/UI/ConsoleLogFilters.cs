// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Collections.Generic;
	using System.Text;
	using UnityEngine;
	using UnityEngine.UIElements;

	[UxmlElement("LogFilter", libraryPath = "Console")]
	[Icon("UIToolkit/Icons/Dropdown.png")]
	internal sealed partial class ConsoleLogFilters : VisualElement
	{
		internal ref readonly ConsoleLogFilter Value => ref _value;

		internal delegate void ChangeEvent(in ConsoleLogFilter filter);

		internal event ChangeEvent onChange;

		public void SetConsole(ConsoleAsset console)
		{
			_console = console;
			SetBitOptions(console._logCategories);
		}

		public ConsoleLogFilters()
		{
			AddToClassList("sm-console__toolbar__filter");
			_flagDropdown = new DropdownField();
			_filterIcon = new Image();
			_filterLabel = new Label();
			_filterLabel.AddToClassList("sm-console__toolbar__filter__label");
			
			_filterIcon.image = ConsoleResources.GetInstance()._icons;
			SetFilterIcon(false);
			
			_filterIcon.AddToClassList("sm-console__toolbar__filter__icon");
			_flagDropdown.choices = _labels;
			_flagDropdown.RegisterValueChangedCallback(OnValueChanged);
			_flagDropdown.AddToClassList("sm-console__toolbar__filter__dropdown");
			
			_flagDropdown.SetEnabled(false);
			Add(_filterIcon);
			Add(_filterLabel);
			_flagDropdown.index = 0;

			_filterLabel.text = GetFilterLabel();

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
		private readonly DropdownField _flagDropdown;
		private static readonly (long, string) _DEF_VALUE = (~0, "All");
		private ConsoleLogFilter _value =  ConsoleLogFilter.Default;

		private int _flagCount;

		private void SetFilterIcon(bool active)
		{
			var xCoord = active ? 0.75f : 0.5f;
			_filterIcon.uv = new Rect(xCoord, 1-0.25f, 0.25f, 0.25f);
			_filterIcon.MarkDirtyRepaint();
		}

		private GenericDropdownMenu GetDropMenu()
		{
			var m = new GenericDropdownMenu();

			for (int i = 0; i < _labels.Count; i++)
			{
				var v = _values[i];
				var l = _labels[i];
				var active = (_value.flags & v) != 0;

				if (i == 0 && _value.flags != _DEF_VALUE.Item1)
				{
					active = false;
				}
				
				m.AddItem(l, active, OnDropdownMenuItem, v);
				if (v == _DEF_VALUE.Item1)
				{
					m.AddSeparator("");
				}
			}
			m.contentContainer.AddToClassList("sm-console__dropdown-menu");
			GetGrandparent(m.contentContainer, 3)?.AddToClassList("sm-console__dropdown-menu__root");
			GetGrandparent(m.contentContainer, 4)?.AddToClassList("sm-console__dropdown-menu__container");
			return m;
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
			if (Value.flags == _DEF_VALUE.Item1)
			{
				_flagCount = _console ? _console._logCategories._names.Length : 0;
				return string.Empty;
			}

			var sb = new StringBuilder();
			var n = 0;
			for (int i = 1; i < _values.Count; i++)
			{
				var v = _values[i];
				if ((Value.flags & v) != 0)
				{
					if (n != 0)
					{
						sb.Append(',');
					}
					sb.Append(_labels[i]);
					n++;
				}
			}
			_flagCount = n;

			if (n == 0)
			{
				return "(none)";
			}
			return sb.ToString();
		}

		private void OnDropdownMenuItem(object v)
		{
			var flag = (long)v;
			if (flag == _DEF_VALUE.Item1)
			{
				_value.flags = _DEF_VALUE.Item1;
				_filterLabel.text = GetFilterLabel();
				SetFilterIcon(false);
				onChange?.Invoke(Value);
				return;
			}

			if ((_value.flags & flag) != 0)
			{
				_value.flags &= (~flag);
			}
			else
			{
				_value.flags |= flag;
			}
			_filterLabel.text = GetFilterLabel();

			SetFilterIcon(_flagCount != _console._logCategories._names.Length);
			
			onChange?.Invoke(Value);
		}

		private void OnValueChanged(ChangeEvent<string> ev)
		{
			_value.flags = _values[_flagDropdown.index];
			onChange?.Invoke(Value);
		}

		private void SetBitOptions(in ConsoleLogBitFlags flags)
		{
			_values.Clear();
			_labels.Clear();
			_values.Add(_DEF_VALUE.Item1);
			_labels.Add(_DEF_VALUE.Item2);

			int i = -1;
			foreach (var l in flags.names)
			{
				i++;
				var bitValue = 1 << i;
				_labels.Add(l);
				_values.Add(bitValue);
			}
		}

		private readonly List<long> _values = new()
		{
			// _DEF_VALUE.Item1
		};

		private readonly List<string> _labels = new()
		{
			// _DEF_VALUE.Item2
		};



	}
}