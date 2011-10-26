using UnityEngine;
using System.Collections;

// GameSettings class is just a collection of read-only
// static variables which can be set from the editor.
public class GameSettings : MonoBehaviour
{
	// We use this pattern, that is, values can be
	// accessible from the editor, but readonly from
	// the game code. Moreover, we don't need to
	// invoke the instance in order to access the property.
	public float			m_ScrollSceneSpeed		= 1.0f;
	public static	float 	ScrollSceneSpeed { get { return GameSettings.Instance.m_ScrollSceneSpeed; } }
	
	public float			m_BaseSurfaceExtent		= 0.5f;
	public static	float 	BaseSurfaceExtent { get { return GameSettings.Instance.m_BaseSurfaceExtent; } }
	
	public bool				m_AsyncSceneLoading		= true;
	public static bool 		AsyncSceneLoading { get { return GameSettings.Instance.m_AsyncSceneLoading; } }
	
	public bool				m_AdditiveSceneLoading	= true;
	public static bool 		AdditiveSceneLoading { get { return GameSettings.Instance.m_AdditiveSceneLoading; } }
	
	public int				m_MaxNumberOfEnemies	= 10;
	public static int 		MaxNumberOfEnemies { get { return GameSettings.Instance.m_MaxNumberOfEnemies; } }

	// Singleton pattern.
	private static GameSettings m_Instance = null;
	public static GameSettings Instance { get { return m_Instance; } }

	// Use this when the level is loaded
	void Awake()
	{
		m_Instance	= this;
		Object.DontDestroyOnLoad( this );
		
		// Avoid any stupid error ¬_¬
		m_MaxNumberOfEnemies = Mathf.Min( 0, m_MaxNumberOfEnemies );
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
