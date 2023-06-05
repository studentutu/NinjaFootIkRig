using System;
using UnityEngine;

namespace _0_App.Runtime.Kinematics
{
    [ExecuteInEditMode]
    public class SetTranformInEditor : MonoBehaviour
    {
        [SerializeField] private bool ApplyPositionFrom;
        [SerializeField] private Transform Source;
        [SerializeField] private Transform Target;

        private void Update()
        {
            if (ApplyPositionFrom)
            {
                Target.position = Source.position;
                Target.rotation = Source.rotation;
            }
        }
    }
}