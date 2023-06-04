using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Scripting;

[RequireComponent(typeof(Animator))]
public class AnimationEvents : MonoBehaviour
{
    public event Action<Animator,int> AnimationIkCallback;

    [SerializeField] private Animator _animator;
    [SerializeField] private ThirdPersonController _controller;

    [Preserve]
    public void OnFootstep(AnimationEvent animationEvent)
    {
        _controller.OnFootstep(animationEvent);
    }

    [Preserve]
    public void OnLand(AnimationEvent animationEvent)
    {
        _controller.OnLand(animationEvent);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        AnimationIkCallback?.Invoke(_animator,layerIndex);
    }
}