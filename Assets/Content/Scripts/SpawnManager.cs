using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
	public GameObject			Actor;
	static public SpawnManager	Ref;
	
	private GameObject			MainActor;
	
	// Use this when the level is loaded
	void Awake()
	{
		MainActor	= null;
		Ref			= this;

		DontDestroyOnLoad(this);
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		// PERF:
		// 	I don't know much about instantiating performance,
		// 	but I guess is quite expensive, therefore we spawn
		// 	the main pawn out of the player view.
	
		if ( Input.GetMouseButtonDown(0) )
		{
			// Destory the main actor if exists
			DestoryPawn( MainActor );
			
			bool createActor = false;
			Vector3 hitPoint = new Vector3( 0.0f, 0.0f, 0.0f );
			
			// If the camera is orthografic, calculation can be easier.
			if ( Camera.main.isOrthoGraphic )
			{
				hitPoint = Camera.main.ScreenToWorldPoint(
					new Vector3( Input.mousePosition.x, Input.mousePosition.y, Camera.main.far ) );
				
				hitPoint.y = Actor.transform.localPosition.y;
				createActor = true;
			}
			else
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	            RaycastHit[] hits = Physics.RaycastAll (ray, Camera.main.far);
	            foreach (RaycastHit hit in hits)
				{
	                if (hit.transform.tag == "Base Surface")
	                {
						// Instantiate the new actor
						hitPoint = new Vector3( hit.point.x, Actor.transform.localPosition.y, hit.point.z );
						createActor = true;
						break;
					}
	            }
			}
			
			if ( createActor )
			{
				MainActor = (GameObject)Instantiate(Actor, hitPoint, Quaternion.identity );
			}
		}
	}
	
	// Destroy the main actor
	public void DestoryPawn( GameObject gameObject )
	{
		// If an other actor already exists, then, we need to destory it.
		if ( gameObject )
		{
			Pawn pawn = (Pawn)gameObject.GetComponent<Pawn>();
			if ( pawn )
			{
				pawn.Kill();
			}

			Object.Destroy( gameObject );
			if ( gameObject == MainActor )
			{
				MainActor = null;
			}
		}
	}
}
