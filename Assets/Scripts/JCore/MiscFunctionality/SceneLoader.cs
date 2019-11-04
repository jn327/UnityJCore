using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject _loadingScreen = default;

    [SerializeField]
    private Transform _loadingBar = default;

    public void StartLoad(string sceneName )
    {
       // Use a coroutine to load the Scene in the background
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync( string sceneName )
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        if (_loadingScreen != null)
        {
            _loadingScreen.SetActive(true);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            if (_loadingBar != null)
            {
                _loadingBar.localScale = new Vector3(asyncLoad.progress, _loadingBar.localScale.y, _loadingBar.localScale.z);
            }
            
            yield return null;
        }

        if (_loadingScreen != null)
        {
            _loadingScreen.SetActive(false);
        }
    }
}
