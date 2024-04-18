using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using MoreMountains.Tools;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace MoreMountains.Feedbacks
{
	[ExecuteAlways]
	[AddComponentMenu("")]
	[FeedbackPath("Audio/Variable Pitch Sound")]
	[FeedbackHelp("This feedback lets you play the specified AudioClip, with the pitch being controlled by a Float Variable ScriptableObject and an Animation Curve")]
	public class MMF_VariablePitchSound : MMF_Sound
	{
		[MMFInspectorGroup("Sound Properties", true, 28)]
		/// a value read from a scriptableObject float value that will drive animationcurve
		[Tooltip("a value read from a scriptableObject float value that will drive animationcurve")]
		public FloatVariable VariablePitch;
		
		/// an animation curve to indicate what pitch to be driven
		[Tooltip("an animation curve to indicate what pitch to be driven")]
		public AnimationCurve VariablePitchCurve = new AnimationCurve();

		/// the minimum random pitch to add to the base pitch
		[Tooltip("the minimum random pitch to add to the base pitch")]
		public float MinRandomPitch = 1f;
		/// the maximum random pitch to add to the base pitch
		[Tooltip("the maximum random pitch to add to the base pitch")]
		public float MaxRandomPitch = 1f;
		
		/// <summary>
		/// Plays a sound differently based on the selected play method
		/// </summary>
		/// <param name="sfx"></param>
		/// <param name="position"></param>
		protected override void PlaySound(AudioClip sfx, Vector3 position, float intensity)
		{
			float volume = Random.Range(MinVolume, MaxVolume);
            
			if (!Timing.ConstantIntensity)
			{
				volume = volume * intensity;
			}
            
			float randomPitch = Random.Range(MinRandomPitch, MaxRandomPitch);

			float pitch = math.remap(VariablePitchCurve.Evaluate(0), VariablePitchCurve.Evaluate(1), MinPitch,
				MaxPitch, VariablePitch.Value);
			
			pitch += randomPitch;

			int timeSamples = NormalPlayDirection ? 0 : sfx.samples - 1;
            
			if (!NormalPlayDirection)
			{
				pitch = -pitch;
			}

			switch (PlayMethod)
			{
				case PlayMethods.Event:
					MMSfxEvent.Trigger(sfx, SfxAudioMixerGroup, volume, pitch, Priority);
					break;
				case PlayMethods.Cached:
					// we set that audio source clip to the one in paramaters
					PlayAudioSource(_cachedAudioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup, Priority);
					break;
				case PlayMethods.OnDemand:
					// we create a temporary game object to host our audio source
					GameObject temporaryAudioHost = new GameObject("TempAudio");
					SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, Owner.gameObject.scene);
					// we set the temp audio's position
					temporaryAudioHost.transform.position = position;
					// we add an audio source to that host
					AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
					PlayAudioSource(audioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup, Priority);
					// we destroy the host after the clip has played
					Owner.ProxyDestroy(temporaryAudioHost, sfx.length);
					break;
				case PlayMethods.Pool:
					_tempAudioSource = GetAudioSourceFromPool();
					if (_tempAudioSource != null)
					{
						PlayAudioSource(_tempAudioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup, Priority);
					}
					break;
			}
		}
		
		/// <summary>
		/// A test method that creates an audiosource, plays it, and destroys itself after play
		/// </summary>
		protected override async void TestPlaySound()
		{
			AudioClip tmpAudioClip = null;

			if (Sfx != null)
			{
				tmpAudioClip = Sfx;
			}

			if (RandomSfx.Length > 0)
			{
				tmpAudioClip = RandomSfx[Random.Range(0, RandomSfx.Length)];
			}

			if (tmpAudioClip == null)
			{
				Debug.LogError(Label + " on " + Owner.gameObject.name + " can't play in editor mode, you haven't set its Sfx.");
				return;
			}

			float volume = Random.Range(MinVolume, MaxVolume);
			
			float randomPitch = Random.Range(MinRandomPitch, MaxRandomPitch);

			float pitch = math.remap(VariablePitchCurve.Evaluate(0), VariablePitchCurve.Evaluate(1), MinPitch,
				MaxPitch, VariablePitch.Value);
			
			pitch += randomPitch;

			GameObject temporaryAudioHost = new GameObject("EditorTestAS_WillAutoDestroy");
			SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, Owner.gameObject.scene);
			temporaryAudioHost.transform.position = Owner.transform.position;
			_editorAudioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
			PlayAudioSource(_editorAudioSource, tmpAudioClip, volume, pitch, 0);
			float length = 1000 * tmpAudioClip.length;
			await Task.Delay((int)length);
			Owner.ProxyDestroyImmediate(temporaryAudioHost);
		}
		
	}
}