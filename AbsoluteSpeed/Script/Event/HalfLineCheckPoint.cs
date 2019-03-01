using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class HalfLineCheckPoint : MonoBehaviour {

    [SerializeField]
    private GameObject mPlayer;
    [SerializeField]
    private GameObject mGoalLargeParent;
    [SerializeField]
    private GameObject mGoalSmallParent;

    private List<Transform> mGoalLargePointList;
    private List<Transform> mGoalSmallPointList;

    public bool PassCheckPointFlag { get; private set; }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void OnEnable()
    {
        PassCheckPointFlag = false;
        mGoalLargeParent.SetActive(false);
        mGoalSmallParent.SetActive(false);
    }

    /// <summary>
    /// フラグを立てて、ゴール可能にする
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mPlayer)
        {
            PassCheckPointFlag = true;

            mGoalLargeParent.SetActive(true);
            mGoalSmallParent.SetActive(true);
        }
    }
}
