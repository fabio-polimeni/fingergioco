using UnityEngine;
using System.Collections;

public class Enemy : Pawn
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
		
		// Set the player layer
		gameObject.layer = LayerMask.NameToLayer("Enemy");
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
