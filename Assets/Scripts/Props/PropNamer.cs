using UnityEngine;

namespace Props
{
    public class PropNamer : MonoBehaviour
    {
        private void OnValidate()
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr) name = $"Prop ({sr.sprite.name})";
        }
    }
}
