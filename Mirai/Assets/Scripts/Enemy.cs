using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f; // Speed at which the enemy follows the player
    private Transform player; // Reference to the player's transform
    private Animator animator; // Reference to the Animator component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    [SerializeField] PlayerController playerController;
    private bool hasCollided = false;
    [SerializeField] private GameObject endObject;
    private bool isChasing = true;

    void Start()
    {
        // Find the player object by tag and store its transform
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Get references to the Animator and SpriteRenderer components
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start the chasing coroutine with a 2-second delay
        StartCoroutine(StartChasing());
    }

    private IEnumerator StartChasing()
    {
        // Wait for 2 seconds before starting the chase
        yield return new WaitForSeconds(2f);

        // Start the chase behavior
        StartCoroutine(ChasePlayer());
    }

    private IEnumerator ChasePlayer()
    {
        while (true)
        {
            // Calculate the new position on the x-axis
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.MoveTowards(transform.position.x, player.position.x, speed * Time.deltaTime);

            // Determine if the enemy is moving
            if (Mathf.Abs(newPosition.x - transform.position.x) > 0.01f)
            {
                // Set the canWalk animation to true when moving
                animator.SetBool("canWalk", true);

                // Flip the sprite based on the direction of movement
                if (newPosition.x > transform.position.x)
                {
                    spriteRenderer.flipX = true; // Facing right
                }
                else if (newPosition.x < transform.position.x)
                {
                    spriteRenderer.flipX = false; // Facing left
                }
            }
            else
            {
                // Set the canWalk animation to false when not moving
              //  animator.SetBool("canWalk", false);
            }

            // Update the enemy's position
            transform.position = newPosition;

            // Wait until the next frame to update the position
            yield return null;
        }
    }
    public void Hide()
    {
        StopAllCoroutines();
        isChasing = false;
        StartCoroutine(HideCoroutine());
    }

    private IEnumerator HideCoroutine()
    {
        // Step 1: Move toward the object with the "barrier" tag
        GameObject barrier = GameObject.FindGameObjectWithTag("barrier");
        if (barrier != null)
        {
            while (Vector3.Distance(transform.position, barrier.transform.position) > 0.1f)
            {
                Vector3 newPosition = transform.position;
                newPosition.x = Mathf.MoveTowards(transform.position.x, barrier.transform.position.x, speed * Time.deltaTime);
                transform.position = newPosition;

                yield return null;
            }
        }
        else
        {
            Debug.LogError("Barrier object not found");
            yield break;
        }

        // Wait a brief moment to ensure the transition is smooth (optional)
        yield return new WaitForSeconds(0.2f);

        // Proceed to endCoroutine
        StartCoroutine(endCoroutine());
    }
    private IEnumerator endCoroutine()
    {

        
        spriteRenderer.flipX = true;
        GameObject end = GameObject.Find("end");
        if (end != null)
        {
            while (Vector3.Distance(transform.position, end.transform.position) > 0.1f)
            {
                Vector3 newPosition = transform.position;
                newPosition.x = Mathf.MoveTowards(transform.position.x, end.transform.position.x, speed * Time.deltaTime);
                transform.position = newPosition;

                // Corrected sprite flipping based on movement direction
    
                yield return null;
            }
        }
        else
        {
            Debug.LogError("End object not found");
            yield break;
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasCollided && isChasing)
        {
            if (collision.CompareTag("Player"))
            {

                if (playerController != null)
                {
                    playerController.DeadEnd();
                    hasCollided = true;
                }
            }
        }
   
             if (collision.CompareTag("barrier"))
            {

            endObject.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(endCoroutine());
        }


        if (collision.CompareTag("end"))
        {
            isChasing = true;
            gameObject.SetActive(false);
        }

    }
}
