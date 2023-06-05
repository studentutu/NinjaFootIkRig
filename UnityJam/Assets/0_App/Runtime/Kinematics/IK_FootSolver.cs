using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_FootSolver : MonoBehaviour
{
    [Serializable]
    public class FootSolution
    {
        [SerializeField] public SpringJoint _springJoint;
        [SerializeField] public Rigidbody _footRb;
        [SerializeField] public Transform _footPositionTarget;
        [SerializeField] public Transform _raycastFrom;
        [SerializeField] public Transform _raycastTo;
        [SerializeField] public Transform _targetToModify;
        [SerializeField] public Vector3 _initialLocalRotationOffset;
        [SerializeField] public Rigidbody _connectedBody;

        [Tooltip("Left foot requires 180 on x, so simply invert normal.")] [SerializeField]
        public bool _invertFinalNormal = false;

        [NonSerialized] public Vector3 PreviousPos;
    }

    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private FootSolution _leftFoot;
    [SerializeField] private FootSolution _rightFoot;

    [SerializeField] private Transform _character;
    [SerializeField] private Transform _hipTarget;
    [SerializeField] private float _hipOffset;
    [SerializeField] private float _slerpFactor;
    [SerializeField] public float _footRaycastLength;
    [SerializeField] private float _anchordDisplacements = 0.4f;
    [SerializeField] private float _maxDistanceBetweenTargetAndAnchor = 0.3f;
    [SerializeField] private float _maxPrevPosDistance = 0.3f;

    private float _minY = 0;
    private float _previousMinYLocal = 0;

    private void Start()
    {
        MoveFootAnchorRbDown(_leftFoot);
        MoveFootAnchorRbDown(_rightFoot);

        _leftFoot.PreviousPos = _leftFoot._footRb.position;
        _rightFoot.PreviousPos = _rightFoot._footRb.position;
    }

    private void OnEnable()
    {
        ResetJoint(_leftFoot);
        ResetJoint(_rightFoot);
        var pos = _character.position;
        _minY = pos.y + _hipOffset;
        
        _previousMinYLocal = _minY;
    }

    private void ResetJoint(FootSolution foot)
    {
        var position = foot._connectedBody.transform.parent.position;
        foot._springJoint.transform.position = position;
        foot._springJoint.autoConfigureConnectedAnchor = true;

        foot._footRb.velocity = Vector3.zero;
        foot.PreviousPos = foot._footRb.position;
    }

    private void MoveFootAnchorRbDown(FootSolution foot)
    {
        var pos = foot._connectedBody.transform.parent.transform.position;
        foot._connectedBody.MovePosition(pos + Vector3.down * _anchordDisplacements);
    }

    private void LateUpdate()
    {
        var pos = _character.position;
        _minY = pos.y + _hipOffset;

        ModifyFoot(_leftFoot);
        ModifyFoot(_rightFoot);

        _hipTarget.position = new Vector3(pos.x, _minY, pos.z);
        var nextPosition = Mathf.Lerp(_previousMinYLocal,_hipTarget.localPosition.y, Time.deltaTime * 6f);
        _hipTarget.localPosition = new Vector3(0, nextPosition, 0);
        _previousMinYLocal = nextPosition;
    }

    private void ModifyFoot(FootSolution foot)
    {
        var previous = foot._targetToModify.rotation;
        var prevPos = foot._footRb.position;

        foot._targetToModify.forward = _character.forward;
        ModifyUp(foot);
        foot._targetToModify.localRotation *= Quaternion.Euler(foot._initialLocalRotationOffset);

        foot._targetToModify.rotation =
            Quaternion.Slerp(previous, foot._targetToModify.rotation, Time.deltaTime * _slerpFactor);

        var targetFootPos = foot._footPositionTarget.position;
        var anchorOriginalPos = foot._connectedBody.position;

        var connectedPos = anchorOriginalPos;
        connectedPos.y = targetFootPos.y;

        var sqrCheck = _maxDistanceBetweenTargetAndAnchor;
        sqrCheck *= sqrCheck;

        if ((targetFootPos - connectedPos).sqrMagnitude > sqrCheck)
        {
            foot._footPositionTarget.position =
                Vector3.Lerp(targetFootPos, connectedPos, Time.deltaTime * _slerpFactor);
        }

        var prevPosSqr = _maxPrevPosDistance;
        prevPosSqr *=prevPosSqr;
        if ((foot.PreviousPos - prevPos).sqrMagnitude < prevPosSqr)
        {
            foot._footRb.velocity = Vector3.zero;
        }

        if (anchorOriginalPos.y >= targetFootPos.y)
        {
            foot._footPositionTarget.position =
                Vector3.Lerp(targetFootPos, anchorOriginalPos, Time.deltaTime * _slerpFactor);
        }

        foot.PreviousPos = foot._footRb.position;
    }

    private void ModifyUp(FootSolution foot)
    {
        var downDirection = CorrectDownDirection(foot);
        var raycast = Physics.Raycast(foot._raycastFrom.position, downDirection, out var hit, _footRaycastLength,
            _raycastLayers.value, QueryTriggerInteraction.Ignore);

        if (raycast)
        {
            foot._targetToModify.rotation = Quaternion.FromToRotation(_character.up, hit.normal) * _character.rotation;
            if (foot._invertFinalNormal)
            {
                foot._targetToModify.rotation *= Quaternion.Euler(-180, 0, 0);
            }

            _minY = Mathf.Min(_minY, hit.point.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizomFoot(_leftFoot);
        DrawGizomFoot(_rightFoot);

        void DrawGizomFoot(FootSolution foot)
        {
            var lineFrom = foot._raycastFrom.position;
            var to = lineFrom + CorrectDownDirection(foot) * _footRaycastLength;
            Gizmos.DrawLine(lineFrom, to);
        }
    }

    private static Vector3 CorrectDownDirection(FootSolution foot)
    {
        var direction = foot._raycastTo.position - foot._raycastFrom.position;
        return direction.normalized;
    }
}