using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPaseNoteController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mPaseNotes;

    private GameStateManager mGameStateManager;

    private void Awake()
    {
        mGameStateManager = FindObjectOfType<GameStateManager>();
    }

    private void OnEnable()
    {
        if(mGameStateManager.CurrentGameState.Value == InGameState.READY)
        {
            foreach(var x in mPaseNotes)
            {
                x.SetActive(false);
            }
        }
    }
}
