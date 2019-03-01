using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBrakePoint : MonoBehaviour {

    UnityStandardAssets.Vehicles.Car.CarAIControl m_AiCon;//取得したAiConの器
    [SerializeField]float m_limitsSpeed;
    private void OnTriggerEnter(Collider other)
    {
        m_AiCon = other.GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>();

        if (m_AiCon == null) { return; }//もしotherがAiでなければ処理を行わない


        if (m_AiCon.CurrentAiSpeed() >= m_limitsSpeed && m_AiCon.m_Driving == true)//m_limitsSpeedより早ければスピードを下げる
        {
            m_AiCon.m_Driving = false;
        }
    }

    //ブレーキポイントに滞在中にm_limitsSpeedを速度が下回った場合ブレーキを離す
    private void OnTriggerStay(Collider other)
    {
        m_AiCon = other.gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>();
        if (m_AiCon == null) { return; }//もしotherがAiでなければ処理を行わない
        if (m_AiCon.CurrentAiSpeed() <= m_limitsSpeed && m_AiCon.m_Driving == false) { m_AiCon.m_Driving = true; }
    }

    //ブレーキポイントから脱した場合強制的にブレーキを離す
    private void nTriggerExit(Collider other)
    {
        if (m_AiCon.m_Driving == false) m_AiCon.m_Driving = true;
    }
}
