using UnityEngine;
using System.Collections;

public class SceneRoot : MonoBehaviour
{
    // Scene index. This should respect the build scene order.
    public int SceneIndex;

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
        // If SceneManager.LastLoadedScene is < 0, then,
        // this is the first root to be initialised.
        if (SceneManager.LastLoadedScene < 0)
        {
            // NOTE: This code should never get triggered in
            // realse build, because we need to load everything
            // from the main menu.
            SceneManager.SetLoadedSceneByIndex(this.SceneIndex);

            // Move the camera to the centre of the new loaded scene.
            Vector3 newCameraPosition = new Vector3(
                SceneManager.CurrentCamera.transform.position.x,
                SceneManager.CurrentCamera.transform.position.y,
                GameSettings.BaseSurfaceExtent * 2.0f * (float)this.SceneIndex);

            SceneManager.CurrentCamera.transform.position = newCameraPosition;
        }
        
        // Once we load this level, we add the root to the level manager.
        if (SceneManager.Roots[SceneManager.LastLoadedScene] == null)
        {
            this.SceneIndex = SceneManager.LastLoadedScene;
            SceneManager.Roots[this.SceneIndex] = this;

            // Need to determine where to load the scene in the space.
            float offset = GameSettings.BaseSurfaceExtent * 2.0f;
            this.transform.localPosition += new Vector3(0.0f, 0.0f, offset * (float)this.SceneIndex);

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
