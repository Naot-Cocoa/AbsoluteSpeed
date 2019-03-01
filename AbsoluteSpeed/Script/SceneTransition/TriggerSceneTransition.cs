using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSceneTransition : MonoBehaviour {

    private GameSceneManager mSceneStateManager;

    /// <summary>
    /// 初期取得
    /// </summary>
    private void Awake()
    {
        mSceneStateManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!mSceneStateManager.mCanInput) return;
        if (other.gameObject.tag == "Player")
        {
            mSceneStateManager.NextSceneAsync();
        }
    }
}
