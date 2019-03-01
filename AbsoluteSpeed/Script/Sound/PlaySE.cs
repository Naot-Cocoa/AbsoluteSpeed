using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySE : SingletonMonoBehaviour<PlaySE> {

    private Dictionary<string, AudioClip> mAudioDictionary;

    private void Awake()
    {
        //リソースフォルダから全SEのファイルを保持
        mAudioDictionary = new Dictionary<string, AudioClip>();

        object[] seList = Resources.LoadAll("SE");
        
        foreach (AudioClip se in seList)
        {
            mAudioDictionary[se.name] = se;
        }
    }

    public void PlayAnimEventSE(AudioSource source, AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void Play(AudioSource source, string clipName)
    {
        if (!mAudioDictionary.ContainsKey(clipName))
        {
            Debug.Log(clipName + "という名前のSEがありません");
            return;
        }

        source.PlayOneShot(mAudioDictionary[clipName] as AudioClip);
    }

    public void PlayOnce(AudioSource source, string clipName)
    {
        if (!mAudioDictionary.ContainsKey(clipName))
        {
            Debug.Log(clipName + "という名前のSEがありません");
            return;
        }

        source.clip = mAudioDictionary[clipName] as AudioClip;
        source.Play();
    }
}

public static class ConstAudio
{
    public const string SE_TITLE_DECISION = "Title_Decision";
    public const string SE_MENU_SELECT = "Menu_Select";
    public const string SE_MENU_DECISION = "Menu_Accel";
    public const string SE_START_COUNT = "CountDown";
    public const string SE_PACENOTE = "PaseNote";
    public const string SE_GOAL = "Goal";
    public const string SE_RESULT = "Result_Start";
    public const string SE_THANK_YOU = "ThankYouWindChaim";
    public const string SE_SWIFT_ANIM = "Result_Motion";
}

