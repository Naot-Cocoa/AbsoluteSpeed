using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiResultRank : UiSuperClass
{
    public override void UiUpdate(float currentValue)
    {
        //ゴールしてから存在させる
        gameObject.SetActive(true);
        base.UiUpdate(currentValue);
    }
}