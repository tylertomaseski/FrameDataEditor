using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDX.Collections;
using GDX.Collections.Generic;

public class AnimationEventCollection : ScriptableObject
{
	public struct EventRange
	{
		public float TimeStart;
		public float TimeEnd;
		public byte EventID;
	}
	public struct Event
	{
		public float Time;
		public byte EventID;
	}
	public struct EventCurve
	{
		public AnimationCurve Curve;
		public byte EventID;
	}

	[NonSerialized]
	public short ID; //asigned at runtime
	public Animation Animation;
	public EventRange[] AnimationEvents;
}

public struct AnimationEventFlags
{
	public BitArray32 ActiveFlags;
	public BitArray32 DownFlags;
	public BitArray32 UpFlags;
}

public static class AnimationManager
{
	//animation data
	static AnimationEventCollection[] animationEventData;

	//runtime event processing
	static SparseSet idToEventProcessingLUT;

	//arrays tied to the sparse set
	static int[] animationEventDataID;
	static AnimationEventFlags[] animationEventFlags;
	static float[] animationTimers;

	static AnimationEventFlags[] GetAllFlags()
	{
		return animationEventFlags;
	}

	static AnimationEventFlags GetFlags(int id)
	{
		return animationEventFlags[id];
	}

	static void UpdateAll(float deltaTime)
	{
		int length = idToEventProcessingLUT.Count;

		//loop through all running events
		for (int i = 0; i < length; i++)
		{
			float prevTime = animationTimers[i];
			float newTime = animationTimers[i] + deltaTime;
			animationTimers[i] += newTime;

			AnimationEventCollection data = animationEventData[animationEventDataID[i]];
			int eventLength = data.AnimationEvents.Length;

			//loop through all relevant events
			for (int eventIndex = 0; eventIndex < eventLength; eventIndex++)
			{
				float eventStartTime = data.AnimationEvents[eventIndex].TimeStart;
				float eventEndTime = data.AnimationEvents[eventIndex].TimeEnd;
				byte eventID = data.AnimationEvents[eventIndex].EventID;

				//are we inside event window?
				if (newTime < eventEndTime && newTime >= eventStartTime)
				{
					animationEventFlags[eventIndex].DownFlags[eventID] = animationEventFlags[eventIndex].ActiveFlags[eventID] == false;
					animationEventFlags[eventIndex].UpFlags[eventID] = false;
					animationEventFlags[eventIndex].ActiveFlags[eventID] = true;
				}
				//did we jump over it entirely?
				else if (prevTime < eventStartTime && newTime > eventEndTime) //we jumped over the event
				{
					animationEventFlags[eventIndex].DownFlags[eventID] = true;
					animationEventFlags[eventIndex].UpFlags[eventID] = true;
					animationEventFlags[eventIndex].ActiveFlags[eventID] = true;
				}
				//oop, it's not throwing
				else
				{
					animationEventFlags[eventIndex].DownFlags[eventID] = false;
					animationEventFlags[eventIndex].UpFlags[eventID] = animationEventFlags[eventIndex].ActiveFlags[eventID];
					animationEventFlags[eventIndex].ActiveFlags[eventID] = false;
				}
			}
		}
	}
}



