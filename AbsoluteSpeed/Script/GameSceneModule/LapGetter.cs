using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
///ラップ地点になるオブジェクトにつけてね！ 
/// </summary>
public class LapGetter : MonoBehaviour {

    UiDinamicPresenter mUiPresenter;
    GameObject mPlayer;
    bool mCheckFlag = true;

    private void Awake()
    {
        mUiPresenter = FindObjectOfType<UiDinamicPresenter>();
        mPlayer = GameObject.Find("Player");//後でstringクラスできたらそれにするよー
    }

    private void Start()
    {
        this.OnCollisionEnterAsObservable()//衝突判定処理
            .Where(collider => collider.gameObject == mPlayer)
            .Where(_ => mCheckFlag == true)
            .Subscribe(_ => { mUiPresenter.LapUpdate(); mCheckFlag = false; });
    }
}
