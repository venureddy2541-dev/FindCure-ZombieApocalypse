using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SpecialOperation : MonoBehaviour
{
    public GameObject timeBombObject;
    public TMP_Text timerText;
    public int timeBombCount = 4;
    public List<GameObject> bombPlantAreas = new List<GameObject>();
    [SerializeField] LayerMask layers;
    [SerializeField] float shootDist;

    public virtual void Perform()
    {
        if(timeBombCount <= 0) 
        { 
            return; 
        }

        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward,out hit,shootDist,layers))
        {
            for (int i = 0; i < bombPlantAreas.Count; i++)
            {
                if (bombPlantAreas[i].name == hit.collider.name)
                {
                    timeBombCount--;
                    Instantiate(timeBombObject, hit.point, Quaternion.LookRotation(hit.normal)).GetComponent<TimeBomb>().timerText = timerText;
                    bombPlantAreas.Remove(bombPlantAreas[i]);
                }
            }
        }
        else
        {
            MessageBox.messageBox.PressentMessage("Go close to the Teliporters and Look towards The Ground then press X to plant the TimeBomb", null);
        }
    }
}
