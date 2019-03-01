using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class MessageManager : MonoBehaviour
{

    private GameObject mMessage;

    private void Awake()
    {
        GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>();
        mMessage = transform.Find(ConstString.Name.TITLE_MESSAGE).gameObject;

        this.OnEnableAsObservable()
            .Where(_ => gameSceneManager.SceneState == SceneState.TITLE)
            .Subscribe(_ => mMessage.SetActive(true))
            .AddTo(gameObject);

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => mMessage.SetActive(false))
            .AddTo(gameObject);
    }
}
