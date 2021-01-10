using EdgeFramework;
using UnityEngine;
using UnityEngine.UI;


public class DemoSceneScript : MonoBehaviour
{
    [Header("Controller Properties")]
    public Slider MusicSlider;
    public Slider SoundSlider;
	public Toggle MusicToggle;
	public Toggle SoundToggle;
	public Toggle MasterToggle;

	[Header("Music Properties")]
	public InputField LinearField;
	public InputField CrossField;

    [Header("Effects Properties")]
    public Slider RepeatSlider;
	public InputField RepeatAmount;
    public Text RepeatText;
	public InputField CallbackField;
    public Slider CallbackSlider;
	public Text CallbackText;

    // reference to the audiosource for the repeat clip
    AudioSource repeatSource = null;
    // reference to the aaudiosource for the callback clip
    AudioSource callbackSource = null;
    // pool index where sound effects information is stored
	int repeatIndex, callbackIndex;


    void Start()
    {
        // retrieve the first background music and play it
		AudioManager.Instance.PlayBGM(GetNextMusicClip(), MusicTransition.Swift);

        // set the default values for the controller properties 
		MusicSlider.value = AudioManager.Instance.MusicVolume;
		SoundSlider.value = AudioManager.Instance.SoundVolume;

		MusicToggle.isOn = AudioManager.Instance.IsMusicOn;
		SoundToggle.isOn = AudioManager.Instance.IsSoundOn;
		MasterToggle.isOn = !AudioManager.Instance.IsMasterMute;

        // set the default calues for the effects properties
        RepeatSlider.value = 0;
        RepeatText.text = "x" + 0;
        CallbackSlider.value = 0;
    }

    void LateUpdate()
    {
        // update the volume of the sliders | this also helps if you change the volume from the controller or mixer
		MusicSlider.value = AudioManager.Instance.MusicVolume;
		SoundSlider.value = AudioManager.Instance.SoundVolume;

		MusicToggle.isOn = AudioManager.Instance.IsMusicOn;
		SoundToggle.isOn = AudioManager.Instance.IsSoundOn;

		// update the sider properties if the repeat sound source exists
        if (repeatSource != null)
        {
            // get index where repeat sound is stored
			repeatIndex = AudioManager.Instance.IndexOfSoundFxPool(AudioManager.Instance.GetClipFromPlaylist("ticktock").name, true);
            // update value if repeat sound exists
            if (repeatIndex >= 0)
            {
                // remainder of time left
				RepeatSlider.value = (AudioManager.Instance.SoundFxPool[repeatIndex].NormalisedTime) * 1;
                // remainder of repeats left
				bool inf = AudioManager.Instance.SoundFxPool[repeatIndex].Time == float.PositiveInfinity;
				RepeatText.text = inf ? "INF" : "x" + Mathf.RoundToInt(AudioManager.Instance.SoundFxPool[repeatIndex].Time / AudioManager.Instance.SoundFxPool[repeatIndex].Length);
            }
        }

        // update the sider properties if the callback sound source exists
        if (callbackSource != null)
        {
            //Debug.Log("Playback Pos: " + callbackSource.time);
            //Debug.Log("Playback Len: " + callbackSource.clip.length);
            //CallbackSlider.value = 1 - (callbackSource.time / callbackSource.clip.length);

			callbackIndex = AudioManager.Instance.IndexOfSoundFxPool(AudioManager.Instance.GetClipFromPlaylist("countdown").name, true);
			// update value if repeat sound exists
			if (callbackIndex >= 0)
			{
				// remainder of time left
				CallbackSlider.value = (AudioManager.Instance.SoundFxPool[callbackIndex].NormalisedTime) * 1;

				CallbackText.text = Mathf.RoundToInt(AudioManager.Instance.SoundFxPool[callbackIndex].Time) + "s";
			}
        }
    }

    public void SetMusicVolume(float value)
    {
		AudioManager.Instance.MusicVolume = value;
    }

    public void SetSoundVolume(float value)
    {
		AudioManager.Instance.SoundVolume = value;
    }

	public void ToggleMusic(bool value)
	{
		AudioManager.Instance.IsMusicOn = value;
	}

	public void ToggleSound(bool value)
	{
		AudioManager.Instance.IsSoundOn = value;
	}

	public void ToggleMaster(bool value)
	{
		AudioManager.Instance.IsMasterMute = value;
		MasterToggle.isOn = !AudioManager.Instance.IsMasterMute;
	}

    // gets the next background clip based on the current one
    AudioClip GetNextMusicClip()
    {
		if (AudioManager.Instance.IsMusicPlaying && AudioManager.Instance.CurrentMusicClip == AudioManager.Instance.LoadClip("MenuMusic"))
        {
			if (AudioManager.Instance.GetClipFromPlaylist("GameMusic") != null) 
			{
				return AudioManager.Instance.GetClipFromPlaylist("GameMusic");
			} 
			else 
			{
				return AudioManager.Instance.LoadClip("GameMusic", true);
			}
        }

		return AudioManager.Instance.LoadClip("MenuMusic");
    } 

    // swift button function
    public void UseSwiftTransition()
    {
		AudioManager.Instance.PlayBGM(GetNextMusicClip(), MusicTransition.Swift);
    }

    // fade button function
    public void UseFadeTransition()
    {
		float duration = Mathf.Clamp(float.Parse (LinearField.text), 0, int.MaxValue);
		LinearField.text = duration.ToString();
		AudioManager.Instance.PlayBGM(GetNextMusicClip(), MusicTransition.LinearFade, duration);
    }

    // crossfade button function
    public void UseCrossfadeTransition()
    {
		float duration = Mathf.Clamp(float.Parse (CrossField.text), 0, int.MaxValue);
		CrossField.text = duration.ToString();
		AudioManager.Instance.PlayBGM(GetNextMusicClip(), MusicTransition.CrossFade, duration);
    }

	public void StopMusic()
	{
		AudioManager.Instance.StopBGM();
	}

    // one shot button function
    public void PlayOneShotSoundEffect()
    {
		AudioManager.Instance.PlayOneShot(AudioManager.Instance.LoadClip("Playlist/oneshot"), CallbackFunction);
    }

    // repeat button function
    public void PlayRepeatSoundEffect()
    {
		int amount = Mathf.Clamp(int.Parse (RepeatAmount.text), int.MinValue, int.MaxValue);
		repeatSource = AudioManager.Instance.RepeatSFX(AudioManager.Instance.GetClipFromPlaylist("ticktock"), amount, true);
    }

    // callback button function
    public void PlayCallbackSoundEffect()
    {
		float duration = Mathf.Clamp(Mathf.Abs(float.Parse(CallbackField.text)), 0, int.MaxValue);
		CallbackField.text = duration.ToString();
		callbackSource = AudioManager.Instance.PlaySFX(AudioManager.Instance.GetClipFromPlaylist("countdown"), duration, true);
    }

    public void CallbackFunction()
    {
		AudioManager.Instance.PlayOneShot(AudioManager.Instance.LoadClip("Playlist/callback"));
    }

	public void StopAllSFX()
	{
		AudioManager.Instance.StopAllSFX();
	}
}
