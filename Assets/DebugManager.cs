using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebugManager : MonoBehaviour
{
    public Text text;
    public GameObject player;
    public GameObject Enemy;
    public Text StrenghtPlayer;
    public Text StrenghtEnemy;
    public float frame = 60;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (frame > 0)
        {
            frame--;
        }
        else
        {
      
            text.text = "FPS: " + (1 / Time.deltaTime).ToString("n2");
            frame = 3;
        }
       

    }
}
