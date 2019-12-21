using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public float Time = 0;
    public Vector3 Pos;
    public Vector3 posTarget;
    public bool isUpdate = false;
    public float Speed;
  
        private void Start()
    {
                    
    }
  public float GetSpeed()
    {
        if(Time!=0)
        return Mathf.Max(Speed,Vector3.Distance(Pos, posTarget))/Time;
        return 0;
    }
    

}
