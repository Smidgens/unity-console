// // smidgens @ github
//
// namespace Smidgenomics.Unity.Console
// {
// 	using System;
// 	using UnityEngine;
//
// #if !CONSOLE_DISABLE_CM
// 	[CreateAssetMenu(menuName = ConsoleConstants.CREATE_MENU_ROOT + "Console Theme", order = 10)]
// #endif
// 	[Obsolete("Styling moved to Console UI + USS")]
// 	internal sealed class ConsoleTheme : BaseSO
// 	{
// 		public Color BackgroundColor => _windowColors.background;
// 		public ConsoleGUIStyles Styles => _styles;
// 		public ConsoleWindowColors WindowColors => _windowColors;
//
// 		public Color FindColor(int l)
// 		{
// 			return _logColors.Select(l);
// 		}
//
// 		[SerializeField] private ConsoleGUIStyles _styles;
//
// 		[Header("Colors")]
// 		[Expand(true)]
// 		[SerializeField] private ConsoleWindowColors _windowColors = new()
// 		{
// 			background = Color.black * 0.5f,
// 			border = Color.black * 1f,
// 			scroll = Color.black * 0.5f
// 		};
//
// 		[Expand(true)]
// 		[SerializeField] private ConsoleLogColors _logColors = ConsoleDefaults.LOG_COLORS;
// 	}
// }