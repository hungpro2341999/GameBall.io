using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerCtrl : MonoBehaviour
{
    public static GamePlayerCtrl Instance;
    public Player[] player;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        InitGame();
    }

    public int CountPlayer()
    {
        return player.Length;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitGame()
    {
        player = GameObject.FindObjectsOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Player getEneMyNearst(Player player)
    {
        if (this.player.Length > 1)
        {
            Player target = null;
            Vector3 pos = player.transform.position;
            float distance = Mathf.Infinity;
            for (int i = 0; i < this.player.Length; i++)
            {
                if (this.player[i].gameObject.name != player.gameObject.name)
                {
                    if (Vector3.Distance(pos, this.player[i].transform.position) < distance)
                    {
                        distance = Vector3.Distance(pos, this.player[i].transform.position);

                        target = this.player[i];
                    }
                }

            }
            //   Debug.Log(player.name + ": Target : " + target.name);

            Debug.DrawLine(player.transform.position, target.transform.position, Color.red);
            return target;
        }
        else
        {
            Debug.Log("You Alone");
            return null;
        }

    }
    public float[] getScoreTarget(Player player)
    {
        float[] Score = new float[this.player.Length - 1];
        ScoreTarget[] Scores = new ScoreTarget[this.player.Length - 1];
        if (this.player.Length > 1)
        {

            Vector3 pos = player.transform.position;

            for (int i = 0; i < this.player.Length; i++)
            {
                if (this.player[i].gameObject.name != player.gameObject.name)
                {

                    float distance = Vector3.Distance(pos, this.player[i].transform.position);
                    float distanceNearstWall = this.player[i].GetComponent<Enemy>().DistanceFromLimitNeart();
                    Scores[i] = new ScoreTarget(distance, distanceNearstWall, this.player[i]);  
                   
                    
                    Debug.Log(player.gameObject.name + "   " + this.player[i].gameObject.name + " " + distance);
                }

            }
            for(int i = 0; i < Score.Length; i++)
            {
                Score[i] = Scores[i].Score(); 
            }
            return Score;

        }
        else
        {


            return Score;

        }

    }
   
    public Player getAllEneMyInRadian(Player player)
    {
        if (this.player.Length > 1)
        {
            Player target = null;
            Vector3 pos = player.transform.position;
            float distance = Mathf.Infinity;
            for (int i = 0; i < this.player.Length; i++)
            {
                if (this.player[i].gameObject.name != player.gameObject.name)
                {
                    if (Vector3.Distance(pos, this.player[i].transform.position) < distance)
                    {
                        distance = Vector3.Distance(pos, this.player[i].transform.position);

                        target = this.player[i];
                    }
                }

            }
            Debug.Log(player.name + ": Target : " + target.name);

            Debug.DrawLine(player.transform.position, target.transform.position, Color.red);
            return target;
        }
        else
        {
            Debug.Log("You Alone");
            return null;
        }

    }

    public void RemovePlayer(Player player)
    {

    }

    public Player getPlayer(int index)
    {
        return this.player[index];
    }

    
    public class ScoreTarget

    {
        public float DistanceNearst;
        public float DistanceNearstNearsLimit;
        public float Magtidue;
        public Player player;

         public ScoreTarget(float DisNearst,float DistanceNearstLimit,Player player)
        {
            this.DistanceNearst = DisNearst;
            this.player = player;
            this.DistanceNearstNearsLimit = DistanceNearstLimit;
        }
         public float Score()
        {
            return DistanceNearst / DistanceNearstNearsLimit;
        }
        public Player GetPlayer()
        {
            return player;
        }
        
        }
    


}
