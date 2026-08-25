using System;
using DG.Tweening;
using UnityEngine;

public class DitheredWallController : MonoBehaviour
{
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");
    private Transform _player;
    [SerializeField] private float triggerDistance = 5f;
    private Material _material;

    [SerializeField] private float tweenTime = .7f;

    private bool IsPlayerClose => Vector3.Distance(_player.position, transform.position) < triggerDistance;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        
        var renderer = GetComponent<MeshRenderer>();
        _material = new Material(renderer.material);
        renderer.material = _material;
    }

    private bool _wasPlayerClose;

    private void FixedUpdate()
    {
        if (IsPlayerClose == _wasPlayerClose) return;
        
        _wasPlayerClose = IsPlayerClose;
        _material.DOFloat(IsPlayerClose ? .2f : 1f, Alpha, tweenTime);
        Debug.Log($"{gameObject} is fading = {IsPlayerClose}");
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Gizmos.color = IsPlayerClose ? Color.green : Color.yellow;
        Gizmos.DrawLine(_player.position, transform.position);
    }
    #endif
}
