using UnityEngine;
using TMPro;

public class Lock : MonoBehaviour
{
    [SerializeField] PasswordChecker passwoedChecker;

    public void Key()
    {
        string key = GetComponentInChildren<TMP_Text>().text;
        passwoedChecker.Adding(key);
    }
}
