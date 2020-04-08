using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using UnityEngine.SceneManagement;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;

public class InitGoogletexttospeech : MonoBehaviour
{
    #region[Variables]
    private GCTextToSpeech _gcTextToSpeech;
    Text Languagecode;

    private Voice[] _voices;
    private Voice _currentVoice;

    private CultureInfo _provider;

    public Toggle ssmlToggle;

    public List<string> languageCodesList = new List<string>();
    public List<string> VoiceModesList = new List<string>();
    public List<string> VoicesList = new List<string>();
    public AudioSource audioSource;
    public static InitGoogletexttospeech InitGoogletexttospeech_intance;
    #endregion

    #region[Monobehaviour methods]
    private void Awake()
    {
        if (InitGoogletexttospeech_intance == null)
        {
            InitGoogletexttospeech_intance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        _gcTextToSpeech = GCTextToSpeech.Instance;

        _gcTextToSpeech.GetVoicesSuccessEvent += _gcTextToSpeech_GetVoicesSuccessEvent;
        _gcTextToSpeech.SynthesizeSuccessEvent += _gcTextToSpeech_SynthesizeSuccessEvent;

        _gcTextToSpeech.GetVoicesFailedEvent += _gcTextToSpeech_GetVoicesFailedEvent;
        _gcTextToSpeech.SynthesizeFailedEvent += _gcTextToSpeech_SynthesizeFailedEvent;

        _provider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        _provider.NumberFormat.NumberDecimalSeparator = ".";

        languageCodesList.Clear();
        VoiceModesList.Clear();
        VoicesList.Clear();
        for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
        {
            languageCodesList.Add(((Enumerators.LanguageCode)i).ToString());
        }

        VoiceModesList.Clear();
        for (int i = 0; i < Enum.GetNames(typeof(Enumerators.VoiceType)).Length; i++)
        {
            VoiceModesList.Add(((Enumerators.VoiceType)i).ToString());
        }
        GetVoicesButtonOnClickHandler();
    }

    #endregion

    #region[Custom Methods]
    public void SynthesizeButtonOnClickHandler(string _content, int voicetype)
    {
        if (!string.IsNullOrEmpty(_content) || _currentVoice != null && voicetype < VoicesList.Count)
        {
            var voice = _voices.ToList().Find(item => item.name.Contains(VoicesList[voicetype].ToString()));
            _currentVoice = voice;

            print("Speaking Content " + _content);
            _gcTextToSpeech.Synthesize(_content, new VoiceConfig()
            {
                gender = _currentVoice.ssmlGender,
                languageCode = _currentVoice.languageCodes[0],
                name = _currentVoice.name
            },
            ssmlToggle.isOn,
            double.Parse("1.0", _provider),
            double.Parse("1.0", _provider),
            _currentVoice.naturalSampleRateHertz);
        }
    }

    private void GetVoicesButtonOnClickHandler()
    {
        _gcTextToSpeech.GetVoices(new GetVoicesRequest() { });
    }


    private void FillVoicesList()
    {
        if (_voices != null)
        {
            for (int i = 0; i < _voices.Length; i++)
            {
                if (_voices[i].name.ToLower().Contains("AU".ToLower()) && _voices[i].name.ToLower().Contains("WAVENET".ToLower()))
                {
                    VoicesList.Add(_voices[i].name);
                }
            }
            var voice = _voices.ToList().Find(item => item.name.Contains(VoicesList[(int)Speaker_gender.Girl].ToString()));
            _currentVoice = voice;
        }
    }

    #endregion

    #region failed handlers

    private void _gcTextToSpeech_SynthesizeFailedEvent(string error)
    {
        Debug.Log(error);
    }

    private void _gcTextToSpeech_GetVoicesFailedEvent(string error)
    {
        Debug.Log(error);
    }

    #endregion failed handlers

    #region sucess handlers

    private void _gcTextToSpeech_SynthesizeSuccessEvent(PostSynthesizeResponse response)
    {
        audioSource.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
        audioSource.Play();
    }

    private void _gcTextToSpeech_GetVoicesSuccessEvent(GetVoicesResponse response)
    {
        _voices = response.voices;
        FillVoicesList();
    }


    #endregion sucess handlers

    #region[Speaker_gender]
    public enum Speaker_gender
    {
        Girl = 0,
        Boy = 1,
        Man = 2
    }
    #endregion

    #region[Stop_current_audio]
    public void Stop_GoogleTextToSpeech_button()
    {
        audioSource.Stop();
    }
    #endregion

}

