using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    public float moveSpeed;
    public GameObject detectionPoint;
    public Animator animator; 

    public float direction; // -1 when goint left, 1 when going right
    public LayerMask groundLayer;
    public bool changeDirection;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveSpeed * Time.deltaTime * direction, 0, 0); // changes direction when multiplied with -1
        transform.localScale = new Vector3(direction, 1, 1); // when changing direction the detection goes to the other side of the goomba
    }

    private void LateUpdate() 
    {
        Debug.DrawRay(detectionPoint.transform.position,Vector2.down, Color.green); // Only Debug: checks if goomba must change direction or not

        // Raycast is important thing to know how to do it.
        RaycastHit2D hit = Physics2D.Raycast(detectionPoint.transform.position, Vector2.down, 1, groundLayer);
        if (hit.collider == null)
        {
            // ray didn't hit anything, so we must change direction
            ChangeDirection();

            Debug.Log("Change direction edge");
            //changeDirection = true;
        }


        RaycastHit2D hit2 = Physics2D.Raycast(detectionPoint.transform.position, Vector2.right * direction, 0.2f, groundLayer);
        if (hit2.collider != null)
        {
            // Ray hits something like a wall. So we change direction
            //ChangeDirection();

            Debug.Log("Change direction wall");
            changeDirection = true;
        }

        /*if(changeDirection == true)
        {
            ChangeDirection();
        }*/
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.transform.position.y > transform.position.y + collision.collider.bounds.size.y / 2)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(collision.gameObject.GetComponent<Rigidbody2D>().velocity.x ,5);
                Die();
            } else 
            {
                //Mario dies. Call Mario Die function here. 
                collision.gameObject.GetComponent<MarioControl>().MarioDie();
            }
        }
    }

    void ChangeDirection() // walk
    {
        direction *= -1; 
    }

    public void Die() // public because MarioControl class is going to call the class 
    {
        animator.SetTrigger("Die");
        moveSpeed = 0;
        // Destroy components
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(gameObject, 1);
    }
}
