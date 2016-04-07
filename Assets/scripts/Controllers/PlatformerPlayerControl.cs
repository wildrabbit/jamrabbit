using UnityEngine;
using System.Collections;

// References:
// - Deranged hermit's blog
// 

public static class RaycastLayers
{
    public static readonly int normalCollisions;
    public static readonly int upRay;
    public static readonly int downRay;  

    static RaycastLayers()
    {
        normalCollisions = 1 << LayerMask.NameToLayer("Ground")
            | 1 << LayerMask.NameToLayer("SoftTop")
            | 1 << LayerMask.NameToLayer("SoftBottom");
        upRay = 1 << LayerMask.NameToLayer("Ground")
            | 1 << LayerMask.NameToLayer("SoftTop");
        downRay = 1 << LayerMask.NameToLayer("Ground")
            | 1 << LayerMask.NameToLayer("SoftBottom");
    }
}

public class PlatformerPlayerControl : MonoBehaviour 
{
    private const float SQRT_2 = 1.41421356237f;

    [Header("Collision tweaks")]
    public LayerMask m_groundTerrain;
    public int m_vertRaysCount = 3;
    public int m_horzRaysCount = 3;
    public float m_angleLeeway = 5.0f; 

    [Header("Jump configuration")]
    [Tooltip("Allow changing x-movement mid-air")]
    public bool m_steerWhileJumping;

    [Tooltip("Max. number of units the player will jump")]
    public float m_maxHeight = 5.0f;
    
    [Tooltip("Time to reach max height")]
    public float m_maxJumpTime = 1.0f;
    
    [Tooltip("Min. number of units the player will jump")]
    public float m_minHeight = 2.0f;
    
    [Tooltip("Max. jump length")]
    public float m_maxJumpLength = 8.0f;

    [Tooltip ("Consecutive jump limit (2 => double jump)")]
    public int m_maxJumps = 1;

    [Tooltip("Max. fall speed")]
    public float m_maxFallSpeed = 30.0f;

    [Header("Walk configuration")]
    public float m_acceleration = 0.0f;
    public float m_maxSpeed = 10.0f;
    public float m_drag = 10.0f;
    public float m_velocityThreshold = 0.5f;

    
    //------------------------------------
    private float m_gravity;
    private float m_initialJumpSpeed;
    private float m_jumpTermVelocity;
    //private float m_jumpTermTime;
    //------------------------------------

    //private Rect m_collisionArea;

    private bool m_wasGrounded;
    private bool m_grounded;

    private bool m_jumping;
    private bool m_wasJumping;
    private int m_jumpCount;
    //private float m_jumpingTime;
    //private bool m_onAir;


    private bool m_facingRight;

    private bool m_falling;
    private bool m_wasFalling;

    private float m_jumpHeight;
    
    private Vector2 m_velocity;

    //----------
    //private Ray[] m_groundRays;
    private BoxCollider2D m_collider;

	// Use this for initialization
	void Start () 
    {
        m_velocity = Vector2.zero;

        //if (transform.position.y < 0.0f)
        //{
        //    transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        //}
        CalculateJumpVars();
        DetectGround();
        m_wasGrounded = m_grounded;
        m_jumping = m_wasJumping = false;
        m_jumpHeight = transform.position.y;

        m_falling = m_wasFalling = !m_grounded;

        m_facingRight = true;
        m_jumpCount = 0;
        
        m_collider = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector2 impulse = Vector2.zero;
        impulse.x = Input.GetAxis("Horizontal");
        
        m_wasGrounded = m_grounded;
        m_wasJumping = m_jumping;
        m_wasFalling = m_falling;

        bool jumpAllowed = !DetectCeiling();

        // Ground checks
        DetectGround();
        
        // Y-movement
        if (!m_grounded)
        {
            m_velocity.y -= m_gravity * Time.deltaTime;
            if (m_velocity.y < -m_maxFallSpeed)
            {
                m_velocity.y = -m_maxFallSpeed;
            }
        }
        else
        {
            m_velocity.y = 0.0f;
            m_jumpCount = 0;
        }


        if (Input.GetButtonUp("Jump"))
        {
            if (m_velocity.y > m_jumpTermVelocity)
            {
                m_velocity.y = m_jumpTermVelocity;
            }
        }
        else
        {
            m_jumping = Input.GetButtonDown("Jump");

            if ((m_jumping && !m_wasJumping) && (m_grounded || m_jumpCount < m_maxJumps))
            {
                m_velocity.y = m_initialJumpSpeed;
                m_jumpCount++;
            }
        }
        m_falling = m_velocity.y < 0.0f;

        // Lateral checks:
        // Constant speed, don't do anything special
        float oldVelocity = m_velocity.x;
        if (Mathf.Approximately(m_acceleration, 0.0f))
        {
            m_velocity.x = impulse.x * m_maxSpeed;
        }
        else // Accelerated movement. Factor drag, too.
        {
            bool directionChange = Mathf.Sign(m_velocity.x) != Mathf.Sign(impulse.x) && !Mathf.Approximately(impulse.x, 0.0f);
            if (directionChange /*|| inputVector.Equals(Vector2.zero) */)
            {
                m_velocity.x = 0.0f;
            }

            if (Mathf.Abs(impulse.x) < 0.1f)
            {
                if (Mathf.Abs(m_velocity.x) > m_velocityThreshold)
                {
                    Vector2 dragVector = Vector2.right * ((m_velocity.x > 0.0f) ? -1 : (m_velocity.x < 0.0f) ? 1 : 0);
                    m_velocity.x += m_drag * Time.deltaTime * dragVector.x;
                }

                if (Mathf.Abs(m_velocity.x) < m_velocityThreshold)
                {
                    m_velocity.x = 0.0f;
                }
            }
            else
            {
                m_velocity.x += m_acceleration * impulse.x * Time.deltaTime;
                if (Mathf.Abs(m_velocity.x) >= m_maxSpeed)
                {
                    m_velocity.x = m_maxSpeed * ((m_velocity.x > 0.0f) ? 1 : (m_velocity.x < 0.0f) ? -1 : 0);
                }
            }
        }
        DetectWalls();
    }

    private bool DetectCeiling()
    {
        if (m_grounded || m_velocity.y > 0.0f)
        {
            float distance = m_collider.size.y * 0.5f + m_velocity.y * Time.deltaTime;
            bool foundCeil = false;
            Vector2 origin = (Vector2)transform.position + m_collider.offset;
            origin.x -= m_collider.size.x * 0.5f;
            float delta = m_collider.size.x / (m_vertRaysCount - 1);
            RaycastHit2D[] upRays = new RaycastHit2D[m_vertRaysCount];
            
            int lastIdx = -1;
            float minDistance = Mathf.Infinity;
            for (int i = 0; i < m_vertRaysCount; ++i )
            {
                upRays[i] = Physics2D.Raycast(origin, Vector2.up, distance, RaycastLayers.upRay);
                if (upRays[i].collider != null)
                {
                    foundCeil = true;
                    if (upRays[i].distance < minDistance)
                    {
                        minDistance = upRays[i].distance;
                        lastIdx= i;
                    }

                }
                origin.x += delta;
            }
            
            if (foundCeil)
            {
                transform.Translate(Vector2.up * (upRays[lastIdx].distance - m_collider.size.y));
                m_velocity.y = 0.0f;
                return true;
            }
        }
        return false;
    }
    private bool DetectWalls()
    {
        float absVelocity = Mathf.Abs(m_velocity.x);
        int signVelocity = m_velocity.x > 0 ? 1 : (m_velocity.x < 0) ? -1 : 0;
        float collH = m_collider.size.y;
        float collW = m_collider.size.x;
        float collHW = collW * 0.5f;
        float collHH = collH * 0.5f;

        RaycastHit2D[] raycasts = new RaycastHit2D[m_horzRaysCount];

        if (absVelocity > m_velocityThreshold)
        {
            Vector2 origin = (Vector2)transform.position + m_collider.offset;
            Vector2 direction = m_velocity.x > 0 ? Vector2.right : Vector2.left;

            const float heightScale = 0.8f;
            const float margin = (1 - heightScale) * 0.5f;

            origin.y += (collHH - margin * collH); // the collider is already offset in the y axis

            float rayDelta = (collH * heightScale) / (m_horzRaysCount - 1);
            float rayDistance = (collHW + absVelocity * Time.deltaTime) * 1.1f;
            Vector3 pos = transform.position;
            float lastFraction = 0.0f;
            int numHits = 0;
            for (int i = 0; i < m_horzRaysCount; ++i)
            {
                //Debug.DrawRay(origin, direction * rayDistance, Color.cyan, 0.4f);
                raycasts[i] = Physics2D.Raycast(origin, direction, rayDistance, RaycastLayers.normalCollisions);
                if (raycasts[i].collider != null)
                {
                    if (lastFraction > 0.0f)
                    {
                        float angle = Vector2.Angle(raycasts[i].point - raycasts[i - 1].point, Vector2.right);
                        if (Mathf.Abs(angle - 90.0f) < m_angleLeeway)
                        {
                            transform.Translate(direction * (raycasts[i].distance - collHW));
                            transform.position = pos;

                            bool newFacingRight = m_velocity.x > 0.0f || (m_facingRight && Mathf.Abs(m_velocity.x) < m_velocityThreshold);
                            if ((newFacingRight && !m_facingRight) || (!newFacingRight && m_facingRight))
                            {
                                Vector3 scale = transform.localScale;
                                m_facingRight = newFacingRight;
                                scale.x = Mathf.Abs(scale.x) * (m_facingRight ? 1 : -1);
                                transform.localScale = scale;
                            }

                            m_velocity.x = 0.0f;
                            return true;        
                        }
                    }
                    numHits++;
                    lastFraction = raycasts[i].fraction;
                    
                }
                origin.y -= rayDelta;                
            }
        }
        return false;
    }

    private void DetectGround()
    {
        if (!m_grounded && !m_falling) return;

        bool foundGround = false;
        Vector2 origin = (Vector2)transform.position + m_collider.offset;
        origin.x -= m_collider.size.x * 0.5f;
        RaycastHit2D[] raycasts = new RaycastHit2D[m_vertRaysCount];
        float minDistance = Mathf.Infinity;
        int idx = -1;

        float rayDelta = m_collider.size.x / (m_vertRaysCount - 1);
        float rayDistance = m_collider.size.y * 0.65f;
        for (int i = 0; i < m_vertRaysCount; ++i)
        {
            //-Debug.DrawRay(origin, Vector2.down * rayDistance, Color.green, 0.4f);
            raycasts[i] = Physics2D.Raycast(origin, Vector2.down, rayDistance, RaycastLayers.normalCollisions);
            if (raycasts[i].collider != null)
            {
                foundGround = true;
                if (raycasts[i].distance < minDistance)
                {
                    minDistance = raycasts[i].distance;
                    idx = i;
                }
            }
            origin.x += rayDelta;
        }
        if (foundGround)
        {
            transform.Translate(Vector2.down * (minDistance - m_collider.size.y * 0.5f));

            m_velocity.y = 0.0f;
            m_jumpCount = 0;
            m_grounded = true;
            m_falling = false;
        }
        else
        {
            m_grounded = false;
        }

    }

    bool TestSlope(RaycastHit2D ray)
    {
        float angle = Vector2.Angle(ray.normal, Vector2.right);
        return ray.collider != null && Mathf.Abs(angle - 90.0f) > m_angleLeeway;
    }

    void LateUpdate()
    {
        if (m_velocity.magnitude > m_velocityThreshold * SQRT_2)
        {
            bool newFacingRight = m_velocity.x > 0.0f || (m_facingRight && Mathf.Abs(m_velocity.x) < m_velocityThreshold);
            if ((newFacingRight && !m_facingRight) || (!newFacingRight && m_facingRight))
            {
                Vector3 scale = transform.localScale;
                m_facingRight = newFacingRight;
                scale.x = Mathf.Abs(scale.x) * (m_facingRight ? 1 : -1);
                transform.localScale = scale;
            }

            Vector2 finalPosition = (Vector2)transform.position + (m_velocity * Time.deltaTime);

            transform.position = finalPosition;
        }

    }

    private void CalculateJumpVars()
    {
        m_gravity = 2 * m_maxHeight / (m_maxJumpTime * m_maxJumpTime);
        m_initialJumpSpeed = Mathf.Sqrt(2 * m_gravity * m_maxHeight);

        m_jumpTermVelocity = Mathf.Sqrt(m_initialJumpSpeed * m_initialJumpSpeed - 2 * m_gravity * (m_maxHeight - m_minHeight));
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CarrotTest>() != null)
        {
            collision.GetComponent<CarrotTest>().KillMe();
        }
    }
}
