using UnityEngine;
using System.Collections;

public class Fruit : PowerUp
{
	/// <summary>
	/// Properties.
	/// </summary>
	public float m_Points;
	
	public GameObject StarEffect;
	
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
		m_Points = Mathf.Max(0.0f, m_Points);
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
		// When the power up is activate, means
		// the finger is inside the trigger,
		// therefore, this check shouldn't be necessary.
		if ( GameManager.FingerComponent )
		{
			// Increment the score
			GameManager.Score += m_Points;
			
			// Before dying spawn an effect.
			GameObject.Instantiate(StarEffect, transform.position, transform.rotation);
			
			GameObject.Destroy(gameObject);
			return true;
		}
		
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
