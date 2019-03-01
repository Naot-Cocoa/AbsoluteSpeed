using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RePlayer : MonoBehaviour
{

    private RePlayCalc mRePlayCalc;
    private GameSceneManager mGameSceneManager;
    private RecoveryData mInitialPosRot;
    [SerializeField]
    private bool mStartRePlay;

    [SerializeField]
    private bool ReRePlay = false;

    [SerializeField]
    private float mInterval = 0.1f;


    private Rigidbody mRigidbody;


    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mGameSceneManager = FindObjectOfType<GameSceneManager>();
        mRePlayCalc = new RePlayCalc(transform, mInterval);
        mInitialPosRot.Position = transform.position;
        mInitialPosRot.Rotation = transform.rotation;
    }

    private void Update()
    {
        mRePlayCalc.UpdateRec();
        if (mStartRePlay)
        {
            mRigidbody.velocity = Vector3.zero;
            mRePlayCalc.UpdateRePlay();
        }

        if (ReRePlay)
        {
            mRigidbody.velocity = Vector3.zero;
            mRePlayCalc.ReRePlay();
            ReRePlay = false;
        }
    }

    //1回だけ呼ぶ
    public void Replay()
    {

        if (mGameSceneManager.SceneState == SceneState.GAME)//再録画処理
        {
            mStartRePlay = false;
            mRePlayCalc.ReRec();
            transform.SetPositionAndRotation(mInitialPosRot.Position, mInitialPosRot.Rotation);//rank trackerとinitializerの座標初期化処理より遅くメソッドが呼ばれるため、ここで初期化する
        }
        else if (mGameSceneManager.SceneState == SceneState.REPLAY) { mStartRePlay = true; }
    }
    public void ReReplay()
    {
        if (mGameSceneManager.SceneState == SceneState.GAME) { ReRePlay = false; }
        else if (mGameSceneManager.SceneState == SceneState.REPLAY) ReRePlay = true;
    }

    public RecoveryData InsertRecovery()
    {
        return mRePlayCalc.InsertRecovery();
    }
}
