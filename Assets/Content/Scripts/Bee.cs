using UnityEngine;
using System.Collections;

public class Bee : Enemy
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
	
	// Threshold under which the bee will turn and prick you with the sting.
	public float StingRange;
	
	// Damage percentage to apply when particles collide.
	// It will be scaled by the particle's size factor.
	public float Damage;
	
	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	protected override void Awake()
	{
		base.Awake();

        particleAnimator = GetComponent<ParticleAnimator>();
        particleRenderer = GetComponent<ParticleRenderer>();

        if (particleAnimator)
        {
            particleAnimator.autodestruct = true;
        }
		
		// Set the player layer
		gameObject.layer = LayerMask.NameToLayer("Enemy");
		
		CollisionScale = Mathf.Clamp01(CollisionScale);
		AttractionRange = Mathf.Max(0.0f,AttractionRange);
		AttractionForce = Mathf.Clamp01(AttractionForce);
		StingRange = Mathf.Max(0.0f,StingRange);
		ForceStep = Mathf.Max(0.0f,ForceStep);
		Damage = Mathf.Max(0.0f,Damage);
		
		if ( AttractionRange == 0.0f || AttractionForce == 0.0f )
		{
			AttractionRange = 0.0f;
			AttractionForce = 0.0f;
		}
	}

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();

        // When the enemy is spawned we want to give an input to
        // the particles, in order to follow the finger position.
        if (particleEmitter && GameManager.Finger)
        {
            Vector3 direction = GameManager.Finger.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            particleEmitter.worldVelocity = direction * ForceStep;
            particleEmitter.localVelocity = Vector3.zero;
        }
		// If no finger is present, then move the bee circularly.
		else if ( particleAnimator )
		{
			particleAnimator.force = Vector3.left * ForceStep;
			particleAnimator.localRotationAxis = Vector3.up;
			particleAnimator.worldRotationAxis = Vector3.up;
		}
	}
	
	// Update physics
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update();

        // Iterate all over particles.
        Particle[] p = particleEmitter.particles;
		
		// Sting flag
		bool useSting = false;
		
        for (int ip = 0; ip < particleEmitter.particles.Length; ++ip)
        {
            Particle particle = p[ip];
			if (particleEmitter && GameManager.Finger)
        	{
				particle.position = new Vector3(particle.position.x, GameManager.Finger.transform.position.y, particle.position.z);

                // Sphere-Sphere intersection.
                Vector3 moveDirection = GameManager.Finger.transform.position - particle.position;
                float distance = moveDirection.magnitude;
				
				//particle.color = Color.white;
				
				// Handle attraction
                if (AttractionForce > 0.0f && AttractionRange > 0.0f)
                {
					//particle.color = Color.magenta;
                    if (distance <= (GameManager.FingerComponent.Size + (particle.size + particle.size * AttractionRange)) * 0.5f)
                    {
                        //particle.color = Color.green;

                        // Add to the current velocity a velocity vector in direction of the finger.
                        float speed = particle.velocity.magnitude;
						particle.velocity =
							particle.velocity.normalized * speed * (1.0f - AttractionForce)
							+ moveDirection.normalized * AttractionForce * (distance + StingRange);
                    }
                }
				
				// Handle sting
				if (distance <= ((GameManager.FingerComponent.Size + particle.size) * 0.5f) + StingRange )
				{
					useSting = true;
				}
                
				// Handle collision
				if (distance <= (GameManager.FingerComponent.Size + particle.size*CollisionScale) * 0.5f)
                {
                    //particle.color = Color.red;
					
					// Apply damage according to the current liftime of the particle.
					float lifetime = 1.0f - particle.energy/particle.startEnergy;
					GameManager.FingerComponent.Energy = -Damage*lifetime;
                }
            }
			
			// Handle rotation
			Vector3 velocity = particle.velocity.normalized;
			float sign = Mathf.Sign(Vector3.Dot(velocity,Vector3.right));
			
			// Change sign if the bee is close enough
			if ( useSting )
			{
				particle.rotation = sign*Vector3.Angle(velocity,Vector3.forward);
			}
			else
			{
				particle.rotation = sign*Vector3.Angle(velocity,Vector3.forward);
			}
			

			// Copy back modified particle.
            p[ip] = particle;

            // Copy back modified particles.
            particleEmitter.particles = p;
        }
	}
	
	/// <summary>
	/// Non-Unity functions. 
	/// </summary>
	
	// Move the pawn
	protected override void Move( Vector3 movement )
	{
		base.Move( movement );
	}
	
	// Rotate the pawn
	protected override void Rotate( float rotation )
	{
		base.Rotate( rotation );
	}
	
	// Spawn the pawn
	public override void Spawn()
	{
		base.Spawn();
	}
	
	// Kill the actor
	public override void Kill()
	{
		base.Kill();
	}
}
