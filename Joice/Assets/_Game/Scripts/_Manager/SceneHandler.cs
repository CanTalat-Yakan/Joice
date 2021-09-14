using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static void SceneChange(int _i)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(_i);
    }

    public static void SceneChange(string _s)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(_s);
    }

    public static void SceneAdd(int _i)
    {
        SceneManager.LoadScene(_i, LoadSceneMode.Additive);
    }

    public static void SceneAdd(string _s)
    {
        SceneManager.LoadScene(_s, LoadSceneMode.Additive);
    }

    internal static bool SceneLoaded(int _i)
    {
        return SceneManager.GetSceneByBuildIndex(_i).isLoaded;
    }

    internal static bool SceneLoaded(string _s)
    {
        return SceneManager.GetSceneByName(_s).isLoaded;
    }

    public static void SceneUnload(int _i)
    {
        SceneManager.UnloadSceneAsync(_i);
    }

    public static void SceneRemove(string _s)
    {
        SceneManager.UnloadSceneAsync(_s);
    }

    public static void Locked(bool _b)
    {
        if (GameManager.Instance)
            GameManager.Instance.LOCKED = _b;
    }

    public static void Continue(string _s)
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.LOCKED = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        SceneManager.UnloadSceneAsync(_s);
    }

    public static void DestroyGameManager()
    {
        Destroy(GameManager.Instance.gameObject);
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
