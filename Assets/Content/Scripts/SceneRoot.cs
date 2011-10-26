using UnityEngine;
using System.Collections;

public class SceneRoot : MonoBehaviour
{
	// This is the destroyable game object root.
	// Every level must have one and one only this object.
	// All objects attached to this one will
	// be destroyed once the level is unloaded.
	private GameObject	m_Root;
	private int			m_SceneIndex;
	private string		m_SceneName;

	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	protected virtual void Awake()
	{
		// The root is the game object itself.
		m_Root = gameObject;

		// Once we load this level, we add this root to the level manager.
		if ( SceneManager.Roots[Application.loadedLevel] == null )
		{
			this.m_SceneIndex	= Application.loadedLevel;
			this.m_SceneName	= Application.loadedLevelName;
			
			SceneManager.Roots[this.m_SceneIndex] = this;
		}
		else
		{
			// We shouldn't be able to load a level which is already loaded.
			Debug.LogError( "Level " + Application.loadedLevelName + " already loaded!" );
		}
	}

	// Use this for initialization
	protected virtual void Start ()
	{
	}
	
	// Update physics
	protected virtual void FixedUpdate()
	{
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{	
	}
	
	/// <summary>
	/// Non-Unity functions.
	/// </summary>
	
	// Get the root game object
	public GameObject Root
	{
		get { return m_Root; }
	}
	
	// Get scene name
	public string SceneName
	{
		get { return m_SceneName; }
	}
}
