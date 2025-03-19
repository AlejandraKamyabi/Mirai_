using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Movement speed of the player
    private float originalSpeed;
    public float runSpeed = 10f; 
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isInteracting = false;
    [SerializeField] private GameObject jar;
    [SerializeField] private GameObject enemy;
    [SerializeField] CursorController cursorController;
    [SerializeField] Enemy enemy_script;
    public static bool enemyShouldBeActive = false;
    private bool isFrame = false;
    private bool isCage = false;
    private bool isSink = false;
    private bool isCloset = false;
    private bool isBedroom = false;
    private bool isOverHideBed = false;
    private bool closetDone = false;
    private bool isPinkDoor = false;
    private bool initial_ = false;
    private bool isLabRoom = false;
    private bool isHallDoor = false;
    private bool isTable = false;
    private bool isJar = false;
    private bool bleed = false;
    public GameObject frame;
    public GameObject cage;
    public GameObject closet;
    public GameObject bloodySink;
    public GameObject loading;
    public GameObject deadEnd;
    private float currentSpeed;
    public GameObject bed;
    private bool isTv;
    private bool isMirror = false;
    private bool isDoor= false;
    private float lastClickTime = 0f;
    private float catchTime = 0.25f; // Time threshold for double click
    private Pathfinding pathfinding; // Reference to your Pathfinding script
    private Vector3[] path; // The calculated path
    private int targetIndex = 0; // Index in the path array

    private Animator animator; // Reference to the Animator component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private TVInteraction tvInteraction; // Reference to the TVInteraction script

    public Text dialogueText; // Reference to the UI Text component for displaying dialogue
    public string dialogueMessage = "This is a sample dialogue that will be typed out letter by letter."; // The dialogue message to display
    public float typingSpeed = 0.0f; // Speed at which each letter is typed (adjust this for slower speed)

    void Start()
    {
        originalSpeed = speed;
        pathfinding = FindObjectOfType<Pathfinding>();
        if (pathfinding == null)
        {
            Debug.LogError("Pathfinding component not found in the scene.");
        }

        isTv = true;
        if (enemyShouldBeActive && enemy != null)
        {
            enemy.SetActive(true);
        }
    
    animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tvInteraction = FindObjectOfType<TVInteraction>();
        if (tvInteraction == null && SceneManager.GetActiveScene().name == "room_1")
        {
            Debug.LogError("TVInteraction component not found in the scene.");
        }

        // Ensure the dialogueText is empty at the start
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
        if(SceneManager.GetActiveScene().name == "room_1")
        {

            StartCoroutine(FirstFear());
        }
    }
    public void ActivateEnemy()
    {
        enemyShouldBeActive = true;
        if (enemy != null)
        {
            enemy.SetActive(true);
        }
    }
    public void DActivateEnemy()
    {
        enemyShouldBeActive = false;
        if (enemy != null)
        {
            enemy.SetActive(false);
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isInteracting)
        {
            HandleClick();
        }

        if (isMoving)
        {
            MoveAlongPath();
        }
        else
        {
            animator.SetBool("canWalk", false); // Stop walking animation
            animator.SetBool("canRun", false);  // Stop running animation
        }
       if (enemy.activeSelf)
       {
           cursorController.canInteract(true);
            cursorController.enemy_Active(true);
       }
       else
       {
           cursorController.canInteract(false);
       }
        if (isInteracting)
        {
            cursorController.canInteract(true);
            MoveAlongXAxis(targetPosition);
        }
    }
    public void DeadEnd()
    {

        isMoving = false;
        speed = 0f; 


        StartCoroutine(HandleDeadEnd());
    }

    private IEnumerator HandleDeadEnd()
    {
   
        animator.SetBool("isDead", true);
        spriteRenderer.flipX = false;
        yield return new WaitForSeconds(3f);
        deadEnd.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(3f); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
    private IEnumerator FirstFear()
    {
        cursorController.gameObject.SetActive(false); 
        Cursor.visible = false;
        initial_ = true;
        yield return StartCoroutine(TypeDialogue("Hina and Tsukasa are not home yet..."));

    

        yield return StartCoroutine(TypeDialogue("I'm so hungry i wonder if i can find anything to eat"));



        //yield return StartCoroutine(TypeDialogue("I just moved to this new place."));
        //
        //yield return new WaitForSeconds(1f);
        //
        //yield return StartCoroutine(TypeDialogue("I live with a couple, hina and tsukasa and they always work so late"));
        //
        //yield return new WaitForSeconds(1f);
        dialogueText.text = "";

        cursorController.gameObject.SetActive(true);
        Cursor.visible = true;
        initial_ = false;
    }

    private void HandleClick()
    {
        
        if (isInteracting || initial_)
        {
            return;
        }

        float timeSinceLastClick = Time.time - lastClickTime;
        if (timeSinceLastClick <= catchTime || enemy.activeSelf)
        {
            speed = runSpeed;
            animator.SetBool("canRun", true);  // Start running animation
            animator.SetBool("canWalk", false); // Ensure walking animation is not playing
        }
        else
        {
            speed = originalSpeed;
            animator.SetBool("canRun", false);  // Ensure running animation is not playing
            animator.SetBool("canWalk", true); // Start walking animation
        }

        lastClickTime = Time.time;

        Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickPosition.z = 0;

        // Check if the clickPosition is within grid boundaries
        Node targetNode = pathfinding.grid.NodeFromWorldPoint(clickPosition);
        if (!targetNode.walkable)
        {
            Debug.Log("Clicked outside of walkable area.");
            return;
        }

        targetPosition = new Vector3(clickPosition.x, transform.position.y, transform.position.z);

        if (pathfinding != null)
        {
            path = pathfinding.FindPath(transform.position, targetPosition);
        }
        else
        {
            Debug.LogError("Pathfinding component not found!");
        }

        if (path != null && path.Length > 0)
        {
            targetIndex = 0;
            isMoving = true;
        }
    }
    

    void MoveAlongPath()
    {
        if (path == null || targetIndex >= path.Length)
        {
            isMoving = false;
            animator.SetBool("canWalk", false); // Stop walking animation
            animator.SetBool("canRun", false);  // Stop running animation
            return;
        }

        Vector3 nextPoint = path[targetIndex];
        nextPoint.y = transform.position.y; // Keep the player's y position constant

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, speed * Time.deltaTime);

        // If moving left, flip the sprite; if moving right, ensure it's not flipped
        if (nextPoint.x < transform.position.x)
        {
            spriteRenderer.flipX = true; // Flip sprite for left direction
        }
        else if (nextPoint.x > transform.position.x)
        {
            spriteRenderer.flipX = false; // Default for right direction
        }

        if (Vector3.Distance(transform.position, nextPoint) < 0.1f)
        {
            targetIndex++;
        }
    }

    void MoveAlongXAxis(Vector3 targetPos)
    {
        cursorController.gameObject.SetActive(false);
        Cursor.visible = false;
        initial_ = true;
        speed = originalSpeed;
        if (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            // Flip sprite based on direction
            if (targetPos.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else if (targetPos.x > transform.position.x)
            {
                spriteRenderer.flipX = false;
            }

            animator.SetBool("canWalk", true); // Ensure walking animation is playing
        }
        else 
        {
            animator.SetBool("canWalk", false); // Stop walking animation
            
            if (isTv == true)
            {
                StartVerticalMovement();
            }
            if (isTv == false)
            {
                StartCoroutine(HandleTableInteraction());
            }
            isInteracting = false; // End interaction movement
            cursorController.canInteract(false);
            cursorController.gameObject.SetActive(true);
            Cursor.visible = true;
            cursorController.cursor_reset();
            initial_ = false;
        }
    }

    public void StartTVInteractionSequence()
    {
        GameObject tv = GameObject.FindWithTag("TVInteraction");
        if (tv != null)
        {
            targetPosition = new Vector3(tv.transform.position.x, transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = false; // Stop any current pathfinding movement
            animator.SetBool("canWalk", true); // Start walking animation
        }
    }
    public void BathroomSinkSequence()
    {
        GameObject tv = GameObject.FindWithTag("bathroom_sink");
        if (tv != null)
        {
            targetPosition = new Vector3(tv.transform.position.x, transform.position.y, transform.position.z);
            isInteracting = true;
            isSink = true;
            isMoving = false; // Stop any current pathfinding movement
            animator.SetBool("canWalk", true); // Start walking animation
        }
    }
    public void ClosetSequence()
    {
        GameObject tv = GameObject.FindWithTag("closet");
        if (tv != null)
        {
            targetPosition = new Vector3(tv.transform.position.x, transform.position.y, transform.position.z);
            isInteracting = true;
            isSink = false;
            isCloset = true;
            isMoving = false; // Stop any current pathfinding movement
            animator.SetBool("canWalk", true); // Start walking animation
        }
    }
    public void Hide_Bed()
    {
        GameObject tv = GameObject.FindWithTag("hide_bed");
        if (tv != null)
        {
            targetPosition = new Vector3(tv.transform.position.x - 2f, transform.position.y, transform.position.z);
            isInteracting = true;
            isSink = false;
            isOverHideBed= true;
            isMoving = false; // Stop any current pathfinding movement
            animator.SetBool("canWalk", true); // Start walking animation
        }
    }
    private void StartVerticalMovement()
    {
        animator.SetBool("canGoUp", true);
        
        if (isDoor == true)
        {
            Vector3 tvPosition = GameObject.FindWithTag("Door_Bedroom").transform.position;
            Vector3 verticalTarget = new Vector3(tvPosition.x, tvPosition.y, transform.position.z);
            float yOffset = tvPosition.y - transform.position.y;

            StartCoroutine(MoveVertically(verticalTarget, yOffset));
        }

        else if (isSink == true)
        {
            Vector3 tvPosition = GameObject.FindWithTag("bathroom_sink").transform.position;
            Vector3 verticalTarget = new Vector3(tvPosition.x, tvPosition.y, transform.position.z);
            float yOffset = tvPosition.y - transform.position.y;

            StartCoroutine(MoveVertically(verticalTarget, yOffset));
        }
        else if (isCloset == true)
        {
            Vector3 tvPosition = GameObject.FindWithTag("closet").transform.position;
            Vector3 verticalTarget = new Vector3(tvPosition.x, tvPosition.y - 2f, transform.position.z);
            float yOffset = tvPosition.y - transform.position.y;

            StartCoroutine(MoveVertically(verticalTarget, yOffset));
        }
        else if (isOverHideBed == true)
        {
            animator.SetBool("canGoUp", false);
            Vector3 tvPosition = GameObject.FindWithTag("hide_bed").transform.position + new Vector3(-2f, 0, 0);
            Vector3 verticalTarget = new Vector3(tvPosition.x, tvPosition.y, transform.position.z);
            float yOffset = tvPosition.y - transform.position.y;

            StartCoroutine(MoveVertically(verticalTarget, yOffset));
        }
        else
        {
            Vector3 tvPosition = GameObject.FindWithTag("TVInteraction").transform.position;
            Vector3 verticalTarget = new Vector3(tvPosition.x, tvPosition.y, transform.position.z);
            float yOffset = tvPosition.y - transform.position.y;

            StartCoroutine(MoveVertically(verticalTarget, yOffset));
        }
    }
    private IEnumerator MoveVertically(Vector3 targetPos, float yOffset)
    {
        float verticalSpeed = 4f; // Speed for vertical movement
        speed = verticalSpeed; 

        yield return new WaitForSeconds(1.5f);
        Vector3 originalScale = transform.localScale;
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            if (isOverHideBed)
            {

                animator.SetBool("canHide_Bed", true);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if (isDoor)
            {
               
                Vector3 newScale = transform.localScale * 0.9999f; 
                transform.localScale = newScale;

            }
            yield return null;
        }

        if (isDoor == true)
        {
            animator.SetBool("canGoUp", false);
            loading.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(1f);
            transform.localScale = originalScale;
            SceneManager.LoadScene("room_2");
        }
        else if (isSink == true)
        {
            animator.speed = 0;
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(TypeDialogue("!!!"));

            yield return new WaitForSeconds(1f);

            bloodySink.GetComponent<SpriteRenderer>().enabled = true;

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(TypeDialogue("is this.... is this blood?!!"));

            yield return new WaitForSeconds(2f);

            bloodySink.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(1f);
            animator.speed = 1;
            yield return new WaitForSeconds(1f);
            isSink = false;
            dialogueText.text = "";
            Vector3 originalYPosition = new Vector3(targetPos.x, targetPos.y - yOffset, transform.position.z);
            while (Vector3.Distance(transform.position, originalYPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalYPosition, speed * Time.deltaTime);
                yield return null;
            }

            speed = originalSpeed; // Reset speed to original
            animator.SetBool("canGoUp", false); // Stop the canGoUp animation
                                                // Move a bit along the x-axis
            Vector3 horizontalTarget = new Vector3(transform.position.x - 6f, transform.position.y, transform.position.z);
            while (Vector3.Distance(transform.position, horizontalTarget) > 0.1f)
            {
                spriteRenderer.flipX = true;
                transform.position = Vector3.MoveTowards(transform.position, horizontalTarget, speed * Time.deltaTime);
                animator.SetBool("canWalk", true); // Ensure walking animation is playing
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            animator.SetBool("canWalk", false);
            yield break;

        }
        else if (isCloset == true)
        {
            GameObject painting = GameObject.Find("closet");
            if (painting != null)
            {
                Animator mirrorAnimator = painting.GetComponent<Animator>();
                if (mirrorAnimator != null)
                {
                    yield return new WaitForSeconds(0.5f);
                    mirrorAnimator.SetBool("isCloset", true);
                }
                else
                {
                    Debug.LogError("No Animator component found on the mirror object.");
                }
            }
            animator.speed = 0;
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(TypeDialogue("....just frozen food"));

            yield return new WaitForSeconds(1f);

            closet.GetComponent<SpriteRenderer>().enabled = true;

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(TypeDialogue("wha...the lights went out!!..."));

            yield return new WaitForSeconds(2f);

            if (jar != null)
            {
                jar.SetActive(true);
            }
            else
            {
                Debug.LogError("Jar game object not found in the scene.");
            }
         
            closet.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(3f);
            animator.speed = 1;
            yield return new WaitForSeconds(1f);
            isCloset = false;
            dialogueText.text = "";
            Vector3 originalYPosition = new Vector3(targetPos.x, targetPos.y - yOffset, transform.position.z);
            while (Vector3.Distance(transform.position, originalYPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalYPosition, speed * Time.deltaTime);
                yield return null;
            }

            speed = originalSpeed; // Reset speed to original
            animator.SetBool("canGoUp", false); // Stop the canGoUp animation
                                                // Move a bit along the x-axis
            Vector3 horizontalTarget = new Vector3(transform.position.x + 6f, transform.position.y, transform.position.z);
            while (Vector3.Distance(transform.position, horizontalTarget) > 0.1f)
            {
                spriteRenderer.flipX = false;
                transform.position = Vector3.MoveTowards(transform.position, horizontalTarget, speed * Time.deltaTime);
                animator.SetBool("canWalk", true); // Ensure walking animation is playing
                yield return null;
            }

    
            yield return new WaitForSeconds(2f);

            animator.SetBool("canWalk", false);
     
            yield break;

        }
        else if (isOverHideBed == true)
        {
            enemy_script.Hide();
            GameObject painting = GameObject.Find("hide_bed");
            if (painting != null)
            {
                Animator mirrorAnimator = painting.GetComponent<Animator>();
                if (mirrorAnimator != null)
                {
                    yield return new WaitForSeconds(0.5f);
                    mirrorAnimator.SetBool("isCloset", true);
                }
                else
                {
                    Debug.LogError("No Animator component found on the mirror object.");
                }
            }
            yield return new WaitForSeconds(3f);
            animator.speed = 0;
            while (enemy.activeSelf)
            {
                yield return null; 
            }
            animator.speed = 1;
            yield return new WaitForSeconds(2f);
            isOverHideBed = false;
            dialogueText.text = "";
            animator.SetBool("canHide_Bed", false);
            Vector3 originalYPosition = new Vector3(targetPos.x, targetPos.y - yOffset, transform.position.z);
            while (Vector3.Distance(transform.position, originalYPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalYPosition, speed * Time.deltaTime);
                yield return null;
            }

            speed = originalSpeed; // Reset speed to original
           // Stop the canGoUp animation
                                                // Move a bit along the x-axis
            Vector3 horizontalTarget = new Vector3(transform.position.x + 6f, transform.position.y, transform.position.z);
            while (Vector3.Distance(transform.position, horizontalTarget) > 0.1f)
            {
                spriteRenderer.flipX = false;
                transform.position = Vector3.MoveTowards(transform.position, horizontalTarget, speed * Time.deltaTime);
                animator.SetBool("canWalk", true); // Ensure walking animation is playing
                yield return null;
            }


            yield return new WaitForSeconds(2f);

            animator.SetBool("canWalk", false);
            DActivateEnemy();
            yield break;

        }
        else
        {
            TurnOffTV();
            yield return new WaitForSeconds(2f);
            Vector3 originalYPosition = new Vector3(targetPos.x, targetPos.y - yOffset, transform.position.z);
            while (Vector3.Distance(transform.position, originalYPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalYPosition, speed * Time.deltaTime);
                yield return null;
            }

            speed = originalSpeed; // Reset speed to original
            animator.SetBool("canGoUp", false); // Stop the canGoUp animation
                                                // Move a bit along the x-axis
            Vector3 horizontalTarget = new Vector3(transform.position.x + 6f, transform.position.y, transform.position.z);
            while (Vector3.Distance(transform.position, horizontalTarget) > 0.1f)
            {
                spriteRenderer.flipX = false;
                transform.position = Vector3.MoveTowards(transform.position, horizontalTarget, speed * Time.deltaTime);
                animator.SetBool("canWalk", true); // Ensure walking animation is playing
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            animator.SetBool("canWalk", false); // Stop walking animation
            TurnOnTV();

            // Start displaying the dialogue after the interaction is complete
            StartCoroutine(TypeDialogue(dialogueMessage));
        }

    }

    public void StartTableInteractionSequence()
    {
        GameObject table = GameObject.FindWithTag("Table");
        if (table != null)
        {
            targetPosition = new Vector3(table.transform.position.x, transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = true;
            isTv = false;

        }
    }
    public void StartMirrorInteractionSequence()
    {
        GameObject mirror = GameObject.FindWithTag("mirror");
        if (mirror != null)
        {
            targetPosition = new Vector3(mirror.transform.position.x+ 8.0f, transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = true;
            isTv = false;
            isMirror = true;

        }
    }
    public void StartFrameInteractionSequence()
    {
        GameObject frame = GameObject.FindWithTag("frame");
        if (frame != null)
        {
            targetPosition = new Vector3(frame.transform.position.x + 6.0f, transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = true;
            isTv = false;
            isMirror = false;
            isCage= false;
            isFrame = true;
          
        }
    }
    public void StartCageInteractionSequence()
    {
        GameObject cage = GameObject.FindWithTag("cage");
        if (cage != null)
        {
            targetPosition = new Vector3(cage.transform.position.x - 3.0f , transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = true;
            isTv = false;
            isMirror = false;
            isCage = true;

        }
    }
    public void JarInteractionSequence()
    {
        GameObject cage = GameObject.FindWithTag("jar");
        if (cage != null)
        {
            if (!closetDone)
            {
                targetPosition = new Vector3(cage.transform.position.x - 1.0f, transform.position.y, transform.position.z);
                isInteracting = true;
                isMoving = true;
                isTv = false;
                isMirror = false;
                isCage = false;
                isJar = true;
            }

        }
    }


    public void HallwayDoor()
    {
        GameObject door = GameObject.FindWithTag("door_pink_bedroom");
        if (door != null)
        {
            targetPosition = new Vector3(door.transform.position.x - 3.0f, transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = true;
            isTv = false;
            isMirror = false;
            isCage = false;
            isPinkDoor = true;

        }
    }
    public void labRoom()
    {
        GameObject door = GameObject.FindWithTag("door_room4");
        if (door != null)
        {
            targetPosition = new Vector3(door.transform.position.x - 3.0f, transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = true;
            isTv = false;
            isMirror = false;
            isCage = false;
            isPinkDoor = false;
            isLabRoom = true;

        }
    }

    public void DoorToHallway()
    {
        GameObject door = GameObject.FindWithTag("door_hallway");
        if (door != null)
        {
            targetPosition = new Vector3(door.transform.position.x - 3.0f, transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = true;
            isTv = false;
            isMirror = false;
            isCage = false;
            isPinkDoor = false;
            isHallDoor = true;

        }
    }

    public void DoorToBedroom()
    {
        GameObject door = GameObject.FindWithTag("bedroom");
        if (door != null)
        {
            targetPosition = new Vector3(door.transform.position.x + 3.0f, transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = true;
            isTv = false;
            isMirror = false;
            isCage = false;
            isPinkDoor = false;
            isHallDoor = false;
            isBedroom = true;
        }
    }
    public void StartBedroomDoorInteraction()
    {
        GameObject door = GameObject.FindWithTag("Door");
        if (door != null)
        {
            targetPosition = new Vector3(door.transform.position.x, transform.position.y, transform.position.z);
            isInteracting = true;
            isMoving = true;
            isDoor = true;
            isTv = true;


        }
    }
    private IEnumerator HandleTableInteraction()
    {
        while (isMoving)
        {
            MoveAlongPath();
            yield return null;
        }
        if (isHallDoor)
        {
            yield return new WaitForSeconds(1f);
            loading.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("hallway_1");

        }
        if (isBedroom)
        {
            yield return new WaitForSeconds(1f);
            loading.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("room_2");

        }
        if (isPinkDoor)
        {
                yield return new WaitForSeconds(1f);
                loading.GetComponent<SpriteRenderer>().enabled = true;
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene("room_3");
            
        }
        if (isLabRoom)
        {
            yield return new WaitForSeconds(1f);
            loading.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("room4");

        }
        if (isFrame)
        {

           yield return new WaitForSeconds(1f);

            frame.GetComponent<SpriteRenderer>().enabled = true;

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(TypeDialogue("I can see a couple and their dog from the window"));

            yield return new WaitForSeconds(2f);

            frame.GetComponent<SpriteRenderer>().enabled = false;
            dialogueText.text = "";
            isFrame = false;
            isInteracting = false;
            yield break;
        }
        if (isCage)
        {


           // cage.GetComponent<SpriteRenderer>().enabled = true;

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(TypeDialogue("A bunch of books...I can't read, I'm a cat..."));

            yield return new WaitForSeconds(2f);

           // cage.GetComponent<SpriteRenderer>().enabled = false;
            dialogueText.text = "";
            isInteracting = false;
            isCage = false;
            yield break;
        }
        if (isJar)
        {

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(TypeDialogue("what... is this...?"));

            yield return new WaitForSeconds(1f);

            loading.GetComponent<SpriteRenderer>().enabled = true;

            yield return StartCoroutine(TypeDialogue("The lights!!!"));

            yield return new WaitForSeconds(2f);

            if (enemy != null)
            {
                ActivateEnemy();
            }
            else
            {
                Debug.LogError("Jar game object not found in the scene.");
            }
            spriteRenderer.flipX = true;
            loading.GetComponent<SpriteRenderer>().enabled = false;

            yield return StartCoroutine(TypeDialogue("who....are...you? n...no...st...stay back!!"));
          
            yield return new WaitForSeconds(2f);
            dialogueText.text = "";
            isInteracting = false;
            isJar = false;
            closetDone = true;
            yield break;
        }
        if (isMirror) {


            //GameObject mirror = GameObject.Find("mirror");
            //if (mirror != null)
            //{
            //    Animator mirrorAnimator = mirror.GetComponent<Animator>();
            //    if (mirrorAnimator != null)
            //    {
            //        yield return new WaitForSeconds(0.5f);
            //        mirrorAnimator.SetBool("isBroken", true);
            //    }
            //    else
            //    {
            //        Debug.LogError("No Animator component found on the mirror object.");
            //    }
            //}
            //else
            //{
            //    Debug.LogError("No game object named 'mirror' found in the scene.");
            //}
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(TypeDialogue("Hina's phone. it has a picture of Tsukasa."));

            yield return new WaitForSeconds(1f);
            dialogueText.text = "";
            isMirror = false;
            yield break;
        }
        // Once the player has reached the table, start the dialogue and animation
        animator.SetBool("canTurn", true); // Start canTurn animation

        // Display the first dialogue
        yield return StartCoroutine(TypeDialogue("A wooden key is inside the cup..."));

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Display the second dialogue
        yield return StartCoroutine(TypeDialogue("Obtained 'Wooden key'"));

        // Set canTurn to false after the second dialogue ends
        animator.SetBool("canTurn", false); // Stop canTurn animation
        dialogueText.text = ""; // Clear the dialogue
        isInteracting = false; // End interaction
        isTv = true;
    }


    public void TurnOnTV()
    {
        if (tvInteraction != null)
        {
            tvInteraction.TurnOn();
        }
    }

    public void TurnOffTV()
    {
        if (tvInteraction != null)
        {
            tvInteraction.TurnOff();
        }
    }
    private IEnumerator PaintingBleed()
    {
       // GameObject painting = GameObject.Find("painting_blood");
       // if (painting != null)
       // {
       //     Animator mirrorAnimator = painting.GetComponent<Animator>();
       //     if (mirrorAnimator != null)
       //     {
       //         yield return new WaitForSeconds(0.5f);
       //         mirrorAnimator.SetBool("isBleed", true);
       //     }
       //     else
       //     {
       //         Debug.LogError("No Animator component found on the mirror object.");
       //     }
       // }
       // else
       // {
       //     Debug.LogError("No game object named 'mirror' found in the scene.");
       // }
       // yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(TypeDialogue("nothing to see here"));

        yield return new WaitForSeconds(1f);
        dialogueText.text = "";
        isMirror = false;
        yield break;

    }
    public void StartPaintingInteractionSequence()
    {

        if (!bleed)
        {
            StartCoroutine(TypeDialogue("just an air conditioner"));
            bleed = true;
        }
        else if (bleed)
        {
            StartCoroutine(PaintingBleed());
        }

    }

    // Coroutine to display dialogue letter by letter
    private IEnumerator TypeDialogue(string message)
    {
        cursorController.gameObject.SetActive(false);
        Cursor.visible = false;
        initial_ = true;
        dialogueText.text = ""; // Clear the dialogue text initiallym

        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(0.08f); // Wait between each letter for the specified typing speed
        }

        yield return new WaitForSeconds(2f); // Wait for 3 seconds before clearing the dialogue
        dialogueText.text = ""; // Clear the dialogue after the delay
        cursorController.gameObject.SetActive(true);
        Cursor.visible = true;
        cursorController.cursor_reset();
        initial_ = false;
    }
}
