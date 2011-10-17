using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody),
                   typeof(MeshCollider),
                   typeof(ParticleEmitter))]

public class Pawn : MonoBehaviour
{
	/// <summary>
	/// Properties.
	/// </summary>
	
	protected	bool				m_IsActive;
	
	protected	float				m_ParticleMinSize;
	protected	float				m_ParticleMaxSize;
	
	protected	ParticleAnimator	particleAnimator;
	protected	ParticleRenderer	particleRenderer;
	protected	Color[]				particleColors;

	public 		float				finalSize;
	
	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	protected virtual void Awake()
	{
		// Initialise particle system
		particleRenderer = null;
		particleAnimator = null;
		
		// Disable the mesh render if exsists.
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if ( meshRenderer )
		{
			Object.Destroy( meshRenderer );
		}
		
		// Store initial particle size information.
		m_ParticleMaxSize = particleEmitter.maxSize;
		m_ParticleMinSize = particleEmitter.minSize;
		
		//particleEmitter.maxSize = 0.0f;
		//particleEmitter.minSize = 0.0f;
		
		// We want the particle emitter follows the object.
		particleEmitter.useWorldSpace = false;
		
		// Get the particle renderer. It will never be null since is a required component.
		particleRenderer = gameObject.GetComponent<ParticleRenderer>();
		if ( particleRenderer == null )
		{
			Debug.LogWarning( "You need to provide a valid particle renderer for the game object: " + gameObject.name );
		}
		
		// Get the particle animator. It will never be null since is a required component.
		particleAnimator = gameObject.GetComponent<ParticleAnimator>();
		if ( particleAnimator == null )
		{
			Debug.LogWarning( "You need to provide a valid particle animator for the game object: " + gameObject.name );
		}
		
		// Store initial animated colors, so we won't brake
		// animations been setup by the artist, since we might
		// want to apply an effect from the script, especially
		// to fade in/out during spawning/dying time.
		if ( particleAnimator && particleAnimator.doesAnimateColor )
		{
			particleColors = particleAnimator.colorAnimation;
		}
	}

	// Use this for initialization
	protected virtual void Start ()
	{
	}
	
	// Update physics
	protected virtual void FixedUpdate()
	{
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{	
	}
	
	/// <summary>
	/// Non-Unity functions. 
	/// </summary>
	
	// Activate the pawn.
	// Returns true if was deactive, false otherwise.
	protected virtual bool Activate()
	{
		if ( this.IsActive == false )
		{
			m_IsActive = true;
			return true;
		}
		
		return false;
	}
	
	// Deactivate the pawn.
	// Returns true if was active, false otherwise.
	protected virtual bool Deactivate()
	{
		if ( this.IsActive )
		{
			m_IsActive = false;
			return true;
		}
		
		return false;
	}
	
	// Move the pawn
	protected virtual void Move( Vector3 movement )
	{
		// NOTE: According to Unity documentation, a kinematic
		//		 body has to be moved by its transform component.
		if ( rigidbody && !rigidbody.isKinematic )
		{
			rigidbody.MovePosition( transform.localPosition + movement );
		}
		else
		{
			//transform.localPosition += movement;
			transform.Translate( movement, Space.World );
		}
	}
	
	// Rotate the pawn
	protected virtual void Rotate( float rotation )
	{
		// NOTE: According to Unity documentation, a kinematic
		//		 body has to be moved by its transform component.
		if ( rigidbody && !rigidbody.isKinematic )
		{
			rigidbody.MoveRotation( Quaternion.AngleAxis( rotation, Vector3.up ) );
		}
		else
		{
			transform.Rotate( new Vector3( 0.0f, rotation, 0.0f ) );
		}
	}
	
	// Spawn the pawn
	public virtual void Spawn()
	{
	}
	
	// Kill the actor
	public virtual void Kill()
	{
	}
	
	// Returns whether the pawn is active or not
	public bool IsActive
	{
		get { return m_IsActive; }
	}
}
