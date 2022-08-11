// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Reflection;
	using System.Text;
	using System.Collections.Generic;
	using UnityObject = UnityEngine.Object;
	using System.Linq;



	[CreateAssetMenu(menuName = Config.CreateAssetMenu.CONSOLE)]
	internal class Console : ScriptableObject
	{
		public IOutputLog Log => _log;

		public struct HandlerOptions
		{
			public int priority;
			public bool invokeUnsafe;
		}

		public class HandlerKey { }

		public void AddLog(string msg) => _log.Append(msg);
		public void AddLog(string msg, int type) => _log.Append(msg, type);

		public void Init()
		{
			_log.Append("Console initialized", 1);
		}

		public void Process(string input)
		{
			var fn = FindSpecialHandler(input);
			if(fn != null)
			{
				fn.Invoke();
				return;
			}
			HandleInput(input);
		}

		public void RemoveHandler(HandlerKey key)
		{
			_handlers.RemoveAll(x => x.key == key);
		}

		public HandlerKey AddHandler
		(
			string keyword,
			UnityObject target,
			MethodInfo method,
			HandlerOptions options = default
		)
		{

			return AddHandler
			(
				target,
				keyword,
				method,
				options.priority,
				options.invokeUnsafe
			);
		}

		//public Color BackgroundColor => _backgroundColor;

		//[SerializeField]
		//private Color _backgroundColor = Color.black * 0.5f;

		private List<RuntimeHandler> _handlers = new List<RuntimeHandler>();
		private readonly OutputLog _log = new OutputLog();

		private class RuntimeHandler
		{
			public HandlerKey key;
			public string name;
			public UnityObject target;
			public MethodInfo method;
			public Type[] parameters;
			public Dictionary<string, Type> optionalParameters;
			public int priority;
			public bool invokeUnsafe;
		}

		private HandlerKey AddHandler
		(
			UnityObject target,
			string name,
			MethodInfo method,
			int priority,
			bool invokeUnsafe
		)
		{
			if (method.ReturnType != typeof(void))
			{
				Debug.LogError($"Invalid handler: '{method.Name}'");
				return null;
			}

			var parameters = method.GetParameters();
			var required = new List<Type>();
			var optional = new Dictionary<string, Type>();
			var invalid = false;

			foreach (var p in parameters)
			{
				invalid = !ConsoleReflection.IsConsoleUsable(p);

				if (invalid) { break; }

				if (p.IsOptional)
				{
					optional[p.Name] = p.ParameterType;
				}
				else
				{
					required.Add(p.ParameterType);
				}
			}

			if (invalid)
			{
				Debug.LogError($"Invalid handler: '{method.Name}'");
				return null;
			}

			var key = new HandlerKey();

			var item = new RuntimeHandler
			{
				key = key,
				name = name,
				target = target,
				method = method,
				parameters = required.ToArray(),
				optionalParameters = optional,
				priority = priority,
				invokeUnsafe = invokeUnsafe,
			};

			_handlers.Add(item);

			_handlers = _handlers
			.OrderBy(x => x.name)
			.ThenBy(x => x.priority)
			.ToList();

			return key;
		}


		private void HandleInput(string input)
		{
			try
			{
				var r = ConsoleParse.Command(input);
				Run(input, r);
			}
			catch (ConsoleParseException e)
			{
				var err = $"Invalid input: [{input}] \n  " + e.Message;
				_log.Append(err, -2);
			}
		}

		private void Run(in string rawInput, in CommandRequest r)
		{
			var h = FindHandler(r);
			if(h == null)
			{
				LogUnhandled(r);
				return;
			}
			_log.Append(rawInput, 3);

			if (!h.invokeUnsafe)
			{
				try
				{
					Invoke(h, r);
				}
				catch
				{
					_log.Append("Unknown error in handler", -2);
				}
			}
			else
			{
				Invoke(h, r);
			}
		}

		private void Invoke(RuntimeHandler h, in CommandRequest r)
		{
			h.method.Invoke(h.target, r.args);
		}

		private RuntimeHandler FindHandler(in CommandRequest r)
		{
			for(var i = 0; i < _handlers.Count; i++)
			{
				var h = _handlers[i];
				if(h.name != r.keyword) { continue; }
				if (!h.target) { continue; }
				if (Match(_handlers[i], r)) { return _handlers[i]; }
			}
			return null;
		}

		private bool Match(RuntimeHandler h, in CommandRequest r)
		{
			if (!h.target) { return false; }
			if(h.parameters.Length != r.args.Length) { return false; }

			if(h.optionalParameters.Count > 0)
			{
				return false;
			}

			for(var i = 0; i < r.args.Length; i++)
			{
				var vtype = r.args[i].GetType();
				var ptype = h.parameters[i];
				if(vtype != ptype) { return false; }
			}
			return true;
		}


		private Dictionary<string, Action> _SPECIAL_HANDLERS = null;

		private Dictionary<string, Action> GetSpecialHandlers()
		{
			if(_SPECIAL_HANDLERS == null)
			{
				_SPECIAL_HANDLERS = new Dictionary<string, Action>()
				{
					{ Keyword.CLEAR, _log.Clear },
					{ Keyword.LIST, ListHandlers },
					{ "hedy", HL },
					{ "Application.Quit",() =>
					{
						if (Application.isEditor)
						{
							#if UNITY_EDITOR
							UnityEditor.EditorApplication.isPlaying = false;
							return;
							#endif
						}
						Application.Quit();
					} }
				};
			}
			return _SPECIAL_HANDLERS;
		}


		private Action FindSpecialHandler(in string input)
		{
			var shandlers = GetSpecialHandlers();
			return shandlers.TryGetValue(input, out var fn) ? fn : null;
		}

		private void HL() => _log.Append("<b>IT'S HEDLEY!</b>");

		private void ListHandlers()
		{
			var s = new StringBuilder();

			s.Append("Global:\n");

			var shandlers = GetSpecialHandlers();

			foreach(var sh in shandlers)
			{
				s.Append("  ");
				s.Append(sh.Key);
				s.Append(" <- []");
				s.Append("\n");
			}

			s.Append("Scene(s):\n");

			if(_handlers.Count == 0)
			{
				_log.Append("No handlers", 1);
			}

			{
				var hi = 0;
				foreach (var h in _handlers)
				{
					s.Append("  ");
					s.Append(h.name);
					s.Append(" <- [");

					var pi = 0;
					foreach (var p in h.parameters)
					{
						s.Append(GetLabel(p));
						if (pi < h.parameters.Length - 1)
						{
							s.Append(", ");
						}
						pi++;
					}
					s.Append("]");
					if (hi < _handlers.Count - 1)
					{
						s.Append("\n");
					}
					hi++;
				}
			}
			


			_log.Append(s.ToString(), 1);
		}
		private void LogUnhandled(in CommandRequest r)
		{
			var s = new StringBuilder();
			s.Append(r.keyword);

			if(r.args.Length > 0)
			{
				s.Append(": [");
				for (var i = 0; i < r.args.Length; i++)
				{
					s.Append(GetLabel(r.args[i].GetType()));
					if(i < r.args.Length - 1)
					{
						s.Append(", ");
					}
				}
				s.Append("]");
			}
			_log.Append($"No handler found:\n  {s}", -1);
		}
		private static string GetLabel(Type t)
		{
			if(t == typeof(int)) { return "int"; }
			if(t == typeof(string)) { return "string"; }
			if(t == typeof(bool)) { return "bool"; }
			if(t == typeof(float)) { return "float"; }
			return t.Name;
		}

		private static class Keyword
		{
			public const string
			LIST = "list",
			CLEAR = "clear";
		}

	
	}
}