using UnityEngine;
using System.Collections;

public class SceneRoot : MonoBehaviour
{
    // Scene index. This should respect the build scene order.
    public int SceneIndex;
	
	// Indicate whether or not the level is entered by the camera entirely..
	public bool Entered;
	
	// Store the scene loop the level has been loaded from.
	public int RegisteredLoop;

    // This is the destroyable game object root.
    // Every level must have one and one only this object.
    // All objects attached to this one will
    // be destroyed once the level is unloaded.
    public GameObject TreeRoot { get { return gameObject; } }

    /// <summary>
    /// Unity functions.
    /// </summary>

    // Use this when the level is loaded
    protected virtual void Awake()
    {
		// If a valid instance of the SceneManager doesn't not
		// exist, then, this is the first time we load a scene.
		if ( !SceneManager.IsInitialised )
		{
			SceneManager.SetLoadedSceneByIndex(0);
		}

        // Once we load this level, we add the root to the level manager.
        if (SceneManager.Roots[SceneManager.LastLoadedScene] == null)
        {
			this.Entered = false;
            this.SceneIndex = SceneManager.LastLoadedScene;
			this.RegisteredLoop = SceneManager.LoopCounter;
			
            SceneManager.Roots[this.SceneIndex] = this;

            // Need to determine where to load the scene in the space.
            float offset = GameSettings.BaseSurfaceExtent * 2.0f;
            this.transform.localPosition += new Vector3(0.0f, 0.0f, offset * (float)(this.SceneIndex + SceneManager.LoopCounter*SceneManager.TotalLevels));

        #if UNITY_EDITOR
            Debug.Log("Scene registered: " + this.SceneIndex);
        #endif
        }
    #if UNITY_EDITOR
        else
        {
            // We shouldn't be able to load a level which is already loaded.
            Debug.LogError("Level " + this.SceneIndex + " already loaded!");
        }
    #endif
    }

    // Use this for initialization
    protected virtual void Start()
    {
    //#if UNITY_EDITOR
    //    Debug.Log("SceneRoot.m_SceneIntex: " + this.m_SceneIndex
    //              + "\nSceneManager.CurrentScene: " + SceneManager.CurrentScene );
    //#endif

        //// Need to determine where to create the scene in the space.
        //// We need to do it here, because GameSettings must be available.
        //int sceneDiff = (this.m_SceneIndex - SceneManager.CurrentScene);
        //if (sceneDiff != 0)
        //{
        //    float offset = GameSettings.BaseSurfaceExtent * 2.0f;
        //    this.transform.localPosition += new Vector3(0.0f, 0.0f, offset * (float)this.m_SceneIndex);
        //}
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
