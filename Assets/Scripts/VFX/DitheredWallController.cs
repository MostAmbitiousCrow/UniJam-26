using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DitheredWallController : MonoBehaviour
{
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");
    // private Transform _player;
    // [SerializeField] private float triggerDistance = 5f;
    private Material _material;

    [SerializeField] private float tweenTime = .7f;

    // private bool IsPlayerClose => Vector3.Distance(_player.position, transform.position) < triggerDistance;

    private void Awake()
    {
        // _player = GameObject.FindGameObjectWithTag("Player").transform;
        
        var renderer = GetComponent<MeshRenderer>();
        _material = new Material(renderer.material);
        renderer.material = _material;
    }

    private Tween _currentTween;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            SetTween(.45f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            SetTween(1);
    }

    private void SetTween(float endValue)
    {
        _currentTween.Kill(true);
        _currentTween = _material.DOFloat(endValue, Alpha, tweenTime);
    }

    /*private bool _wasPlayerClose;

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
    #endif*/
}
