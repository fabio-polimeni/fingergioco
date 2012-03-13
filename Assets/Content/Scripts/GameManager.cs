using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject FingerPrefab;

    private GameObject m_Finger;
    private Finger m_FingerComponent;
	
    public static Finger FingerComponent
    {
        get { return GameManager.Instance.m_FingerComponent; }
    }

    public static GameObject Finger
    {
        get { return GameManager.Instance.m_Finger; }
        set
        {
            GameManager.Instance.m_Finger = value;
            if (GameManager.Instance.m_Finger)
            {
                GameManager.Instance.m_FingerComponent = GameManager.Instance.m_Finger.GetComponent<Finger>();
            }
            else
            {
                GameManager.Instance.m_FingerComponent = null;
            }
        }
    }
	
	// Difficult level must be accessible from everywhere
	private float 		m_DifficultyLastTime;
	private float 		m_Difficulty;
	public static float	Difficulty
	{
		get { return GameManager.Instance.m_Difficulty; }
	}
	
	// Handle global score
	private float		m_ScoreLastTime;
	private float		m_Score;
    public static float	Score
    {
        get { return GameManager.Instance.m_Score; }
        set { GameManager.Instance.m_Score = value; }
    }
	
    // Singleton pattern
    private static GameManager m_Instance = null;
    public static GameManager Instance  { get { return m_Instance; } }

    // Use this when the level is loaded
    void Awake()
    {
        m_Instance = this;
        m_Finger = null;
        m_FingerComponent = null;
		
		// Clamp the score.
		m_Score = 0.0f;
		m_ScoreLastTime = 0.0f;
		
		// Difficulty initialisation
		m_Difficulty = 1.0f;
		m_DifficultyLastTime = 0.0f;
		
		// Store initial time
		m_ScoreLastTime = m_DifficultyLastTime = Time.time;
		
		//SceneManager.LoadSceneByIndex(0, GameSettings.AdditiveSceneLoading, !GameSettings.AsyncSceneLoading);

        Object.DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		float currentTime = Time.time;
        if (Input.GetMouseButtonDown(0))
        {
            // Destroy the main actor if exists
            if (m_Finger && m_FingerComponent)
            {
				m_Score = 0.0f;
                m_FingerComponent.Kill();
            }

            bool createActor = false;
            Vector3 hitPoint = new Vector3(0.0f, 0.0f, 0.0f);

            // If the camera is orthographic, calculation can be easier.
            if (SceneManager.CurrentCamera.isOrthoGraphic)
            {
                hitPoint = SceneManager.CurrentCamera.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, SceneManager.CurrentCamera.far));

                hitPoint.y = FingerPrefab.transform.localPosition.y;
                createActor = true;
            }
            else
            {
                Ray ray = SceneManager.CurrentCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, SceneManager.CurrentCamera.far);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.tag == "Base Surface")
                    {
                        // Determine the new spawned location.
                        hitPoint = new Vector3(hit.point.x, FingerPrefab.transform.localPosition.y, hit.point.z);
                        createActor = true;
                        break;
                    }
                }
            }

            if (createActor)
            {
                // Instantiate the new actor
                m_Finger = (GameObject)Object.Instantiate(FingerPrefab, hitPoint, Quaternion.identity);
                m_FingerComponent = m_Finger.GetComponent<Finger>();

            #if UNITY_EDITOR
                if (!m_FingerComponent)
                {
                    Debug.LogWarning("The game object " + FingerPrefab.name + " needs a Finger script component attached to it!");
                }
            #endif
            }
        }
        else if (m_Finger && m_FingerComponent && m_FingerComponent.IsActive)
        {
            // Move the camera along z-axis only.
            if (Input.GetMouseButton(0))
            {
				// Compute level extents, x->bottom, y->top
				Vector2 levelExtents = new Vector2(
					(SceneManager.CurrentScene * GameSettings.BaseSurfaceExtent * 2.0f) - GameSettings.BaseSurfaceExtent,
					(SceneManager.CurrentScene * GameSettings.BaseSurfaceExtent * 2.0f) + GameSettings.BaseSurfaceExtent );
				
				// Compute camera extents, x->bottom, y->top
				Vector2 cameraExtents = new Vector2(
					SceneManager.CurrentCamera.transform.position.z - (SceneManager.CurrentCamera.orthographicSize),
					SceneManager.CurrentCamera.transform.position.z + (SceneManager.CurrentCamera.orthographicSize));
				
				// Get the new camera translation from the finger position.
                Vector3 cameraTranslation = new Vector3(0.0f, 0.0f, (m_Finger.transform.localPosition.z - SceneManager.CurrentCamera.transform.position.z));
				
				// Tranaslate camera
				SceneManager.CurrentCamera.transform.position += cameraTranslation * GameSettings.CameraSpeed;
				
				int levelIndex = SceneManager.SceneIndex;
				if ( levelIndex < SceneManager.TotalLevels && levelIndex > 0 )
				{
					SceneRoot currentScene = SceneManager.Roots[levelIndex];
					if ( currentScene )
					{
						// Store information into the rootscene.
						if ( ( currentScene.Entered == false ) && ( levelExtents.x < cameraExtents.x ))
						{
							currentScene.Entered = true;
						}
						
						// We apply any lmitation only if the camera was entirely inside a level previously.
						if ( currentScene.Entered )
						{
							// Limit camera on the -z axis.
							if ( cameraTranslation.z < 0.0f )
							{					
								// Compute the centre of the current level.
								float bottomOfTheLevel = (SceneManager.CurrentScene * GameSettings.BaseSurfaceExtent * 2.0f) - GameSettings.BaseSurfaceExtent;
								float bottomOfTheCamera = SceneManager.CurrentCamera.transform.position.z - (SceneManager.CurrentCamera.orthographicSize);
								if ( bottomOfTheCamera < bottomOfTheLevel )
								{
									Vector3 cameraCorrection = new Vector3(0.0f,0.0f,bottomOfTheCamera - bottomOfTheLevel);
									SceneManager.CurrentCamera.transform.position -= cameraCorrection;
								}
							}
						}
					}
				}
				
				// Load next scene if necessary.
				SceneManager.LoadNextScene(GameSettings.AdditiveSceneLoading, !GameSettings.AsyncSceneLoading);
            }
			
			// Increment the score
//			if ( (currentTime - m_ScoreLastTime) >= GameSettings.ScoreDeltaTime )
//			{
//				m_Score += GameSettings.ScoreIncRate;
//				m_ScoreLastTime = currentTime;
//			}
			
			// Increment the difficulty
			if ( (currentTime - m_DifficultyLastTime) >= GameSettings.DifficultyDeltaTime )
			{
				m_Difficulty += GameSettings.DifficultyStep;
				m_DifficultyLastTime = currentTime;
			}
        }
    }
	
	// Gui
	void OnGUI()
	{
		if ( m_Finger && m_FingerComponent )
		{
			// Score
			string scoreString = "Score: " + m_Score.ToString("F2");
			GUI.Label(new Rect(10, 10, 10 * scoreString.Length, 20), scoreString);
			
			// Energy
			string energyString = "Energy: " + m_FingerComponent.Energy.ToString("P1");
			GUI.Label(new Rect(10, 25, 10*energyString.Length, 20), energyString);
			
			// Difficulty
			string difficultyString = "Difficulty: " + m_Difficulty.ToString("F2");
			GUI.Label(new Rect(10, 40, 10 * difficultyString.Length, 20), difficultyString);
		}
	}
}
