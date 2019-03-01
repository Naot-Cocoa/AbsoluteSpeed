using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayCameraActive : MonoBehaviour {

    private GameSceneManager mGameSceneManager;
    private GameObject mReplayCamera;

    private void Awake()
    {
        mGameSceneManager = GameObject.FindObjectOfType<GameSceneManager>();
        mReplayCamera = GameObject.Find("ReplayCamera");
    }

    private void OnEnable()
    {
        if (mGameSceneManager.SceneState != SceneState.REPLAY)
        {
            mReplayCamera.gameObject.SetActive(false);
        }
        else
        {
            mReplayCamera.gameObject.SetActive(true);
        }
    }

}
