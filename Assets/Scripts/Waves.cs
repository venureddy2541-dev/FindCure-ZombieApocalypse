using UnityEngine;
using TMPro;

public class Waves : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public string key;
    [SerializeField] private int count;
    public int Count { get { return count; }}
    Helper helper;

    void Start()
    {
        if (GameManager.gameManager.transform.Find(gameObject.name))
        {
            Destroy(gameObject);
        }
        helper = GetComponent<Helper>();
        text = GameObject.FindWithTag("PassCode").GetComponent<TMP_Text>();
    }

    public void Counting()
    {
        count--;
        if (count == 0)
        {
            if (helper)
            {
                if(helper.level == 2)
                {
                    helper.SpawnersDead();
                }
                else
                {
                    helper.ResetMusic();
                }
            }
            GameManager.gameManager.ManageLevel();
            text.text = "CODE : " + key;
            transform.parent = GameManager.gameManager.transform;
            GameManager.gameManager.levels.Add(gameObject);
        }
    }

    public void CountAdder(int remaingZombies)
    {
        count += remaingZombies;
    }
}
