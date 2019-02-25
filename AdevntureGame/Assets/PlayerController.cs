using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float rotationSpeed = 3f;
    public float jumpSpeed = 15f;
    public float gravity = 30f;
    public LayerMask whatIsDangerous;
    public float restartTime = 3f;
    public GameObject projectile;
    private GameObject newProjectile;
    public Transform firePosition;
    public float bulletForce = 500F;

    private CharacterController myCC;
    private Animator myAnim;
    private float yVelocity = 0;
    private bool alive = true;

    public SaveData playerSaveData;
    public const string startingPositionKey = "starting position";

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;


    // Use this for initialization
    void Start()
    {
        myCC = GetComponent<CharacterController>();
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Initialize some variables
        Vector3 input = Vector3.zero;
        Vector2 mouseInput = Vector2.zero;
        float speed = walkSpeed;


        // Gravity - accelerate toward the floor
        yVelocity -= gravity * Time.deltaTime;

        // If you are dead, stop taking player input
        if (alive)
        {
            // Are we on the ground? Stop falling so fast
            if (myCC.isGrounded)
            {
                yVelocity = -gravity * Time.deltaTime;

                // If grounded, you may jump (NO DOUBLE JUMP)
                if (Input.GetButtonDown("Jump"))
                {
                    yVelocity = jumpSpeed;
                }
            }

            // Talk to the animator
            myAnim.SetBool("Grounded", myCC.isGrounded);

            // Get WASD and mouse movement from the player
            input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));


            if (Input.GetButtonDown("Fire1"))
            {
                newProjectile = Instantiate(projectile, firePosition.transform.position, firePosition.transform.rotation) as GameObject;
                newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce);
            }
            // How fast are we moving this frame?
            if (Input.GetButton("Fire3"))
            {
                speed = runSpeed;
            }

            // Talk to the animator
            myAnim.SetFloat("Speed", input.magnitude * speed);
        }
        // Move and rotate the player
        if (axes == RotationAxes.MouseXAndY)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
        myCC.Move(transform.TransformDirection(input * speed * Time.deltaTime + yVelocity * Vector3.up * Time.deltaTime));
    }

    void OnControllerColliderHit(ControllerColliderHit myHit)
    {
        // Did we hit something dangerous? If so, die.
        if (IsInMyLayerMask(whatIsDangerous, myHit.gameObject))
            Die();
    }

    void OnTriggerEnter(Collider myCollider)
    {
        // Did we walk into something dangerous? If so, die.
        if (IsInMyLayerMask(whatIsDangerous, myCollider.gameObject))
            Die();
    }

    // How to die
    void Die()
    {
        alive = false;
        // Update the Animator
        myAnim.SetTrigger("Death");
        // Restart in 3 seconds
        Invoke("RestartLevel", restartTime);
    }

    void RestartLevel()
    {
        // Must have scene in build index first
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    bool IsInMyLayerMask(LayerMask mask, GameObject obj)
    {
        // Check to see if layer in game object matches selected layer(s)
        return ((mask.value & (1 << obj.layer)) > 0);
    }
}