using UnityEngine;

public class Character : MonoBehaviour
{
    protected bool isBeingGrabbed;
    public Transform RoomPoint { get; private set; }
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Initialize(VisitorData data, Transform roomPoint)
    {
        RoomPoint = roomPoint;

        spriteRenderer.sprite = data.GetVariant();
    }

    public virtual void OnGrabbed()
    {
        isBeingGrabbed = true;
    }

    public virtual void OnStolen()
    {
        
    }

}
