using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioControl : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    
    public Animator animator;
    private Rigidbody2D rb2D;

    public bool grounded;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundDetect();

        transform.Translate(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, 0); // movement character

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.localScale = new Vector3(Input.GetAxisRaw("Horizontal"), 1, 1);
            animator.SetBool("Walk", true);
        } else 
        {
            animator.SetBool("Walk", false);
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            animator.SetBool("Jump", true);
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
        } 

        // better jump code
        if (rb2D.velocity.y < 0)
        {
            rb2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb2D.velocity.y > 0 && !Input.GetButton("Jump")) 
        {
            rb2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }


    public void MarioDie()
    {
        Debug.Log("Mario is dead!");
        // Display dead image of Mario
        // Stop time
        // Bounce mario up
        // Stop all controls from player
        // Stop camera to move
        // Destroy Mario in 6 seconds
        // need to be able to continue time

        animator.SetTrigger("Die");
        rb2D.velocity = new Vector2(0, 10); // Bounce
        Destroy(GetComponent<BoxCollider2D>());
        moveSpeed = 0; // user cannot move Mario
        Camera.main.transform.parent = null; // Camera does not follow Mario anymore
        Destroy(gameObject, 6); // destroy Mario in 6 seconds



        StartCoroutine("ContinueTime"); // Start coroutine, that waits for 1 second and then put time back to normal
        Time.timeScale = 0; // Stops the game

    }

    IEnumerable ContinueTime()
    {
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1; // Back to normal running
        yield return new WaitForSecondsRealtime(4); 
        // Run here method that restarts the level
        Debug.Log("LEVEL RESTART");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GroundDetect()
    {
        Vector3 boxPosition = transform.position;
        RaycastHit2D rayHit = Physics2D.BoxCast(boxPosition, new Vector2(1.2f, 0.2f), 0, Vector2.zero, 0, LayerMask.GetMask("Ground", "Enemy"));

        if (rayHit == true)
        {
            grounded = true;
            animator.SetBool("Jump", false);
        } else
        {
            grounded = false; // Mario is in the air
            animator.SetBool("Jump", true);
        }
    }

    private void OnDrawGizmos() 
    {
        Vector3 boxPosition = transform.position;
        Gizmos.DrawWireCube(boxPosition, new Vector2(1.2f, 0.2f));
    }

}
