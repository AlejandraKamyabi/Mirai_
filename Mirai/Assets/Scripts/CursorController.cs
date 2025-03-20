using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private RectTransform cursorTransform;
    private Camera mainCamera;
    public Vector2 cursorOffset = Vector2.zero;
    private bool isOverTVInteraction = false;
    private bool isOverMirror = false;
    private bool isOverFrame = false;
    private bool isOverJar = false;
    public bool isInteracting = false;
    public bool enemyActive = false;
    private bool isOverCage = false;
    public bool  interaction = false;
    private bool isOverSink = false;
    private bool isOverCloset = false;
    private bool isOverBedroom = false;
    private bool isOverHideBed = false;
    private bool isOverDoor_Pink= false;
    private bool isOverRoom_4 = false;
    private bool isOverDoor_Hall = false;
    private Animator cursorAnimator; 
    private bool isOverTable = false;
    public bool hasTable = false;
    private bool isOverPainting = false;
    private bool isOverDoor = false;
    void Start()
    {
        cursorTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        cursorAnimator = GetComponent<Animator>();

        if (cursorTransform == null)
        {
            Debug.LogError("RectTransform component missing from the Cursor GameObject.");
            return;
        }

        if (cursorAnimator == null)
        {
            Debug.LogError("Animator component missing from the Cursor GameObject.");
            return;
        }

        Cursor.visible = false; // Hide the default system cursor

        // Initialize cursor position at (0, 0, 0)
        cursorTransform.localPosition = Vector3.zero;
    }
    public void canInteract(bool interact)
    {
        isInteracting = interact;
    }
    public void enemy_Active(bool interact)
    {
        enemyActive = interact;
    }
    public void cursor_reset()
    {
        cursorAnimator.SetBool("can_trigger", false);
    }
    void Update()
    {
        // Get the current mouse position
        Vector2 mousePosition = Input.mousePosition;

        // Convert screen point to world point
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0; // Keep the cursor at the same z-plane if in 2D

        // Apply the offset and set the cursor position
        cursorTransform.position = worldPosition + (Vector3)cursorOffset;

        if (isOverTVInteraction && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.StartTVInteractionSequence();
            }
        }
        else if (isOverPainting && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.StartPaintingInteractionSequence();
            }
        }
        else if (isOverBedroom && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DoorToBedroom();
            }
        }
        else if (isOverRoom_4 && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.labRoom();
            }
        }
        else if (isOverMirror && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.StartMirrorInteractionSequence();
            }
        }
        else if (isOverTable && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                hasTable = true;
                playerController.StartTableInteractionSequence();
            }
        }
        else if (isOverFrame && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.StartFrameInteractionSequence();
            }
        }
        else if (isOverJar && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.JarInteractionSequence();
            }
        }
        else if (isOverSink && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.BathroomSinkSequence();
            }
        }
        else if (isOverCloset && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.ClosetSequence();
            }
        }
      
        else if (isOverCage && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.StartCageInteractionSequence();
            }
        }
        else if (isOverDoor_Pink && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.HallwayDoor();
            }
        }
        else if (isOverDoor_Hall && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DoorToHallway();
            }
        }

        else if (isOverDoor && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.StartBedroomDoorInteraction();
            }
        }
        else if (isOverHideBed && Input.GetMouseButtonDown(0))
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.Hide_Bed();
            }
        }
    }
    void OnEnable()
    {
        cursorAnimator = GetComponent<Animator>();
        Cursor.visible = false;
        cursorAnimator.Rebind();
        cursorAnimator.Update(0f);
    }

 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInteracting)
        {
            if (other.CompareTag("TVInteraction"))
            {
                isOverTVInteraction = true;
                cursorAnimator.SetBool("can_trigger", true);
            }

            if (other.CompareTag("bathroom_sink"))
            {
                isOverSink = true;
                cursorAnimator.SetBool("can_trigger", true);
            }
            if (other.CompareTag("closet"))
            {
                isOverCloset = true;
                cursorAnimator.SetBool("can_trigger", true);
            }
            if (other.CompareTag("jar"))
            {
                isOverJar = true;
                cursorAnimator.SetBool("can_trigger", true);
            }
            else if (other.CompareTag("mirror"))
            {
                isOverMirror = true;
                cursorAnimator.SetBool("can_trigger", true);
            }
            else if (other.CompareTag("Painting"))
            {
                isOverPainting = true;
                cursorAnimator.SetBool("can_trigger", true); // Set the Animator parameter to true
            }
            else if (other.CompareTag("frame"))
            {
                isOverFrame = true;
                cursorAnimator.SetBool("can_trigger", true);

            }
            else if (other.CompareTag("cage"))
            {
                isOverCage = true;
                cursorAnimator.SetBool("can_trigger", true);

            }

            else if (other.CompareTag("Table") && !hasTable)
            {
                isOverTable = true;
                cursorAnimator.SetBool("can_trigger", true); // Set the Animator parameter to true
            }
            else if (other.CompareTag("door_hallway"))
            {
                isOverDoor_Hall = true;
                cursorAnimator.SetBool("can_trigger", true);

            }
            else if (other.CompareTag("Door") && hasTable)
            {
                isOverDoor = true;
                cursorAnimator.SetBool("can_trigger", true); // Set the Animator parameter to false
            }
            else if (other.CompareTag("door_pink_bedroom"))
            {
                isOverDoor_Pink = true;
                cursorAnimator.SetBool("can_trigger", true);

            }
            else if (other.CompareTag("door_room4"))
            {
                isOverRoom_4 = true;
                cursorAnimator.SetBool("can_trigger", true);
            }
            else if (other.CompareTag("bedroom"))
            {
                isOverBedroom = true;
                cursorAnimator.SetBool("can_trigger", true);
            }
   

        }
        else if (!isInteracting || enemyActive) {
            if (other.CompareTag("door_hallway"))
            {
                isOverDoor_Hall = true;
                cursorAnimator.SetBool("can_trigger", true);

            }
            else if (other.CompareTag("Door") && hasTable)
            {
                isOverDoor = true;
                cursorAnimator.SetBool("can_trigger", true); // Set the Animator parameter to false
            }
            else if (other.CompareTag("door_pink_bedroom"))
            {
                isOverDoor_Pink = true;
                cursorAnimator.SetBool("can_trigger", true);

            }
            else if (other.CompareTag("hide_bed"))
            {
                isOverHideBed = true;
                cursorAnimator.SetBool("can_trigger", true);
            }
            else if (other.CompareTag("bedroom"))
            {
                isOverBedroom = true;
                cursorAnimator.SetBool("can_trigger", true);
            }
            else if (other.CompareTag("door_room4"))
            {
                isOverRoom_4 = true;
                cursorAnimator.SetBool("can_trigger", true);
            }
      

        }

        
    }

    void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("door_room4"))
        {
            isOverRoom_4 = false;
            cursorAnimator.SetBool("can_trigger", false);
        }
          else  if (other.CompareTag("TVInteraction"))
            {
                isOverTVInteraction = false;
                cursorAnimator.SetBool("can_trigger", false); // Set the Animator parameter to false
            }
            else if (other.CompareTag("Painting"))
            {
                isOverPainting = false;
                cursorAnimator.SetBool("can_trigger", false); // Set the Animator parameter to false
            }
            else if (other.CompareTag("mirror"))
            {
                isOverMirror = false;
                cursorAnimator.SetBool("can_trigger", false);
            }
        else if (other.CompareTag("jar"))
            {
                isOverJar = false;
                cursorAnimator.SetBool("can_trigger", false);
            }
        else if (other.CompareTag("closet"))
            {
                isOverCloset = false;
                cursorAnimator.SetBool("can_trigger", false);
            }
           else if(other.CompareTag("bathroom_sink"))
            {
                isOverSink = false;
                cursorAnimator.SetBool("can_trigger", false); // Set the Animator parameter to true
            }
            else if (other.CompareTag("Table"))
            {
                isOverTable = false;
                cursorAnimator.SetBool("can_trigger", false); // Set the Animator parameter to false
            }
            else if (other.CompareTag("door_hallway"))
            {
                isOverDoor_Hall = false;
                cursorAnimator.SetBool("can_trigger", false);

            }
            else if (other.CompareTag("Door"))
            {
                isOverDoor = false;
                cursorAnimator.SetBool("can_trigger", false); // Set the Animator parameter to false
            }
            else if (other.CompareTag("frame"))
            {
                isOverFrame = false;
                cursorAnimator.SetBool("can_trigger", false); // Set the Animator parameter to false
            }
            else if (other.CompareTag("cage"))
            {
                isOverCage = false;
                cursorAnimator.SetBool("can_trigger", false);
            }
            else if (other.CompareTag("door_pink_bedroom"))
            {
                isOverDoor_Pink = false;
                cursorAnimator.SetBool("can_trigger", false);
            }
        else if (other.CompareTag("hide_bed"))
        {
            isOverHideBed = false;
            cursorAnimator.SetBool("can_trigger", false);
        }
        else if (other.CompareTag("bedroom"))
        {
            isOverBedroom = false;
            cursorAnimator.SetBool("can_trigger", false);
        }
    }
}
