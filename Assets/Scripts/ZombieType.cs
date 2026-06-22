using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ZombieType
{
    public varient type;
    public int size;
    public Transform[] spawnpoints;
    public List<Vector3> positions = new List<Vector3>();
}
