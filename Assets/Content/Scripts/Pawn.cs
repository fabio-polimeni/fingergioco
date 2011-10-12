using UnityEngine;
using System.Collections;

public class Pawn : MonoBehaviour
{
	protected 	float	m_Size;

	public 		float	SizeRate;
	public 		float	FinalSize;
	public 		float	Speed;

	// Use this when the level is loaded
	protected virtual void Awake()
	{
	}

	// Use this for initialization
	protected virtual void Start ()
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
