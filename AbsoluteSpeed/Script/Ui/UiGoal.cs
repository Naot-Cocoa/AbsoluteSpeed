using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiGoal : MonoBehaviour {

    [SerializeField, Tooltip("")]
    private UiResultRank mUiResultRank;
    [SerializeField, Tooltip("")]
    private UiResultTime mUiResultTime;
    [SerializeField, Tooltip("")]
    private GameObject mResultUI;
    
    /// <summary>
    /// ゲームシーンがRESULTになった際にゴールUIを表示させる処理
    /// </summary>
    /// <param name="goalTime">ゴールまでかかった時間</param>
    /// <param name="goalRank">レース内順位</param>
    /// <param name="lapList">各中間地点までかかった時間のラップリスト</param>
    public void Goal(float goalTime,float goalRank)
    {
        mResultUI.SetActive(true);
        mUiResultTime.UiUpdate(goalTime);
        mUiResultRank.UiUpdate(goalRank);
    }


}
