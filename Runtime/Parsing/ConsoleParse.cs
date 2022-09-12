// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	internal static class ConsoleParse
	{
		public static CommandRequest Command(string input)
		{
			input = input.Trim();

			var r = new CommandRequest
			{
				args = new object[0],
				optionalArgs = new (string, object)[0],
				type = CommandType.MethodCall,
			};


			var i = 0;
			var cmd = ReadKey(ref i, input);

			ReadSpace(ref i, input);

			r.keyword = cmd;

			if (i >= input.Length) { return r; }

			var args = new List<object>();
			var keyedArgs = new List<(string, object)>();

			if (input[i] == '=')
			{
				r.type = CommandType.Assignment;
				i++;
				var v = ReadValue(ref i, input);
				args.Add(v);
			}
			else if (input[i] == '(')
			{
				if (input[input.Length - 1] != ')')
				{
					throw new ConsoleParseException($"Expecting closing parentheses at {input.Length - 1}");
				}

				if(input[i + 1] != ')')
				{
					i++;
					while (i < input.Length)
					{
						var v = ReadValue(ref i, input);
						args.Add(v);
						ReadSpace(ref i, input);
						if (input[i] == ',')
						{
							ReadSpace(ref i, input);
							if (input[i] == ')')
							{
								throw new ConsoleParseException($"Expecting more args {i}");
							}
						}
						i++;
					}
				}
			}
			else
			{
				while (i < input.Length)
				{
					var checkNum = false;

					if (input[i] == '-' && i + 1 < input.Length)
					{
						checkNum = Token.IsNumericToken(input[i + 1]);
					}

					if (input[i] == '-' && !checkNum)
					{
						i++;
						var a = ReadKeyedArg(ref i, input);
						keyedArgs.Add(a);
					}
					else
					{
						var v = ReadValue(ref i, input);
						args.Add(v);
					}

					ReadSpace(ref i, input);
				}
			}
			r.args = args.ToArray();
			r.optionalArgs = keyedArgs.ToArray();
			return r;
		}

		private static string ReadKey(ref int i, in string input)
		{
			ReadSpace(ref i, input);
			if (i >= input.Length) { return null; }
			if (!Token.IsKeyStart(input[i]))
			{
				throw new ConsoleParseException($"Keyword expected, got token '{input[i]}'");
			}
			var s = i;
			while (i < input.Length && Token.IsKeyToken(input[i])) { i++; }
			if (i == s)
			{
				throw new ConsoleParseException($"Invalid keyword at {s}, expected length > 0");
			}

			if (input[i - 1] == '.')
			{
				throw new ConsoleParseException($"Invalid keyword at {s}, invalid end token '.'");
			}

			return input.Substring(s, i - s);
		}

		private static void ReadSpace(ref int i, in string input)
		{
			while (i < input.Length && Token.IsSpace(input[i])) { i++; }
		}

		private static (string, object) ReadKeyedArg(ref int i, in string input)
		{
			if (i >= input.Length)
			{
				throw new ConsoleParseException($"Expected key at {i}");
			}
			var k = ReadKey(ref i, input);
			var v = ReadValue(ref i, input);
			return (k, v);
		}

		private static object ReadValue(ref int i, in string input)
		{
			ReadSpace(ref i, input);
			if (i >= input.Length)
			{
				throw new ConsoleParseException($"Expecting value at {i}, got EOL");
			}

			if (Token.IsNumericStart(input[i]))
			{
				return ReadNumeric(ref i, input);
			}
			else if (input[i] == '"')
			{
				return ReadClosingToken(ref i, input, '"');
			}
			else if (Token.IsBool(input[i]))
			{
				return ReadBool(ref i, input);
			}
			else if (input[i] == '#')
			{
				return ReadColor(ref i, input);
			}
			else if (input[i] == '<')
			{
				var rawValue = ReadClosingToken(ref i, input, '>');
				if(!TryParseVector(rawValue, out object vec))
				{
					throw new ConsoleParseException($"Invalid vector '{rawValue}'");
				}
				return vec;
			}

			throw new ConsoleParseException($"Unexpected input '{input[i]}' at {i}");
		}

		private static object ReadColor(ref int i, in string input)
		{
			if((i + 1) >= input.Length)
			{
				throw new ConsoleParseException($"Expected color at '{i}', got end of input");
			}
			var s = i;

			i++;
			while(i < input.Length && Token.IsHex(input[i]))
			{
				i++;
			}

			var rawColor = input.Substring(s, i - s);

			if(!ColorUtility.TryParseHtmlString(rawColor, out var c))
			{
				throw new ConsoleParseException($"Invalid color string '{rawColor}'");
			}

			return c;
		}

		private static bool TryParseVector(string rawValue, out object o)
		{
			o = null;

			rawValue = rawValue.Replace(" ", "");
			var rawValues = rawValue.Split(',');

			if(rawValues.Length < 2 || rawValues.Length > 4)
			{
				return false;
			}

			float[] values = new float[rawValues.Length];

			for(var i = 0; i < rawValues.Length; i++)
			{
				var rv = rawValues[i];

				if(rv.Length > 1 && rv[rv.Length - 1] == 'f')
				{
					rv = rv.Substring(0, rv.Length - 1);
				}

				if (!float.TryParse(rv, out float v))
				{
					return false;
				}
				values[i] = v;
			}
			if(values.Length == 2)
			{
				o = new Vector2(values[0], values[1]);
			}
			else if (values.Length == 3)
			{
				o = new Vector3(values[0], values[1], values[2]);
			}
			else if (values.Length == 4)
			{
				o = new Vector4(values[0], values[1], values[2], values[3]);
			}
			return true;
		}

		private static bool ReadBool(ref int i, in string input)
		{
			return input[i++] == 'f' ? false : true;
		}

		private static object ReadNumeric(ref int i, in string input)
		{
			var s = i;
			var isFloat = false;

			while (i < input.Length && !Token.IsArgumentBreak(input[i]))
			{
				if (!Token.IsNumericToken(input[i]))
				{
					throw new ConsoleParseException($"Invalid num token at {i}");
				}
				isFloat |= input[i] == '.' || input[i] == 'f';
				i++;
			}

			var rawNum = input.Substring(s, i - s);

			if (rawNum[rawNum.Length - 1] == 'f')
			{
				rawNum = rawNum.Substring(0, rawNum.Length - 1);
			}

			try
			{
				if (isFloat)
				{
					return Convert.ToSingle(rawNum);
				}
				return Convert.ToInt32(rawNum);
			}
			catch
			{
				throw new ConsoleParseException($"Expected number at {s}");
			}
		}

		private static string ReadClosingToken(ref int i, in string input, in char token)
		{
			var s = i;
			var e = -1;
			i++;
			while (i < input.Length)
			{
				if (input[i] == token)
				{
					e = i - 1;
					break;
				}
				i++;
			}
			if (e < 0)
			{
				throw new ConsoleParseException($"'{token}' expected, got end of input");
			}
			i++;
			return input.Substring(s + 1, e - s);
		}

		private static class Token
		{
			public static bool IsVectorStart(in char c) => c == '<';
			public static bool IsNumericToken(in char c) => c == '.' || IsDigit(c) || c == 'f' || c == '-';
			public static bool IsNumericStart(in char c) => c == '.' || IsDigit(c) || c == '-';
			public static bool IsBool(in char c) => c == 't' || c == 'f';
			public static bool IsSpace(in char c) => c == ' ' || c == '\t';
			public static bool IsArgumentBreak(in char c)
			{
				return IsSpace(c) || c == ')' || c == ',';
			}
			public static bool IsDigit(in char c) => c >= 48 && c <= 57; // 0-9
			public static bool IsKeyToken(in char c)
			{
				return IsDigit(c) || IsKeyStart(c) || c == '.';
			}

			public static bool IsHex(in char c)
			{
				return
				(c >= 'a' && c <= 'f')
				|| IsDigit(c);
			}

			public static bool IsKeyStart(in char c)
			{
				return
				c == '_'
				|| (c >= 97 && c <= 122) // a-z
				|| (c >= 65 && c <= 90); // A-Z
			}
		}

	}
}