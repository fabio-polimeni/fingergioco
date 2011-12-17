using UnityEngine;
using System.Collections;

public class Spawnable : MonoBehaviour
{
	/// <summary>
	/// Properties.
	/// </summary>

    // Rotation applaied to all particles
    public float ParticlesRotation;

    // Number of frames showed
    private float m_ShowTime;
    private float m_FramesToShow;

    // Whether or not it spawned.
    private bool m_Spawned;

    protected ParticleAnimator particleAnimator = null;
    protected ParticleRenderer particleRenderer = null;

    // Returns whether the pawn is active or not
    protected bool m_IsActive;
    public bool IsActive
    {
        get { return m_IsActive; }
    }

	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	void Awake()
	{
		// Set layer and tag
		gameObject.layer = LayerMask.NameToLayer("Enemy");
        gameObject.tag = "Spawnable";
       
        m_IsActive = false;
        m_Spawned = false;

        m_ShowTime = 0.0f;
        m_FramesToShow = -1.0f;

        particleAnimator = GetComponent<ParticleAnimator>();
        particleRenderer = GetComponent<ParticleRenderer>();
        if (particleEmitter && particleAnimator && particleRenderer)
        {
            particleEmitter.enabled = false;
            particleEmitter.emit = false;
            particleEmitter.useWorldSpace = false;
            particleEmitter.minEnergy = particleEmitter.maxEnergy;

            particleAnimator.autodestruct = false;
            particleAnimator.doesAnimateColor = false;

            //m_ShowTime = particleRenderer.uvAnimationXTile * particleRenderer.uvAnimationYTile * (int)particleRenderer.uvAnimationCycles;
            m_ShowTime = particleEmitter.maxEnergy / particleRenderer.uvAnimationCycles;

        }
    }

	// Use this for initialization
	void Start()
	{
	}

	// Update physics
	void FixedUpdate()
	{
	}

	// Update is called once per frame
	void Update()
	{
        // We want to rotate all particles by  given angle.
        // In this case we choose the rotation around the y-axsis.
        if (this.IsActive && particleEmitter && particleEmitter.enabled)
        {
            Particle[] p = new Particle[particleEmitter.particles.Length];

        #if UNITY_EDITOR
            Debug.Log("Useful::Update: Number of particles: " + particleEmitter.particles.Length);
        #endif

            for ( int ip = 0; ip < particleEmitter.particles.Length; ++ip )
            {
                p[ip] = particleEmitter.particles[ip];
                p[ip].rotation = ParticlesRotation;
            }

            // Copy back modified particles
            particleEmitter.particles = p;

            // Increment the frame time.
            if ((m_ShowTime > 0.0f) && (m_FramesToShow < m_ShowTime))
            {
                m_FramesToShow += Time.deltaTime;
                if (m_FramesToShow >= m_ShowTime)
                {
                    Deactivate();

                    // Reinitialise the frame time, 
                    // and signal it has spawned.
                    m_Spawned = true;

                    // TODO: Change the emitter material texture with he glowed one.
                }

                // TODO: if m_Spawned is true and it is the
                //       right time, then, spawn the enemy.
            }
        }
	}

    // Something exited the collider
    void OnTriggerExit(Collider other)
    {
    }
	
	// Something entered the collider
	void OnTriggerEnter(Collider other)
	{
		// If the player has entered its area,
		// then, start the spawn effect.
		if ( other.gameObject.tag == "Finger" )
		{
			// Active it if is not yet.
            if ( ((m_FramesToShow < 0.0f) || m_Spawned) && !IsActive )
			{
                m_FramesToShow = 0.0f;
				this.Activate();
			}
		}
	}

    // Something is inside the collider
    void OnTriggerStay(Collider other)
    {
        // If the player is in its area,
        // then, start the spawn effect.
        if (other.gameObject.tag == "Finger")
        {
            // Active it if is not yet.
            if (((m_FramesToShow < 0.0f) || m_Spawned) && !IsActive)
            {
                m_FramesToShow = 0.0f;
                this.Activate();
            }
        }
    }

	/// <summary>
	/// Non-Unity functions. 
	/// </summary>

	// Activate the powerup
	protected bool Activate()
	{
        if (this.IsActive == false)
        {
            particleAnimator = GetComponent<ParticleAnimator>();
            if (particleEmitter && particleAnimator)
            {
                particleEmitter.enabled = true;
                particleEmitter.emit = true;
            }
            else
            {
                return false;
            }

            m_IsActive = true;
        }

        return m_IsActive;
	}

    // Deactivate
    protected void Deactivate()
    {
        m_IsActive = false;
        if (particleEmitter)
        {
            particleEmitter.enabled = false;
            particleEmitter.emit = false;
        }
    }

	// Move the pawn
	protected void Move( Vector3 movement )
	{
	}

    // Rotate the pawn
	protected void Rotate( float rotation )
	{
	}

}
