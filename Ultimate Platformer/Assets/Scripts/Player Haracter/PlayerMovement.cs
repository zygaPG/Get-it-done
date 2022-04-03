using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CameraMovement myCamera;
    public Transform playerObiect;
    Rigidbody rbody;
    int wallMask;
    int floorMask;

    statePosition positionState;

    public float speed;
    public float speedOnAir;
    public float jumpPower = 8000;


    //================Wall==Clipping=====================
    float currentWallClipingTime;
    float timeToCantClipWall = 0.25f;


    //=========== test Section ==============

    public MeshRenderer meshRenderer;
    public Material materialNormal;
    public Material materialCliped;

    float moveAngular
    {
        get
        {
            return Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * (180 / Mathf.PI);
        }
    }

    enum statePosition
    {
        floor,
        air,
        wall,
    }

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
        wallMask = LayerMask.GetMask("Climbing", "Wall");
        floorMask = LayerMask.GetMask("Floor", "Climbing", "Wall");
        positionState = statePosition.floor;
    }

    

    private void Update()
    {
        float currentMoveAngular = moveAngular;

        if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 )&& positionState != statePosition.wall)
        {
            transform.eulerAngles = myCamera.centralPoint.eulerAngles;
            //myCamera.centralPoint.localEulerAngles = Vector3.zero;

            playerObiect.localEulerAngles = new Vector3(0, currentMoveAngular, 0);
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump(currentMoveAngular);
        }

        if (positionState == statePosition.floor)
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {

                Vector3 moveVelocity = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
                //rbody.MovePosition(transform.position +  moveVelocity * currentSpeed * Time.fixedDeltaTime);

                moveVelocity *= speed * Time.fixedDeltaTime;
                moveVelocity.y = rbody.velocity.y;
                // Debug.Log(moveVelocity);
                rbody.velocity = moveVelocity;

            }
        }
    }


    void FixedUpdate()
    {
        

        if (positionState == statePosition.air)
        {
            if (Input.GetAxis("Vertical") > 0.3f || Input.GetAxis("Vertical") < -0.3f || Input.GetAxis("Horizontal") > 0.3f || Input.GetAxis("Horizontal") < -0.3f)
            {
                Vector3 currentVelocity = rbody.velocity;
                currentVelocity.y = 0;

                Vector3 moveVelocity = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));

                Vector3 currentMooveVelocity = moveVelocity * speed * Time.fixedDeltaTime;

                currentMooveVelocity -= currentVelocity;

                Vector3 velocity3 = currentMooveVelocity * speedOnAir * Time.fixedDeltaTime;

                velocity3.y = 0;

                rbody.velocity += velocity3;

                //Debug.Log(currentMooveVelocity);

            }
        }


        RaycastHit hit;
        var hitRay = Physics.Raycast(transform.position, -transform.up, out hit, 1.2f, floorMask);
        if (hitRay)
        {
            if (positionState != statePosition.floor)
            {
                positionState = statePosition.floor;
                rbody.useGravity = true;
                meshRenderer.material = materialNormal;
            }
        }
        else
        {
            if (positionState != statePosition.air && positionState != statePosition.wall)
            {
                positionState = statePosition.air;
                rbody.useGravity = true;
                meshRenderer.material = materialNormal;
            }
        }

        CheckWall();


        if (positionState == statePosition.wall)
        {
           PullDown();
        }
    }



    void CheckWall()
    {
        if (currentWallClipingTime <= 0)
        {
            
            RaycastHit hit;
            var hitRay = Physics.Raycast(playerObiect.position, playerObiect.forward, out hit, 0.6f, wallMask);
            if (hitRay)
            {
                if (positionState == statePosition.wall)
                {

                }
                else
                {
                    ClipToWall(playerObiect.forward, 1.2f);
                    meshRenderer.material = materialCliped;
                }

            }
        }
        else
        {
            currentWallClipingTime -= Time.deltaTime;
        }
    }


    void Jump(float currentMoveAngular)
    {
        if (positionState == statePosition.floor)
        {
            rbody.AddRelativeForce(transform.up * jumpPower, ForceMode.Impulse);
            positionState = statePosition.air;
            meshRenderer.material = materialNormal;
        }

        if (positionState == statePosition.wall)
        {
            Vector3 oldRotation = transform.up;

            Debug.Log(oldRotation);
            Debug.Log(new Vector3(0, myCamera.centralPoint.eulerAngles.y + currentMoveAngular, 0));

            float looking = Vector3.Dot(oldRotation.normalized, new Vector3(0, myCamera.centralPoint.eulerAngles.y + currentMoveAngular, 0).normalized);

            Debug.Log(looking);

            // myCamera.centralPoint.eulerAngles = myCamera.centralPoint.eulerAngles - new Vector3(0, currentMoveAngular, 0);
            if (looking < 0.65f)
            {
                transform.eulerAngles = myCamera.centralPoint.eulerAngles + new Vector3(0, currentMoveAngular, 0);
                rbody.AddForce(transform.forward * jumpPower * 0.8f + transform.up * jumpPower * 1.2f, ForceMode.Impulse);
            }
            else
            {
                rbody.AddForce(-transform.forward * jumpPower * 2 * 0.8f + transform.up * jumpPower * 1.2f, ForceMode.Impulse);
                Debug.Log("back");
            }

            currentWallClipingTime = timeToCantClipWall;
            rbody.useGravity = true;

            positionState = statePosition.air;
            meshRenderer.material = materialNormal;
        }
    }


    void PullDown()
    {
        Vector3 pullForce = new Vector3(0, -1, 0);
        rbody.MovePosition(transform.position + Time.deltaTime * pullForce);
    }




    void ClipToWall(Vector3 direction, float range)
    {

        RaycastHit hit;
        var hitRay = Physics.Raycast(playerObiect.position, direction, out hit, range, wallMask);
       // Debug.DrawRay(transform.position, direction, Color.red, range);

        if (hitRay)
        {
            //set player rotation
            float newYDegree = Vector3.SignedAngle(hit.normal, transform.forward, Vector3.up) - 180;
            this.transform.Rotate(0, -newYDegree, 0, Space.World);
           // myCamera.centralPoint.transform.Rotate(0, newYDegree, 0, Space.World);

            //set playerObiect rotation
            playerObiect.localEulerAngles = Vector3.zero;


            //set phisics settings 
            positionState = statePosition.wall;

            rbody.velocity = Vector3.zero;
            rbody.useGravity = false;

        }
    }


    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Climb")
        {
            ClipToWall(playerObiect.forward, 3);
        }

        if (collision.gameObject.tag == "Floor")
        {
            positionState = statePosition.floor;
        }
    }
    */
}
