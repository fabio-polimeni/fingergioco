using UnityEngine;
using System.Collections;
using System.Threading;

public class SceneManager : MonoBehaviour
{
    // An array of loaded level trees.
    // A level tree is a tree-graph of
    // destroyable game objects in the level.
    private SceneRoot[] m_Roots;
    public static SceneRoot[] Roots
    {
        get { return SceneManager.Instance.m_Roots; }
        set { SceneManager.Instance.m_Roots = value; }
    }

    // Since we can move just along the z-axis we store the current level index, and, at the
    // same time, assume the previous and the next ones are available, if they make sense.
    // They might are not available in case the current level index is zero, or is equel to
    // the last available scene in the game.
    public static int CurrentScene
    {
        get
        {
            // Calculate the scene index the camera is on, or clamp values to reasonable ones.
            return Mathf.Clamp( (int)((SceneManager.Instance.m_CurrentCamera.transform.position.z + GameSettings.BaseSurfaceExtent)
                / (GameSettings.BaseSurfaceExtent * 2.0f)), 0, SceneManager.TotalLevels);
        }
    }

    // Number of total levels to load.
    public static int TotalLevels
    {
        // Return number of currently loaded scenes.
        get { return Application.levelCount-1; }
    }

    // Number of loaded scenes.
    private int m_NumberOfLoadedScenes;
    public static int NumberOfLoadedScenes
    {
        // Return number of currently loaded scenes.
        get { return SceneManager.Instance.m_NumberOfLoadedScenes; }
    }

    // Last loaded scene index
    private int m_LastLoadedScene;
    public static int LastLoadedScene
    {
        // Return the index of the current scene.
        get { return SceneManager.Instance.m_LastLoadedScene; }
    }

    private int m_SceneLoading;
    public static int SceneLoading
    {
        // Return the index of the scene currently loading, a negative number otherwise.
        get { return SceneManager.Instance.m_SceneLoading; }
    }

    private Camera m_CurrentCamera;
    public static Camera CurrentCamera
    {
        get { return SceneManager.Instance.m_CurrentCamera;  }
    }

    // Singleton pattern.
    private static SceneManager m_Instance = null;
    public static SceneManager Instance //{ get { return m_Instance; } }
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject go = new GameObject();
                m_Instance = go.AddComponent<SceneManager>();
                go.name = "SceneManager";

                // Instantiate level roots array.
                // Level roots are not initialised at this point.
                m_Instance.m_Roots = new SceneRoot[SceneManager.TotalLevels];

                // Nothing is loading at the beginning,
                // but we have the first scene loaded.
                m_Instance.m_SceneLoading = -1;
                m_Instance.m_LastLoadedScene = -1;
                m_Instance.m_NumberOfLoadedScenes = 0;

                // Load the main camera.
                if (Camera.main == null)
                {
                    GameObject co = (GameObject)GameObject.Instantiate(Resources.Load("MainCamera", typeof(GameObject)));
                    m_Instance.m_CurrentCamera = co.GetComponent<Camera>();
                }
                else
                {
                    m_Instance.m_CurrentCamera = Camera.main;
                }

                Object.DontDestroyOnLoad(go);
                Object.DontDestroyOnLoad(m_Instance.m_CurrentCamera.gameObject);
            }

            return m_Instance;
        }
    }

    // Use this when the level is loaded
    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SceneManager.DestroyTermporaries();
    }

    /// <summary>
    /// Non-Unity functions.
    /// </summary>

    // Load a new level.
    private void LoadSceneSync(int index, bool additive)
    {
        if (additive)
        {
            Application.LoadLevelAdditive(index);
        }
        else
        {
            Application.LoadLevel(index);
        }

        m_LastLoadedScene = index;
        m_SceneLoading = -1;

    //#if UNITY_EDITOR
    //    Debug.Log("Scene loaded: " + this.m_LastLoadedScene);
    //#endif
    }

    // Load a new level.
    private IEnumerator LoadSceneCoroutine(int index, bool additive)
    {
        if (additive)
        {
            Application.LoadLevelAdditive(index);
        }
        else
        {
            Application.LoadLevel(index);
        }

        m_LastLoadedScene = index;

    //#if UNITY_EDITOR
    //    Debug.Log("Scene loaded: " + this.m_LastLoadedScene);
    //#endif

        yield return m_SceneLoading = -1;
    }

    // It should be called as a co-routine.
    private void LoadSceneAsync(int index, bool additive)
    {
        this.StartCoroutine(LoadSceneCoroutine(index, additive));

        // NOTE: The following method is preferred, but works on PRO version only :(
        //( additive ) ? Application.LoadLevelAdditiveAsync( index ) : Application.LoadLevelAsync( index );
    }

    // Load a new level by index and add it to the current scene.
    public static bool LoadSceneByIndex(int index, bool additive, bool blocking)
    {
        if (   (Application.isLoadingLevel == false)            // The application is still loading
            && (Application.CanStreamedLevelBeLoaded(index))    // The level cannot be loaded
            && (SceneManager.SceneLoading < 0)                  // The scene manager is still loading
            && (SceneManager.IsSceneValid(index))               // Not a valid index
            && (SceneManager.IsSceneLoaded(index) == false))    // Scene already loaded
        {
            SceneManager.Instance.m_SceneLoading = index;
            if (blocking)
            {
                SceneManager.Instance.LoadSceneSync(index, additive);
            }
            else
            {
                SceneManager.Instance.LoadSceneAsync(index, additive);
            }

            ++SceneManager.Instance.m_NumberOfLoadedScenes;

        #if UNITY_EDITOR
            Debug.Log(
                  "LoadSceneByIndex(" + index + ")"
                + "\nApplication.isLoadingLevel: " + Application.isLoadingLevel
                + "\nApplication.CanStreamedLevelBeLoaded(index): " + Application.CanStreamedLevelBeLoaded(index)
                + "\nSceneManager.SceneLoading < 0: " + SceneManager.SceneLoading
                + "\nSceneManager.IsSceneValid(index): " + SceneManager.IsSceneValid(index)
                + "\nSceneManager.IsSceneLoaded(index): " + SceneManager.IsSceneLoaded(index)
            );
        #endif

            SceneManager.DestroyTermporaries();
            return true;
        }

        return false;
    }

    // Load the next scene. Return the index of loading level.
    // If returns a negative number, then, is not possibile to load the next level.
    public static int LoadNextScene(bool additive, bool blocking)
    {
        // Determine the next level index.
        int nextLevelIndex = -1;

        // If the current one is not the last one.
        if ((SceneManager.CurrentScene < (SceneManager.TotalLevels - 1))
            && (SceneManager.NumberOfLoadedScenes < SceneManager.TotalLevels))
        {
    #if UNITY_EDITOR
        Debug.Log("LoadNextScene()"
            + "\nSceneManager.CurrentScene: " + SceneManager.CurrentScene
            + "\nIndexToLoad: " + (SceneManager.CurrentScene + 1)
        );
    #endif
            // Load next scene.
            if (SceneManager.LoadSceneByIndex(
                SceneManager.CurrentScene + 1, additive, blocking))
            {
                nextLevelIndex = SceneManager.CurrentScene + 1;
            }
        }

        return nextLevelIndex;
    }

    // Load the previous scene. Return the index of loading level.
    // If returns a negative number, then, is not possibile to load the previous level.
    public static int LoadPreviousScene(bool additive, bool blocking)
    {
        // Determine previous level index.
        int prevLevelIndex = -1;

        // If the current one is greater than one.
        if (    (SceneManager.CurrentScene >= 0)
            && (SceneManager.NumberOfLoadedScenes < SceneManager.TotalLevels))
        {
        #if UNITY_EDITOR
            Debug.Log("LoadPreviousScene()"
                + "\nSceneManager.CurrentScene: " + SceneManager.CurrentScene
                + "\nIndexToLoad: " + (SceneManager.CurrentScene - 1)
            );
        #endif
            // Load previous index.
            if (SceneManager.LoadSceneByIndex(
                SceneManager.CurrentScene - 1, additive, blocking))
            {
                prevLevelIndex = SceneManager.CurrentScene - 1;
            }
        }

        return prevLevelIndex;
    }

    // Destroy a given scene by index.
    // Return true if the scene was loaded and now has been destroyed successfully.
    public static bool UnloadSceneByIndex(int index)
    {
        if ((index >= 0) && (index < SceneManager.Roots.Length) && (SceneManager.NumberOfLoadedScenes > 0))
        {
            SceneRoot root = SceneManager.Roots[index];
            if (root)
            {
                Object.Destroy(SceneManager.Roots[index].TreeRoot);
                SceneManager.Roots[index] = null;

            #if UNITY_EDITOR
                Debug.Log("UnloadSceneByIndex(" + index + ")");
            #endif

                --SceneManager.Instance.m_NumberOfLoadedScenes;
                return true;
            }
        }

        return false;
    }

    // Set the loaded scene. It means we have loaded
    // a scene without going throug the SceneManager.
    public static void SetLoadedSceneByIndex(int index)
    {
        if (SceneManager.IsSceneLoaded(index) == false)
        {
            ++SceneManager.Instance.m_NumberOfLoadedScenes;
            SceneManager.Instance.m_LastLoadedScene = index;
            SceneManager.DestroyTermporaries();
        }
    }

    // Destroy all temporary objects.
    public static void DestroyTermporaries()
    {
#if UNITY_EDITOR
        GameObject[] temporaries = GameObject.FindGameObjectsWithTag("Temporary");
        foreach (GameObject temp in temporaries)
        {
            GameObject.DestroyImmediate(temp);
        }
#endif
    }

    // Return true if the index is a valid scene, false otherwise.
    public static bool IsSceneValid(int index)
    {
        return ((index >= 0) && (index < SceneManager.TotalLevels)) ? true : false;
    }

    // Return true if the scene is loaded, false otherwise.
    public static bool IsSceneLoaded(int index)
    {
        return (SceneManager.IsSceneValid(index) && (SceneManager.Roots[index] != null)) ? true : false;
    }

}
