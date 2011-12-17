using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody),
				   typeof(MeshCollider))]

public class Pawn : MonoBehaviour
{
	/// <summary>
	/// Properties.
	/// </summary>
	
	protected	bool	m_IsActive;
	protected	float	m_ParticleMinSize;
	protected	float	m_ParticleMaxSize;

	public 		float	finalSize;
	
	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	protected virtual void Awake()
	{		
		// Disable the mesh render if exists.
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if ( meshRenderer )
		{
			Object.Destroy( meshRenderer );
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
