using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public Image fadeImage;
        public GameObject fadeCanvas;
        public static SceneLoader Instance{get;private set;}
        void Awake()
        {
            
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                Destroy(fadeCanvas);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(fadeCanvas);
        }
        public void ToMap()
        {
            StartCoroutine(MoveScene("MapScene"));
        }
        public void ToStage()
        {   
            StartCoroutine(MoveScene("StageScene"));
        }
        public void ToLobby()
        {
            StartCoroutine(MoveScene("LobbyeScene"));
        }
        IEnumerator MoveScene(string sceneName)
        {
            yield return Fading(0,1);
            SceneManager.LoadScene(sceneName);
            yield return null;
            fadeCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            switch(sceneName)
            {
                case "MapScene":
                    GManager.Instance.SetGameState(GManager.GameState.onMap);
                    break; 
                case "StageScene":
                    GManager.Instance.SetGameState(GManager.GameState.onStage);
                    break;
                case "LobbyScene":
                    GManager.Instance.SetGameState(GManager.GameState.onLobby);
                    break;
            }
            yield return Fading(1,0);
        }
        IEnumerator Fading(float start,float end)
        {
            float t = 0f;
            float fadeTime = 0.5f;
            fadeImage.color = new Color(fadeImage.color.r,fadeImage.color.g,fadeImage.color.b,start);
            Color fadeColor = fadeImage.color;
            while(true)
            {
                if(t >= fadeTime)break;
                t += Time.deltaTime;
                fadeColor = fadeImage.color;
                fadeColor.a = Mathf.Lerp(start,end,t/fadeTime);
                fadeImage.color = fadeColor;
                yield return null;
            }
            fadeColor.a = end;
            fadeImage.color = fadeColor;
        }
        public void SetButton(string stageName)
        {
            
        }
    }
}

