using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メニュー画面の制御クラス
/// </summary>
public class MenuSelect : MonoBehaviour
{
    [SerializeField]
    private AudioSource mAudioSource;
    [SerializeField]
    private RectTransform mRightUI;
    [SerializeField]
    private GameObject mRightEffect;
    [SerializeField]
    private RectTransform mLeftUI;
    [SerializeField]
    private GameObject mLeftEffect;
    [SerializeField]
    private Vector3 mLargeSize = new Vector3(1f, 1f, 1f);
    [SerializeField]
    private Vector3 mSmallSize = new Vector3(0.7f, 0.7f, 0.7f);
    [SerializeField]
    private float mSizeTransitionLerp = 1;

    private HundleControllerInput mHundleControllerInput;
    private ControllerInput mControllerInput;

    private bool mSelectAT = true;
    private bool mSelectMT = false;

    private void Awake()
    {
        mControllerInput = transform.root.GetComponentInChildren<ControllerInput>(true);
        mHundleControllerInput = transform.root.GetComponentInChildren<HundleControllerInput>(true);
    }

    void Update()
    {
        // コントローラークラスの取得を確認
        if (mHundleControllerInput == null && mControllerInput == null) return;

        var accel = Input.GetAxis(ConstString.Input.ACCEL);
        print(accel);

        //選ばれているトランスミッションに合わせてゲームシーンのトランスミッションを変える
        if (Input.GetKeyDown(KeyCode.Return) || accel >= 0.1 || Input.GetButtonDown(ConstString.Input.SUBMIT))
        {
            if (mSelectAT) { ConvertTransmission(Transmission.AT); }
            else { ConvertTransmission(Transmission.MT); }
        }

        var hundle = Input.GetAxis(ConstString.Input.HORIZONTAL);


        //ATを選んだ場合、UIのサイズとエフェクトを変更
        if (hundle >= 0.8f)
        {
            PlaySE.Instance.PlayOnce(mAudioSource, ConstAudio.SE_MENU_SELECT);

            mRightEffect.gameObject.SetActive(true);
            mLeftEffect.gameObject.SetActive(false);

            mRightUI.localScale = mLargeSize;
            mLeftUI.localScale = mSmallSize;

            mSelectMT = false;
            mSelectAT = true;
        }
        //MTを選んだ場合、UIのサイズとエフェクトを変更
        else if(hundle <= -0.8f)
        {
            PlaySE.Instance.PlayOnce(mAudioSource, ConstAudio.SE_MENU_SELECT);

            mLeftEffect.gameObject.SetActive(true);
            mRightEffect.gameObject.SetActive(false);

            mLeftUI.localScale = mLargeSize;
            mRightUI.localScale = mSmallSize;

            mSelectAT = false;
            mSelectMT = true;
        }
    }

    private void ConvertTransmission(Transmission transmission)
    {
        if(mHundleControllerInput == null) mControllerInput.GetGearManager.ConvertTo(transmission);
        else mHundleControllerInput.GetGearManager.ConvertTo(transmission);
    }
}
