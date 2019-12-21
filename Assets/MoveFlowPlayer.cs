using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFlowPlayer : MonoBehaviour
{
   public GameObject player;
    public float offset = 5;
    public float Speed = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = Vector3.MoveTowards(transform.position,new Vector3(player.transform.position.x, transform.position.y,player.transform.position.z-offset),Speed*Time.deltaTime);
    }
}
