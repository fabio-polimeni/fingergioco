using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
	public GameObject			Surface;
	static public LevelManager	Ref;
	
	// Use this when the level is loaded
	void Awake()
	{
		Ref = this;
		DontDestroyOnLoad( this );
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
