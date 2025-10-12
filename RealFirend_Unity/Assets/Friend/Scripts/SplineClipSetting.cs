using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SplineClipSetting
{
    [SerializeField] private HumanBodyBones m_Parent; public HumanBodyBones Parent => m_Parent;
    [SerializeField] private float m_duration = 1; public float Duration => m_duration;
    [SerializeField] private float m_speed = 1; public float Speed { get { return m_speed; } set { m_speed = value; } }
    [SerializeField] private AnimationCurve m_curve; public AnimationCurve Curve => m_curve;
    [SerializeField] private List<Controller> m_controllers; public List<Controller> Controllers => m_controllers;

    [System.Serializable]
    public class Controller
    {
        public Effector TargetEffector;
        public bool IntertXAxis = false;
        public int UsingSplineIndex = 0;
        public bool Enable = true;
    }
    public enum Effector
    {
        Head,
        LeftHand,
        RightHand,
        LeftFoot,
        RightFoot,
    }
}