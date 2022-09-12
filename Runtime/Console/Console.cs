// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using UnityEngine;
	using System;
	using System.Reflection;
	using System.Text;
	using System.Collections.Generic;


#if !CONSOLE_DISABLE_CM
	[CreateAssetMenu(menuName = Config.CreateAssetMenu.CONSOLE)]
#endif
	internal class Console : ScriptableObject, IConsole
	{
		public static class LogMsg
		{
			public const string
			INIT_MSG = "Console initialized",
			NO_RESULTS = "No results",
			VARIABLES = "Variables",
			METHODS = "Methods",
			UNKNOWN_ERR = "Unknown error in handler",
			VARIABLE_NF = "Variable not found:\n  '{0}'",
			METHOD_NF = "Method not found:\n  '{0}'";
		}

		public void AddLog(object o) => AddLog(o.ToString());
		public void AddLog(string msg) => AddLog(msg, 0);
		public void AddLog(string msg, int type) => _log.Append(msg, type);
		public void Clear()
		{
			_log.Clear();
		}

		public void Process(string input)
		{
			HandleInput(input);
		}

		public void RemoveCommand(in CommandHandle key)
		{
			if(!_htypes.TryGetValue(key, out var type)) { return; }
			_htypes.Remove(key);

			switch (type)
			{
				case HandlerType.Method:
					Remove(key, _methods);
					break;
				case HandlerType.Prop:
					Remove(key, _variables);
					break;
			}
		}
		
		public CommandHandle AddCommand
		(
			string name,
			MemberInfo mi,
			object context,
			string description
		)
		{
			switch (mi.MemberType)
			{
				case MemberTypes.Method:
					return AddMethod(name, mi as MethodInfo, context, description);
				case MemberTypes.Property:
					return AddProperty(name, mi as PropertyInfo, context, description);
				case MemberTypes.Field:
					return AddField(name, mi as FieldInfo, context, description);
			}
			return default;
		}

		internal CommandHandle AddField
		(
			string name,
			FieldInfo field,
			object context,
			string description = ""
		)
		{
			if (field.IsInitOnly)
			{
				return default;
			}

			if (!ConsoleReflection.IsConsoleUsable(field.FieldType))
			{
				return default;
			}

			var key = _keys.Next();

			var item = new VariableHandler
			{
				key = key,
				name = name,
				context = context,
				field = field,
				description = description
			};
			_htypes[key] = HandlerType.Prop;
			_variables.Add(item);
			return key;
		}

		internal CommandHandle AddProperty
		(
			string name,
			PropertyInfo property,
			object context,
			string description = ""
		)
		{
			var err = ValidateProperty(property, context);

			if(err != null)
			{
				throw new ConsoleException(err);
			}

			var key = _keys.Next();

			var item = new VariableHandler
			{
				key = key,
				name = name,
				context = context,
				property = property,
				description = description
			};
			_htypes[key] = HandlerType.Prop;
			_variables.Add(item);

			return key;
		}

		internal CommandHandle AddMethod
		(
			string name,
			MethodInfo method,
			object context,
			string description = ""
		)
		{
			var err = ValidateMethod(method, context);

			if (!string.IsNullOrEmpty(err))
			{
				throw new ConsoleException(err);
			}

			var parameters = method.GetParameters();
			var invalid = false;

			foreach (var p in parameters)
			{
				invalid = !ConsoleReflection.IsConsoleUsable(p);
				if (invalid) { break; }
			}

			if (invalid)
			{
				throw new ConsoleException("Method invalid");
			}

			var required = new List<Type>();
			var optional = new Dictionary<string, Type>();

			foreach (var p in parameters)
			{
				if (p.IsOptional)
				{
					optional[p.Name] = p.ParameterType;
				}
				else
				{
					required.Add(p.ParameterType);
				}
			}

			var key = _keys.Next();

			var item = new MethodHandler
			{
				key = key,
				name = name,
				context = context,
				method = method,
				description = description,
				parameters = required.ToArray(),
			};

			_htypes[key] = HandlerType.Method;
			_methods.Add(item);

			return key;
		}

		internal IOutputLog Log => _log;
		internal void Init() => _initFn.Invoke(this);

		internal static class Keyword
		{
			public const string
			LIST = "list",
			HELP = "describe",
			INSPECT = "value_of",
			CLEAR = "clear";
		}

		[Expand]
		[SerializeField]
		private ConsoleSettings _settings = new ConsoleSettings();

		private List<VariableHandler> _variables = new List<VariableHandler>();
		private List<MethodHandler> _methods = new List<MethodHandler>();

		private Dictionary<CommandHandle, HandlerType>
		_htypes = new Dictionary<CommandHandle, HandlerType>();

		private delegate void InitFn(Console c); // lazy init
		private static InitFn _initFn = InitStart;

		private readonly OutputLog _log = new OutputLog();
		private HandleFactory _keys = new HandleFactory();

		internal struct HandleFactory
		{
			public CommandHandle Next() => new CommandHandle(++_current);
			private uint _current;
		}

		private enum HandlerType { Method, Prop, Field }

		private static void InitStart(Console c)
		{
			c.InitDefaultHandlers();
			c.InitAttributeHandlers();
			c.AddLog(LogMsg.INIT_MSG, 1);
			_initFn = NoOp.Action.A1;
		}

		private void InitAttributeHandlers()
		{
			if(_settings.attributes == AttributeSearchScope.None)
			{
				return;
			}
			ConsoleHelper.FindAttributes(this, _settings.attributes);
		}

		private void InitDefaultHandlers()
		{
			var c = this as IConsole;
			c.AddCommand(Keyword.CLEAR, MethodHelper.GetMethod(Clear), this);
			c.AddCommand(Keyword.LIST, MethodHelper.GetMethod(ListHandlers), this);
			Action<string> listFilter = ListHandlers;
			Action<string> help = Describe;
			Action<string> inspect = Inspect;
			c.AddCommand(Keyword.LIST, listFilter.Method, this);
			c.AddCommand(Keyword.HELP, help.Method, this);
			c.AddCommand(Keyword.INSPECT, inspect.Method, this);
		}

		private static string Validate(FieldInfo f, object ctx)
		{
			if (f.IsInitOnly)
			{
				return "Static field must support read/write";
			}

			if (f.IsStatic && ctx != null)
			{
				return "Cannot bind static field with context";
			}
			if (!f.IsStatic && ctx == null)
			{
				return "Cannot bind instance field without context";
			}
			return null;
		}

		private static string ValidateProperty(PropertyInfo p, object ctx)
		{
			if (!ConsoleReflection.IsConsoleUsable(p.PropertyType))
			{
				throw new ConsoleException("Property type not supported");
			}

			if (!p.IsReadWrite())
			{
				throw new ConsoleException("Property should support read/write");
			}

			var setter = p.SetMethod;

			if (setter.IsStatic && ctx != null)
			{
				return "Cannot bind static property with context";
			}
			if (!setter.IsStatic && ctx == null)
			{
				return "Cannot bind instance property without context";
			}

			if (!setter.IsStatic && ctx.GetType() != setter.ReflectedType)
			{
				return "Cannot bind property with context of different type";
			}

			return null;
		}

		private static string ValidateMethod(MethodInfo m, object ctx)
		{
			if (m.ReturnType != typeof(void))
			{
				return $"Cannot bind method with return value: '{m.Name}'";
			}

			if (m.IsStatic && ctx != null)
			{
				return "Cannot bind static method with context";
			}

			if (!m.IsStatic && ctx == null)
			{
				return "Cannot bind instance method without context";
			}

			if (!m.IsStatic && ctx.GetType() != m.ReflectedType)
			{
				return "Cannot bind method with context of different type";
			}
			return null;
		}

		private interface IKeyedHandler
		{
			public string Name { get; }
			public string Description { get; }
			public bool Match(in CommandHandle k);
			public void Stringify(StringBuilder b, uint indent, bool newline);
		}

		private struct MethodHandler : IKeyedHandler
		{
			public string Name => name;
			public string Description => description;
			public CommandHandle key;
			public string name, description;
			public object context;
			public Type[] parameters;
			public MethodInfo method;

			public bool Match(in CommandHandle o) => o == key;

			public void Stringify(StringBuilder s, uint indent = 1, bool newLine = true)
			{
				for (int i = 0; i < indent; i++)
				{
					s.Append("  ");
				}
				s.Append(name);
				s.Append(" (");
				var pi = 0;
				foreach (var p in parameters)
				{
					s.Append(p.GetNameOrAlias());
					if (pi < parameters.Length - 1)
					{
						s.Append(", ");
					}
					pi++;
				}
				s.Append(")");
				if (newLine) { s.Append('\n'); }
			}
		}

		private struct VariableHandler : IKeyedHandler
		{
			public string Name => name;
			public string Description => description;
			public bool Match(in CommandHandle o) => o == key;
			public CommandHandle key;
			public string name, description;
			public object context;
			public PropertyInfo property;
			public FieldInfo field;

			public void Stringify(StringBuilder s, uint indent = 1, bool newLine = true)
			{
				if(property == null && field == null) { return; }

				for(int i = 0; i < indent; i++)
				{
					s.Append("  ");
				}
				s.Append(name);
				s.Append(": ");
				if(property != null)
				{
					s.Append(property.PropertyType.GetNameOrAlias());
				}
				else
				{
					s.Append(field.FieldType.GetNameOrAlias());
				}

				if (newLine) { s.Append('\n'); }
			}
		}

		private void Remove<T>(in CommandHandle k, List<T> l)
		where T : IKeyedHandler
		{
			for (var i = 0; i < l.Count; i++)
			{
				if (!l[i].Match(k)) { continue; }
				l.RemoveAt(i);
				return;
			}
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

		private int IndexOfHandler(in CommandRequest r)
		{
			return r.type switch
			{
				CommandType.MethodCall => IndexOfMethod(r),
				CommandType.Assignment => IndexOfVariable(r),
				_ => -1
			};
		}

		private void LogNF(in CommandRequest r)
		{
			switch (r.type)
			{
				case CommandType.Assignment:
					LogVariableNF(r.keyword);
					break;
				case CommandType.MethodCall:
					LogMethodNF(r.keyword, r.args);
					break;
			}
		}

		private void Invoke(int i, in CommandRequest r)
		{
			switch (r.type)
			{
				case CommandType.MethodCall:
					InvokeMethod(i, r);
					break;
				case CommandType.Assignment:
					InvokeVariable(i, r);
					break;
			}
		}

		private void Run(string rawInput, in CommandRequest r)
		{
			var hi = IndexOfHandler(r);

			if(hi < 0)
			{
				LogNF(r);
				return;
			}

			AddLog(rawInput, 3);
		
			try
			{
				Invoke(hi, r);
			}
			catch(Exception e)
			{
#if SMIDGENOMICS_DEV
				Debug.Log(e.Message);
#endif
				LogUnknownError();
			}
		}


		private int IndexOfVariable(in CommandRequest r)
		{
			return IndexOfVariable(r.keyword, r.args.FirstOrDefault()?.GetType());
		}

		private int IndexOfVariable(string name, Type type)
		{
			if (type == null) { return -1; }
			for (int i = 0; i < _variables.Count; i++)
			{
				var v = _variables[i];

				var vtype =
				v.property?.PropertyType
				?? v.field.FieldType;

				if (vtype != type) { continue; }
				if (_variables[i].name != name) { continue; }
				return i;
			}
			return -1;
		}

		private int IndexOfMethod(in CommandRequest r)
		{
			for (var i = 0; i < _methods.Count; i++)
			{
				if (MatchMethod(_methods[i], r)) { return i; }
			}
			return -1;
		}

		private void InvokeMethod(int i, in CommandRequest r)
		{
			var m = _methods[i];
			m.method.Invoke(m.context, r.args);
		}

		private void InvokeVariable(int i, in CommandRequest r)
		{
			var p = _variables[i];
			if(p.property != null)
			{
				p.property.SetValue(p.context, r.args[0]);
			}
			else
			{
				p.field.SetValue(p.context, r.args[0]);
			}
		}

		private bool MatchMethod(in MethodHandler h, in CommandRequest r)
		{
			if (h.method == null) { return false; }
			if (h.name != r.keyword) { return false; }
			if (h.parameters.Length != r.args.Length) { return false; }

			for (var i = 0; i < r.args.Length; i++)
			{
				var vtype = r.args[i].GetType();
				var ptype = h.parameters[i];
				if (vtype != ptype) { return false; }
			}
			return true;
		}


		private void LogVariableNF(string name)
		{
			var msg = string.Format(LogMsg.VARIABLE_NF, name);
			AddLog(msg, -1);
		}

		private void LogUnknownError()
		{
			AddLog(LogMsg.UNKNOWN_ERR, -2);
		}

		private void LogMethodNF(string name, object[] args)
		{
			var sb = new StringBuilder();
			sb.Append("Not found:\n  ");
			sb.Append(name);
			sb.Append(' ');
			sb.Append('(');
			for (var i = 0; i < args.Length; i++)
			{
				sb.Append(args[0].GetType().GetNameOrAlias());
				if (i < args.Length - 1)
				{
					sb.Append(',');
				}
			}
			sb.Append(')');
			AddLog(sb.ToString(), -1);
		}


		private void Inspect(string name)
		{
			var i = IndexOf(name, _variables);

			if(i < 0)
			{
				LogVariableNF(name);
				return;
			}

			try
			{
				var variable = _variables[i];

				var v = variable.property != null
				? variable.property.GetValue(variable.context)
				: variable.field.GetValue(variable.context);
				AddLog(v.ToString());
			}
			catch { }
		}

		private int IndexOf<T>(string name, List<T> l) where T : IKeyedHandler
		{
			for (var i = 0; i < l.Count; i++)
			{
				if (l[i].Name == name) { return i; }
			}
			return -1;
		}

		private void SortByName<T>(List<T> l) where T : IKeyedHandler
		{
			l.Sort(SortByName);
		}

		private int SortByName<T>(T a, T b) where T : IKeyedHandler
		{
			return a.Name.CompareTo(b.Name);
		}

		private void Describe(string name)
		{
			var i = IndexOf(name, _variables);

			if (i > -1)
			{
				AddLog(_variables[i].description);
				return;
			}

			i = IndexOf(name, _methods);

			if(i > -1)
			{
				AddLog(_methods[i].description);
				return;
			}

			AddLog($"Unknown command: '{name}'", -1);
		}

		private void ListHandlers(string filter)
		{
			var s = new StringBuilder();
			StringifyHandlers(s, h => h.Name.IsMatch(filter));
			_log.Append(s.ToString(), 1);
		}

		private void ListHandlers()
		{
			var s = new StringBuilder();
			StringifyHandlers(s);
			_log.Append(s.ToString(), 1);
		}

		private void StringifyHandlers
		(
			StringBuilder s,
			Func<IKeyedHandler, bool> filter = null
		)
		{
			var vs = new StringBuilder();
			vs.AppendLine(LogMsg.VARIABLES);
			var vcount = 0;
			foreach (var p in _variables)
			{
				if(filter != null && !filter.Invoke(p)) { continue; }
				vcount++;
				p.Stringify(vs);
			}

			if(vcount > 0)
			{
				s.Append(vs);
			}

			var ms = new StringBuilder();
			ms.AppendLine(LogMsg.METHODS);
			var mcount = 0;
			foreach (var m in _methods)
			{
				if (filter != null && !filter.Invoke(m)) { continue; }
				mcount++;
				m.Stringify(ms);
			}
			if (mcount > 0)
			{
				s.Append(ms);
			}

			if(mcount == 0 && vcount == 0)
			{
				s.AppendLine(LogMsg.NO_RESULTS);
			}
		}
	}
}