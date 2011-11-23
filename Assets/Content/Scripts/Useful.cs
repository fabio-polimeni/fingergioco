using UnityEngine;
using System.Collections;

public class Useful : MonoBehaviour
{
	/// <summary>
	/// Properties.
	/// </summary>

    // Rotation applaied to all particles
    public float ParticlesRotation;

    // Number of frames showed
    private float m_ShowedFrames;
    private float m_FramesToShow;

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
		gameObject.layer = LayerMask.NameToLayer("Friend");
        gameObject.tag = "Useful";
        m_IsActive = false;

        m_ShowedFrames = 0.0f;
        m_FramesToShow = 0.0f;

        ParticleAnimator particleAnimator = GetComponent<ParticleAnimator>();
        ParticleRenderer particleRenderer = GetComponent<ParticleRenderer>();
        if (particleEmitter && particleAnimator && particleRenderer)
        {
            particleEmitter.enabled = false;
            particleEmitter.emit = false;
            particleEmitter.useWorldSpace = false;
            particleEmitter.minEnergy = particleEmitter.maxEnergy;

            particleAnimator.autodestruct = false;
            particleAnimator.doesAnimateColor = false;

            //m_ShowedFrames = particleRenderer.uvAnimationXTile * particleRenderer.uvAnimationYTile * (int)particleRenderer.uvAnimationCycles;
            m_ShowedFrames = particleEmitter.maxEnergy / particleRenderer.uvAnimationCycles;

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
        if (this.IsActive && particleEmitter && particleEmitter.emit)
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

            if ((m_ShowedFrames > 0.0f) && (m_FramesToShow < m_ShowedFrames))
            {
                m_FramesToShow += Time.deltaTime;
                if (m_FramesToShow >= m_ShowedFrames)
                {
                    Deactivate();
                }
            }
        }
	}

    // Something exited the collider
    void OnTriggerExit(Collider other)
    {
        // If the player has exited its area,
        // then, stop the spawn effect.
        //if (other.gameObject.tag == "Finger")
        //{
        //    // Deactivate it if is not yet.
        //    if (this.IsActive == true)
        //    {
        //        this.Deactivate();
        //    }
        //}
    }
	
	// Something entered the collider
	void OnTriggerEnter(Collider other)
	{
		// If the player has entered its area,
		// then, start the spawn effect.
		if ( other.gameObject.tag == "Finger" )
		{
			// Active it if is not yet.
            if ((this.IsActive == false) && (m_FramesToShow == 0))
			{
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
        ParticleAnimator particleAnimator = GetComponent<ParticleAnimator>();
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
