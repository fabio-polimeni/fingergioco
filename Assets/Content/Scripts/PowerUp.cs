using UnityEngine;
using System.Collections;

public class PowerUp : Pawn
{
	/// <summary>
	/// Properties.
	/// </summary>
	
	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	protected override void Awake()
	{
		base.Awake();
		
		// By default the particle emitter must be switched off.
		particleEmitter.emit = false;
		
		// Set the powerup layer
		gameObject.layer = LayerMask.NameToLayer("Friend");
	}

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
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
	
	// Something entered the collider
	void OnTriggerEnter( Collider other )
	{
		// Get the player layer
		int playerLayer = LayerMask.NameToLayer("Player");
		
		// If the player has entered its area,
		// then, activate the powerup.
		if ( other.gameObject.layer == playerLayer )
		{
			// We active it if is not active yet.
			if ( this.IsActive == false )
			{
				this.Activate();
			}
		}
	}
	
	/// <summary>
	/// Non-Unity functions. 
	/// </summary>
	
	// Activate the powerup
	protected override bool Activate()
	{
		// Activate the powerup.
		if ( base.Activate() )
		{
			// Start the emitter.
			particleEmitter.emit = true;
			return true;
		}
		
		return false;
	}
	
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
