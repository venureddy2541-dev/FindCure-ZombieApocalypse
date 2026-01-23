using UnityEngine;

public class UI : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.NewGame += DisableUI;
    }
    
    void OnDisable()
    {
        GameManager.NewGame -= DisableUI;
    }

    void DisableUI()
    {
        gameObject.SetActive(false);
    }
}
