using UniRx;
using UnityEngine;
using UnityEngine.UI;
namespace Maps
{
    public class QuitButton : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            gameObject.GetComponent<Button>().OnClickAsObservable()
                .Subscribe(_ =>
                {
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                })
                .AddTo(gameObject);
        }
    }
}

