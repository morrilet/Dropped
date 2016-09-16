using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager> 
{
	public float globalVolumeModifier;
	public float musicVolumeModifier;
	public float effectsVolumeModifier;

	public AudioClip[] effects;
	public AudioClip[] music;

	public AudioSource musicSource;
	public AudioSource effectSource;

	private float effectStartPitch; //The default pitch of the effect source.

	public override void Awake()
	{
		//isPersistant = true;

		effectStartPitch = effectSource.pitch;

		base.Awake ();
	}

	public void Start()
	{
		globalVolumeModifier = 1f;
		musicVolumeModifier = 1f;
		effectsVolumeModifier = 1f;
	}

	public void PlayMusic(string songName)
	{
		for (int i = 0; i < music.Length; i++) 
		{
			if (music [i].name == songName) 
			{
				musicSource.clip = music [i];
				musicSource.loop = true;
				musicSource.Play ();
			}
		}
	}
		
	public void PlaySoundEffect(string clipName)
	{
		for (int i = 0; i < effects.Length; i++)
		{
			if (effects [i].name == clipName) 
			{
				effectSource.pitch = effectStartPitch;
				effectSource.PlayOneShot (effects [i]);
			}
		}
	}

	public void PlaySoundEffectVariation(string clipName, float minPitch, float maxPitch)
	{
		float pitch = Random.Range (minPitch, maxPitch);
		for (int i = 0; i < effects.Length; i++) 
		{
			if (effects [i].name == clipName) 
			{
				effectSource.pitch = pitch;
				effectSource.PlayOneShot (effects [i]);
			}
		}
	}

	public void SetEffectVolume()
	{
		effectSource.volume = Mathf.Clamp01 (1f * globalVolumeModifier * effectsVolumeModifier);
	}

	public void SetMusicVolume()
	{
		musicSource.volume = Mathf.Clamp01 (1f * globalVolumeModifier * musicVolumeModifier);
	}
}
