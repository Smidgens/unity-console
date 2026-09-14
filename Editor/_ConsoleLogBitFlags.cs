// smidgens @ github

#pragma warning disable 0414

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Console.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ConsoleLogBitFlags))]
	internal sealed class _ConsoleLogBitFlags : PropertyDrawer
	{
		private GUIStyle _LabelStyle => EditorStyles.boldLabel;
		private float _ItemHeight => EditorGUIUtility.singleLineHeight;
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var boxStyle = EditorStyles.helpBox;
			var boxPad = boxStyle.padding.bottom + boxStyle.padding.top;
			var labelHeight = _LabelStyle.CalcHeight(GUIContent.none, 100);
			var n = property.FindPropertyRelative("_names").arraySize;
			var listHeight = n * _ItemHeight + (Mathf.Max(n, 0) * EditorGUIUtility.standardVerticalSpacing);
			if (n > 0)
			{
				listHeight += EditorGUIUtility.standardVerticalSpacing;
			}

			if (n == 0)
			{
				listHeight += _ItemHeight;
			}
			
			return boxPad + labelHeight + listHeight;
		}

		public readonly GUIStyle headerBackground = "RL Header";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			
			var arr = property.FindPropertyRelative("_names");
			var boxStyle = EditorStyles.helpBox;
			GUI.Box(position, GUIContent.none, boxStyle);

			var headerBGRect = position;
			headerBGRect.height = EditorGUIUtility.singleLineHeight;
			GUI.Box(headerBGRect, GUIContent.none, headerBackground);
			
			position = position.Padded(boxStyle.padding);

			var labelRow = position.SliceTop(_LabelStyle.CalcHeight(GUIContent.none, 100));
			var btnSize = EditorStyles.iconButton.CalcSize(_addIcon);

			var addBtnRect = labelRow.SliceRight(btnSize.x);

			GUI.Label(labelRow, label, _LabelStyle);
			
			var tEnabled = GUI.enabled;
			GUI.enabled &= arr.arraySize < ConsoleLogBitFlags.MAX;
			if (GUI.Button(addBtnRect, _addIcon, EditorStyles.iconButton))
			{
				arr.InsertArrayElementAtIndex(arr.arraySize);
				arr.GetArrayElementAtIndex(arr.arraySize - 1).stringValue = "";
			}
			GUI.enabled = tEnabled;

			if (arr.arraySize > 0)
			{
				position.SliceTop(EditorGUIUtility.standardVerticalSpacing);
			}

			var deleteIndex = -1;

			if (arr.arraySize == 0)
			{
				GUI.Label(position, "No categories defined", EditorStyles.miniLabel);
			}

			position.SliceTop(EditorGUIUtility.standardVerticalSpacing);

			for (int i = 0; i < arr.arraySize; i++)
			{
				var iProp = arr.GetArrayElementAtIndex(i);

				var iRow = position.SliceTop(_ItemHeight);
				var indexRect = iRow.SliceLeft(iRow.height * 1.5f);

				// var bitLabel = (1 << i).ToString();
				var bitLabel = $"2^{i}";
				GUI.Label(indexRect, bitLabel, EditorStyles.miniLabel);

				var rmButtonRect = iRow.SliceRight(btnSize.x);
				iRow.SliceRight(EditorGUIUtility.standardVerticalSpacing);

				if (GUI.Button(rmButtonRect, _removeIcon, EditorStyles.iconButton))
				{
					deleteIndex = i;
				}

				EditorGUI.PropertyField(iRow, iProp, GUIContent.none);

				if (i < arr.arraySize - 1)
				{
					position.SliceTop(EditorGUIUtility.standardVerticalSpacing);
				}
			}

			if (deleteIndex > -1)
			{
				arr.DeleteArrayElementAtIndex(deleteIndex);
			}
			
			EditorGUI.EndProperty();

		}
		
		private readonly GUIContent _addIcon = EditorGUIUtility.IconContent("Toolbar Plus");
		private readonly GUIContent _removeIcon = EditorGUIUtility.IconContent("Toolbar Minus");
		
	}
}

#endif