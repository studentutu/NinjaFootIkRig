using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_FootSolver : MonoBehaviour
{
    [Serializable]
    public class FootSolution
    {
        [SerializeField] public Transform _raycastFrom;
        [SerializeField] public Transform _raycastTo;
        [SerializeField] public Transform _targetToModify;
        [SerializeField] public Vector3 _initialLocalRotationOffset;
        [SerializeField] public float _raycastLength;
        [SerializeField] public Rigidbody _connectedBody;

        [Tooltip("Left foot requires 180 on x, so simply invert normal.")] [SerializeField]
        public bool _invertFinalNormal = false;
    }

    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private FootSolution _leftFoot;
    [SerializeField] private FootSolution _rightFoot;

    [SerializeField] private Transform _character;
    [SerializeField] private Transform _hipTarget;
    [SerializeField] private float _slerpFactor;
    [SerializeField] private float _anchordDisplacements = 0.4f;

    private float _minY = 0;

    private IEnumerator Start()
    {
        yield return null;
        MoveFootAnchorRbDown(_leftFoot);
        MoveFootAnchorRbDown(_rightFoot);
    }

    private void MoveFootAnchorRbDown(FootSolution foot)
    {
        var pos = foot._connectedBody.transform.parent.transform.position;
        foot._connectedBody.MovePosition(pos + Vector3.down * _anchordDisplacements);
    }

    private void LateUpdate()
    {
        var pos = _character.position;
        _minY = pos.y;

        ModifyFoot(_leftFoot);
        ModifyFoot(_rightFoot);

        _hipTarget.position = new Vector3(pos.x, _minY, pos.z);
    }

    private void ModifyFoot(FootSolution foot)
    {
        var previous = foot._targetToModify.rotation;
        
        foot._targetToModify.forward = _character.forward;
        ModifyUp(foot);
        foot._targetToModify.localRotation *= Quaternion.Euler(foot._initialLocalRotationOffset);

        foot._targetToModify.rotation = Quaternion.Slerp(previous,foot._targetToModify.rotation ,Time.deltaTime * _slerpFactor);
    }

    private void ModifyUp(FootSolution foot)
    {
        var downDirection = CorrectDownDirection(foot);
        var raycast = Physics.Raycast(foot._raycastFrom.position, downDirection, out var hit, foot._raycastLength,
            _raycastLayers.value, QueryTriggerInteraction.Ignore);
        
        if(raycast)
        {
            foot._targetToModify.rotation = Quaternion.FromToRotation(_character.up, hit.normal) * _character.rotation;
            if (foot._invertFinalNormal)
            {
                foot._targetToModify.rotation *= Quaternion.Euler(-180, 0, 0);
            }

            _minY = Mathf.Min(_minY,hit.point.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizomFoot(_leftFoot);
        DrawGizomFoot(_rightFoot);

        void DrawGizomFoot(FootSolution foot)
        {
            var lineFrom = foot._raycastFrom.position;
            var to = lineFrom + CorrectDownDirection(foot) * foot._raycastLength;
            Gizmos.DrawLine(lineFrom, to);
        }
    }

    private static Vector3 CorrectDownDirection(FootSolution foot)
    {
        var direction = foot._raycastTo.position - foot._raycastFrom.position;
        return direction.normalized;
    }
}