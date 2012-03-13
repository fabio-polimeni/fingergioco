using UnityEngine;
using System.Collections;

public class DragonFly : MonoBehaviour
{
	/// <summary>
	/// Properties.
	/// </summary>
	
	// It could happen that particle size is larger than
	// what we actually see, so we apply this multiplayer
	// to the particle size to tweak the collision.
	public float CollisionScale;
	
	// The ray of the sphere which identify the range of
	// the particle, inside which attraction is applied.
	// This value is times the current size of the particle.
	public float AttractionRange;
		
	// Gravitational attraction between the finger and the particle.
	public float AttractionForce;
	
    // Force to apply to particles in finger direction.
    public float ForceStep;
	
	// Attack speed.
	public float AttackSpeed;
	
	// How much of the particle energy to conserve
	// once it collide against the finger.
	public float DyingFactor;

	// Damage percentage to apply when particles collide.
	// It will be scaled by the particle's size factor.
	public float Damage;
	
	// How much the speed of the particle is reduced,
	// in percentage, when it hits the finger.
	public float HitSpeedReduction;
	
	// Whether or not it has been touched already.
    private bool m_Touched;
    public bool IsTouched
    {
        get { return m_Touched; }
    }
	
	protected ParticleAnimator particleAnimator = null;
    protected ParticleRenderer particleRenderer = null;
	protected AudioSource	   audioSource = null;
	
	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	void Awake()
	{
		particleAnimator = GetComponent<ParticleAnimator>();
        particleRenderer = GetComponent<ParticleRenderer>();
		
		//sphereCollider = GetComponent<SphereCollider>();
        if (particleEmitter && particleRenderer && particleAnimator)
        {
            particleEmitter.enabled = true;
            particleEmitter.emit = false;
            particleEmitter.useWorldSpace = false;

            particleAnimator.autodestruct = false;
            particleAnimator.doesAnimateColor = false;
        }
		
		// Deactivate the audio source
		if ( audio )
		{
			audio.enabled = false;
		}
		
		// Set the player layer
		gameObject.layer = LayerMask.NameToLayer("Enemy");
		
		CollisionScale = Mathf.Clamp01(CollisionScale);
		AttractionRange = Mathf.Max(0.0f,AttractionRange);
		AttractionForce = Mathf.Clamp01(AttractionForce);
		AttackSpeed = Mathf.Max(0.0f,AttackSpeed);
		ForceStep = Mathf.Max(0.0f,ForceStep);
		Damage = Mathf.Max(0.0f,Damage);
		DyingFactor = Mathf.Clamp01(DyingFactor);
		HitSpeedReduction = Mathf.Clamp01(HitSpeedReduction);
		
		if ( AttractionRange == 0.0f || AttractionForce == 0.0f )
		{
			AttractionRange = 0.0f;
			AttractionForce = 0.0f;
		}
		
		m_Touched = false;
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update physics
	void FixedUpdate()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
        // Iterate all over particles.
        Particle[] p = particleEmitter.particles;
		
        for (int ip = 0; ip < particleEmitter.particles.Length; ++ip)
        {
            Particle particle = p[ip];
			
			// Handle finger interaction per particle.
			if (particleEmitter && GameManager.Finger)
        	{
				// Set the particle position at the same hight of the finger.
				// NOTE: the position of the particle is relative to its
				// object transformation matrix.
				Vector3 particlePos = new Vector3(
					particle.position.x + transform.position.x,
					GameManager.Finger.transform.position.y,
					particle.position.z + transform.position.z);

                // Sphere-Sphere intersection.
                Vector3 moveDirection = GameManager.Finger.transform.position - particlePos;
                float distance = moveDirection.magnitude;
								
				// This factor reduce the speed of the insect once it collide with finger
				bool hit = false;
				
				// Handle collision
				if (distance <= (GameManager.FingerComponent.Size + particle.size*CollisionScale) * 0.5f)
                {
					// Apply damage according to the current liftime of the particle.
					float lifetime = 1.0f - particle.energy/particle.startEnergy;
					GameManager.FingerComponent.Energy = -Damage*lifetime;
					
					// Dragonflies tend to die once they touch the enemy.
					particle.energy *= DyingFactor;

					hit = true;
                }
				
				// Handle attraction
                //if (AttractionForce > 0.0f && AttractionRange > 0.0f)
                {
                    if (distance <= (GameManager.FingerComponent.Size + (particle.size + particle.size * AttractionRange)) * 0.5f)
                    {
                        // Add to the current velocity a velocity vector in direction of the finger.
                        float speed = particle.velocity.magnitude;
						particle.velocity =
							particle.velocity.normalized * speed * (1.0f - AttractionForce)
							+ moveDirection.normalized * AttractionForce * (distance + AttackSpeed);

						// Apply the speed reduction
						if ( hit )
						{
							particle.velocity = particle.velocity.normalized * particle.velocity.magnitude * (1.0f-HitSpeedReduction);
						}
					}
                }
            }
			
			// Handle rotation
			Vector3 velocity = particle.velocity.normalized;
			float sign = Mathf.Sign(Vector3.Dot(velocity,Vector3.right));
			particle.rotation = sign*Vector3.Angle(velocity,Vector3.forward);

			// Copy back modified particle.
            p[ip] = particle;
        }
	
       	// Copy back modified particles.
       	particleEmitter.particles = p;
	}
	
	void LateUpdate()
	{
		// Stop playing sounds when all particle are gone.
		if ( audio && audio.enabled && (particleEmitter.particleCount == 0) && (audio.isPlaying && m_Touched == false) )
		{
			SceneManager.TurnVolume(1.0f);
			audio.enabled = false;
			audio.Stop();
		}
	}
	
    // Something exited the collider
    void OnTriggerExit(Collider other)
    {
		if (other.gameObject.tag != "Finger") return;
		if ( particleEmitter && particleEmitter.emit )
		{
			particleEmitter.emit = false;
			m_Touched = false;
		}
    }
	
	// Something entered the collider
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag != "Finger") return;

		// When the enemy is spawned we want to give an input to
        // the particles, in order to follow the finger position.
        if (particleEmitter && (particleEmitter.emit == false) && GameManager.Finger)
        {
            Vector3 direction = GameManager.Finger.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            particleEmitter.worldVelocity = direction * ForceStep;
            particleEmitter.localVelocity = Vector3.zero;
			
			particleEmitter.emit = true;
			m_Touched = true;
			
			if ( audio && audio.enabled == false )
			{
				SceneManager.TurnVolume(GameSettings.SoundtrackAttenuation);
				audio.enabled = true;
				audio.Play();
			}
        }
	}
}
