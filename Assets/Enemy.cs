using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WallCollison { TOP, DOWN, LEFT, RIGHT }
public enum Type_Status { ATTACK, DODGE, MOVE, RUN_AWAY }
public enum Type_AI { BLOOD_WAR, TROLL, NOBLE, NORMAL, SMART, FRIEND }
public enum RANK { BLOOD_WALL, DODGE, MOVE, RUN_AWAY }
public enum WARRING_LIMIT {LV1,LV2,LV3,LV4,LV5}
public enum WARRING_ENEMY {LV1,LV2,LV3,LV4 }
public enum POWER {LV1,LV2,LV3,LV4,LV5}
public class Enemy : MonoBehaviour
{

    
   
    public AiStatus Status;
    public Text text;
    public ForceMode ForceModeWhenMove;
    public ForceMode ForceModeWhenInteraction;
    public ForceMode ForceModeWhenBrake;
    public Type_Status status = Type_Status.MOVE;
    public WARRING_LIMIT type_warring;
    public POWER type_power;
    public WARRING_ENEMY type_warring_enemy;
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
    public float Range;
   

    //Local Variable
    public List<Transform> ListRay = new List<Transform>();

    float initPoint;

    //Info Strenght Ball
    public float weight;
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
    public bool isRunAway = false;
    public bool isDodge = false;
    public float ForceIntertion;
    public Vector3 DirectMove = Vector3.zero;
    public float maxTime = 4;
    public float minTime = 2;
    public bool isMoving = false;
    public bool isLoopDirect = false;
    public int index_Enemy = 0;
    public int index_Limit = 0;
    public int index_Power = 0;
    public int index_Skill = 0;
    public int index_scoward = 0;
    public float indexPower;

    public int index_Blood_War = 0;
    public int index_Dodge = 0;

    // RunAway
    public Player[] playerRunAway;
    public Player PlayerDodge;
    public Vector3[] directX8 = { new Vector3(0, 0, 1) ,new Vector3(0.5f,0,0.5f),new Vector3(1,0,0),new Vector3(0.5f,0,-0.5f),new Vector3(-0.5f,0,-0.5f),new Vector3(-0.5f,0,0.5f),new Vector3(-1,0,0),new Vector3(0,0,-1)};
    public Vector3[] directx4 = { new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, -1) };


    //Local Variable

    public float Distance = 50;
    bool Stop = false;
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
        //  CheckGround
        if (Input.GetKeyDown(KeyCode.B))
        {


            Debug.Log(gameObject.name + "  " + GetEnemyInRadius(10, transform.position, body.velocity.normalized));

        }

        if (Physics.Raycast(new Ray(transform.position, -Vector3.up), CheckGround))
        {
            body.constraints = RigidbodyConstraints.FreezePositionY;
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


        /// Move

        if (isGround)
        {
            Power();
            Warring_Enemy();
            Warring_Limit();
            MoveFollowDirect();
           
            if (type_warring == WARRING_LIMIT.LV5)
            {
                FindAgain();
            }




        }
        else
        {
            body.constraints = RigidbodyConstraints.None;
        }
        
           

       

        //InforBall

        Force = Vector3.Magnitude(body.velocity);
    }
    public void FindAgain()
    {
        if(status == Type_Status.MOVE)
        {
            isMoving = false;
            StartCoroutine(RandomDirect(directX8));
        }
        else if(status == Type_Status.DODGE)
        {
            isDodge = false;
            StartCoroutine(Start_Dodge());
          
        }else if(status == Type_Status.RUN_AWAY)
        {
            isRunAway = false;
            StartCoroutine(Start_RunAway());
        }
      
    }

    private void MoveFollowDirect()
    {
        // int direcy = (int)ScoreOfDirectx8();

        if (!isMoveBack)
        {
            // Try it   
            Process_Status();

            if (!isMoveLimit)
            {


                if (Vector3.Magnitude(body.velocity) <= MaxVelocity)
                {
                  
                    switch (status)
                    {
                        case Type_Status.MOVE:

                            Move();
                            break;
                        case Type_Status.ATTACK:
                            Attack();
                            break;
                        case Type_Status.DODGE:
                            StartCoroutine(Start_Dodge());

                            break;
                        case Type_Status.RUN_AWAY:
                            StartCoroutine(Start_RunAway());
                            break;
                    }
                    body.AddForce(DirectMove * Speed, ForceModeWhenMove);
                 //   Debug.Log("Forece");
                }
                else
                {

               
                 //   Debug.Log("Forece_Limit");
                }

                ICanNotDead();

            }

            else
            {
                
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


    private void FixedUpdate()
    {
       
        //ICanNotDead();
    }
    public void ICanNotDead()
        {
        Ray ray = new Ray(transform.position,Direct.normalized);

       

           RaycastHit[] hit = null;
       
       
        hit = Physics.RaycastAll(new Ray(transform.position,body.velocity.normalized), distanceRay);

            for (int i = 0; i < hit.Length; i++)
            {
                     


                if (hit[i].collider.gameObject.layer == 12)
                {
              //  Debug.Log("COll");

                body.velocity = body.velocity / Drag;

            
                  //   Debug.Log("Coll" + (hit[i].collider.gameObject.name));
             
                StopAllCoroutines();
                if (status == Type_Status.MOVE)
                {

                    isMoving = false;
                    StartCoroutine(RandomDirect(directX8));

                    //   Debug.Log("Change : ");

                }
                else if(status == Type_Status.DODGE)
                {
                    isDodge = false;
                    StartCoroutine(Start_Dodge());
                   
                }
                else if (status == Type_Status.RUN_AWAY)
                {
                    isRunAway = false;
                    StartCoroutine(Start_RunAway());

                }

                break;
            }
          
        }
        }

        public void MoveBack(GameObject player)
        {

       
       
       
        float ForcePlayer = player.GetComponent<Enemy>().Force;
        float weightPlayer = player.GetComponent<Enemy>().weight;
        float BoundPlayer = player.GetComponent<Enemy>().Bound;
        Rigidbody body_1 = player.GetComponent<Enemy>().body;
        float ForceBack = ForcePlayer+Force/8;
        float ForceIntertion_1 = 0;

        if (ForcePlayer < Force)
        {
            //    Debug.Log("Force Player");
            body.velocity = Vector3.zero;
            ForceIntertion = ForceBack; 
            ForceIntertion_1 = ForceBack * BoundPlayer;
            //      Debug.Log(ForceIntertion);
            isMoveBack = true;
            AddForce((-DirectMove.normalized * ForceIntertion_1/weight),ForceModeWhenInteraction,ForceIntertion_1);
            
            player.GetComponent<Enemy>().AddForce((DirectMove.normalized * ForceIntertion/weightPlayer), ForceModeWhenInteraction, ForceIntertion);

        }
       
 
             
     



        }

    public void AddForce(Vector3 Force,ForceMode Force_Mode,float ForceInteraction)
    {
        isMoveBack = true;
        body.velocity = Vector3.zero;
        this.ForceIntertion = ForceInteraction; 
            body.AddForce(Force, Force_Mode);
      
     
           
        
      
    }

    private void OnCollisionEnter(Collision collision)
    {
       // Debug.Log("Coll");
        if (collision.gameObject.layer == 10)
        {
            MoveBack(collision.gameObject);
        }
         
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

        public IEnumerator Start_Dodge()
    {
        if (!isDodge)
        {
            Dodge();
           
           
            StartCoroutine(Restore_Dodge(Random.Range(minTime, maxTime)));
            yield return new WaitForSeconds(0);


        }


    }

    public IEnumerator Restore_Dodge(float time)
    {
        isDodge = true;
        yield return new WaitForSeconds(time);
        isDodge = false;
    }
    public IEnumerator Start_RunAway()
    {
        if (!isRunAway)
        {


            RunAway();
            StartCoroutine(Restore_RunAway(Random.Range(minTime, maxTime)));
            yield return new WaitForSeconds(0);


        }


    }



    public IEnumerator Restore_RunAway(float time)
    {
        isRunAway = true;
        yield return new WaitForSeconds(time);
        isRunAway = false;
    }
    public void RunAway()
    {
        if (playerRunAway != null)
        {
            List<Vector3> direct = new List<Vector3>();
            for (int i = 0; i < directX8.Length; i++)
            {
                direct.Add(directX8[i]);
            }
            float[] Score;
            for (int i = 0; i < playerRunAway.Length; i++)
            {


                for (int j = 0; j < directX8.Length; j++)
                {
                    if (IsCollWithPlayer(directX8[j], playerRunAway[i]))
                    {
                      //  Debug.Log("Remove Direct : " + gameObject.name + " " + playerRunAway[i].gameObject.name + "  " + directX8[j]);
                        if (direct.Contains(directX8[j]))
                        {
                            direct.Remove(directX8[j]);

                        }
                    }


                }



            }
            Score = new float[direct.Count];
            string s1 = "";
            for (int i = 0; i < direct.Count; i++)
            {
                s1 += " " + direct[i];
            }

            int index = ScoreOfDirect(direct.ToArray());

            DirectMove = direct[index];
           // Debug.Log(s1);
            //Debug.Log(DirectMove);
        }
        else
        {
            status = Type_Status.MOVE;
        }
        

    }



    public void Dodge()
    {
        if (PlayerDodge != null)
        {
            bool isDodge = false;
            List<Vector3> direct = new List<Vector3>();
            float[] Score;
            for (int i = 0; i < directX8.Length; i++)
            {
                direct.Add(directX8[i]);
            }


            for (int j = 0; j < directX8.Length; j++)
            {
                
                if (IsCollWithPlayer(directX8[j], PlayerDodge))
                {
                    if (direct.Contains(directX8[j]))
                    {
                        isDodge = true;
                        direct.Remove(directX8[j]);
                        //       
                    }
                }

            }
            if (isDodge)
            {
                status = Type_Status.DODGE;
            }
            else
            {
                status = Type_Status.ATTACK;
            }






            Score = new float[direct.Count];
          //    Debug.Log(direct.Count + "  "+directX8.Length);

            int index = ScoreOfDirect(direct.ToArray());

             DirectMove = direct[index];
           //Debug.Log(DirectMove);

        }
        else
        {
            status = Type_Status.ATTACK;
        }

    }

    public bool IsCollWithPlayer(Vector3 direct,Player player)
    {
        bool isColl = false;
       for(int i = 0; i < ListRay.Count; i++) 
        
        {
           
            RaycastHit[] hits = Physics.RaycastAll(ListRay[i].transform.position, direct, Mathf.Infinity, MaskPlayer);
            for(int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider.gameObject == player.gameObject)
                {
                    isColl = true; 
                }
            }
            /*
            if(status == Type_Status.DODGE)
            {
                RaycastHit[] hits1 = Physics.RaycastAll(ListRay[i].transform.position, -direct, Mathf.Infinity, MaskPlayer);
                for (int j = 0; j < hits1.Length; j++)
                {
                    if (hits1[j].collider.gameObject == player.gameObject)
                    {
                        isColl = true;
                    }
                }
            }
            */
           
        }
        return isColl;
    }

    public void Process_Status()
    {
     
        Player[] player;
        if (GetEnemyInRadius(Radius, transform.position, transform.up, out player)!=0)
        {
            int indexBall = index_Limit + index_Enemy;
            int isDodge = index_Blood_War + index_Dodge;
            if (indexBall <= index_Blood_War)
            {
              
                status = Type_Status.ATTACK;
                Target = GetTargert(Radius);

                
            }
            else if (isDodge <= index_Dodge && index_Dodge >= index_Blood_War)
            {
             



                    Player[] dodgePlayer;
                    if (GetEnemyInRadius(Radius/2, transform.position, transform.up, out dodgePlayer) != 0)
                    {
                        PlayerDodge = PlayerStrongerst(player);

                        if (PlayerDodge == null)
                        {
                            Debug.Log("Attack_tt");
                            status = Type_Status.ATTACK;
                        }
                        else
                        {
                      //      Debug.Log("Attack_Dodge");
                            status = Type_Status.DODGE;
                        }
                    }
                    else
                    {
                        status = Type_Status.ATTACK;
                    }
                  

                
            }
            else
            {

                // Run Away
                playerRunAway = player;
                status = Type_Status.RUN_AWAY;

            }

        }
        else
        {
          //  Debug.Log("Move");
           // status = Type_Status.MOVE;
            ChangeStatus(Type_Status.MOVE);
        }
        
        
    }
    public void SetPlayerEnemy(Player[] player)
    {
        
    }

    public void ChangeStatus(Type_Status status)
    {
        switch (status)
        {
            case Type_Status.ATTACK:
                PlayerDodge = null;
                playerRunAway = null;
                break;
            case Type_Status.MOVE:
                status = Type_Status.MOVE;
                Target = null;
                PlayerDodge = null;
                playerRunAway = null;
                break;
            case Type_Status.RUN_AWAY:
                Target = null;
                PlayerDodge = null;
                break;
            case Type_Status.DODGE:
                Target = null;
                playerRunAway = null;
                break;
           ;
        }
    }
   
   

    public void Attack()
    {
        if (Target != null)
        {


            if (!Target.GetComponent<Enemy>().isGround)
            {
                Target = null;
                status = Type_Status.MOVE;
                return;
            }
            else
            {
                float ForceTarget = Target.GetComponent<Enemy>().Force;

                float AttackPower = ((Force / DistanceFromLimitNeart()) * Mass);



                DirectMove = (new Vector3(Target.transform.position.x, 0, Target.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;

            }
        }
        else
        {
            status = Type_Status.MOVE;



        }


    }
  
    // DDOGE :::::::::::::
  
   

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
                     //       Debug.Log(hits[j].collider.gameObject.name + "  TARGET " + distance);
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
    
    
   
    private int ScoreOfDirect(Vector3[] directs)
    {
        string s1 = "";
        string s2 = "";

        int index = 0;
        /*
        for(int i = 0; i < directX8.Length; i++)
        {
            Ray ray = new Ray(transform.position, directX8[i]);
          RaycastHit[] hits =   Physics.SphereCastAll(ray,Radius,Range,MaskPlayer);
            
            Debug.Log(gameObject.name + " " + directX8[i] + "  " + hits.Length);
              
        }
        */
      
        float[] score = new float[directs.Length];
        float[] DistanceToWall = new float[directs.Length];
        float[] Time = new float[directs.Length];
        for (int i = 0; i < directs.Length; i++)
        {
            score[i] = DistanceFromWall(directs[i]);

        }

      
        float[] ScoreGood = GetArrayMax(score, directs.Length/2);
        int r = Random.Range(0, directs.Length / 2);
     
        index =  getIndex(score, ScoreGood[r]);
        
        
      for(int i = 0; i < score.Length; i++)
        {
            s1 += "  " + score[i];
        }
      if(gameObject.name == "ball (3)")
        {
         //   Debug.Log(s1);
          //  Debug.Log(gameObject.name + " " + ScoreGood[r] + "  " + index);
        }
     
       
        return index;
       
    
    

    }
    private int ScoreOfDirect(Vector3[] directs,float power)
    {
        string s1 = "";
        string s2 = "";

        int index = 0;
        /*
        for(int i = 0; i < directX8.Length; i++)
        {
            Ray ray = new Ray(transform.position, directX8[i]);
          RaycastHit[] hits =   Physics.SphereCastAll(ray,Radius,Range,MaskPlayer);
            
            Debug.Log(gameObject.name + " " + directX8[i] + "  " + hits.Length);
              
        }
        */

        float[] score = new float[directs.Length];
        float[] DistanceToWall = new float[directs.Length];
        float[] Time = new float[directs.Length];
        for (int i = 0; i < directs.Length; i++)
        {
            score[i] = DistanceFromWall(directs[i]);

        }


        float[] ScoreGood = GetArrayMax(score, directs.Length / 2);
        int r = Random.Range(0, directs.Length / 4);

        index = getIndex(score, ScoreGood[r]);

        for (int i = 0; i < score.Length; i++)
        {
            s1 += "  " + score[i];
        }
        Debug.Log(s1);
        Debug.Log(gameObject.name + " " + ScoreGood[r] + "  " + index);

        return index;



    }




    private int getCountEnemyInDirect(float time,Vector3 direct)
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
   
    


    private IEnumerator RandomDirect(Vector3[] Direct)
    {
        if (!isMoving)
        {

            isMoving = true;
            
            StartCoroutine(Create_Direct(Random.Range(minTime, maxTime)));
           
            int direct = ScoreOfDirect(Direct);
        
            DirectMove = Direct[direct];
         //   Debug.Log(DirectMove);
            yield return new WaitForSeconds(0);
        }


    }
    private IEnumerator Create_Direct(float time)
    {
        yield return new WaitForSeconds(time);
        isMoving = false;
    }

    public void Move()
    {
        StartCoroutine(RandomDirect(directX8));

        // Is Swapped Attack
        if (GetEnemyInRadius(Radius, transform.position, transform.up)!=0)
        {

            Target = GetTargert(Radius);
            status = Type_Status.ATTACK;
          
        }
       
    
       
    }
   
    // Move Random
  
    private int GetEnemyInRadius(float radius, Vector3 pos, Vector3 direct)
    {
        int number = 0;
        Ray ray = new Ray(pos, direct);
        RaycastHit[] hits = Physics.SphereCastAll(ray,radius,0,MaskPlayer);
        number = hits.Length;
        
        string s ="";
        for(int i = 0; i < hits.Length; i++)
        {
           if( hits[i].collider.gameObject != this.gameObject)
            {
                s += "  " + hits[i].collider.gameObject.name;
            }
          
        }
        number--;
      //  Debug.Log(gameObject.name+ " "+s);
        return number;
    }
    private int GetEnemyInRadius(float radius, Vector3 pos, Vector3 direct, out Player[] player)
    {
        int number = 0;

        Ray ray = new Ray(pos, direct);
        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, 0, MaskPlayer);
        number = hits.Length;
        number--;
        int index = 0;
        if (number > 0)
        {
            player = new Player[number];
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject != this.gameObject)
                {
                    player[index] = hits[i].collider.gameObject.GetComponent<Player>();
                    index++;
                }

            }
            return number;
        }
        else
        {
            player = null;
            return 0;
        }

     
     

    }
    /// <summary>
    /// ///////////  
    ///    Caculate Score of Enemy
    /// </summary>
    /// 
    public Player GetTargert(float radius)
    {
        Player[] player;
        float[] Score = new float[GetEnemyInRadius(radius,transform.position,body.velocity.normalized,out player)];
        if (player != null)
        {
            for (int i = 0; i < player.Length; i++)
            {
                float Magtidue = player[i].GetComponent<Enemy>().Magtidue();
                float DistanceNearLimit = player[i].GetComponent<Enemy>().DistanceFromLimitNeart();
                float DistanceToItsSelf = Vector3.Distance(transform.position, player[i].GetComponent<Enemy>().transform.position);
                float indexPower = player[i].GetComponentInChildren<Enemy>().index_Power;
                Score[i] = (DistanceToItsSelf + DistanceNearLimit)*index_Power;

            }
            int indexMin = getIndexMin(Score);
            return player[indexMin];
        }
        else
        {
            return null;
        }
        

       
      



    }
    public Player PlayerStrongerst(Player[] player)
    {
        List<float> Score = new List<float>();
        if (player != null)
        {
            for (int i = 0; i < player.Length; i++)
            {
                Vector3 direct = player[i].GetComponent<Enemy>().body.velocity.normalized;
                if (player[i].GetComponent<Enemy>().IsCollWithPlayer(direct, GetComponent<Player>()))
                {
                  
                    float Magtidue = player[i].GetComponent<Enemy>().Magtidue();
                    float DistanceNearLimit = player[i].GetComponent<Enemy>().DistanceFromLimitNeart();
                    float DistanceToItsSelf = Vector3.Distance(transform.position, player[i].GetComponent<Enemy>().transform.position);
                    float indexPower = player[i].GetComponentInChildren<Enemy>().index_Power;
                    Score.Add(((DistanceToItsSelf + DistanceNearLimit) * index_Power));
                    Debug.Log(gameObject.name + " :: " + player[i]);
                }      
               
            }
            int indexMax = getIndexMax(Score.ToArray());
            return player[indexMax];
        }
        else
        {
            return null;
        }
       
    }

   
    public Vector3 getDirect()
    {
     return   body.velocity.normalized;
    }
    public Player[] player;



   

    public void Warring_Enemy()
    {
        index_Enemy = 0;
       int  level_power = 0;
        Player[] players = null;

        GetEnemyInRadius(Radius*2, transform.position, transform.up, out players);
        for(int i = 0; i < player.Length; i++)
        {
            Vector3 direct = player[i].GetComponent<Enemy>().DirectMove;
            if (player[i].GetComponent<Enemy>().IsCollWithPlayer(direct, GetComponent<Player>()))
            {
                level_power++;
                if (player[i].GetComponent<Enemy>().Force > 40)
                {
                    level_power++;
                }
            }
         
        }
        if (level_power == 0)
        {

            index_Enemy = 1;
            type_warring_enemy = WARRING_ENEMY.LV1; 
        }
        else if( level_power>0 && level_power <2)
        {
            index_Enemy = 2;
            type_warring_enemy = WARRING_ENEMY.LV2;
        }
        else if(level_power>=2 && level_power <3)
        {
            index_Enemy = 3;
            type_warring_enemy = WARRING_ENEMY.LV3;
        }
        else if (level_power >= 3)
        {
            index_Enemy = 4;
            type_warring_enemy = WARRING_ENEMY.LV4;
        }


    }
    

   
    public void Power()
    {
         index_Power = 0;
        float percent = 7;
        for (int i = 0; i < 5; i++)
        {
            if (Force> i * percent && Force < (i + 1) * percent)
            {
                index_Power = i++;
                break;
            }

        }
        Swapped_To_Enmu_Power((int)index_Power);
    }
   

    public void Warring_Limit()
    {
        index_Limit = 0;
        float Percent = 10;
        for(int i = 0; i < 5; i++)
        {
            if(DistanceFromLimitNeart()>i*Percent && DistanceFromLimitNeart() < (i + 1) * Percent)
            {
                index_Limit = i++;
                break;
            }

        }
        Swapped_To_Enmu_Limit_Level((int)index_Limit);
    }

    public void Power_Player()
    {

    }


    public void Swapped_To_Enmu_Power(int level)
    {
        switch (level)
        {
            case 1:
                type_power = POWER.LV1;
                break;
            case 2:
                type_power = POWER.LV2;
                break;
            case 3:
                type_power = POWER.LV3;
                break;
            case 4:
                type_power = POWER.LV4;
                break;
            case 5:
                type_power = POWER.LV5;
                break;
            
               
        }


    }


    public void Swapped_To_Enmu_Limit_Level(int level)
    {
        switch (level)
        {
            case 1:
                type_warring = WARRING_LIMIT.LV5;
                index_Limit = 4;
                break;
            case 2:
                type_warring = WARRING_LIMIT.LV4;
                index_Limit = 3;
                break;
            case 3:
                type_warring = WARRING_LIMIT.LV3;
                index_Limit = 2;
                break;
            case 4:
                type_warring = WARRING_LIMIT.LV2;
                index_Limit = 1;
                break;
            case 5:
                type_warring = WARRING_LIMIT.LV1;
                index_Limit = 0;
                break;
            default:
                index_Limit = 5;
                type_warring = WARRING_LIMIT.LV5;
                break;
        }


    }

    private void OnDrawGizmos()
    {
        if (Target != null)
        {
            Gizmos.DrawLine(transform.position, Target.transform.position);
        }
       
        Gizmos.DrawWireSphere(transform.position, Radius);
      //  Debug.Log(gameObject.name + " " + GetEnemyInRadius(Radius, transform.position, transform.up,out player));
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
        return max;
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
        return min;
    }
   
    public int getIndexMax(float[] score)
    {
        int index = 0;
        float max = 0;
        for (int i = 0; i < score.Length; i++)
        {
            if (score[i] > max)
            {
                index = i;
                max = score[i];
            }
        }
        return index;
    }
    public int getIndexMin(float[] score)
    {
        int index = 0;
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
    public  float[] GetArrayMax(float[] Score ,int number)
    {
        float[] ScoreCopy = new float[8];
        Score.CopyTo(ScoreCopy, 0);

        string s = "";
        string s1 = "";

        List<float> Scores = new List<float>();
        List<float> ScoresMax = new List<float>();
        for (int i=0;i< ScoreCopy.Length; i++)
        {
            Scores.Add(ScoreCopy[i]);
            s += " " + ScoreCopy[i];
        }

        for(int i = 0; i < number; i++)
        {
            float max = getValueMax(ScoreCopy);
            ScoresMax.Add(max);
            ScoreCopy = RemoveArray(max, ScoreCopy);
            s1 += " " + max;
        
        }
      
        return ScoresMax.ToArray();
    }
  

    public  void GetArrayMax(int number)
    {

    }
    

    public float[] RemoveArray(float value,float[] Score)
    {
        int number = Score.Length;
        List<float> list = new List<float>();
        for(int i = 0; i < Score.Length; i++)
        {
            if(Score[i] != value)
            {
                list.Add(Score[i]);
            }
        }
        return list.ToArray();
    }

    public int getIndex(float[] Score,float value)
    {
        int index = 0;
        for(int i = 0; i < Score.Length; i++)
        {
            if(value == Score[i])
            {
             //   Debug.Log("Found :" +value +"  "+ i);
                index = i;
                return index;
            }
        }
        Debug.Log("Not Found");
        return 0;
    }


   

}





