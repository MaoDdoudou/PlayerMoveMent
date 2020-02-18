using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public CharacterController cc;
    public Transform Cam;
    public float currentSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float crounchSpeed;
    public float SlindSpeed;
    public float jumpSpeed;
    public float gravit;
    
    public enum PlayerState
    {
        walk,
        run,
        crounch,
        slid
    }
    public PlayerState playerState;

    public Camera myCam;
    void Awake()
    {
     playerState = PlayerState.walk;   
     myCam = Camera.main;
     Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        
    }

   
    void Update()
    {
        PlayerMove();
       IsChangerState();
       UpdatePlayerSate();
        Fire();
    }
    
    void LateUpdate()
    {
        CamMove();
    }
    public float mouseSpeed;
     float angleMouse;
    void CamMove()
    {
        float mousex = Input.GetAxisRaw("Mouse X");
        float mousey = Input.GetAxisRaw("Mouse Y");
        transform.Rotate(new Vector3(0f,mousex*mouseSpeed,0f),Space.Self);
        angleMouse -= mousey*mouseSpeed;
        angleMouse = Mathf.Clamp(angleMouse,-90f,90f);
        Cam.localEulerAngles = new Vector3(angleMouse,0f,0f);
    }
    Vector3 MoveVelocity;
    float playerY;
    public KeyCode JumpCode;
    public bool isJumping;
    Vector3 jumpVelocity;
    void PlayerMove()
    {
        float movex = Input.GetAxisRaw("Horizontal");
        float movey = Input.GetAxisRaw("Vertical");
        if(cc.isGrounded)
        {
            playerY = 0f;
            isJumping = false;
            cc.slopeLimit = 45f;
            MoveVelocity = transform.right*movex+transform.forward*movey;
            if(IsInputJump())
            {
                isJumping = true;
                cc.slopeLimit = 90f;
                playerY = Mathf.Sqrt(jumpSpeed*-2*gravit);

            }

        }
        playerY+=gravit*Time.deltaTime;
        jumpVelocity.y=playerY;
        cc.Move(Vector3.ClampMagnitude(MoveVelocity,1f)*Time.deltaTime*currentSpeed);
        cc.Move(jumpVelocity*Time.deltaTime);
        if(movex!=0f||movey!=0f&&CheckPlayerIsOnPo())
        {
            cc.Move(Vector3.down*cc.height/2*Pozhong*Time.deltaTime);
        }
    }
  
    public float Pozhong;
   void UpdatePlayerSate()
   {
       switch(playerState)
       {
           default:
           case PlayerState.walk:
            currentSpeed = Mathf.Lerp(currentSpeed,walkSpeed,0.2f);
            cc.height = Mathf.Lerp(cc.height,2f,0.2f);
            myCam.fieldOfView = Mathf.Lerp(myCam.fieldOfView,60f,0.3f);
           break;
           case PlayerState.run:
            currentSpeed = Mathf.Lerp(currentSpeed,runSpeed,0.2f);
            cc.height = Mathf.Lerp(cc.height,2f,0.2f);
            myCam.fieldOfView = Mathf.Lerp(myCam.fieldOfView,60f,0.3f);
           break;
           case PlayerState.crounch:
            currentSpeed = Mathf.Lerp(currentSpeed,crounchSpeed,0.2f);
            cc.height = Mathf.Lerp(cc.height,0.5f,0.2f);
            myCam.fieldOfView = Mathf.Lerp(myCam.fieldOfView,60f,0.3f);
           break;
           case PlayerState.slid:
            currentSpeed = Mathf.Lerp(currentSpeed,SlindSpeed,0.2f);
            cc.height = Mathf.Lerp(cc.height,0.5f,0.2f);
            myCam.fieldOfView = Mathf.Lerp(myCam.fieldOfView,100f,0.3f);
           
           break;
       }
   }
    
   public bool isRuning;
    void IsChangerState()
    {
        if(MoveVelocity!=Vector3.zero)
        {
            playerState = PlayerState.walk;
        }
        if(Input.GetKey(KeyCode.W)&&Input.GetKey(KeyCode.LeftShift))
        {
            playerState = PlayerState.run;
            isRuning = true;
        }else
        {
            isRuning = false;
             playerState = PlayerState.walk;
        }
        
        if(Input.GetKey(KeyCode.W)&&Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.LeftControl))
        {
            playerState = PlayerState.crounch;
        }
        if(isRuning&&Input.GetKey(KeyCode.C))
        {
            playerState = PlayerState.slid;
        }
    }
    

    bool IsInputJump()
    {
        return Input.GetKeyDown(JumpCode);
    }
    public float lastTime;
    public float fireRate;
    public GameObject Bullet;
    public Transform point;
    void Fire()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(Time.time-lastTime>1/fireRate)
            {
                lastTime = Time.time;
                GameObject go = Instantiate(Bullet,point.position,point.rotation);
                Rigidbody ri  = go.GetComponent<Rigidbody>();
                ri.velocity = point.forward*100f;
                Destroy(go,3f);
            }
        }
    }
   public float CheckChang;
    bool CheckPlayerIsOnPo()
    {
        if(isJumping)
        {
            return false;
        }
        RaycastHit hit ;
        if(Physics.Raycast(transform.position,Vector3.down,out hit,cc.height/2*CheckChang))
        {
            if(hit.normal!=Vector3.up)
            {
                return true;
            }
        }
        return false;
    }
   
}
