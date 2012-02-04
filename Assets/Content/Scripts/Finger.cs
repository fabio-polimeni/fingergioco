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
	
	private float	m_Score;
    public  float  	Score
    {
        get { return m_Score; }
        set
		{
			if ( !m_IsSpawning && !m_IsDying )
			{
				m_Score += value;
				if ( m_Score < 0.0f )
				{
					m_Score = 0.0f;
				}
			}
		}
    }

	public 	float	spawnRate;
	public 	float	dieRate;
	public	float	rotationRate;
	public	float	approachSpeed;
	
	// Each scoreTime (in seconds) the score will is increased by 1 times scoreRate.
	public	float	scoreTime;
	public	float 	scoreRate;
	private float	m_IncTime;
	
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
		// Declare the displacement vector
		Vector3 movement = new Vector3( 0.0f, 0.0f, 0.0f );

		// Calculate the movement due to the stationary position
		// of the finger, which the actor tends to approach.
		// If the camera is orthographic, calculation can be easier.
		if ( SceneManager.CurrentCamera.isOrthoGraphic )
		{
			Vector3 reachPoint = SceneManager.CurrentCamera.ScreenToWorldPoint(
				new Vector3( Input.mousePosition.x, Input.mousePosition.y, SceneManager.CurrentCamera.far ) );

			Vector3 approachMov = new Vector3(reachPoint.x,transform.localPosition.y,reachPoint.z);
			movement = (approachMov - transform.localPosition) * approachSpeed;
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
				// Change the alpha according to the energy left.
		        for (int ip = 0; ip < particles.Length; ++ip)
		        {
					particles[ip].color = Color.yellow;
					Color particleColor = particles[ip].color;
					particles[ip].color = new Color(particleColor.r,particleColor.g,particleColor.b, Mathf.Max(0.2f,m_Energy));
		        }
			}
			
			// Increment the score if necessary
			m_IncTime += Time.deltaTime;
			if ( m_IncTime >= scoreTime )
			{
				this.Score = 1*scoreRate;
				m_IncTime = 0.0f;
			}
		}
		
		if ( particles != null )
		{
			// Copy back modified particles
		    particleEmitter.particles = particles;
		}
	}
	
	// Gui
	void OnGUI()
	{
		string scoreString = "Score: " + m_Score.ToString("F2");
		GUI.Label(new Rect(10, 10, 10 * scoreString.Length, 20), scoreString);
		string energyString = "Energy: " + m_Energy.ToString("P1");
		GUI.Label(new Rect(10, 25, 10*energyString.Length, 20), energyString);
	}
	
	// Spawn the actor, and initialise the size.
	public override void Spawn()
	{
		base.Spawn();
		
		m_Size = 0.0f;
        m_Energy = 1.0f;
		m_Score = 0.0f;
		
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
		
		// Clamp the score time.
		scoreTime = Mathf.Max( 0.0f, scoreTime );
		scoreRate = Mathf.Max( 0, scoreRate );
	}
	
	// Kill the actor
	public override void Kill()
	{
        GameManager.Finger = null;

        m_Size = 0.0f;
        m_Energy = 0.0f;
		m_Score = 0.0f;
		m_IncTime = 0.0f;

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
