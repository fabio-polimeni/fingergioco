using UnityEngine;
using System.Collections;

public class Enemy : Pawn
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
            direction.Normalize();
            particleAnimator.force = direction * ForceStep;
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
