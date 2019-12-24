using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class RollBall : MonoBehaviour
{

   
    public Vector2 posMouse;
    public float moveSpeed = 2f;
    public Vector3 direct;
    public Vector3 Velocity;
    public Vector3 targetPosition;
    public bool isClick1 = false;
    public bool isClick2 = false;
    public Rigidbody Body;
    bool draw = true;
    public bool isGround = false;
    public float CheckGround = 1;
    //Time
    bool UpdateDirect = false;
    bool getPosOrignal = false;
    float timeDirect = 0;
    Vector2 posOriginal = Vector2.zero;
    public Vector3 LastDirect=Vector2.zero;
    // Velocity
    public ForceMode ForceModeMoce;
    public ForceMode ForceModeInteracable;
    public float maxVelocity;
    public float LerpTime;
    public float SpeedVelocityStart;
    public float SpeedVelocityMax;
    public float SpeedVelocity;
    public float SpeedNode;
    public float SpeedRoll = 3;
    public float maxVec = 5;
    // Gird
    Vector2[,] Gird;
    int Width = Screen.width;
    int Height = Screen.height;
    public int offSetX;
    public int offSetY;

    // Recover Path
    public List<Vector3> ListPoint= new List<Vector3>();
    public List<Vector3> ListDirect = new List<Vector3>();
    public List<Point> ListPath = new List<Point>();
    public bool IncreTimeRecover = false;
    public bool isNewPath = false;
    public float SpeedTimeRecover;
    public bool  isMovePath = false;
    
    //GoBack
    public bool isGoBack = false;

    // Strenght
    public float Bound = 1.25f;
    public float Force;
    public float Strenght;
    public float Mass = 2;
    private void Awake()
    {

       
    }
    public void RotateToVelocity(float turnSpeed, bool ignoreY) {
    Vector3 dir;    

        dir = new Vector3(Body.velocity.x, 0f, Body.velocity.z);

    
     {
       
          
        }
 }

   
void Start()
    {

        InitGird();
        Body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {


        Body.maxAngularVelocity = Mathf.Infinity; 


        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
       
      if(Physics.Raycast(new Ray(transform.position, -Vector3.up),CheckGround))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
            Body.constraints = RigidbodyConstraints.None;
        }
   


        if (isGround)
        {
            if (!isGoBack)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isClick1 = true;
                }
                int count = ListPoint.Count;
                int x = (int)(Width / offSetX);
                int y = (int)(Height / offSetY);



                if (isClick1)
                {
                    if (count < 1)
                    {

                    }
                    //    Debug.Log("Get");

                    Vector3 point = getPosWordSpace(Input.mousePosition);

                    AddPoint(point);
                    int LastPoint = ListPoint.Count - 1;
                    if (ListPoint.Count > 1)
                    {
                        LastDirect = ListPoint[LastPoint] - ListPoint[LastPoint - 1];
                       
                        AddDirect(LastDirect.normalized);
                    }
                    GeneratePath();

                 



                }



            }
            else
            {
                {
                    if (Vector3.Magnitude(Body.velocity) < Mass)
                    {
                        isGoBack = false;
                    }
                }




            }
            if (Input.GetMouseButtonUp(0))
            {
                isClick1 = false;
                LerpTime = 0;
                ClearRecover();

            }
            if (isClick1)
            {
                MoveToWardMouse();


              
            }
        }
        else
        {
            StartCoroutine(GameOver());
            isClick1 = false;
            LerpTime = 0;
            ClearRecover();

        }
       
       
    }
    private void LateUpdate()
    {
     //   SpinBall(LastDirect.normalized,LastDirect.magnitude);
    }
    public float rotationX = 0;
    public float rotationY = 0;
    public float SpeedRotaion = 20;

   
    public void SpinBall(Vector3 direct,float Speed)
    {

        Quaternion quaternionX =Quaternion.identity;
        Quaternion quaternionY = Quaternion.identity;
        if (Mathf.Sign(direct.z)==-1)
        {
            quaternionY = Quaternion.Euler(rotationX += direct.y*Speed, 0, 0);
        }
        else if (Mathf.Sign(direct.z) == 1)
        {
            quaternionY = Quaternion.Euler(rotationX += direct.y*Speed, 0, 0);
        }
        else if (Mathf.Sign(direct.z) == 0)
        {
            quaternionY = Quaternion.Euler(0 , 0, 0);
        }

        if (Mathf.Sign(direct.x) == -1)
        {
            quaternionX = Quaternion.Euler(0,0,rotationY +=  direct.x*Speed);
        }
        else if (Mathf.Sign(direct.x) == 1)
        {
            quaternionX = Quaternion.Euler(0,0,rotationY +=  direct.x*Speed);
        }
        else if (Mathf.Sign(direct.x) == 0)
        {
            quaternionX = Quaternion.Euler(0,0,0);
        }


        transform.rotation = quaternionY*quaternionX;

    }
    public void RotationBall()
    {
      
    }


    public void GoBack(float Force,Vector3 Direct)
    {

        isGoBack = true;
        Body.AddForce((Direct*Force),ForceModeInteracable);
        isClick1 = false;
        LerpTime = 0;
        ClearRecover();
        //    Debug.Log(-getDirect() * SpeedVelocity);
        // ClearRecover();
    }


    public void ClearRecover()
    {
       IncreTimeRecover = false;
       isNewPath = false;
       ListPath.Clear();
       ListDirect.Clear();
       ListPoint.Clear();
    }

    public void AddPoint(Vector3 point)
    {
        int count = ListPoint.Count;
       if(count == 0)
        {
            ListPoint.Add(point);
        }
        else
        {
            if (ListPoint[count - 1] != point)
            {
                IncreTimeRecover = true;
                ListPoint.Add(point);
            }
            else
            {
                IncreTimeRecover = false;
            }
           
        }
    }
    public void AddDirect(Vector3 direct)
    {
        int count = ListDirect.Count;
        if (count == 0)
        {
            ListDirect.Add(direct);
        }
        else
        {
            if (ListDirect[count - 1] != direct)
            {
                isNewPath = false;
                ListDirect.Add(direct);
            }
            else
            {
                isNewPath = true;
            }
           
        }
    }
    public void  GeneratePath()
    {
        if (isNewPath)
        {

            Point point = new Point();
            point.Pos = transform.position;
            point.Time = 0;
            if (ListPath.Count > 0)
            {
                point.Speed = ListPath[ListPath.Count - 1].Speed;
            }
            ListPath.Add(point);

        }
        else
        {
            if (ListPath.Count != 0)
            {
                RunningRecover();
            }
        }
    }

    public void RunningRecover()
    {
        int lastIndex = ListPath.Count - 1;
        if (ListDirect.Count != 0)
        {
            if (IncreTimeRecover)
            {
                ListPath[lastIndex].Time += Time.deltaTime * SpeedTimeRecover;
            }
          

        }
    }
    public Vector3 getDirect()
    {
        if (ListDirect.Count != 0)
        {
            return  new Vector3(ListDirect[ListDirect.Count - 1].x,0,-ListDirect[ListDirect.Count - 1].y).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }


    public  void ResetStatus()
    {
                
    }

    public Vector3 DetectDirect(List<Vector3> listPoint)

    {
        Vector3 direct;
        
            if (ListPoint.Count == 1)
            {
                return Vector3.zero;
            }

            direct = ListPoint[ListPoint.Count] - ListPoint[ListPoint.Count-1];
        return direct;

        
    }

    public void MoveToWardMouse()
    {
       
        //   SetTargetPosition();
        if (LerpTime >= 0 && LerpTime <0.1f)
        {
            LerpTime += Time.deltaTime*SpeedVelocityStart;
        }
        else
        {
            LerpTime += Time.deltaTime * SpeedVelocityMax;
        }

        SpeedVelocity = maxVelocity;


        //   Body.velocity = new Vector3(LastDirect.x * SpeedVelocity, 0, -LastDirect.y * SpeedVelocity);
      
        if (!isMovePath)
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                LastDirect = ((new Vector3(hit.point.x, 0, hit.point.z) - new Vector3(transform.position.x, 0, transform.position.z))).normalized;
                LastDirect = new Vector3(LastDirect.x,0, LastDirect.z);
                
            }

            Body.AddForce(LastDirect*SpeedVelocity,ForceModeMoce);
          //  Body.AddForce(new Vector3(LastDirect.x * SpeedVelocity, 0, -LastDirect.y * SpeedVelocity));
           // Body.AddForce(LastDirect* SpeedVelocity,ForceMode.VelocityChange);
         //   Debug.Log("ADD FORCE_1");
        }   
        else
        {
            Debug.Log("ADD FORCE_2");
            Body.AddForce(getDirect() * SpeedVelocity);

        }

      //  Body.velocity = Vector3.ClampMagnitude(Body.velocity, maxVec);
        Force = Vector3.Magnitude(Body.velocity);



    }
  
    public void RotateAroundMouse(Vector3 direct)

    {

      //  transform.rotation = Quaternion.Euler(Time.deltaTime * moveSpeed, 1, 1);

    }

    public void Limit_Velocity()
    {


    }

    

    public void SetTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, 1000))
        {
            LastDirect = hit.point;
        }

    }

    //Path
 

    Vector3 PosTarget = Vector3.zero;
    public bool isNext()
    {
        if (ListPath.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
   
    
    public void InitGird()
    {
       // Debug.Log(Width + "  " + Height);
        int x = (int)( Width / offSetX);
        int y = (int)(Height / offSetY);
        Gird = new Vector2[x, y];
      //  Debug.Log(x+" "+y);
        for(int i=y-1; i>=0; i --)
            for(int j = 0; j <x; j++)
            {
                Gird[j,i] = new Vector2(j*offSetX,Height-i*offSetY);
            }
        
    }
    public void IncreTime()
    {
        if (ListPath.Count > 0)
        {
            ListPath[0].Time += Time.deltaTime;
        }
    }
    public Point GetNodeCurr()
    {
        return ListPath[0];
    }
  



    private void OnDrawGizmos()
    {
        
        Rect rectInt = new Rect(1, 1, 1, 1);
        int x = (int)(Width / offSetX);
        int y = (int)(Height / offSetY);
        Vector3 PosInit = Camera.main.ScreenToViewportPoint(new Vector3(0, 1280, 0));
        InitGird();
        for (int i = 0; i <=y; i++)
        {
           

            Debug.DrawLine(new Vector3(0, i * offSetY, 0), new Vector3(720, i * offSetY, 0));
        }
        for (int i = 0; i <=x; i++)
        {
            Gizmos.color = Color.blue;
            
            Debug.DrawLine(new Vector3(i * offSetX, 0, 0), new Vector3(i * offSetX, 1280, 0));

        }
        
        



    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
          //  collision.gameObject.GetComponent<Enemy>().MoveBack(gameObject);        
        }
    }


    public Vector3 getPosWordSpace(Vector3 vector3) {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            Vector3 pos = new Vector3(hit.point.x, hit.point.y,hit.point.z) ;
            return pos;
        }
        else
        {
            return Vector3.zero;
        }
       

    }



    private void FixedUpdate()
    {
       
       // Body.AddTorque(Body.angularVelocity.normalized * 30, ForceMode.Impulse);
    }
    public IEnumerator GameOver()
    {
      
        yield  return new WaitForSeconds(0.75f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }




}