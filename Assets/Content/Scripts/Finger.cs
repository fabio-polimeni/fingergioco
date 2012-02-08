using UnityEngine;
using System.Collections;

public class Finger : Pawn
{
	protected ParticleAnimator particleAnimator = null;
    protected ParticleRenderer particleRenderer = null;

	private bool	m_IsDying;
	private bool 	m_IsSpawning;

	private	float	m_Size;
    public  float   Size
    {
        get { return m_Size; }
    }

    private float   m_Energy;
    public  float   Energy
    {
        get { return m_Energy; }
        set
		{
			if ( !m_IsSpawning && !m_IsDying )
			{
				m_Energy += value;
				if ( m_Energy < 0.0f )
				{
					m_Energy = 0.0f;
				}
			}
		}
    }

	public 	float	spawnRate;
	public 	float	dieRate;
	public	float	rotationRate;
	public	float	approachSpeed;
	
	// Use this when the level is loaded
	protected override void Awake()
	{
		base.Awake();

		particleAnimator = GetComponent<ParticleAnimator>();
        particleRenderer = GetComponent<ParticleRenderer>();
		
		// Set the player layer
		gameObject.layer = LayerMask.NameToLayer("Player");
		
		// We want this object to be persistent across the levels
		Object.DontDestroyOnLoad( this );
		
		// Spawn the finger
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
		// Point to reach in the space.
		Vector3 moveToPoint = transform.localPosition;

		// Calculate the movement due to the stationary position
		// of the finger, which the actor tends to approach.
		// If the camera is orthographic, calculation can be easier.
		if ( SceneManager.CurrentCamera.isOrthoGraphic )
		{
			Vector3 reachPoint = SceneManager.CurrentCamera.ScreenToWorldPoint(
				new Vector3( Input.mousePosition.x, Input.mousePosition.y, SceneManager.CurrentCamera.far ) );
			
			moveToPoint = new Vector3(reachPoint.x,transform.localPosition.y,reachPoint.z);
		}
		else
		{
			Ray ray = SceneManager.CurrentCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll (ray, SceneManager.CurrentCamera.far);
			foreach (RaycastHit hit in hits)
			{
				if (hit.transform.tag == "Base Surface")
				{					
					// Calculate the movement due to the stationary position
					// of the finger, which the actor tends to approach.
					moveToPoint = new Vector3( hit.point.x, transform.localPosition.y, hit.point.z );					
					break;
				}
			}
		}
		
		// We need to limit the finger movement, so that, we can just go forward.
		Vector2 farest = new Vector2(
			SceneManager.CurrentCamera.transform.position.x + GameSettings.BaseSurfaceExtent - m_Size*0.5f,
			SceneManager.CurrentCamera.transform.position.z - SceneManager.CurrentCamera.orthographicSize + m_Size*0.5f);
		
		// Limit on the x axis
		if ( moveToPoint.x > farest.x )
		{
			moveToPoint.x = farest.x;
		}
		else if ( moveToPoint.x < -farest.x )
		{
			moveToPoint.x = -farest.x;
		}
		
		// Limit on the -z axis
		if ( moveToPoint.z < farest.y )
		{
			moveToPoint.z = farest.y;
		}
		
		// Return the movement vecotr.
		return (moveToPoint - transform.localPosition) * approachSpeed;
	}
	
	// Move the actor
	protected override void Move( Vector3 movement )
	{
		base.Move( movement );
	}
	
	// Update physics
	protected override void FixedUpdate()
	{
		//base.FixedUpdate();

		// If the actor is not spawning or dying,
		// then we can keep track of its movement.
		// Move the actor only if the mouse button is pressed.
        // Unfortunatelly moving the actor inside
        // this function can cause hitchings.
        //if (rigidbody && this.IsActive)
        //{
        //    Move(ApproachPoint());
        //    Rotate(rotationRate);
        //}
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		
		// Reset states
		m_IsSpawning	= false;
		m_IsDying		= false;
		
		// Deactivate the player.
		this.Deactivate();
		
		Particle[] particles = null;
		if ( particleEmitter )
		{
			// Store finger's particles.
		    particles = particleEmitter.particles;
		}

		// We keep active the pawn until the mouse button is pressed
		if ( Input.GetMouseButton(0) )
		{
			// Until we reach the 100% in size we keep resizing
			if ( m_Size < finalSize )
			{
				m_Size += spawnRate;				
				if ( m_Size < finalSize )
				{
					m_IsSpawning = true;
				}
				else
				{
					m_IsSpawning = false;
                    m_Size = finalSize;
				}
				
				// The transform component is always present, therefore,
				// we don't need to check whether or not it exists.
				transform.localScale = new Vector3( m_Size, transform.localScale.y, m_Size );
					
				// This check shouldn't be necessary
				if ( particles != null )
				{
					// Resize the finger's particle system.
		            for (int ip = 0; ip < particles.Length; ++ip)
		            {
						particles[ip].size = m_Size;
		            }
				}
			}

			// Move the pawn if it is not spawning
			if ( m_IsSpawning == false )
			{
				this.Activate();
				
                // Move tha actor regardelss whether or
                // not it has got a rigid-body component.
				//if ( rigidbody == null )
				{
					Move( ApproachPoint() );
					Rotate( rotationRate );
				}
			}
		}
		else
		{
			// Resizing the actor until it will disappear
			if ( m_Size > 0.0f )
			{
				m_Size -= dieRate;
				m_IsDying = true;
				
				if ( m_Size < 0.0f )
				{
					m_Size = 0.0f;
				}
				
				transform.localScale = new Vector3( m_Size, transform.localScale.y, m_Size );
				
				// This check shouldn't be necessary
				if ( particles != null )
				{
					// Resize the finger's particle system.
		            for (int ip = 0; ip < particles.Length; ++ip)
		            {
						particles[ip].size = m_Size;
		            }
				}
			}
		}

        // Kill the actor if necessary
        if ((m_Size <= 0.0f) || (m_Energy <= 0.0f))
        {
            //m_IsDying = false;
            this.Kill();
        }
		else if ( !m_IsSpawning && !m_IsDying )
		{
			// This check shouldn't be necessary
			if ( particles != null )
			{
		        for (int ip = 0; ip < particles.Length; ++ip)
		        {
					// Change the alpha according to the energy left.
					Color particleColor = particles[ip].color;
					particles[ip].color = new Color(particleColor.r,particleColor.g,particleColor.b, Mathf.Max(0.2f,m_Energy));
					
					// Set the current finger size.
					particles[ip].size = m_Size;
		        }
			}
		}
		
		if ( particles != null )
		{
			// Copy back modified particles
		    particleEmitter.particles = particles;
		}
	}
	
	// Spawn the actor, and initialise the size.
	public override void Spawn()
	{
		base.Spawn();
		
		m_Size = 0.0f;
        m_Energy = 1.0f;
		
		m_IsSpawning = false;
		m_IsDying = false;
		
		// Deactivate the player.
		this.Deactivate();
		
		// Spawning and dying rate will be calculate according to the final size.
		// They should be given in percentage values.
		spawnRate	= Mathf.Clamp01( spawnRate ) * finalSize;
		dieRate		= Mathf.Clamp01( dieRate ) * finalSize;
		
		// Set the local scale. It won't affect particles nor the y-scale.
		transform.localScale = new Vector3( m_Size, transform.localScale.y, m_Size );

		// Clamp the approach speed.
		approachSpeed = Mathf.Clamp01( approachSpeed );
	}
	
	// Kill the actor
	public override void Kill()
	{
        GameManager.Finger = null;
		GameManager.Score = 0.0f;

        m_Size = 0.0f;
        m_Energy = 0.0f;

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
