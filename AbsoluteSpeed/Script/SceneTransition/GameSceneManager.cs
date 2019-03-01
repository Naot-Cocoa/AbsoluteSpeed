using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System.Threading.Tasks;

/// <summary>
/// ゲームシーン管理クラス
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    private GameObject mStage;
    private GameObject[] mScenes;   //遷移させる各シーンObject
    private int mScenesNum = 0;     //シーン管理番号
    public SceneState SceneState { get; private set; } = SceneState.TITLE; //シーンステート

    [SerializeField, Tooltip("フェードの時間")]    //あとで定数化
    private float FADE_TIME = 1f;
    [SerializeField, Tooltip("入力を制限させる時間")]    //あとで定数化
    private float INPUT_INTERVAL = 3f;

    [SerializeField, Tooltip("各シーンのprefab")]
    private GameObject[] mScenesPrefab;
    [SerializeField, Tooltip("フェード管理クラス")]
    private FadeManager mFadeManger;
    [SerializeField]
    private GameObject mStagePrefab;
    public bool mCanInput { get; private set; } = true;      //入力可能フラグ
    //公開用シーン管理番号Property
    public int SceneNumProperty
    {
        get { return mScenesNum; }
        private set { }
    }

    /// <summary>
    /// 起動時初期化処理
    /// </summary>
    private void Awake()
    {
        mStage = Instantiate(mStagePrefab, transform);
        mScenes = new GameObject[mScenesPrefab.Length];

        foreach(var x in mScenesPrefab.Select((x,i) => new {x,i}))
        {
            mScenes[x.i] = Instantiate(x.x, transform);
        }
    }    

    /// <summary>
    /// Titleを起動
    /// </summary>
    private void Start()
    {
        mFadeManger.Init();
        mStage.SetActive(false);
        foreach(var x in mScenes) x.SetActive(false);
        mScenes[0].SetActive(true);
    }

    /// <summary>
    /// SceneStateを正しいモノへと変更する
    /// </summary>
    private void UpdateSceneEnum()
    {
        switch (mScenesNum)
        {
            case 0:
                SceneState = SceneState.TITLE;
                break;
            case 1:
                SceneState = SceneState.MENU;
                break;
            case 2:
                SceneState = SceneState.GAME;
                break;
            case 3:
                SceneState = SceneState.PLAY_END;
                break;
            case 4:
                SceneState = SceneState.REPLAY;
                break;
        }
    }

    /// <summary>
    /// フェードアウト、フェードイン処理と共に次のシーンへ変更する
    /// フェードインとアウト逆やったわ、後で直す
    /// </summary>
    /// <param name="fade"></param>
    /// <returns></returns>
    public async Task NextSceneAsync()
    {
        if (!mCanInput) return;
        mCanInput = false;
        //フェードアウト後、シーン変更
        await mFadeManger.FadeInStart(FADE_TIME);
        NextScene();
        await mFadeManger.FadeOutStart(FADE_TIME);
        Observable.Timer(TimeSpan.FromSeconds(INPUT_INTERVAL))
            .Subscribe(_=> mCanInput = true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task PrevSceneAsync()
    {
        if (!mCanInput) return;
        mCanInput = false;
        //フェードアウト後、シーン変更
        await mFadeManger.FadeInStart(FADE_TIME);
        PrevScene();
        await mFadeManger.FadeOutStart(FADE_TIME);
        Observable.Timer(TimeSpan.FromSeconds(INPUT_INTERVAL))
            .Subscribe(_ => mCanInput = true);
    }

    /// <summary>
    /// タイトルシーンフェードアウト開始
    /// </summary>
    /// <returns></returns>
    public async Task TitleFadeOutStart()
    {
        if (!mCanInput) return;
        mCanInput = false;
        await mFadeManger.FadeOutStart(FADE_TIME);
        mCanInput = true;
    }

    /// <summary>
    /// 次のシーンへと変更
    /// </summary>
    private void NextScene()
    {
        mScenes[mScenesNum].SetActive(false);
        mScenesNum = (mScenesNum + 1) % mScenes.Length;
        UpdateSceneEnum();
        mScenes[mScenesNum].SetActive(true);
        
    }

    private void PrevScene()
    {
        mScenes[mScenesNum].SetActive(false);
        mScenesNum = (mScenesNum + (mScenes.Length - 1)) % mScenes.Length;
        UpdateSceneEnum();
        mScenes[mScenesNum].SetActive(true);
        
    }

    public void AwakeStage()
    {
        mStage.SetActive(true);
    }

    public void SleepStage()
    {
        mStage.SetActive(false);
    }
}
