using UnityEngine;
using System.Collections;

public class Pollen : Enemy
{
	/// <summary>
	/// Properties.
	/// </summary>

    // Force to apply to particles in finger direction.
    public float ForceStep;

    protected ParticleAnimator particleAnimator = null;
    protected ParticleRenderer particleRenderer = null;

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
//#if UNITY_EDITOR
//            Debug.Log("Enemy's particles: " + particleEmitter.particles.Length);
//#endif

            // Iterate all over particles.
            Particle[] p = particleEmitter.particles;
            for (int ip = 0; ip < particleEmitter.particles.Length; ++ip)
            {
                Particle particle = p[ip];
                particle.position = new Vector3(particle.position.x, GameManager.Finger.transform.position.y, particle.position.z);

                // Sphere-Sphere intersection
                Vector3 displacement = GameManager.Finger.transform.position - particle.position;
                float distance = displacement.magnitude;
                if (distance <= (GameManager.FingerComponent.Size + particle.size)*0.5f)
                {
                    if (particle.color != Color.red)
                    {
                        particle.color = Color.red;
//#if UNITY_EDITOR
//                        Debug.Log("Particle's size: " + particle.size
//                            + "\nFinger's size: " + GameManager.FingerComponent.Size
//                            + "\nDistance: " + distance);

//#endif
                    }
                }

                p[ip] = particle;
            }

            // Copy back modified particles
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
