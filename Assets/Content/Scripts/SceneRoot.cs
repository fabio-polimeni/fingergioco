using UnityEngine;
using System.Collections;

public class SceneRoot : MonoBehaviour
{
	// This is the destroyable game object root.
	// Every level must have one and one only this object.
	// All objects attached to this one will
	// be destroyed once the level is unloaded.
	private GameObject  m_TreeRoot;
	public GameObject   TreeRoot { get { return m_TreeRoot; } }

	private int			m_SceneIndex;

	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	protected virtual void Awake()
	{
		// The root is the game object itself.
		m_TreeRoot = gameObject;

		// Once we load this level, we add the root to the level manager.
		if ( SceneManager.Roots[SceneManager.LastLoadedScene] == null )
		{
			this.m_SceneIndex = SceneManager.LastLoadedScene;
			SceneManager.Roots[this.m_SceneIndex] = this;
			
		#if UNITY_EDITOR
			Debug.Log( "Scene registered: " + this.m_SceneIndex );
		#endif
		}
	#if UNITY_EDITOR
		else
		{
			// We shouldn't be able to load a level which is already loaded.
			Debug.LogError( "Level " + this.m_SceneIndex + " already loaded!" );
		}
	#endif
	}

	// Use this for initialization
	protected virtual void Start()
	{
	}
	
	// Update physics
	protected virtual void FixedUpdate()
	{
	}
	
	// Update is called once per frame
	protected virtual void Update()
	{	
	}
	
	/// <summary>
	/// Non-Unity functions.
	/// </summary>
	
}
