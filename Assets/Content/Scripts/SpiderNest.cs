using UnityEngine;
using System.Collections;

public class SpiderNest : MonoBehaviour
{
	/// <summary>
	/// Properties.
	/// </summary>

    // Spider prefab.
    public GameObject SpiderPrefab;
	
	// Number of spiders
	public int NumberOfSpiders;
	
	// Spiders
	private Spider[] m_Spiders;

    // Particle size
    private float m_Size;

    // Whether or not it has been touched already.
    private bool m_Touched;
    public bool IsTouched
    {
        get { return m_Touched; }
    }
	
	// Whether or not it has been attached to the scene manager.
    private bool m_Attached;
    public bool IsAttached
    {
        get { return m_Attached; }
    }
	
	// The index the we has been attached to the scene manager.
    private int m_SceneIndex;
    public int SceneIndex
    {
        get { return m_SceneIndex; }
    }

    // Returns whether the pawn is active or not
    protected bool m_IsActive;
    public bool IsActive
    {
        get { return m_IsActive; }
    }
	
	
    protected ParticleAnimator particleAnimator = null;
    protected ParticleRenderer particleRenderer = null;

	/// <summary>
	/// Unity functions.
	/// </summary>

	// Use this when the level is loaded
	void Awake()
	{
		// Set layer and tag
		gameObject.layer = LayerMask.NameToLayer("Enemy");
        gameObject.tag = "Web";
       
        m_IsActive = false;
		m_Touched = false;

        particleAnimator = GetComponent<ParticleAnimator>();
        particleRenderer = GetComponent<ParticleRenderer>();
        if (particleEmitter && particleAnimator && particleRenderer)
        {
            particleEmitter.enabled = false;
            particleEmitter.emit = false;
            particleEmitter.useWorldSpace = false;

            particleAnimator.autodestruct = false;
            particleAnimator.doesAnimateColor = false;
        }
		
		// We need at leat one spider
		NumberOfSpiders = Mathf.Min( 1, NumberOfSpiders );
		m_Spiders = new Spider[NumberOfSpiders];
    }

	// Use this for initialization
	void Start()
	{
		// Signal the game manager that this web is loaded.
		// We insert this object into the first free slot.
		foreach ( Web web in GameManager.Webs )
		{
			if ( web == null )
			{
				m_SceneIndex = GameManager.Webs.Length;
				m_Attached = true;
				GameManager.Webs[m_SceneIndex] = this;
			}
		}
	}

	// Update physics
	void FixedUpdate()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

    // Something exited the collider
    void OnTriggerExit(Collider other)
    {
    }
	
	// Something entered the collider
	void OnTriggerEnter(Collider other)
	{

	}

    // Something is inside the collider
    void OnTriggerStay(Collider other)
    {
        // If the player is in its area,
        // then, start the spawn effect.
        if (other.gameObject.tag == "Finger")
        {
            this.Activate();
        }
    }

	/// <summary>
	/// Non-Unity functions. 
	/// </summary>

	// Activate the powerup
	protected bool Activate()
	{
        if (this.IsActive == false)
        {
            if (particleEmitter && particleRenderer)
            {
                particleEmitter.enabled = true;

                particleEmitter.Emit(Vector3.zero, Vector3.zero,
                    particleEmitter.maxSize, particleEmitter.maxEnergy,
                    Color.white, 0.0f, 0.0f);
            }

            m_IsActive = true;
        }

        return m_IsActive;
	}

    // Deactivate
    protected void Deactivate()
    {
        m_IsActive = false;
        if (particleEmitter)
        {
            particleEmitter.enabled = false;
            particleEmitter.emit = false;
        }
    }

}
