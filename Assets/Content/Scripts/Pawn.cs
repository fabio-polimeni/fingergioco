using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody),
                   typeof(MeshCollider))]

public class Pawn : MonoBehaviour
{
	protected 	float	m_Size;
	
	public		float	approachSpeed;
	public 		float	finalSize;

	// Use this when the level is loaded
	protected virtual void Awake()
	{
	}

	// Use this for initialization
	protected virtual void Start ()
	{
		// Make sure we start the pawn in kinematic mode
		rigidbody.isKinematic = true;
	}
	
	// Update physics
	protected virtual void FixedUpdate()
	{
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{	
	}
	
	// Spawn the pawn
	public virtual void Spawn()
	{
	}
	
	// Kill the actor
	public virtual void Kill()
	{
	}
}
