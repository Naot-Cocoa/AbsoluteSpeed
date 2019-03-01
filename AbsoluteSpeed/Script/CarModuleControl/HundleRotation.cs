using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HundleRotation : MonoBehaviour {

    [SerializeField]
    private float rotationAngle = 90;
    private float horizontal;
    private GameSceneManager mGameSceneManager;
    private Quaternion rotation;

    private void Awake()
    {
        mGameSceneManager = FindObjectOfType<GameSceneManager>();
        rotation = transform.rotation;
    }

    private void OnEnable()
    {
        transform.rotation = rotation;
    }

    void Update () {
        if(mGameSceneManager.SceneState == SceneState.REPLAY) return;
        if (Input.GetAxis("Horizontal") != 0)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, horizontal * -90);
        }
	}
}
