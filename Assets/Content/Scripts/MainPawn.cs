using UnityEngine;
using System.Collections;

public class MainPawn : Pawn
{
	private bool	m_IsDying;
	private bool 	m_IsSpawning;

	public 	float	spawnRate;
	public 	float	dieRate;
	
	// Use this when the level is loaded
	protected override void Awake()
	{
		base.Awake();
		Spawn();
	}

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();		
		transform.localScale = new Vector3(
			m_Size, transform.localScale.y, m_Size );
	}
	
	// Move the actor
	private void MoveActor()
	{	
		Vector3 movement = new Vector3( 0.0f, 0.0f, 0.0f );
		
		/*
		// NOTE: We compute the position we want to reach
		//		 from the ray we spawn from the camera.
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
       	if ( Physics.Raycast(ray, out hit, Camera.main.far) )
		{
			// Calculate the movement due to the stationary position
			// of the finger, which the actor tends to approach.
			Vector3 reachPoint = new Vector3( hit.point.x, transform.localPosition.y, hit.point.z );
			movement = (reachPoint - transform.localPosition) * approachSpeed;
		}
		*/
		
		// NOTE: Alternative method for computing the reach point.
		//		 The advantage here is that we don't need a base to hit.
		{			
			// Calculate the movement due to the stationary position
			// of the finger, which the actor tends to approach.
			Vector3 reachPoint = Camera.main.ScreenToWorldPoint(
				new Vector3( Input.mousePosition.x, Input.mousePosition.y, Camera.main.far ) );
			
			Vector3 approachMov = new Vector3(reachPoint.x,transform.localPosition.y,reachPoint.z);
			movement = (approachMov - transform.localPosition) * approachSpeed * ( Time.fixedDeltaTime * 100.0f );
		}

		// Move the actor.
		rigidbody.MovePosition( transform.localPosition + movement );
	}
	
	// Update physics
	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		// If the actor is not spawning or dying,
		// then we can, keep track of its movement.
		if ( !m_IsSpawning && !m_IsDying )
		{
			MoveActor();
		}
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		
		// Reset states
		m_IsSpawning	= false;
		m_IsDying		= false;
		
		// We keep active the pawn until the mouse button is pressed
		if ( Input.GetMouseButton(0) )
		{
			// Until we reach the 100% in size we keep resizing
			if ( m_Size < finalSize )
			{
				m_Size += spawnRate;
				
				// The transform component is always present, therefore,
				// we don't need to check whether or not it exists.
				transform.localScale = new Vector3( m_Size, transform.localScale.y, m_Size );
				
				if ( m_Size < finalSize )
				{
					m_IsSpawning = true;
				}
			}
		}
		else
		{
			// Resizing the actor until it will desappear
			if ( m_Size > 0.0f )
			{
				m_Size -= dieRate;
				transform.localScale = new Vector3( m_Size, transform.localScale.y, m_Size );
				
				m_IsDying = true;
				
				// Kill the actor if necesasry
				if ( m_Size <= 0.0f )
				{
					SpawnManager.Ref.DestoryPawn( this.gameObject );
					m_IsDying = false;
				}
			}
		}
	}
	
	// Spawn the actor, and initialise the size.
	public override void Spawn()
	{
		base.Spawn();
		
		m_IsSpawning	= false;
		m_IsDying		= false;
		m_Size			= 0.0f;
	}
	
	// Kill the actor
	public override void Kill()
	{
		base.Kill();
	}
	
	public bool IsDying
	{
		get { return m_IsDying; }
	}
	
	public bool IsSpawning
	{
		get { return m_IsSpawning; }
	}
}
