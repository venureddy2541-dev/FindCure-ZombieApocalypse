using UnityEngine;

public class tier : MonoBehaviour
{
    [SerializeField] WheelCollider wheelCo;
    [SerializeField] Transform mesh;

    void Update()
    {
        wheelCo.GetWorldPose(out Vector3 pos,out Quaternion rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
