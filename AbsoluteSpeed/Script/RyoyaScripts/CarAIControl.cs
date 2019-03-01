using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarAIControl : MonoBehaviour
    {
        public enum BrakeCondition
        {
            NeverBrake,                 // �Ԃ̓t���X���b�g���ŏ�ɉ������܂�.
            TargetDirectionDifference,  // �Ԃ͖ڕW�̕����̎��̕ω��ɉ����ău���[�L��������B���[�g�x�[�X��AI�ɖ𗧂��A�R�[�i�[�̑��x���x���Ȃ�܂�
            TargetDistance,             // �^�[�Q�b�g�̕����ɂ�����炸�A�Ԃ̓^�[�Q�b�g�ɋ߂Â��ɂ�ău���[�L�������܂��B���Ȃ����Ԃɂ������ꍇ�ɕ֗��ł�
                                        // �Î~�����ڕW�̂��߂ɓ������낵�A�����ɒ�������x������B
        }

        // This script provides input to the car controller in the same way that the user control script does.
        // As such, it is really 'driving' the car, with no special physics or animation tricks to make the car behave properly.

        // "wandering" is used to give the cars a more human, less robotic feel. They can waver slightly
        // in speed and direction while driving towards their target.

        [SerializeField] [Range(0, 1)] private float m_CautiousSpeedFactor = 0.05f;               // percentage of max speed to use when being maximally cautious
        [SerializeField] [Range(0, 180)] private float m_CautiousMaxAngle = 50f;                  // angle of approaching corner to treat as warranting maximum caution
        [SerializeField] private float m_CautiousMaxDistance = 100f;                              // distance at which distance-based cautiousness begins
        [SerializeField] private float m_CautiousAngularVelocityFactor = 30f;                     // how cautious the AI should be when considering its own current angular velocity (i.e. easing off acceleration if spinning!)
        [SerializeField] private float m_SteerSensitivity = 0.05f;                                // how sensitively the AI uses steering input to turn to the desired direction
        [SerializeField] private float m_AccelSensitivity = 0.04f;                                // How sensitively the AI uses the accelerator to reach the current desired speed
        [SerializeField] private float m_BrakeSensitivity = 1f;                                   // How sensitively the AI uses the brake to reach the current desired speed
        [SerializeField] private float m_LateralWanderDistance = 3f;                              // how far the car will wander laterally towards its target
        [SerializeField] private float m_LateralWanderSpeed = 0.1f;                               // how fast the lateral wandering will fluctuate
        [SerializeField] [Range(0, 1)] private float m_AccelWanderAmount = 0.1f;                  // how much the cars acceleration will wander
        [SerializeField] private float m_AccelWanderSpeed = 0.1f;                                 // how fast the cars acceleration wandering will fluctuate
        [SerializeField] private BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance; // what should the AI consider when accelerating/braking?
        [SerializeField] private Transform m_Target;                                              // 'target' the target object to aim for.
        [SerializeField] private bool m_StopWhenTargetReached;                                    // should we stop driving when we reach the target?
        [SerializeField] private float m_ReachTargetThreshold = 2;                                // proximity to target to consider we 'reached' it, and stop driving.

        /// <summary>AI���v���C���[�ɓ��������ꍇ�ɃX�s�[�h�ɏ�Z�����l</summary>
        [SerializeField] private float mPlayerColDeceleration=0.0f;
        /// <summary>AI���m���Ԃ������ꍇ�ɃX�s�[�h�ɏ�Z�����l</summary>
        [SerializeField] private float mEnemyColDeceleration=0.4f;
        /// <summary>����s������鎞��</summary>
        [SerializeField] private float mAvoidSettingTime=5;
        /// <summary>�v���C���[�ƐڐG������s�������v���C���[�Ƃ̊p�x</summary>
        [SerializeField] private float mPlayerAvoidAngle=140;
        /// <summary>AI�ƐڐG������s�������AI�Ƃ̊p�x</summary>
        [SerializeField] private float mEnemyAvoidAngle=90;

        private float m_RandomPerlin;             // A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
        private CarController m_CarController;    // Reference to actual car controller we are controlling
        private float m_AvoidOtherCarTime;        // time until which to avoid the car we recently collided with
        private float m_AvoidOtherCarSlowdown;    // how much to slow down due to colliding with another car, whilst avoiding
        private float m_AvoidPathOffset;          // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding
        private Rigidbody m_Rigidbody;
        [SerializeField]
        public bool m_Driving;                    // �u���[�L�𓥂�ŃX�s�[�h�𗎂Ƃ���
        public bool m_StartFrag;                  //�ŏ��̈��
        public GameObject m_Player;
        private GameStateManager mGameStateManager;
        [SerializeField]Material mBrakeMaterial;
        Color mBreakcolor = new Color(0.85f, 0, 0, 1);

        private void Awake()
        {
            // get the car controller reference
            m_CarController = GetComponent<CarController>();
            // give the random perlin a random value
            m_RandomPerlin = Random.value * 100;

            m_Rigidbody = GetComponent<Rigidbody>();

            mGameStateManager = GameObject.FindObjectOfType<GameStateManager>();

        }
        public void OnEnable()
        {
            mBrakeMaterial.SetColor("_EmissionColor", Vector4.zero);
            m_Rigidbody.velocity = Vector3.zero;
            m_Driving = false;
            m_StartFrag = true;
        }

        //���݂̃X�s�[�h��Ԃ�
        private float m_CurrentSpeed;

        /// <summary>
        /// ���ݑ��x�Ԃ���
        /// </summary>
        public float CurrentAiSpeed()
        {
            m_CurrentSpeed = m_Rigidbody.velocity.magnitude;
            return m_CurrentSpeed;
        }
        private void FixedUpdate()
        {

            if (mGameStateManager.CurrentGameState.Value == InGameState.PLAY && m_StartFrag==true) { m_Driving = true; m_StartFrag = false; }
            //if (IsGround ) { OkSwitch = true; }
            //else { OkSwitch = false; }

            //if (!OkSwitch)
            //{
            //    eulerAngles = transform.rotation.eulerAngles;
            //    eulerAngles.x = 20.0f;
            //    eulerAngles.z = 0;
            //    print(eulerAngles);
            //    Quaternion fixRot = Quaternion.Euler(eulerAngles);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, fixRot, 2f * Time.deltaTime);
            //    return;
            //}

            if (m_Target == null || !m_Driving)
            {
                //�u���[�L�����v�����点��
                if (mBrakeMaterial.GetColor("_EmissionColor") != mBreakcolor&& mGameStateManager.CurrentGameState.Value == InGameState.PLAY) { mBrakeMaterial.SetColor("_EmissionColor", mBreakcolor); }
                // �n���h�u���[�L���g�p���Ē�~����
                if (m_Rigidbody.velocity.magnitude > 1) 
                {
                    m_CarController.Move(0, 0, -1f, 0f);
                }
                if (m_Rigidbody.velocity.magnitude < 1)
                {
                    m_Rigidbody.velocity = new Vector3(0, 0, 0);
                    m_CarController.Move(0, 0, 0f, 0f);

                }

            }
            else
            {
                if (mBrakeMaterial.GetColor("_EmissionColor") == mBreakcolor) { mBrakeMaterial.SetColor("_EmissionColor", Vector4.zero); }
                Vector3 fwd = transform.forward;
                if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed * 0.1f)
                {
                    fwd = m_Rigidbody.velocity;
                }

                float desiredSpeed = m_CarController.MaxSpeed;

                // ��������ׂ����ǂ��������߂鎞...
                switch (m_BrakeCondition)
                {
                    case BrakeCondition.TargetDirectionDifference:
                        {
                            // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.

                            // check out the angle of our target compared to the current direction of the car
                            float approachingCornerAngle = Vector3.Angle(m_Target.forward, fwd);

                            // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                            float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

                            // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                            float cautiousnessRequired = Mathf.InverseLerp(0, m_CautiousMaxAngle,
                                                                           Mathf.Max(spinningAngle,
                                                                                     approachingCornerAngle));
                            desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
                                                      cautiousnessRequired);
                            break;
                        }

                    case BrakeCondition.TargetDistance:
                        {
                            // �^�[�Q�b�g�̕����ɂ�����炸�A�Ԃ̓^�[�Q�b�g�ɋ߂Â��ɂ�ău���[�L�������܂�
                            // �Î~�����ڕW�̓����ɓ��B�����Ƃ��ɋx������

                            // check out the distance to target
                            Vector3 delta = m_Target.position - transform.position;
                            float distanceCautiousFactor = Mathf.InverseLerp(m_CautiousMaxDistance, 0, delta.magnitude);

                            // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                            float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

                            // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                            float cautiousnessRequired = Mathf.Max(
                                Mathf.InverseLerp(0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);
                            desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
                                                      cautiousnessRequired);
                            break;
                        }

                    case BrakeCondition.NeverBrake:
                        break;
                }

                // Evasive action due to collision with other cars:

                // our target position starts off as the 'real' target position
                Vector3 offsetTargetPos = m_Target.position;

                // ���̎ԂɂԂ���Ȃ��悤�ɉ���s��������Ă���ꍇ
                if (Time.time < m_AvoidOtherCarTime)
                {
                    // �K�v�ɉ����Č�������i�Փ˂����������Ƃ��ɑ��̎Ԃ̌��ɂ���ꍇ�j
                    desiredSpeed *= m_AvoidOtherCarSlowdown;
                    if (mBrakeMaterial.GetColor("_EmissionColor") != mBreakcolor && mGameStateManager.CurrentGameState.Value == InGameState.PLAY) { mBrakeMaterial.SetColor("_EmissionColor", mBreakcolor); }
                    //  ���̎Ԃ��痣��Ă��鎄�����̓�����ڕW�ւ̑��Ɍ�������
                    offsetTargetPos += m_Target.right * m_AvoidPathOffset;
                }
                else
                {
                    // no need for evasive action, we can just wander across the path-to-target in a random way,
                    // which can help prevent AI from seeming too uniform and robotic in their driving
                    offsetTargetPos += m_Target.right *
                                       (Mathf.PerlinNoise(Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) *
                                       m_LateralWanderDistance;
                }

                // use different sensitivity depending on whether accelerating or braking:
                float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed)
                                                  ? m_BrakeSensitivity
                                                  : m_AccelSensitivity;

                // decide the actual amount of accel/brake input to achieve desired speed.
                float accel = Mathf.Clamp((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);

                // add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
                // i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
                accel *= (1 - m_AccelWanderAmount) +
                         (Mathf.PerlinNoise(Time.time * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);

                // calculate the local-relative position of the target, to steer towards
                Vector3 localTarget = transform.InverseTransformPoint(offsetTargetPos);

                // work out the local angle towards the target
                float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

                // get the amount of steering needed to aim the car towards the target
                float steer = Mathf.Clamp(targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign(m_CarController.CurrentSpeed);

                // feed input to the car controller.
                m_CarController.Move(steer, accel, accel, 0f);

                // if appropriate, stop driving when we're close enough to the target.
                if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold)
                {
                    m_Driving = false;
                }
            }
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="col"></param>
        private void OnCollisionStay(Collision col)
        {
            // ���̎ԂƂ̏Փ˂����o���A����s�����Ƃ邱�Ƃ��ł��܂�
            if (col.rigidbody != null)
            {
                var otherCar = col.gameObject.GetComponent<Utility.AiRankTracker>();
                if (otherCar != null)
                {
                    //���b�ԉ���s�����H
                    m_AvoidOtherCarTime = Time.time +mAvoidSettingTime;

                    // ���ʂɂ��ꂩ����̂�
                    if (Vector3.Angle(transform.forward, otherCar.transform.position - transform.position) < mEnemyAvoidAngle&& otherCar.gameObject != m_Player)
                    {
                        // �������Ă�Ԃ͑O�ɂ����Ō������܂��傤
                        m_AvoidOtherCarSlowdown = mEnemyColDeceleration;
                    }
                    else if (otherCar.gameObject  == m_Player && Vector3.Angle(transform.forward, otherCar.transform.position - transform.position) < mPlayerAvoidAngle)
                    {
                        // �v���C���[�ƐڐG���Ă�挸�������
                        m_AvoidOtherCarSlowdown = mPlayerColDeceleration;
;
                    }
                    else
                    {
                        //���܂��ɂ��邩��u���[�L�Ȃ�Ă����ˁ[��(�v���C���[�͉������ɂ����猸�����Ȃ�)
                        m_AvoidOtherCarSlowdown = 1;
                    }

                    
                    //��]�s��������Ăق��̎Ԃ��痣����
                    var otherCarLocalDelta = transform.InverseTransformPoint(otherCar.transform.position);
                    float otherCarAngle = Mathf.Atan2(otherCarLocalDelta.x, otherCarLocalDelta.z);
                    m_AvoidPathOffset = m_LateralWanderDistance * -Mathf.Sign(otherCarAngle);
                }
            }
        }


        public void SetTarget(Transform target)
        {
            m_Target = target;
            m_Driving = true;
        }
    }
}
