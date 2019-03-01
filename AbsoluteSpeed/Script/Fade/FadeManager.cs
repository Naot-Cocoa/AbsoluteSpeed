using System.Collections;
using System.Linq;
using UniRx;
using Unity.Linq;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    private GameSceneManager mSceneStateManager;
    private GameStateManager mGameStateManager;

    private GameObject mFadeScreen;

    private Fade[] mFade = new Fade[5];

    private void Awake()
    {
        mFadeScreen = GameObject.Find("GameFadeScreen");
    }

    /// <summary>
    /// 初期取得処理
    /// </summary>
    public void Init()
    {
        var go = gameObject.Descendants();
        mFade[0] = go.Where(x => x.name == "TitleFadeScreen").Select(x =>x.GetComponent<Fade>()).First();
        mFade[1] = go.Where(x => x.name == "MenuFadeScreen").Select(x => x.GetComponent<Fade>()).First();
        mFade[2] = go.Where(x => x.name == "GameFadeScreen").Select(x => x.GetComponent<Fade>()).First();
        mFade[3] = go.Where(x => x.name == "ThankFadeScreen").Select(x => x.GetComponent<Fade>()).First();
        mFade[4] = go.Where(x => x.name == "ReplayFadeScreen").Select(x => x.GetComponent<Fade>()).First();

        mGameStateManager = GameObject.FindObjectOfType<GameStateManager>();
    }

    /// <summary>
    /// フェードイン処理起動
    /// </summary>
    /// <param name="fadeTime"></param>
    public IEnumerator FadeInStart(float fadeTime)
    {
        mFade[mSceneStateManager.SceneNumProperty].FadeIn(fadeTime);
        //フェードの時間分待機
        yield return Observable.Timer(System.TimeSpan.FromSeconds(fadeTime)).ToYieldInstruction(); //このストリームをコルーチン化して、処理終了後return
    }

    /// <summary>
    /// フェードアウト処理起動
    /// </summary>
    /// <param name="fadeTime"></param>
    public IEnumerator FadeOutStart(float fadeTime)
    {
        mFade[mSceneStateManager.SceneNumProperty].FadeOut(fadeTime);
        //フェードの時間分待機
        yield return Observable.Timer(System.TimeSpan.FromSeconds(fadeTime));
    }

}
