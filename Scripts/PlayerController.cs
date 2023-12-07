using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private bool isMoving;
    [SerializeField] private Transform gunPoint1;
    [SerializeField] private Transform gunPoint2;
    [SerializeField] private Transform aimRig;
    [SerializeField] private Transform rightHandRig;
    [SerializeField] private Transform leftHandRig;
    [SerializeField] private Camera cam;
    private Rigidbody rb;
    private Animator anim;
    private const string IsWalking = "IsWalking";

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        gunPoint2.gameObject.SetActive(false);
        aimRig.GetComponent<MultiAimConstraint>().weight = 0;
        rightHandRig.GetComponent<TwoBoneIKConstraint>().weight = 0;
        leftHandRig.GetComponent<TwoBoneIKConstraint>().weight = 0;
    }

    private void Update()
    {
        // float x = Input.GetAxis("Mouse X");
        // float y = Input.GetAxis("Mouse Y");
        //
        // Quaternion qx = Quaternion.Euler(0, x, 0);
        // Quaternion qy = Quaternion.Euler(y, 0, 0);
        // cam.transform.rotation = qx * transform.rotation;
        // cam.transform.rotation *= qy;

        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
// 再按下收枪
        if (Input.GetKey(KeyCode.F))
        {
            gunPoint1.gameObject.SetActive(false);
            gunPoint2.gameObject.SetActive(true);
            aimRig.GetComponent<MultiAimConstraint>().weight = 1;
            rightHandRig.GetComponent<TwoBoneIKConstraint>().weight = 1;
            leftHandRig.GetComponent<TwoBoneIKConstraint>().weight = 1;
            anim.SetBool("Fire", true);
        }
        // else
        // {
        //     gunPoint1.gameObject.SetActive(true);
        //     gunPoint2.gameObject.SetActive(false);
        //     aimRig.GetComponent<MultiAimConstraint>().weight = 0;
        //     rightHandRig.GetComponent<TwoBoneIKConstraint>().weight = 0;
        //     leftHandRig.GetComponent<TwoBoneIKConstraint>().weight = 0;
        //     anim.SetBool("Fire", false);
        // }

        // Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // input = input.normalized;
        
        float x = Input.GetAxis("Horizontal");
        float y  = Input.GetAxis("Vertical");
        Vector3 forward = transform.forward;
        Vector3 f = new Vector3(forward.x,0, forward.z).normalized;
        Vector3 r = transform.right;
        Vector3 input = f * y + r * x;
        

        isMoving = input != Vector3.zero;

        if (isMoving)
        {
            transform.position += input * speed * Time.deltaTime;
            anim.SetBool(IsWalking, true);
            
            // gunPoint1.gameObject.SetActive(true);
            // gunPoint2.gameObject.SetActive(false);
            // aimRig.GetComponent<MultiAimConstraint>().weight = 0;
            // rightHandRig.GetComponent<TwoBoneIKConstraint>().weight = 0;
            // leftHandRig.GetComponent<TwoBoneIKConstraint>().weight = 0;
            // anim.SetBool("Fire", false);
        }
        else
        {
            anim.SetBool(IsWalking, false);
            Debug.Log("Stop");
        }

        // if (input.magnitude > 0.1)
        //     transform.forward = Vector3.Lerp(transform.forward, input, 0.5f);
    }
}