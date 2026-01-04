using UnityEngine;
using TMPro;

public class FinalLock : MonoBehaviour
{
    [SerializeField] FinalStagePasswordChecker finalStagePasswoedChecker;

    public void Key()
    {
        string key = GetComponentInChildren<TMP_Text>().text;
        finalStagePasswoedChecker.Adding(key);
    }
}
