using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FrameDataEditor))]
public class FrameDataEditor : EditorWindow
{
	public VisualTreeAsset MonInspectorXML;

	// Add menu named "My Window" to the Window menu
	[MenuItem("Tools/Frame Data Authoring")]
	static void Init()
	{
		// This method is called when the user selects the menu item in the Editor
		EditorWindow wnd = GetWindow<FrameDataEditor>();
		wnd.titleContent = new GUIContent("Frame Data Authoring");

		// Limit size of the window
		wnd.minSize = new Vector2(450, 200);
		wnd.maxSize = new Vector2(1920, 720);
	}
}
