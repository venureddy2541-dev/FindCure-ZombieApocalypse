using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader sceneLoader;
    [SerializeField] GameObject sceneLoadingImage;
    [SerializeField] Scrollbar sceneLoadSlider;
    [SerializeField] TMP_Text sceneLoadText;
    [SerializeField] GameStart gameStart;
    public VideoPlayer videoPlayer;

    void Awake()
    {
        if(sceneLoader != null && sceneLoader != this)
        {
            Destroy(gameObject);
            return;
        }

        sceneLoader = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SceneLoadManager(int currentScene,bool backToMainMenu)
    {
        sceneLoadingImage.SetActive(true);
        StartCoroutine(SceneLoading(currentScene,backToMainMenu));
    }

    IEnumerator SceneLoading(int currentScene,bool backToMainMenu)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene);
        asyncLoad.allowSceneActivation = false;
        float timer = 0;
        float waitTime = 2f;
        while(!asyncLoad.isDone)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(asyncLoad.progress*(timer/waitTime));
            sceneLoadSlider.size = progress;
            sceneLoadText.text = Mathf.RoundToInt(progress*100) + "%";
            if(asyncLoad.progress >= 0.9f && timer >= waitTime)
            {
                sceneLoadSlider.size = 1;
                sceneLoadText.text = 100 + "%";
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        sceneLoadingImage.SetActive(false);

        if(backToMainMenu)
        {
            gameStart.ActivateButtons();
        }
    }
}
