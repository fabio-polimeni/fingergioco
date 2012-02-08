using UnityEngine;
using System.Collections;

public class Useful : MonoBehaviour
{
	/// <summary>
	/// Properties.
	/// </summary>

    // Rotation applaied to all particles
    public float ParticlesRotation;

    // Idle texture
    public Texture IdleTexture;

    // Number of frames showed
    private float m_ShowedFrames;
    private float m_FramesToShow;

    // Initial particle size
    private float m_InitialParticleSize;

    // Initial color
    private Color m_InitialColor;

    protected ParticleAnimator particleAnimator = null;
    protected ParticleRenderer particleRenderer = null;

    // Returns whether useful is active
    protected bool m_IsActive;
    public bool IsActive
    {
        get { return m_IsActive; }
    }

    // Returns whether the useful is in idle
    protected bool m_IsIdle;
    public bool IsIdle
    {
        get { return m_IsIdle; }
    }

	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	void Awake()
	{
		// Set layer and tag
		gameObject.layer = LayerMask.NameToLayer("Enemy");
        gameObject.tag = "Useful";
        
        m_IsActive = false;
        m_IsIdle = false;

        m_ShowedFrames = 0.0f;
        m_FramesToShow = 0.0f;

        particleAnimator = GetComponent<ParticleAnimator>();
        particleRenderer = GetComponent<ParticleRenderer>();
        if (particleEmitter && particleAnimator && particleRenderer)
        {
            particleEmitter.enabled = false;
            particleEmitter.emit = false;
            particleEmitter.useWorldSpace = false;

            particleEmitter.minEnergy = particleEmitter.maxEnergy;
            particleEmitter.minEmission = particleEmitter.maxEmission;
            m_InitialParticleSize = particleEmitter.minSize = particleEmitter.maxSize;

            particleAnimator.autodestruct = false;
            particleAnimator.doesAnimateColor = false;

            m_ShowedFrames = (particleEmitter.maxEnergy - 0.009f) / particleRenderer.uvAnimationCycles;

            //m_InitialColor = particleRenderer.material.GetColor("_TintColor");
            //if (m_InitialColor.a == 0.0f)
            {
                m_InitialColor = Color.black;
            }
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
        // We want to rotate all particles by a given angle.
        // In this case we choose the rotation around the y-axsis.
        if (this.IsActive && particleEmitter && particleEmitter.enabled)
        {
            //if (this.m_IsIdle)
            //{
            //    Particle[] p = new Particle[particleEmitter.particles.Length];

            //    for (int ip = 0; ip < particleEmitter.particles.Length; ++ip)
            //    {
            //        p[ip] = particleEmitter.particles[ip];
            //        p[ip].rotation = ParticlesRotation;
            //    }

            //    // Copy back modified particles
            //    particleEmitter.particles = p;
            //}

            if ((m_ShowedFrames > 0.0f) && (m_FramesToShow < m_ShowedFrames))
            {
                m_FramesToShow += Time.deltaTime;
                if (m_FramesToShow >= m_ShowedFrames)
                {
                    // Once we finished the spawning animation,
                    // we want to activate the idle one.
                    // We just need  to swap the tiled texture of
                    // particle renderer's material.
                    if (particleRenderer && IdleTexture)
                    {
                        // Restore idle particle animation.
                        m_IsIdle = true;

                        particleEmitter.minSize = particleEmitter.maxSize = m_InitialParticleSize * (6.0f / 4.0f);

                        particleRenderer.uvAnimationXTile = 4;
                        particleRenderer.uvAnimationYTile = 4;
                        particleRenderer.material.mainTexture = IdleTexture;

                        particleEmitter.Emit(Vector3.zero, Vector3.zero,
                           particleEmitter.maxSize, particleEmitter.maxEnergy,
                           m_InitialColor, ParticlesRotation, 0.0f);
						
						// We also want to activate all fruits associated with it.
						Fruit[] fruits = this.GetComponentsInChildren<Fruit>();
				        foreach (Fruit fruit in fruits)
				        {
				            if ( fruit.gameObject.particleEmitter )
							{
								fruit.gameObject.particleEmitter.emit = true;
							}
				        }
					}

                    m_FramesToShow = 0.0f;
                }
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
        if (particleEmitter && particleRenderer)
        {
            particleEmitter.enabled = true;
            particleEmitter.Emit(Vector3.zero, Vector3.zero,
                   particleEmitter.maxSize, particleEmitter.maxEnergy,
                   m_InitialColor, ParticlesRotation, 0.0f);
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
