using UnityEngine;

public class SpawnerRotater : MonoBehaviour
{
    [SerializeField] GameObject core1;
    [SerializeField] GameObject core2;
    [SerializeField] GameObject core3;
    [SerializeField] GameObject core4;
    [SerializeField] float rcotateSpeed = 150f;

    void Update()
    {
        float speed = 1 * Time.deltaTime * rcotateSpeed;
        core1.transform.Rotate(new Vector3(1,1,1)*speed);
        core2.transform.Rotate(new Vector3(-1,-1,-1)*speed);
        core3.transform.Rotate(new Vector3(1,-1,1)*speed);
        core4.transform.Rotate(new Vector3(-1,1,-1)*speed);
    }
}
