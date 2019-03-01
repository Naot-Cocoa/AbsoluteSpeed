using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUIEnableController : MonoBehaviour
{
    private GameSceneManager mGameSceneManager;

    [SerializeField]
    private GameObject mSceneUI;

    private void Awake()
    {
        mGameSceneManager = FindObjectOfType<GameSceneManager>();
    }

    private void OnEnable()
    {
        if(mGameSceneManager.SceneState == SceneState.REPLAY)
        {
            mSceneUI.SetActive(false);
        }
        else
        {
            mSceneUI.SetActive(true);
        }
    }
}
