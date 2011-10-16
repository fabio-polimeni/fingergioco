using UnityEngine;
using System.Collections;

public class MainPawn : Pawn
{
	private bool	m_IsDying;
	private bool 	m_IsSpawning;
	private bool	m_IsActive;

	public 	float	spawnRate;
	public 	float	dieRate;
	public	float	rotationRate;
	
	// Use this when the level is loaded
	protected override void Awake()
	{
		base.Awake();
		Spawn();
	}

	// Use this for initialization
	protected override void Start()
	{
		base.Start();
	}
	
	// Calculate the approaching point
	private Vector3 ApproachPoint()
	{
		// Declare the deisplacement vector
		Vector3 movement = new Vector3( 0.0f, 0.0f, 0.0f );

		// Calculate the movement due to the stationary position
		// of the finger, which the actor tends to approach.
		// If the camera is orthografic, calculation can be easier.
		if ( Camera.main.isOrthoGraphic )
		{
			Vector3 reachPoint = Camera.main.ScreenToWorldPoint(
				new Vector3( Input.mousePosition.x, Input.mousePosition.y, Camera.main.far ) );

			Vector3 approachMov = new Vector3(reachPoint.x,transform.localPosition.y,reachPoint.z);
			movement = (approachMov - transform.localPosition) * approachSpeed * ( Time.fixedDeltaTime * 100.0f );
		}
		else
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll (ray, Camera.main.far);
            foreach (RaycastHit hit in hits)
			{
                if (hit.transform.tag == "Base Surface")
                {					
					// Calculate the movement due to the stationary position
					// of the finger, which the actor tends to approach.
					Vector3 reachPoint = new Vector3( hit.point.x, transform.localPosition.y, hit.point.z );
					movement = (reachPoint - transform.localPosition) * approachSpeed;
					
					break;
				}
            }
		}
		
		return movement;
	}
	
	// Move the actor
	protected override void Move( Vector3 movement )
	{
		base.Move( movement );
	}
	
	// Update physics
	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		// If the actor is not spawning or dying,
		// then we can, keep track of its movement.
		// Move the actor only if the mouse button is pressed.
		if ( rigidbody && m_IsActive )
		{
			Move( ApproachPoint() );
			Rotate( rotationRate );
		}
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		
		// Reset states
		m_IsSpawning	= false;
		m_IsDying		= false;
		m_IsActive		= false;

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

			// Move the pawn if it is not spawning
			if ( m_IsSpawning == false )
			{
				m_IsActive = true;
				
				if ( rigidbody == null )
				{
					Move( ApproachPoint() );
					Rotate( rotationRate );
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
			}

			// Kill the actor if necesasry
			if ( m_Size <= 0.0f )
			{
				SpawnManager.Ref.DestoryPawn( this.gameObject );
				m_IsDying = false;
			}
		}
	}
	
	// Spawn the actor, and initialise the size.
	public override void Spawn()
	{
		base.Spawn();
		
		m_IsSpawning	= false;
		m_IsDying		= false;
		m_IsActive		= false;

		m_Size			= 0.0f;
		
		// Spawning and dying rate will be calculate according to the final size.
		// They should be given in percentage values.
		spawnRate	= Mathf.Clamp01( spawnRate ) * finalSize;
		dieRate		= Mathf.Clamp01( dieRate ) * finalSize;
		
		// Set the local scale. It won't affect particles nor the y-scale.
		transform.localScale = new Vector3( m_Size, transform.localScale.y, m_Size );
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
	
	public bool IsActive
	{
		get { return m_IsActive; }
	}
}
