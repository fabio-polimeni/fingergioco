using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{	
	// An array of loaded level trees.
	// A level tree is a tree graph of
	// destroyable game objects in the level.
	public SceneRoot[]	roots;

	// Since we can move just along the z-axis
	// we store the current level index, and,
	// at the same time, assume the previous and
	// the next ones are available, if they make sense.
	// They might are not available in case the current
	// level index is zero, or is equel to the last
	// available scene in the game.
	private int	m_CurrentSceneIndex;
	
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
			}

			return m_Instance;
		}
	}
	
	// Use this when the level is loaded
	void Awake()
	{
		// Do not destroy the scene manager on loading.
		Object.DontDestroyOnLoad( this );
		
		// Instantiate level roots array.
		// Level roots are not initialised yet.
		roots = new SceneRoot[Application.levelCount];
		
		// Set the current scene index.
		m_CurrentSceneIndex = Application.loadedLevel;
	}

	// Use this for initialization
	void Start ()
	{
		this.LoadNextScene( false, false );
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	/// <summary>
	/// Non-Unity functions.
	/// </summary>
	
	// Load a new level.
	private IEnumerator LoadSceneSync( int index, bool additive )
	{
		if ( additive )
		{
			Application.LoadLevelAdditive( index );
		}
		else
		{
			Application.LoadLevel( index );
		}
		
		return null;
	}
	
	// It should be called as a co-routine.
	private IEnumerator LoadSceneAsync( int index, bool additive )
	{
		yield return LoadSceneSync( index, additive );
		
		// NOTE: The following method is preferred, but works on PRO version only :(
		//yield return  ( additive ) ? Application.LoadLevelAdditiveAsync( index ) : Application.LoadLevelAsync( index );
	}
	
	// Load a new level by index and add it to the current scene.
	private bool LoadSceneByIndex( int index, bool additive, bool blocking )
	{
		if ( 	( Application.isLoadingLevel == false )
		    &&	( Application.CanStreamedLevelBeLoaded( index ) )
		    && 	( this.IsSceneValid( index ) )
		    && 	( this.IsSceneLoaded( index ) == false) )
		{
			if ( blocking )
			{
				LoadSceneSync( index, additive );
			}
			else
			{
				StartCoroutine( LoadSceneAsync( index, additive ) );
			}
			
			return true;
		}
		
		return false;
	}
	
	// Load the next scene.
	// Returns the index of loading level.
	// If returns a negative number, then,
	// is not possibile to loat the next level.
	public int LoadNextScene( bool additive, bool blocking )
	{
		// Determine the next level index.
		int nextLevelIndex = -1;
		
		// If the current one is not the last one.
		if ( m_CurrentSceneIndex < (Application.levelCount-1) )
		{
			// Load next scene.
			if ( LoadSceneByIndex( m_CurrentSceneIndex+1, additive, blocking ) )
			{
				nextLevelIndex = m_CurrentSceneIndex+1;
			}
		}
		
		return nextLevelIndex;
	}
	
	// Load the previous scene.
	// Returns the index of loading level.
	// If returns a negative number, then,
	// is not possibile to loat the previous level.
	public void LoadPreviousScene( bool additive, bool blocking )
	{
	}
	
	// Return true if the index is a valid scene, false otherwise.
	public bool IsSceneValid( int index )
	{
		return ( (index > 0) && (index < Application.levelCount) ) ? true : false;
	}
	
	// Return true if the scene is loaded, false otherwise.
	public bool IsSceneLoaded( int index )
	{
		return ( this.IsSceneValid( index ) && (roots[index] != null) ) ? true : false;
	}
}
