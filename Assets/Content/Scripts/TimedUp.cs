using UnityEngine;
using System.Collections;

public class TimedUp : PowerUp
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
	
	// LateUpdate is called once per frame
	protected override void LateUpdate ()
	{
		base.LateUpdate();
	}
	
	/// <summary>
	/// Non-Unity functions. 
	/// </summary>
	
	// Activate the powerup
	protected override bool Activate()
	{
		return false;
	}
	
	// Deactivate the poweup.
	protected override bool Deactivate()
	{
		return false;
	}
	
	// Spawn the pawn
	public override void Spawn()
	{
	}
	
	// Kill the actor
	public override void Kill()
	{
	}
}
