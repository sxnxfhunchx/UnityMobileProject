using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string bootstrapSceneName = "BootstrapScene";

    private IEnumerator Start()
    {
        if (GameManager.Instance != null)
            yield break;

        yield return SceneManager.LoadSceneAsync(bootstrapSceneName, LoadSceneMode.Additive);

        if (GameManager.Instance == null)
            Debug.LogError("BootstrapScene loaded, but GameManager is still null.");
    }
}
