using UnityEngine;
using System.Collections;

public class Spawnable : MonoBehaviour
{
	/// <summary>
	/// Properties.
	/// </summary>

    // Rotation applaied to all particles
    public float ParticlesRotation;

    // Glow texture
    public Texture GlowTexture;

    // Blow texture
    public Texture BlowTexture;

    // Enemy
    public GameObject Enemy;

    // Useful attached object
    private Useful m_AttachedUseful;

    // Spawn texture
    private Texture m_SpawnTexture;

    // Number of frames showed
    private float m_AnimationTime;
    private float m_ElapesedTime;

    // Initial particle size
    private float m_InitialParticleSize;

    // Initial color
    private Color m_InitialColor;

    // Whether or not it spawned.
    private bool m_Glowing;
    public bool IsGlowing
    {
        get { return m_Glowing; }
    }

    // Whether or not it playing the glow animation
    private bool m_Blowing;
    public bool IsBlowing
    {
        get { return m_Blowing; }
    }

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
        m_Glowing = false;
        m_Blowing = false;

        m_AnimationTime = 0.0f;
        m_ElapesedTime = 0.0f;

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

            m_AnimationTime = particleEmitter.maxEnergy / particleRenderer.uvAnimationCycles;

            m_SpawnTexture = particleRenderer.material.mainTexture;

            //m_InitialColor = particleRenderer.material.GetColor("_TintColor");
            //if (m_InitialColor.a == 0.0f)
            {
                m_InitialColor = Color.black;
            }
        }

        // Every spawnable has to be child of an useful.
        if (transform.parent)
        {
            m_AttachedUseful = transform.parent.GetComponent<Useful>();
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
        //    Particle[] p = new Particle[particleEmitter.particles.Length];

        //#if UNITY_EDITOR
        //    Debug.Log("Useful::Update: Number of particles: " + particleEmitter.particles.Length);
        //#endif

        //    for ( int ip = 0; ip < particleEmitter.particles.Length; ++ip )
        //    {
        //        p[ip] = particleEmitter.particles[ip];
        //        p[ip].rotation = ParticlesRotation;
        //    }

        //    // Copy back modified particles
        //    particleEmitter.particles = p;

            // Increment the frame time.
            if ((m_AnimationTime > 0.0f) && (m_ElapesedTime < m_AnimationTime))
            {
                m_ElapesedTime += Time.deltaTime;
                if (m_ElapesedTime >= m_AnimationTime)
                {
                    // If is not already spawning, that is, using the glow animation
                    if (!IsGlowing)
                    {
                        // Once we finished the spawning animation,
                        // we want to activate the glow one.
                        // We just need  to swap the tiled texture of
                        // particle renderer's material.
                        if (particleRenderer && GlowTexture)
                        {
                            particleRenderer.material.mainTexture = GlowTexture;

                            particleEmitter.minSize = particleEmitter.maxSize = m_InitialParticleSize;

                            particleEmitter.Emit(Vector3.zero, Vector3.zero,
                                particleEmitter.maxSize, particleEmitter.maxEnergy,
                                m_InitialColor, ParticlesRotation, 0.0f);

                            m_Glowing = true;
                            m_Blowing = false;
                        }
                    }
                    else if (!IsBlowing)
                    {
                        // Create the enemy.
                        if (Enemy)
                        {
                            Vector3 enemyPosition = transform.position;
                            enemyPosition.y = 0.1f;
                            GameObject.Instantiate(Enemy,enemyPosition, Quaternion.identity);
                        }
                        
                        // Blow up the spawnable.
                        if (particleRenderer && BlowTexture)
                        {
                            particleRenderer.material.mainTexture = BlowTexture;
                            particleEmitter.Emit(Vector3.zero, Vector3.zero,
                                particleEmitter.maxSize, particleEmitter.maxEnergy,
                                m_InitialColor, ParticlesRotation, 0.0f);
                        }

                        m_Glowing = true;
                        m_Blowing = true;
                    }
                    else
                    {
                        Deactivate();

                        m_Glowing = false;
                        m_Blowing = false;
                    }

                    m_ElapesedTime = 0.0f;
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

	}

    // Something is inside the collider
    void OnTriggerStay(Collider other)
    {
        // If the player is in its area,
        // then, start the spawn effect.
        if (other.gameObject.tag == "Finger")
        {
            this.Activate();
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
            if (m_AttachedUseful && m_AttachedUseful.IsIdle)
            {
                if (particleEmitter && particleRenderer)
                {
                    particleEmitter.enabled = true;

                    particleRenderer.material.mainTexture = m_SpawnTexture;
                    particleEmitter.Emit(Vector3.zero, Vector3.zero,
                        particleEmitter.maxSize, particleEmitter.maxEnergy,
                        m_InitialColor, ParticlesRotation, 0.0f);
                }
                else
                {
                    return false;
                }

                m_IsActive = true;
            }
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


    public bool Blowing { get; set; }
}
