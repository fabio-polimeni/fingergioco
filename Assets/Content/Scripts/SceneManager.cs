using UnityEngine;
using System.Collections;
using System.Threading;

public class SceneManager : MonoBehaviour
{	
	// An array of loaded level trees.
	// A level tree is a tree graph of
	// destroyable game objects in the level.
	private SceneRoot[]	m_Roots;
	public static SceneRoot[] Roots
	{
		get { return SceneManager.Instance.m_Roots; }
		set { SceneManager.Instance.m_Roots = value; }
	}

    // Since we can move just along the z-axis we store the current level index, and, at the
    // same time, assume the previous and the next ones are available, if they make sense.
    // They might are not available in case the current level index is zero, or is equel to
    // the last available scene in the game.
    //private int m_CurrentScene;
    public static int CurrentScene
    {
        get
        {
            // Calculate the scene the camera is on.
            return (int)(Camera.main.transform.position.z / GameSettings.BaseSurfaceExtent);
        }
    }

    // Last loaded scene index
	private int	m_LastLoadedScene;
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
	
	// Singleton pattern.
	private static SceneManager m_Instance = null;
	public static SceneManager Instance
	{
		get
		{
			if ( m_Instance == null )
			{
				GameObject go = new GameObject();
				m_Instance = go.AddComponent< SceneManager >();
				go.name = "SceneManager";
				
				// Instantiate level roots array.
				// Level roots are not initialised at this point.
				m_Instance.m_Roots = new SceneRoot[Application.levelCount];
				
				// Nothing is loading at the beginning
				m_Instance.m_SceneLoading = -1;
			}

			return m_Instance;
		}
	}
	
	// Use this when the level is loaded
	void Awake()
	{
		// Do not destroy the scene manager on loading.
		Object.DontDestroyOnLoad( this );
		
		// Set the current scene index.
		m_LastLoadedScene = Application.loadedLevel;
	}

	// Use this for initialization
	void Start()
	{
		SceneManager.LoadNextScene(
			GameSettings.AdditiveSceneLoading,
			!GameSettings.AsyncSceneLoading );
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	/// <summary>
	/// Non-Unity functions.
	/// </summary>
	
	// Load a new level.
	private void LoadSceneSync( int index, bool additive )
	{
		if ( additive )
		{
			Application.LoadLevelAdditive( index );
		}
		else
		{
			Application.LoadLevel( index );
		}

		m_LastLoadedScene = index;
		m_SceneLoading = -1;

#if UNITY_EDITOR
		Debug.Log("Scene loaded: " + this.m_LastLoadedScene );
#endif
	}
	
	// Load a new level.
	private IEnumerator LoadSceneCoroutine( int index, bool additive )
	{
		if ( additive )
		{
			Application.LoadLevelAdditive( index );
		}
		else
		{
			Application.LoadLevel( index );
		}
		
		m_LastLoadedScene = index;

#if UNITY_EDITOR
		Debug.Log("Scene loaded: " + this.m_LastLoadedScene);
#endif

		yield return m_SceneLoading = -1;
	}
	
	// It should be called as a co-routine.
	private void LoadSceneAsync( int index, bool additive )
	{
		this.StartCoroutine( LoadSceneCoroutine( index, additive ) );
		
		// NOTE: The following method is preferred, but works on PRO version only :(
		//( additive ) ? Application.LoadLevelAdditiveAsync( index ) : Application.LoadLevelAsync( index );
	}
	
	// Load a new level by index and add it to the current scene.
	public static bool LoadSceneByIndex( int index, bool additive, bool blocking )
	{
		if ( 	( Application.isLoadingLevel == false )             // The application is still loading something else
			&&	( Application.CanStreamedLevelBeLoaded( index ) )   // The level cannot be loaded
			&&	( SceneManager.SceneLoading < 0 )                   // The scene manager is still loading
			&& 	( SceneManager.IsSceneValid( index ) )              // Not a valid index
			&& 	( SceneManager.IsSceneLoaded( index ) == false) )   // Scene already loaded
		{
			SceneManager.Instance.m_SceneLoading = index;
			if ( blocking )
			{
				SceneManager.Instance.LoadSceneSync( index, additive );
			}
			else
			{
				SceneManager.Instance.LoadSceneAsync( index, additive );
			}
			
			return true;
		}
		
		return false;
	}
	
	// Load the next scene. Return the index of loading level.
	// If returns a negative number, then, is not possibile to load the next level.
	public static int LoadNextScene( bool additive, bool blocking )
	{
		// Determine the next level index.
		int nextLevelIndex = -1;
		
		// If the current one is not the last one.
        if ( SceneManager.CurrentScene < (Application.levelCount - 1) )
		{
			// Load next scene.
			if ( SceneManager.LoadSceneByIndex(
				SceneManager.CurrentScene+1, additive, blocking ) )
			{
                nextLevelIndex = SceneManager.CurrentScene + 1;
			}
		}
		
		return nextLevelIndex;
	}
	
	// Load the previous scene. Return the index of loading level.
	// If returns a negative number, then, is not possibile to load the previous level.
	public static int LoadPreviousScene( bool additive, bool blocking )
	{
		// Determine previous level index.
		int prevLevelIndex = -1;
		
		// If the current one is greater than one.
        if ( SceneManager.CurrentScene > 0 )
		{
			// Load previous index.
			if ( SceneManager.LoadSceneByIndex(
                SceneManager.CurrentScene - 1, additive, blocking)  )
			{
                prevLevelIndex = SceneManager.CurrentScene- 1;
			}
		}
		
		return prevLevelIndex;
	}

	// Destroy a given scene by index.
	// Return true if the scene was loaded and now has been destroyed successfully.
	public static bool DestroySceneByIndex( int index )
	{
		if ( index > 0 && index < SceneManager.Roots.Length )
		{
			SceneRoot root = SceneManager.Roots[index];
			if ( root )
			{
				Object.Destroy( SceneManager.Roots[index].TreeRoot );
				SceneManager.Roots[index] = null;

            #if UNITY_EDITOR
                Debug.Log("Scene loaded: " + index);
            #endif

				return true;
			}
		}

		return false;
	}
	
	// Return true if the index is a valid scene, false otherwise.
	public static bool IsSceneValid( int index )
	{
		return ( (index > 0) && (index < Application.levelCount) ) ? true : false;
	}
	
	// Return true if the scene is loaded, false otherwise.
	public static bool IsSceneLoaded( int index )
	{
		return ( SceneManager.IsSceneValid( index ) && (SceneManager.Roots[index] != null) ) ? true : false;
	}

}
