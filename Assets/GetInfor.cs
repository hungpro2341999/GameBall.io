using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetInfor : MonoBehaviour
{
   public  Player player;
    public Text textVelcocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(player.transform.position);
        if (player.GetComponent<Enemy>() != null)
        {
            textVelcocity.text =  "VELOCITY : "+(int)Vector3.Magnitude(player.GetComponent<Enemy>().body.velocity)+ "\n" + " Direct :"+ player.GetComponent<Enemy>().body.velocity.normalized +
                "\n" + " ForceInteration :" + player.GetComponent<Enemy>().ForceIntertion + "\n" + " ForceInteration :" + player.GetComponent<Enemy>().status.ToString();
        }
        else
        {
            textVelcocity.text = "VELOCITY : " + (int)Vector3.Magnitude(player.GetComponent<RollBall>().Body.velocity)+ "/n FORCE :" ;
        }
    }
}
