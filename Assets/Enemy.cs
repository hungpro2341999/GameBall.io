using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{

   public enum WallCollison {TOP,DOWN,LEFT,RIGHT}
    public enum Type_Status { ATTACK, DODGE, MOVE, RUN_AWAY }
    public enum Type_AI { BLOOD_WAR,TROLL, NOBLE, NORMAL,SMART,FRIEND }
    public enum RANK { BLOOD_WALL, DODGE, MOVE, RUN_AWAY }
    public enum WARRINGLEVEL {SAFE,ATTACK, WARRING }
    public AiStatus Status;
    public Text text;
    public ForceMode ForceModeWhenMove;
    public ForceMode ForceModeWhenInteraction;
    public ForceMode ForceModeWhenBrake;
    public GameObject Ground = null;
    public Rigidbody body;
    public Player Target;
    public float Speed=3;
    public Vector3 Direct;
    public  bool isMoveBack = false;
    public LayerMask MaskPlayer;
    public LayerMask WallLayer;
    public float Force;
    public float Bonnd;
    public float Mass;
    public float ForceMass=1;
    public float ForceIntereact;
    public float CheckGround = 1;
    public bool isGround = false;
    
    
    //Local Variable
    public List<Transform> ListRay = new List<Transform>();

    float initPoint;

    //Info Strenght Ball
    public float distanceRay;
    public  float Brake = 2;
    public float Drag = 2;
    public float Radius = 1;
    // Go Back
    public float MinForce;
    public float MaxVelocity;

    public bool isMoveLimit = false;
    public float Bound;
    public float MassLimit;
    public float Weight = 1;

    //AI
    
    public float ForceIntertion;
    public Vector3 DirectMove = Vector3.zero;
    public float maxTime = 4;
    public float minTime = 2;
    public bool isMoving = false;

    // RunAway

    public Player playerRunAway;
    public Vector3[] directX8 = { new Vector3(0, 0, 1) ,new Vector3(0.5f,0,0.5f),new Vector3(1,0,0),new Vector3(0.5f,0,-0.5f),new Vector3(-0.5f,0,-0.5f),new Vector3(-0.5f,0,0.5f),new Vector3(-1,0,0),new Vector3(0,0,-1)};
    public Vector3[] directx4 = { new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, -1) };


    //Local Variable

    public float Distance = 50;
    // Start is called before the first frame update
    void Start()
    {
     
        initPoint = Speed;
        body = GetComponent<Rigidbody>();
        Target = GamePlayerCtrl.Instance.getEneMyNearst(GetComponent<Player>());
    }

    // Update is called once per frame
    void Update()
    {
        //  UI



        StartCoroutine(LetRun());

        if (Input.GetKeyDown(KeyCode.B))
        {
           



        }

        if (Physics.Raycast(new Ray(transform.position, -Vector3.up), CheckGround))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
            body.constraints = RigidbodyConstraints.None;
        }

        if (Physics.Raycast(new Ray(transform.position, -Vector3.up), CheckGround))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
        if (Stop)
        {
            isGround = false;
        }


        if (isGround)
        {
           
            if (!isMoveBack)
            {
             // Try it   





                if (Target != null)
                {




                    if (!isMoveLimit)
                    {
                        Direct = new Vector3(Target.transform.position.x, 0, Target.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z);



                        
                  
                        if (Vector3.Magnitude(body.velocity) <= MaxVelocity)
                        {
                         
                            body.AddForce(DirectMove * Speed,ForceModeWhenMove);

                        }
                        else
                        {
                      
                            body.AddForce(DirectMove , ForceMode.VelocityChange);
                        }
                        Force = Vector3.Magnitude(body.velocity);
                        ICanNotDead();


                    }

                    else
                    {
                        ICanNotDead();
                        if (Vector3.Magnitude(body.velocity) < MassLimit)
                        {
                            isMoveLimit = false;
                        }
                    }

                }
            }
            else
            {
                if (Vector3.Magnitude(body.velocity) < Mass)
                {
                    isMoveBack = false;
                }
            }



        }
        else
        {
            body.constraints = RigidbodyConstraints.None;
        }
    }
    bool Stop = false;
    private void FixedUpdate()
    {
       
        //ICanNotDead();
    }
    public void ICanNotDead()
        {
        Ray ray = new Ray(transform.position,Direct.normalized);

       

           RaycastHit[] hit = null;
        int count = CountEnemyFoward();
        if (count > 1)
        {
         //   Debug.Log(count +""+gameObject.name);
        }
       
        for(int i = 0; i < ListRay.Count; i++)
        {
            Ray ray_check = new Ray(ListRay[i].transform.position, body.velocity.normalized);
          
        }
       
        hit = Physics.RaycastAll(new Ray(transform.position,body.velocity.normalized), distanceRay);

            for (int i = 0; i < hit.Length; i++)
            {
                     


                if (hit[i].collider.gameObject.layer == 12)
                {
               

                body.velocity = body.velocity / Drag;

                body.AddForce(Direct.normalized * Mathf.Clamp(Vector3.Magnitude(body.velocity), 20, 70) * Brake, ForceModeWhenBrake);
                //     Debug.Log("Coll" + (hit[i].collider.gameObject.name));
                MoveRandom();
                StartCoroutine(TurnoffLimit(0.75f));


                isMoveLimit = true;


                break;
            }
          
        }
        }

        public void MoveBack(GameObject player)
        {

       
        
       
        float ForcePlayer = player.GetComponent<RollBall>().Force;
        float BoundPlayer = player.GetComponent<RollBall>().Bound;
        float ForceBack = ForcePlayer+Force;
        float Mass = player.GetComponent<RollBall>().Mass;
        
        if (ForcePlayer > Force)
        {
            Debug.Log("Force Player");
            ForceIntertion = ForceBack * Bound;
            Debug.Log(ForceIntertion);
          
            body.AddForce(-Direct.normalized *ForceIntertion,ForceModeWhenInteraction);
            player.GetComponent<RollBall>().GoBack(Mathf.Clamp(ForceIntertion, MinForce, Mathf.Infinity)/3, Direct.normalized);

        }
        else
        {
            Debug.Log("Force Enemy");
            ForceIntertion = ForceBack * Bound;
            ForceIntertion = Mathf.Clamp(ForceIntertion, MinForce, Mathf.Infinity);
            body.AddForce(-Direct.normalized *Mathf.Clamp(ForceIntertion,MinForce,MinForce*2), ForceModeWhenInteraction);
            player.GetComponent<RollBall>().GoBack(Mathf.Clamp(ForceIntertion, MinForce, Mathf.Infinity)/ 5, Direct.normalized);
            isMoveBack = true;
        }
       
               
             
            /*
            else
            {

                 Debug.Log("Force Enemy");
                ForceBack = (Force - ForcePlayer);
                //     Debug.Log(Direct.normalized * ForceBack * Bonnd);
                body.AddForce(-Direct.normalized * ForceBack*BoundPlayer/Weight,ForceMode.VelocityChange);
                player.GetComponent<RollBall>().GoBack(ForceBack * Bonnd/5, Direct);


            }

        */
          



        }

    public int CountEnemyFoward()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        Vector3 pos = new Vector3(x, transform.position.y, z);
        Vector3 direct = body.velocity.normalized;
        Ray ray_check = new Ray(new Vector3(x, transform.position.y, z), direct);

        int j = 1;
        RaycastHit hit_2;
        GameObject game;
        for (int i = 0; i< ListRay.Count; i++){
            x = ListRay[0].position.x;
            y = ListRay[1].position.y;
            z = ListRay[2].position.z;

            while (Physics.Raycast(new Ray(new Vector3(x, y, z), body.velocity.normalized), out hit_2, distanceRay, MaskPlayer))
            {
                game = hit_2.collider.gameObject;

                direct = game.GetComponent<Enemy>().body.velocity.normalized;
                x = game.transform.position.x;
                y = game.transform.position.y;
                z = game.transform.position.z;

                j += 1;
                /*
                if (Physics.Raycast(new Ray(new Vector3(x, y, z), new Vector3(0, 0, 1)), out hit_2, distance, MaskPlayer)){
                    game = hit_2.collider.gameObject;
                    x = game.transform.position.x;
                    y = game.transform.position.y;
                    z = game.transform.position.z;
                    Gizmos.DrawLine(transform.position, game.transform.position);
                }
                if (Physics.Raycast(new Ray(new Vector3(x, y, z), new Vector3(0, 0, 1)), out hit_2, distance, MaskPlayer))
                {
                    game = hit_2.collider.gameObject;
                    x = game.transform.position.x;
                    y = game.transform.position.y;
                    z = game.transform.position.z;
                    Gizmos.DrawLine(transform.position, game.transform.position);
                }
                   */



            }
        
        }
        return j;


    }

    
    private void OnGUI()
    {
        
    }
    public IEnumerator TurnoffLimit(float time)
    {
        
        isMoveLimit = true;
        yield return new   WaitForSeconds(time);
        isMoveLimit = false;
    }


    //


    public float Range;




    public void RangeRadio()
    {
         
    }

    public GameObject getEnemyNearts()
    {


        return null;


    }
   

    public void MoveAround()
    {

    }
    public void RunAway()
    {
       
    }
    public void Dodge()
    {
        playerRunAway = GamePlayerCtrl.Instance.getEneMyNearst(GetComponent<Player>());
        

    }
    public void Warring()
    {

    }
    public void MoveRandom(Vector3 direct)
    {
        Direct = direct;
    }
    public void MovePath(Vector3[] ListDirect, float time)
    {
        
    }
    IEnumerator StartMovePath(Vector3[] ListDirect, float[] time)
    {
        int count = ListDirect.Length;
        int i = 0;
        while (i < count)
        {

            MoveRandom(ListDirect[i]);
            yield return new WaitForSeconds(time[i]);
            i++;

        }

    }
    public void Attack(Player player)
    {
        Target = player;
    }
    public void DistanceFromLimit()
    {

    }
    public float Magtidue()
    {
        return Vector3.Magnitude(body.velocity);
    }

    public float DistanceFromLimitNeart()
    {
        float distance = Mathf.Infinity;
       for(int i = 0; i < directx4.Length; i++)
        {
            Ray ray = new Ray(transform.position, directx4[i]);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
            if (hits.Length > 0)
            {
                for(int j = 0; j < hits.Length; j++)
                {
                    if(hits[j].collider.gameObject.layer == 12)
                    {
                        if (Vector3.Distance(transform.position, hits[j].point) < distance)
                        {
                          
                            distance = Vector3.Distance(transform.position, hits[j].point);
                            Debug.Log(hits[j].collider.gameObject.name + "  TARGET " + distance);
                        }
                    }
                }
            }
           
        }
        return distance;
    }


    public float DistanceFromWall(Vector3 direct)
    {
        float distance = 0;
        Ray ray = new Ray(transform.position,direct);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        if (hits.Length > 0)
        {
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider.gameObject.layer == 12)
                {
                   
                        distance = Vector3.Distance(transform.position, hits[j].point);
                //        Debug.Log(hits[j].collider.gameObject.name + "  TARGET " + distance);
                    break;
                }
            }
        }
                return distance;
       

       
    }
    
    

    public float ScoreOfDirectx8()
    {
        /*
        for(int i = 0; i < directX8.Length; i++)
        {
            Ray ray = new Ray(transform.position, directX8[i]);
          RaycastHit[] hits =   Physics.SphereCastAll(ray,Radius,Range,MaskPlayer);
            
            Debug.Log(gameObject.name + " " + directX8[i] + "  " + hits.Length);
              
        }
        */
        float[] score = new float[8];
        float[] DistanceToWall = new float[8];
        float[] Time = new float[8];
        for (int i = 0; i < directX8.Length; i++)
        {
            score[i] = DistanceFromWall(directX8[i]);

        }
        float index = getValueMax(score);

        for(int i = 0; i < directX8.Length; i++)
        {
            Ray ray = new Ray(transform.position,directX8[i]);
            RaycastHit[] hits = Physics.RaycastAll(ray,distanceRay,WallLayer);
            for(int j = 0; j < hits.Length; j++)
            {
              
            }
          
        }
        return index;
       
    

    }
    

    public int getCountEnemyInDirect(float time,Vector3 direct)
    {
        int count = 0;
        for(int i = 0; i < ListRay.Count; i++)
        {
            Ray ray = new Ray(ListRay[i].transform.position, direct);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, MaskPlayer);
            count += hits.Length;

        }
        return count;
    }

    public void MoveRandom()
    {
        int direcy = (int)ScoreOfDirectx8();
        DirectMove = directX8[direcy];
    }
    public IEnumerator LetRun()
    {
        if (!isMoving)
        {
            isMoving = true;
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
            isMoving = false;
            MoveRandom();
        }
       
       
    }

    public void Dodge(Vector3 directAttack)
    {

    }
    // Move Random
    public void ScoreTarget()
    {
        

            float[] ScoreTarget = GamePlayerCtrl.Instance.getScoreTarget(GetComponent<Player>());
        
      
      
    }

    
    private void OnDrawGizmos()
    {
        if (Target != null)
        {
            Gizmos.DrawLine(transform.position, Target.transform.position);
        }
        ScoreOfDirectx8();
        Gizmos.DrawWireSphere(transform.position, Radius+Range);
        Gizmos.color = Color.blue;
       for (int i = 0; i < ListRay.Count; i++)
        {
                Gizmos.DrawLine(ListRay[i].transform.position, ListRay[i].transform.position + body.velocity.normalized * 10);
        }

        /*
        
        for (int i = 0; i < ListRay.Count; i++)
        {
         //   Debug.Log("Finddd "+i);
           
        }
       float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        Vector3 pos = new Vector3(x, transform.position.y, z);
        Ray ray_check = new Ray(new Vector3(x,transform.position.y,z), new Vector3(0, 0, 1));
        Gizmos.DrawRay(ray_check);
        int j = 0;
        RaycastHit hit_2;
        GameObject game;
        while(Physics.Raycast(new Ray(new Vector3(x,y, z), new Vector3(0, 0, 1)),out hit_2,distance,MaskPlayer))
        {
            game = hit_2.collider.gameObject;
            Debug.Log(game.name);

            x = game.transform.position.x;
            y= game.transform.position.y;
            z = game.transform.position.z;
          
            Gizmos.DrawLine(transform.position, game.transform.position);
            Gizmos.color = Color.black;
            Gizmos.DrawRay(new Ray(new Vector3(x, transform.position.y, z), new Vector3(0, 0, 1)));
            j += 1;
            /*
            if (Physics.Raycast(new Ray(new Vector3(x, y, z), new Vector3(0, 0, 1)), out hit_2, distance, MaskPlayer)){
                game = hit_2.collider.gameObject;
                x = game.transform.position.x;
                y = game.transform.position.y;
                z = game.transform.position.z;
                Gizmos.DrawLine(transform.position, game.transform.position);
            }
            if (Physics.Raycast(new Ray(new Vector3(x, y, z), new Vector3(0, 0, 1)), out hit_2, distance, MaskPlayer))
            {
                game = hit_2.collider.gameObject;
                x = game.transform.position.x;
                y = game.transform.position.y;
                z = game.transform.position.z;
                Gizmos.DrawLine(transform.position, game.transform.position);
            }
               */

       
    }

    public float getValueMax(float[] score)
    {
        float index = 0;
        float max = 0;
        for(int i = 0; i < score.Length; i++)
        {
            if (score[i] > max)
            {
                index = i;
                max = score[i];
            }
        }
        return index;
    }

    public float getValueMin(float[] score)
    {
        float index = 0;
        float min = Mathf.Infinity;
        for (int i = 0; i < score.Length; i++)
        {
            if (score[i] < min)
            {
                index = i;
                min = score[i];
            }
        }
        return index;
    }

  

   
    
}





