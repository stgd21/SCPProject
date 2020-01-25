using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject playerCamera;
    public float mouseSensitivity;
    public float moveSpeed;
    public float maxVelocity;

    private Rigidbody playerRigid;
    private float xAxisClamp;

    // Start is called before the first frame update
    void Start()
    {
        playerRigid = GetComponent<Rigidbody>();

        //Keep mouse invisible and locked to center of play
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraRotation();
        UpdateMovement();

        //Unlock mouse
        if (Input.GetKeyDown("escape"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void UpdateMovement()
    {
        //Physics-based movement, Inputs from 0 to 1 based on bindings in Unity window
        //Combines forward and horizontal movement
        playerRigid.AddForce(transform.forward * Input.GetAxis("Vertical") * moveSpeed
            + transform.right * Input.GetAxis("Horizontal") * moveSpeed, ForceMode.VelocityChange);

        //Clamp max speed
        if (playerRigid.velocity.magnitude > maxVelocity)
        {
            playerRigid.velocity = playerRigid.velocity.normalized;
            playerRigid.velocity *= maxVelocity;
        }
    }

    //Not my original code
    void UpdateCameraRotation()
    {
        //Get the change in mouse movement per frame
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Make it significant and preferable
        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;

        //Camera will rotate vertically
        Vector3 targetRotCam = playerCamera.transform.rotation.eulerAngles;
        //Player will rotate horizontally with physics
        Vector3 targetRotBody = playerRigid.rotation.eulerAngles;

        //While I don't understand this math, these lines of code agree not to hurt me if I keep them in
        targetRotCam.x -= rotAmountY;
        targetRotCam.z = 0;
        targetRotBody.y += rotAmountX;

        if (xAxisClamp > 90)
        {
            xAxisClamp = 90;
            targetRotCam.x = 90;
        }
        else if (xAxisClamp < -90)
        {
            xAxisClamp = -90;
            targetRotCam.x = 270;
        }

        //Rotate the camera (up and down)
        playerCamera.transform.rotation = Quaternion.Euler(targetRotCam);
        //Rotate the player (left and right)
        playerRigid.rotation = Quaternion.Euler(targetRotBody);
    }
}
