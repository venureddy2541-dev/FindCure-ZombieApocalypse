using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData" , menuName = "Scriptable Object / PlayerData")]
public class PlayerData : ScriptableObject
{
    public int health = 150;
    public int minHealth;
    public float lowHealthBlinkingSpeed = 1.2f;
    public float coolDownTime = 25f;
    public float invisibleTime = 5f;
    public int healthIncForHealthKit = 40;
    public int healthKitCount = 0;
}
