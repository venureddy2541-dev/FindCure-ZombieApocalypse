using UnityEngine;

[CreateAssetMenu(fileName = "SensitivitySB",menuName = "Sensy / ScriptableObject")]
public class SensitivitySB : ScriptableObject
{
    public float min = 0f;
    public float max = 5f;

    /*[Header("ADS Sensy min&max values")]
    [SerializeField] [Range(min,max)] float ads;

    [Header("pistol Sensy min&max values")]
    [SerializeField] [Range(min,max)] float pistol;

    [Header("auto Sensy min&max values")]
    [SerializeField] [Range(min,max)] float auto;

    [Header("shotgun Sensy min&max values")]
    [SerializeField] [Range(min,max)] float shotgun;

    [Header("sniper Sensy min&max values")]
    [SerializeField] [Range(min,max)] float sniper;*/
}
