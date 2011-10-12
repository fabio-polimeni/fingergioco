using UnityEngine;
using System.Collections;

public class MainPawn : Pawn
{	
	private bool	m_IsDying;
	private bool 	m_IsSpawning;
	
	// Use this when the level is loaded
	protected override void Awake()
	{
		base.Awake();
		Spawn();
	}

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		transform.localScale = new Vector3( m_Size, transform.localScale.y, m_Size );
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update();
		
		// Reset states
		m_IsSpawning	= false;
		m_IsDying		= false;
		
		// We keep active the pawn until the mouse button is pressed
		if ( Input.GetMouseButton(0) )
		{
			// Until we reach the 100% in size we keep resizing
			if ( m_Size < FinalSize )
			{
				m_Size += SizeRate;
				
				// The transform component is always present, therefore,
				// we don't need to check whether or not it exists.
				// CHECK:
				//	Although, since the localScale property is relative
				// 	to the parent, we must be certain that the parent does
				// 	not exist, or if exists, its scale is zero.
				transform.localScale = new Vector3( m_Size, transform.localScale.y, m_Size );
				
				if ( m_Size < FinalSize )
				{
					m_IsSpawning = true;
				}
			}
		}
		else
		{
			// Resizing the actor until it will desappear
			if ( m_Size > 0.0f )
			{
				m_Size -= SizeRate;
				transform.localScale = new Vector3( m_Size, transform.localScale.y, m_Size );
				
				m_IsDying = true;
				
				// Kill the actor if necesasry
				if ( m_Size <= 0.0f )
				{
					SpawnManager.Ref.DestoryPawn( this.gameObject );
					m_IsDying = false;
				}
			}
		}
		
		// If the actor is not spawning or dying,
		// then we can, keep track of its movement.
		if ( !m_IsSpawning && m_IsDying )
		{
			
		}
	}
	
	// Spawn the actor, and initialise the size.
	public override void Spawn()
	{
		base.Spawn();
		
		m_IsSpawning		= false;
		m_IsDying			= false;
	}
	
	// Kill the actor
	public override void Kill()
	{
		base.Kill();
	}
	
	public bool IsDying
	{
		get { return m_IsDying; }
	}
	
	public bool IsSpawning
	{
		get { return m_IsSpawning; }
	}
}
