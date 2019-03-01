using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UiResultLap : UiSuperClass
{

    [SerializeField]
    List<GameObject> lapUI = new List<GameObject>();//アクティブになるLap表示オブジェクト
    List<Text> lapTime = new List<Text>();//↑のtextコンポーネント集

    private void Awake()
    {
        //Selectで変換しWhereで変換できないものをはじきToListでlapTimeに落とし込む
        lapTime = lapUI.Select(x => x.GetComponent<Text>()).Where(x => x != null).ToList();
        //foreach(var x in lapTime)
        //{
        //    Debug.Log(x);
        //}
    }
    public void UiUpdate(List<float> lapList)
    {
        for (int i = 0; i > lapUI.Count; i++)
        {
            //最初に存在してはいけないのでここで存在させる
            lapUI[i].SetActive(true);
            lapTime[i].text = lapList[i].ToString();
        }
    }

}