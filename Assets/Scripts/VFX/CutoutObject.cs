using UnityEngine;
using System.Linq;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float cutoutSize = 0.1f;
    [SerializeField] private float falloffSize = 0.05f;
    [SerializeField] private float raycastCircleRadius = 0.25f;
    [SerializeField] private float raycastAngleOffset = 0f;
    private GameObject[] cutoutObjects;

    void Awake()
    {
        cutoutObjects = GameObject.FindGameObjectsWithTag("Cutout");
    }

    void FixedUpdate()
    {
        bool[] cutoutsHit = new bool[cutoutObjects.Length];
        Camera mainCamera = Camera.main;

        if (mainCamera != null && targetObject != null)
        {
            Vector3 targetPosition = targetObject.transform.position;
            Vector3 toTarget = targetPosition - mainCamera.transform.position;
            float targetDistance = toTarget.magnitude;
            Vector3 circleRight = mainCamera.transform.right;
            Vector3 circleUp = mainCamera.transform.up;

            if (targetDistance > 0f)
            {
                RaycastForCutout(mainCamera.transform.position, targetPosition, cutoutsHit);

                float angleStep = 360f / 8f;
                for (int rayIndex = 0; rayIndex < 8; ++rayIndex)
                {
                    float angle = (raycastAngleOffset + rayIndex * angleStep) * Mathf.Deg2Rad;
                    Vector3 rayTarget = targetPosition +
                        (circleRight * Mathf.Cos(angle) + circleUp * Mathf.Sin(angle)) * raycastCircleRadius;

                    RaycastForCutout(mainCamera.transform.position, rayTarget, cutoutsHit);
                }
            }
        }

        for (int i = 0; i < cutoutObjects.Length; ++i)
        {
            Material[] materials = cutoutObjects[i].transform.GetComponent<Renderer>().materials;
            float modifiedCutoutSize = cutoutsHit[i] ? cutoutSize : 0f;

            for(int m = 0; m < materials.Length; ++m)
            {                
                materials[m].SetFloat("_CutoutSize", modifiedCutoutSize);
                materials[m].SetFloat("_FalloffSize", falloffSize);
            }
        }
    }

    private void RaycastForCutout(Vector3 origin, Vector3 target, bool[] cutoutsHit)
    {
        Vector3 direction = target - origin;
        Debug.DrawRay(origin, direction, Color.red);
        RaycastHit[] hits = Physics.RaycastAll(origin, direction.normalized, direction.magnitude, wallMask);
        for (int hitIndex = 0; hitIndex < hits.Length; ++hitIndex)
        {
            Transform hitTransform = hits[hitIndex].transform;
            for (int i = 0; i < cutoutObjects.Length; ++i)
            {
                Transform cutoutTransform = cutoutObjects[i].transform;
                cutoutsHit[i] |= hitTransform == cutoutTransform || hitTransform.IsChildOf(cutoutTransform);
            }
        }
    }
}
