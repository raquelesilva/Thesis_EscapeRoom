using UnityEngine;
using UnityEngine.Events;

public class BallPlayerMovement : MonoBehaviour
{
    private bool canMove = true;
    [SerializeField] private float moveSpeed;
    private Rigidbody rb;


    [SerializeField] float distance = 15.0f;
    private Vector3 originalPos;

    [SerializeField] TrailRenderer trailRenderer;

    [SerializeField] UnityEvent endMaze;

    PipeScreen pipeGameManager;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        originalPos = this.transform.position;
        pipeGameManager = GetComponentInParent<PipeScreen>();
    }

    void Update()
    {
        Movement();

        RayCastFunction();
    }

    void Movement()
    {
        if (!canMove) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Calculate movement direction
        Vector3 movement = new Vector3(-moveX, moveY, 0).normalized * moveSpeed * Time.deltaTime;

        // Apply movement using Translate method (relative to the player's local orientation)
        transform.Translate(movement);
    }

    void RayCastFunction()
    {
        // The origin of the ray is the object's position
        Vector3 origin = transform.position;

        // The direction of the ray is straight down the z-axis
        Vector3 direction = transform.forward * -1;


        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, distance))
        {
            if (hit.collider.gameObject.tag == "MazePath")
            {
                //Inside maze
            }
            else if (hit.collider.gameObject.tag == "MazeExit")
            {
                endMaze?.Invoke();
                BallActive(false);
            }
            else
            {
                BallActive(true);
                pipeGameManager.EnableDisableRotation(true);
            }
        }

        // Draw the ray in the scene view for debugging purposes
        Debug.DrawRay(origin, direction * distance, Color.red);
    }


    public void ResetPosition()
    {
        this.transform.localPosition = Vector3.zero;
        trailRenderer.Clear();
    }

    public void BallActive(bool decision)
    {
        ResetPosition();
        gameObject.SetActive(decision);
    }

}
