using UnityEngine;

[CreateAssetMenu(fileName = "ZombieData" , menuName = "Scriptable Object / ZombieData")]
public class ZombieData : ScriptableObject
{
    public int points;
    public int health;
    public int provokeRange;
    public float stopValue;
}
