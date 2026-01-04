using UnityEngine;

public class VillianScript : MonoBehaviour
{
    [SerializeField] GameObject villianHand;
    [SerializeField] GameObject vaccine;

    public void AssigningVaccineParent()
    {
        vaccine.transform.parent = villianHand.transform;
    }
}
