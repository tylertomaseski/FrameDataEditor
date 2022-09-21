using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
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
		FrameDataEditor wnd = CreateWindow<FrameDataEditor>();
		wnd.titleContent = new GUIContent("Frame Data Authoring");

		// Limit size of the window
		wnd.minSize = new Vector2(450, 200);
		//wnd.maxSize = new Vector2(1920, 720);

		wnd.CreateGUI();
	}

	GameObject selection;
	AnimationClip animation;
	float time = 0f;

	void CreateGUI()
	{
		var root = rootVisualElement;
		MonInspectorXML.CloneTree(root);

		ObjectField animatorGOField = root.Q<ObjectField>(name = "Animator_Object");
		animatorGOField.objectType = typeof(GameObject);
		animatorGOField.RegisterCallback<ChangeEvent<Object>>(val =>
		{
			selection = (GameObject)val.newValue;
		});

		Toggle toggleField = root.Q<Toggle>(name = "Animation_Toggle");
		if (toggleField != null)
		{
			toggleField.RegisterCallback<ChangeEvent<bool>>(val =>
			{
				if (val.newValue)
				{
					AnimationMode.StartAnimationMode();
				}
				else
				{
					AnimationMode.StopAnimationMode();
				}
			});
		}

		Slider timeField = root.Q<Slider>(name = "Animation_Time");
		timeField.RegisterCallback<ChangeEvent<float>>(val =>
		{
			time = val.newValue;
		});

		ObjectField animationField = root.Q<ObjectField>(name = "Animation_Object");
		animationField.objectType = typeof(AnimationClip);
		animationField.RegisterCallback<ChangeEvent<Object>>(val =>
		{
			this.animation = (AnimationClip)val.newValue;
			if (animation != null)
				timeField.highValue = this.animation.length;
		});
	}

	private void Update()
	{
		if (selection != null && animation != null)
		{
			if (AnimationMode.InAnimationMode() == false && selection != null)
				AnimationMode.StartAnimationMode();
			if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
			{
				AnimationMode.BeginSampling();
				AnimationMode.SampleAnimationClip(selection, animation, time);
				AnimationMode.EndSampling();

				SceneView.RepaintAll();
			}
		}

		//rootVisualElement.Q<VisualElement>(name = "Editor").visible = selection != null;
	}


}
