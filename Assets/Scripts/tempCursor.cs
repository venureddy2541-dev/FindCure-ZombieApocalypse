using UnityEngine;

public class tempCursor : MonoBehaviour
{
    [SerializeField] RectTransform gunCrossHair;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        gunCrossHair.anchoredPosition = Input.mousePosition;
    }
}
