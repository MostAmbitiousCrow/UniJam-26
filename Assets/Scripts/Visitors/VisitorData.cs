using UnityEngine;

[CreateAssetMenu(fileName = "Visitor Data", menuName = "ScriptableObjects/Visitor Data", order = 1)]
public class VisitorData : ScriptableObject
{
    public VisitorType visitorType;
    public Sprite[] variants;

    public AudioClip rejectionSound, acceptedSound;
    [Range(0f, 1f)] public float appearanceWeight = .1f;

    public GameObject characterPrefab;

    public Sprite GetVariant()
    {
        return variants[Random.Range(0, variants.Length)];
    }

}

public enum VisitorType { Vampire, Survivor, Imposter }
