using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{

    #region STATIC_MEMBER_VARIBLES
    public static SoundController soundsControllerInstance;
    #endregion

    #region PUBLIC_MEMBER_VARIABLES
    [Header("Sound and Music References")]
    public AudioSource button_audioSource;
    public AudioClip button_SoundAudioClip;
    System.Random rand = new System.Random();



   
    [Header("SoundClip Collection List")]
    public List<AudioClip> All_CorrectSoundClips = new List<AudioClip>();
    public List<AudioClip> All_IncorrectSoundClips = new List<AudioClip>();
    public List<AudioClip> TryAgain_SoundClips = new List<AudioClip>();

    #endregion

    #region MONOBEHAVIOUR_METHODS
    void Awake()
    {
        if (soundsControllerInstance == null)
            soundsControllerInstance = this;

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region[Custom methods]

    public void PlayButtonsound()
    {
        if (button_audioSource != null && button_SoundAudioClip != null)
        {
            button_audioSource.clip = button_SoundAudioClip;
            button_audioSource.Play();
            Debug.Log("Playing Button Sound");
        }
    }
    public void Play_Random_AllCorrectSoundClip()
    {
        print("Sound Played for all correct clips"+All_CorrectSoundClips[rand.Next(0, All_CorrectSoundClips.Count)].name);
        button_audioSource.clip = All_CorrectSoundClips[rand.Next(0, All_CorrectSoundClips.Count)];
        button_audioSource.Play();
    }
    public void Play_Random_AllInCorrectSoundClip()
    {
        button_audioSource.clip = All_IncorrectSoundClips[rand.Next(0, All_IncorrectSoundClips.Count)];
        button_audioSource.Play();

    }
    public void Play_Random_TryAgainSoundClip()
    {
        button_audioSource.clip = TryAgain_SoundClips[rand.Next(0, TryAgain_SoundClips.Count)];
        button_audioSource.Play();
    }
    #endregion

}



