using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("Objects")]
    private Camera m_camera;
    [SerializeField]
    private Animator m_animator;
    [SerializeField]
    private Transform m_spriteChild;
    
    [SerializeField, Tooltip("The speed at which the arrow will move in degrees per second.")]
    private float m_arrowSpeed = 0.5f;
    [SerializeField, Tooltip("The range of freedom for the arrow, in degrees.")]
    private float m_arrowRange = 140;
    [SerializeField]
    private float m_cameraLerpSpeed = 0.7f;


    [SerializeField, Header("Arrow")]
    private float m_degreesPerSecond = 180f;
    [SerializeField, Range(0, 4)]
    private float m_arrowDistance = 0.5f;

    private bool m_cameraLerp = false;
    private bool m_isDashing = false;
    private float m_arrowDirection;
    private String m_currentKey = "";
    private List<GameObject> m_collisionsList;
    private GameObject m_pivot;

    // Start is called before the first frame update
    void Start()
    {
        // 
        m_arrowDirection = 0f;
        m_pivot = transform.GetChild(0).gameObject;
        m_pivot.transform.GetChild(0).localPosition = new Vector3(0, m_arrowDistance, 0);
    }

    // Update is called once per frame
    void Update()
    {
        /**********PLAYER INPUT**********/
        if(Input.GetKeyDown(KeyCode.RightArrow))
            m_currentKey = "r";
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            m_currentKey = "l";
        if(Input.GetKeyUp(KeyCode.RightArrow) && m_currentKey.Equals("r"))
            m_currentKey = "";
        else if(Input.GetKeyUp(KeyCode.LeftArrow) && m_currentKey.Equals("l"))
            m_currentKey = "";


        if(Input.GetKeyDown(KeyCode.LeftControl))
            m_arrowDirection = -m_arrowDirection;

        m_arrowDirection = Mathf.Max(Mathf.Min(m_arrowDirection, m_arrowRange/2), -m_arrowRange/2);
        var rotVec = new Vector3(m_pivot.transform.eulerAngles.x, m_pivot.transform.eulerAngles.y, m_arrowDirection);
        m_pivot.transform.eulerAngles = rotVec;
    
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Dash(rotVec);
        }
        /**********END PLAYER INPUT**********/

        if(m_currentKey.Equals("r"))
            m_arrowDirection -= m_degreesPerSecond * Time.deltaTime;
        else if(m_currentKey.Equals("l"))
            m_arrowDirection += m_degreesPerSecond * Time.deltaTime;
    }
    private void FixedUpdate() 
    {
        if(m_isDashing)
        {
            // Getting x and y values of the vector using cosine and sine (cos = x, sin = y)
            transform.Translate(Mathf.Cos(Mathf.Deg2Rad * (m_spriteChild.eulerAngles.z + 90)) * m_speed * Time.deltaTime, 
                Mathf.Sin(Mathf.Deg2Rad * (m_spriteChild.eulerAngles.z + 90)) * m_speed * Time.deltaTime, 0);
        }
        if(m_cameraLerp)
        {
            MoveCamera();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(m_collisionsList.Contains(other.gameObject)) return;
        m_collisionsList.Add(other.gameObject);
        // TODO: Nice effect for collision
        m_animator.setBool("ShouldDash", false);
        // The same purple as the player:
        other.gameObject.GetComponent<SpriteRenderer>().color = new Color(118f/255f, 66f/255f, 138f/255f, 255f/255f);;
        m_cameraLerp = true;
        m_isDashing = false;
        m_spriteChild.gameObject.SetActive(false);
        transform.position = other.gameObject.transform.GetChild(0).position;
        m_pivot.SetActive(true);
    }

    private void MoveCamera()
    {
        Vector3 pos = m_camera.transform.position;
        Vector2 lerp = Vector2.Lerp((Vector2)pos, (Vector2)transform.position, m_cameraLerpSpeed * Time.deltaTime);
        m_camera.transform.position = new Vector3(lerp.x, lerp.y, pos.z);
        if(Vector2.Distance((Vector2)pos, (Vector2)transform.position) < 0.1) {
            m_camera.transform.position = new Vector3(transform.position.x, transform.position.y, pos.z);
            m_cameraLerp = false;
        }
    }

    private void Dash(Vector3 direction)
    {
        // Starts the animation, hides the arrow, sets the direction of the player sprite, and starts the dashing sequence with the boolean.
        m_animator.SetBool("ShouldDash", true);
        m_pivot.SetActive(false);
        m_spriteChild.eulerAngles = direction;
        m_isDashing = true;
        
        if(m_collisionsList.Count != 0)
        {
            // Reactivate the sprite and turn the shadow back to black
            m_spriteChild.gameObject.SetActive(true);
            m_collisionsList[m_collisionsList.Count - 1].GetComponent<SpriteRenderer>().color = Color.black;
        }
    }
}
