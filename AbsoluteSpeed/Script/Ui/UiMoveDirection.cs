using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiMoveDirection : MonoBehaviour {

    //表示する進行方向UI
    [SerializeField]
    GameObject mDirectionImage;

    private void OnTriggerEnter(Collider other)
    {
        mDirectionImage.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        mDirectionImage.SetActive(false);
    }
}
