using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStartInputInterval : MonoBehaviour {

    private GameSceneManager mSceneStateManager;

    private void Awake()
    {
        mSceneStateManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    private void Start()
    {
        
    }
}
