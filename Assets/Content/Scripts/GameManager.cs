using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public	GameObject	PlayerPawn;	
	private	GameObject	m_Finger;
    private Finger      m_FingerComponent;

	// Singleton pattern.
	private static GameManager m_Instance = null;
	public static GameManager Instance { get { return m_Instance; } }

	// Use this when the level is loaded
	void Awake()
	{
		m_Finger	= null;
		m_Instance	= this;
		Object.DontDestroyOnLoad( this );
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetMouseButtonDown(0))
        {
            // Destroy the main actor if exists
            DestoryPawn(m_Finger);

            bool createActor = false;
            Vector3 hitPoint = new Vector3(0.0f, 0.0f, 0.0f);

            // If the camera is orthographic, calculation can be easier.
            if (Camera.main.isOrthoGraphic)
            {
                hitPoint = Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.far));

                hitPoint.y = PlayerPawn.transform.localPosition.y;
                createActor = true;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, Camera.main.far);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.tag == "Base Surface")
                    {
                        // Determine the new spawned location.
                        hitPoint = new Vector3(hit.point.x, PlayerPawn.transform.localPosition.y, hit.point.z);
                        createActor = true;
                        break;
                    }
                }
            }

            if (createActor)
            {
                // Instantiate the new actor.
                m_Finger = (GameObject)Object.Instantiate(PlayerPawn, hitPoint, Quaternion.identity);
                m_FingerComponent = m_Finger.GetComponent<Finger>();

            #if UNITY_EDITOR
                if (!m_FingerComponent)
                {
                    Debug.LogWarning("The game object " + PlayerPawn.name + " needs a Finger script component attached to it!" );
                }
            #endif
            }
        }
        // We move the camera only if the pawn is not spawning.
        else if (m_FingerComponent && m_FingerComponent.IsActive )
        {
            // Move the camera along z-axis only.
            if (Input.GetMouseButton(0))
            {
                Vector3 cameraTranslation = new Vector3(0.0f, 0.0f, Input.GetAxis("Mouse Y") * GameSettings.CameraSpeed);
                Camera.main.transform.localPosition += cameraTranslation;
            }
        }
	}
	
	// Destroy the main actor
	public void DestoryPawn( GameObject gameObject )
	{
		// If an other actor already exists, then, we need to destroy it.
		if ( gameObject )
		{
			Pawn pawn = (Pawn)gameObject.GetComponent<Pawn>();
			if ( pawn )
			{
				pawn.Kill();
			}

			Object.Destroy( gameObject );
			if ( gameObject == m_Finger )
			{
				m_Finger = null;
			}
		}
	}
}
