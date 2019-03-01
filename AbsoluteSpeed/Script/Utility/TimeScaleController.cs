using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    [SerializeField]
    private float mTimeScale = 5f;
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.timeScale == mTimeScale)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = mTimeScale;
            }
        }
    }
}
