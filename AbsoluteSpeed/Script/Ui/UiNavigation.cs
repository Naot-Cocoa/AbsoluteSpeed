using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

//UIペースノートの表示を管理するクラス
public class UiNavigation : MonoBehaviour
{
    private readonly Vector3 NORMAL_ROTATE = new Vector3(0, 0, 0);      // 通常の向き
    private readonly Vector3 REVERSE_ROTATE = new Vector3(0, 180, 0);   // 逆向き

    [SerializeField, Tooltip("再生させるAudioSource")]
    private AudioSource mAudioSource;
    [SerializeField,Tooltip("表示する枠色")]
    private NavigationColor mNavigationColor = new NavigationColor();
    [SerializeField,Tooltip("表示する方向：左右")]
    private NavigationRotation mNavigationRotation = new NavigationRotation();

    private GameObject mNavigation;         //表示させるノートの枠(色)
    private Image mNavigationArrow;         //表示する方向
    private Image mNavigationArrowFrame;    //表示する方向の枠
    [SerializeField, Tooltip("ペースノートに表示させるsprite")]
    private Sprite mArrowSprite;
    [SerializeField, Tooltip("ペースノートに表示させるsprite")]
    private Sprite mArrowFrameSprite;

    private GameSceneManager mGameSceneManager;
    private PaseNoteInstanceSetting mPaseNoteInstanceSetting;
    private GameObject mPlayer;

    private bool mCheckFlag = true; // ペースノートを表示したか

    /// <summary>
    /// 初期取得処理
    /// </summary>
    private void Awake()
    {
        mGameSceneManager = GameObject.FindObjectOfType<GameSceneManager>();
        mPlayer = GameObject.Find(ConstString.Name.PLAYER);//後でstringクラスできたらそれにするよー
        mPaseNoteInstanceSetting = Resources.Load<PaseNoteInstanceSetting>(ConstString.Setting.PASE_NOTE_INSTANCE_SETTING);

        // 通常のFindだと取得できない為、Passで指定してインスタンスを取得
        switch (mNavigationColor)
        {
            case NavigationColor.NORMAL:
                mNavigation = GameObject.Find(ConstString.Path.PASE_NOTE_NORMAL);
                FindChildImage();
                break;
            case NavigationColor.HARD:
                mNavigation = GameObject.Find(ConstString.Path.PASE_NOTE_HARD);
                FindChildImage();
                break;
        }

        Observable.Timer(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => mNavigation.gameObject.SetActive(false));
    }

    /// <summary>
    /// Imageコンポーネント取得
    /// </summary>
    private void FindChildImage()
    {
        //子ObjectのImage取得
        mNavigationArrow = mNavigation.transform.Find(ConstString.Name.PASE_NOTE).GetComponent<Image>();
        mNavigationArrowFrame = mNavigationArrow.transform.Find(ConstString.Name.PASE_NOTE_GRASS).GetComponent<Image>();
    }

    /// <summary>
    /// 矢印の向き変更
    /// </summary>
    private void RotateChange(Vector3 rotate)
    {
        mNavigationArrow.gameObject.transform.localRotation = Quaternion.Euler(rotate);
    }

    /// <summary>
    /// 各ノートに合わせたTransformに変更
    /// </summary>
    private void SpriteTransformChange()
    {
        switch (mNavigationArrow.name)
        {
            case ConstString.PaseNoteUI.ARROW_RIGHT:
                SetPaceNoteData(mPaseNoteInstanceSetting.ArrowCarve);
                break;

            case ConstString.PaseNoteUI.ARROW_RIGHT_TURN:
                SetPaceNoteData(mPaseNoteInstanceSetting.ArrowTurn);
                break;

            case ConstString.PaseNoteUI.ARROW_RIGHT_WAVE:
                SetPaceNoteData(mPaseNoteInstanceSetting.ArrowWave);
                break;

            case ConstString.PaseNoteUI.EXCLAMATION:
                SetPaceNoteData(mPaseNoteInstanceSetting.Exclamation);
                break;
        }
    }

    /// <summary>
    /// ペースノートの出現位置情報をセットする
    /// </summary>
    /// <param name="data"></param>
    private void SetPaceNoteData(PaceNoteData data)
    {
        switch (mNavigationRotation)
        {
            case NavigationRotation.NORMAL:
                mNavigationArrow.transform.localPosition = data.RightPosition;
                if (mNavigationArrow.name == ConstString.PaseNoteUI.ARROW_RIGHT_WAVE)
                {
                    mNavigationArrowFrame.gameObject.SetActive(false);
                }
                else
                {
                    mNavigationArrowFrame.gameObject.SetActive(true);
                }
                break;

            case NavigationRotation.REVERSE:
                mNavigationArrow.transform.localPosition = data.LeftPosition;
                if (mNavigationArrow.name == ConstString.PaseNoteUI.ARROW_RIGHT_WAVE)
                {
                    mNavigationArrowFrame.gameObject.SetActive(false);
                }
                else
                {
                    mNavigationArrowFrame.gameObject.SetActive(true);
                }
                break;
        }
        mNavigationArrow.transform.localScale = data.Scale;
        mNavigationArrowFrame.transform.localPosition = data.FrameRightPosition;
        mNavigationArrowFrame.transform.localScale = data.FrameScale;

    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void OnEnable()
    {
        mCheckFlag = true;
    }

    /// <summary>
    /// ペースノート管理ストリーム生成
    /// </summary>
    void Start()
    {
        //ペースノート表示
        this.OnTriggerEnterAsObservable()
        .Where(collider => collider.gameObject == mPlayer)
        .Where(_ => mCheckFlag)
        .Subscribe(_ =>
        {
            ShowPaseNote(true);
            PaseNoteUpdate();
            mCheckFlag = false;

            // リプレイシーンではない場合
            if (mGameSceneManager.SceneState != SceneState.REPLAY)
            {
                PlaySE.Instance.Play(mAudioSource, ConstAudio.SE_PACENOTE);
            }

            // ペースノートの左右どちらに向けるか
            switch (mNavigationRotation)
            {
                case NavigationRotation.NORMAL:
                    RotateChange(NORMAL_ROTATE);
                    SpriteTransformChange();
                    break;
                case NavigationRotation.REVERSE:
                    RotateChange(REVERSE_ROTATE);
                    SpriteTransformChange();
                    break;
            }
        });

        //ペースノート非表示
        this.OnTriggerExitAsObservable()
            .Where(collider => collider.gameObject == mPlayer)
            .Subscribe(_ => ShowPaseNote(false));
    }

    /// <summary>
    /// ペースノート表示・非表示
    /// </summary>
    /// <param name="show"></param 表示フラグ>
    private void ShowPaseNote(bool show)
    {
        mNavigation.SetActive(show);
    }

    /// <summary>
    /// 表示Sprite変更
    /// </summary>
    private void PaseNoteUpdate()
    {
        mNavigationArrow.sprite = mArrowSprite;
        mNavigationArrowFrame.sprite = mArrowFrameSprite;
    }
}

/// <summary>
/// ペースノートの出現データまとめ
/// </summary>
[CreateAssetMenu(menuName = "PaseNoteInstanceSetting", fileName = "PaseNoteInstanceSetting")]
public class PaseNoteInstanceSetting : ScriptableObject
{
    [SerializeField]
    private PaceNoteData mArrowaCarve = new PaceNoteData()
    {
        LeftPosition = new Vector3(-5f, 14f, 0f),
        RightPosition = new Vector3(-20f, 14f, 0f),
        Scale = new Vector3(0.5677209f, 0.5013877f, 1f),
        FrameRightPosition = new Vector3(-13f, -7f, 0f),
        FrameScale = new Vector3(1.23f, 1.22f, 1f)
    };

    [SerializeField]
    private PaceNoteData mArrowTurn = new PaceNoteData()
    {
        LeftPosition = new Vector3(-19f, 14f, 0f),
        RightPosition = new Vector3(25f, 14f, 0f),
        Scale = new Vector3(0.5677209f, 0.5013877f, 1f),
        FrameRightPosition = new Vector3(-13, -7, 0),
        FrameScale = new Vector3(1.23f, 1.22f, 1f)
    };

    [SerializeField]
    private PaceNoteData mArrowWave = new PaceNoteData()
    {
        LeftPosition = new Vector3(-19f, 14f, 0f),
        RightPosition = new Vector3(-19f, 14f, 0f),
        Scale = new Vector3(0.3955f, 0.5665681f, 1f),
        FrameRightPosition = new Vector3(1.5f, -0.3f, 0),
        FrameScale = new Vector3(1.115f, 1.115f, 1f)
    };

    [SerializeField]
    private PaceNoteData mExclamation = new PaceNoteData()
    {
        LeftPosition = new Vector3(-4.4f, 2.2f, 0f),
        RightPosition = new Vector3(-4.4f, 2.2f, 0f),
        Scale = new Vector3(0.1327241f, 0.485107f, 1f),
        FrameRightPosition = new Vector3(1f, -16f, 0f),
        FrameScale = new Vector3(1.35f, 1.33f, 1f)
    };

    public PaceNoteData ArrowCarve { get { return mArrowaCarve; } }
    public PaceNoteData ArrowTurn { get { return mArrowTurn; } }
    public PaceNoteData ArrowWave { get { return mArrowWave; } }
    public PaceNoteData Exclamation { get { return mExclamation; } }
}

/// <summary>
/// 各ペースノートが持つ出現情報
/// </summary>
[System.Serializable]
public class PaceNoteData
{
    [SerializeField]
    private Vector3 mLeftPosition;
    [SerializeField]
    private Vector3 mRightPosition;
    [SerializeField]
    private Vector3 mScale;
    [SerializeField]
    private Vector3 mFrameRightPosition;
    [SerializeField]
    private Vector3 mFrameScale;

    public Vector3 LeftPosition { get { return mLeftPosition; } set { mLeftPosition = value; } }
    public Vector3 RightPosition { get { return mRightPosition; } set { mRightPosition = value; } }
    public Vector3 Scale { get { return mScale; } set { mScale = value; } }
    public Vector3 FrameRightPosition { get { return mFrameRightPosition; } set { mFrameRightPosition = value; } }
    public Vector3 FrameScale { get { return mFrameScale; } set { mFrameScale = value; } }
}
