// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Reflection;

	[Serializable]
	internal sealed class CommandBindingRef
	{
		public bool IsBound => _handle != CommandHandle.Empty && _console != null;

		public void Bind(IConsole c)
		{
			if (IsBound)
			{
				throw new ConsoleException("Command is already bound");
			}
			BindHandler(c);
		}

		public void Unbind()
		{
			if (!IsBound) { return; }
			_console.Unbind(_handle);
			_console = null;
		}

		internal string GetKeyword()
		{
			if (_keywordMode == KeywordMode.Custom)
			{
				return _keyword;
			}

			if (_keywordMode == KeywordMode.NameMethod)
			{
				if (!_handler.Target || _handler.Name.Length == 0)
				{
					return "-";
				}
				var fnName = _handler.Name;
				if (fnName.Length > 4 && fnName[3] == '_')
				{
					fnName = fnName.Substring(4);
				}
				return $"{_handler.Target.name}.{fnName}";
			}
			if (_keywordMode == KeywordMode.NameTypeMethod)
			{
				if (!_handler.Target || _handler.Name.Length == 0)
				{
					return "-";
				}
				var fnName = _handler.Name;
				if (fnName.Length > 4 && fnName[3] == '_')
				{
					fnName = fnName.Substring(4);
				}
				var tt = _handler.Target.GetType().Name;
				return $"{_handler.Target.name}.{tt}.{fnName}";
			}
			return string.Empty;
		}

		[SerializeField] internal KeywordMode _keywordMode;
		[SerializeField] internal string _keyword = string.Empty;
		[TextArea(1, 4)]
		[SerializeField] internal string _description = string.Empty;
		[SerializeField] public SerializedMethod _handler;

		private CommandHandle _handle = CommandHandle.Empty;
		private IConsole _console;

		internal enum KeywordMode
		{
			Custom,
			[InspectorName("target.method")]
			NameMethod,
			[InspectorName("target.type.method")]
			NameTypeMethod,
		}

		private void BindHandler(IConsole c)
		{
			var m = _handler.GetMethod();

			if (m == null)
			{
				throw new ConsoleException("Error binding command handler");
			}

			var keyword = GetKeyword();

			if (keyword.Length == 0)
			{
				throw new ConsoleException("Error binding command handler");
			}

			if (m.IsPropertyMethod() && m.Name[0] == 's')
			{
				_handle = BindAsProperty(c, keyword, m);
			}
			else
			{
				_handle = BindAsMethod(c, keyword, m);
			}
			_console = c;
		}

		private CommandHandle BindAsProperty(IConsole c, string keyword, MethodInfo m)
		{
			var p = m.GetBackingProperty();
			if (p == null)
			{
				throw new ConsoleException("Error binding command handler");
			}
			return c.Bind(keyword, p, _handler.Target, _description);
		}

		private CommandHandle BindAsMethod(IConsole c, string keyword, MethodInfo m)
		{
			return c.Bind(keyword, m, _handler.Target, _description);
		}
	}
}


#if UNITY_EDITOR


namespace Smidgenomics.Unity.Console.Editor
{
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(CommandBindingRef))]
	internal sealed class _CommandBindingRef : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent l)
		{
			EnsureInit();
			var h = 0f;
			// space between fields
			h += Mathf.Max(0f, _fields.Count - 1) * EditorGUIUtility.standardVerticalSpacing;
			foreach (var f in _fields)
			{
				var inner = prop.FindPropertyRelative(f.Name);
				h += EditorGUI.GetPropertyHeight(inner, true);
			}
			return h;
		}

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			EditorGUI.BeginProperty(pos, l, prop);
			{
				int i = -1;
				
				var kwMode = prop.FindPropertyRelative(nameof(CommandBindingRef._keywordMode));
				
				foreach (var f in _fields)
				{
					i++;
					var inner = prop.FindPropertyRelative(f.Name);
					var fRect = pos.SliceTop(EditorGUI.GetPropertyHeight(inner));
					
					if (i < _fields.Count - 1)
					{
						pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);
					}

					if (i == 1 && kwMode.enumValueIndex != 0)
					{
						var kw = ((CommandBindingRef)prop.boxedValue).GetKeyword();
						GUI.Box(fRect, GUIContent.none);
						GUI.Label(fRect, kw, EditorStyles.miniLabel);
						continue;
					}
					EditorGUI.PropertyField(fRect, inner);
					
				}
			}
			EditorGUI.EndProperty();
		}

		private bool _hasInit;
		private IReadOnlyList<FieldInfo> _fields;

		private void EnsureInit()
		{
			if (_hasInit)
			{
				return;
			}
			_hasInit = true;
			_fields = typeof(CommandBindingRef).FindInspectorFields<object>();
		}

	}

}


#endif