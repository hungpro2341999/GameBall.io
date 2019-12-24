using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WallCollison { TOP, DOWN, LEFT, RIGHT }
public enum Type_Status { ATTACK, DODGE, MOVE, RUN_AWAY }
public enum Type_AI { BLOOD_WAR, TROLL, NOBLE, NORMAL, SMART, FRIEND }
public enum RANK { BLOOD_WALL, DODGE, MOVE, RUN_AWAY }
public enum WARRINGLEVEL { SAFE, ATTACK, WARRING }
public class Enemy : MonoBehaviour
{

    

    public AiStatus Status;
    public Text text;
    public ForceMode ForceModeWhenMove;
    public ForceMode ForceModeWhenInteraction;
    public ForceMode ForceModeWhenBrake;
    public Type_Status status = Type_Status.MOVE;
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
    public bool isLoopDirect = false;
    // RunAway

    public Player[] playerRunAway;
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
            switch (status)
            {
                case Type_Status.MOVE:
                 
                    Move();
                    break;
                case Type_Status.ATTACK:
                    Attack();
                    break;
                case Type_Status.DODGE:
                    StartCoroutine(Dodge());
                   
                    break;
                case Type_Status.RUN_AWAY:

                    break;
            }

            MoveFollowDirect();





        }
        else
        {
            body.constraints = RigidbodyConstraints.None;
        }
       
     

       

        //InforBall

        Force = Vector3.Magnitude(body.velocity);
    }


    private void MoveFollowDirect()
    {
        // int direcy = (int)ScoreOfDirectx8();

        if (!isMoveBack)
        {
            // Try it   


            if (!isMoveLimit)
            {


                if (Vector3.Magnitude(body.velocity) <= MaxVelocity)
                {

                    body.AddForce(DirectMove * Speed, ForceModeWhenMove);

                }
                else
                {

                    body.AddForce(DirectMove, ForceMode.VelocityChange);
                }

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
               

                body.velocity = body.velocity / Drag;

                body.AddForce(Direct.normalized * Mathf.Clamp(Vector3.Magnitude(body.velocity), 20, 70) * Brake, ForceModeWhenBrake);
                //     Debug.Log("Coll" + (hit[i].collider.gameObject.name));

                isMoving = false;
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


    



   

  
  
   
    public IEnumerator Dodge()
    {



    
        List<Vector3> direct = new List<Vector3>() ;
        float[] Score;
       for(int i = 0; i < playerRunAway.Length; i++)
        {
          

                for(int j = 0; j < directX8.Length; j++)
                {
                    if (!IsCollWithPlayer(directX8[j], playerRunAway[i]))
                    {
                        if (!direct.Contains(direct[j]))
                        {
                            direct.Add(directX8[j]);
                        }
                    }
                    else
                    {
                        if (direct.Contains(direct[j]))
                        {
                            direct.Remove(directX8[j]);

                        }
                    }
                 }
            

            yield return new WaitForSeconds(0);
        }
        Score = new float[direct.Count];
      
       for(int i = 0; i < direct.Count; i++)
        {
            Score[i] = DistanceFromWall(direct[i]);


        }
        int index = getIndexMax(Score);

        DirectMove = direct[index];
        

       

    }

    public bool IsCollWithPlayer(Vector3 direct,Player player)
    {
       for(int i = 0; i < ListRay.Count; i++) 
        
        {

            RaycastHit[] hits = Physics.RaycastAll(ListRay[i].transform.position, direct, Mathf.Infinity, MaskPlayer);
            for(int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider.gameObject == player.gameObject)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Warring()
    {

    }
   
   

    public void Attack()
    {

        if (Target != null)
        {
            DirectMove = new Vector3(Target.transform.position.x, 0, Target.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z).normalized;

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
    
    
   
    private int ScoreOfDirectx8()
    {
        int index = 0;
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

        float[] CopyScore;
        float[] ScoreGood = GetArrayMax(score, 4);
        int r = Random.Range(0, 4);
        Debug.Log(r);
        index =  getIndex(score, ScoreGood[r]);
        

       
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
   
    


    private IEnumerator RandomDirect()
    {
        if (!isMoving)
        {
            isMoving = true;
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
            isMoving = false;
            int direct = ScoreOfDirectx8();
            DirectMove = directX8[direct];


        }


    }
    public void Move()
    {
        StartCoroutine(RandomDirect());

        if (GetEnemyInRadius(Radius, transform.position, transform.up)!=0)
        {
            Target = GetTargert(Radius);
          
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
                Score[i] = (DistanceToItsSelf - DistanceNearLimit) / Magtidue;

            }
            int indexMax = getIndexMax(Score);
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
        string s = "";
        string s1 = "";

        List<float> Scores = new List<float>();
        List<float> ScoresMax = new List<float>();
        for (int i=0;i< Score.Length; i++)
        {
            Scores.Add(Score[i]);
            s += " " + Score[i];
        }

        for(int i = 0; i < number; i++)
        {
            float max = getValueMax(Score);
            ScoresMax.Add(max);
            Score = RemoveArray(max, Score);
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
                return index;
            }
        }
        return 0;
    }

}





