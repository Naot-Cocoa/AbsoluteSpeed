using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Threading.Tasks;

public struct RecoveryData
{
    private Vector3 mPosition;
    private Quaternion mRotation;
    public Vector3 Position { get { return mPosition; } set { mPosition = value; } }
    public Quaternion Rotation { get { return mRotation; } set { mRotation = value; } }
    public RecoveryData(Vector3 position, Quaternion rotation) { mPosition = position; mRotation = rotation; }
}

public class RecoveryBoy : MonoBehaviour
{
    private GameSceneManager mGameSceneManager;
    private GameStateManager mGameStateManager;
    private static float RECOVERY_TIME = 3.0f;
    private RePlayer mRePlayer;
    private Rigidbody mRigidbody;
    private float doubleClickInterval = 1000.0f;
    private int mClickCount = 2;
    private FadeManager mFadeManager;
    private void Awake()
    {
        mGameSceneManager = FindObjectOfType<GameSceneManager>();
        mGameStateManager = FindObjectOfType<GameStateManager>();
        mRePlayer = GetComponent<RePlayer>();
        mRigidbody = GetComponent<Rigidbody>();
        mFadeManager = FindObjectOfType<FadeManager>();

        this.UpdateAsObservable()
            .Where(x => Input.GetKeyDown(KeyCode.F5) || Input.GetKeyDown(KeyCode.Escape))
            //.TimeInterval()
            //.Select(t => t.Interval.TotalMilliseconds)
            //.Buffer(mClickCount, 1)
            //.Where(list => list[0] > doubleClickInterval)
            //.Where(list => list[1] <= doubleClickInterval)
            .Subscribe(async x =>
          {
              if (mGameSceneManager.SceneState != SceneState.GAME) { return; }
              if (mGameStateManager.CurrentGameState.Value != InGameState.PLAY) { return; }
              if (gameObject.name == ConstString.Name.PLAYER) { await FadeRecovery(); }
              else { Recovery(); }
          });
    }

    private async Task FadeRecovery()
    {
        await mFadeManager.FadeInStart(0.5f);

        Recovery();

        await mFadeManager.FadeOutStart(0.5f);
    }

    private void Recovery()
    {
        RecoveryData data = mRePlayer.InsertRecovery();
        transform.position = data.Position;
        transform.rotation = data.Rotation;
        mRigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// intervalから戻るフレーム数を算出する
    /// </summary>
    /// <returns>The recovery index.</returns>
    /// <param name="interval">Interval.</param>
    public static int CalcRecoveryIndex(float interval)
    {
        return (int)(RECOVERY_TIME / interval);
    }
}

/*
入力を受け付ける。
計算で求めた分ログを消去。
rigidbodyのvelocityを0にする。
最新のログのpositionとrotationを適用。
*/
