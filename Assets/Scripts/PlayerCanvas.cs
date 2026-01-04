using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] GameObject playerCanavs;

    public void CanvasActivator()
    {
        playerCanavs.SetActive(true);
    }
}
