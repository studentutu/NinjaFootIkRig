using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_FootSolver : MonoBehaviour
{
    [Serializable]
    public class FootSolution
    {
        [Tooltip("Local orientation will be taken.")] [SerializeField]
        public Transform _raycastFrom;

        [SerializeField] public Transform _raycastTo;
        [SerializeField] public Transform _targetToModify;
        [SerializeField] public Vector3 _initialLocalRotationOffset;
        [SerializeField] public float _raycastLength;

        [Tooltip("Left foot requires 180 on x, so simply invert normal.")] [SerializeField]
        public bool _invertFinalNormal = false;
    }

    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private List<string> _ignoreTags;

    [SerializeField] private FootSolution _leftFoot;
    [SerializeField] private FootSolution _rightFoot;

    [SerializeField] private Transform _character;
    [SerializeField] private Transform _hipRoot;


    private RaycastHit[] _results = new RaycastHit[5];
    private float _minY = 0;

    private void Update()
    {
        _minY = 0;

        ModifyFoot(_leftFoot);
        ModifyFoot(_rightFoot);


        var pos = _hipRoot.position;
        _hipRoot.position = new Vector3(pos.x, _minY, pos.z);
    }

    private void ModifyFoot(FootSolution foot)
    {
        foot._targetToModify.forward = _character.forward;
        ModifyUp(foot);
        foot._targetToModify.localRotation *= Quaternion.Euler(foot._initialLocalRotationOffset);
    }

    private void ModifyUp(FootSolution foot)
    {
        var downDirection = CorrectDownDirection(foot);
        var raycast = Physics.RaycastNonAlloc(foot._raycastFrom.position, downDirection, _results, foot._raycastLength,
            _raycastLayers.value, QueryTriggerInteraction.Ignore);
        if (raycast <= 0) return;
        for (var i = 0; i < raycast; i++)
        {
            var hit = _results[i];
            if (_ignoreTags.Contains(hit.transform.tag))
                continue;

            foot._targetToModify.rotation = Quaternion.FromToRotation(_character.up, hit.normal) * _character.rotation;
            if (foot._invertFinalNormal)
            {
                foot._targetToModify.rotation *= Quaternion.Euler(-180, 0, 0);
            }

            _minY = Mathf.Min(hit.point.y);
            return;
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