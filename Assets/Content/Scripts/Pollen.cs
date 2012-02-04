using UnityEngine;
using System.Collections;

public class Pollen : Enemy
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
        if (particleAnimator && GameManager.Finger)
        {
            Vector3 direction = GameManager.Finger.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            //particleAnimator.force = direction * ForceStep;
            particleEmitter.worldVelocity = direction * ForceStep;
            particleEmitter.localVelocity = Vector3.zero;
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

        if (particleEmitter && GameManager.Finger)
        {
            // Iterate all over particles.
            Particle[] p = particleEmitter.particles;
            for (int ip = 0; ip < particleEmitter.particles.Length; ++ip)
            {
                Particle particle = p[ip];
                particle.position = new Vector3(particle.position.x, GameManager.Finger.transform.position.y, particle.position.z);

                // Sphere-Sphere intersection.
                Vector3 displacement = GameManager.Finger.transform.position - particle.position;
                float distance = displacement.magnitude;
				
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
							+ displacement.normalized * speed * AttractionForce;
                    }
                }
                
				// Handle collision
				if (distance <= (GameManager.FingerComponent.Size + particle.size*CollisionScale) * 0.5f)
                {
                    //particle.color = Color.red;
					
					// Apply damage according to the current liftime of the particle.
					float lifetime = 1.0f - particle.energy/particle.startEnergy;
					GameManager.FingerComponent.Energy = GameManager.FingerComponent.Energy - (GameManager.FingerComponent.Energy*(1.0f - Damage*lifetime));
                }
				
				// Copy back modified particle.
                p[ip] = particle;
            }

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
