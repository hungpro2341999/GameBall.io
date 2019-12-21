using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleInforBall : MonoBehaviour
{
   public GameObject InforBall;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i <GamePlayerCtrl.Instance.CountPlayer(); i++)
        {
          var a =   Instantiate(InforBall,transform);
            a.GetComponent<GetInfor>().player = GamePlayerCtrl.Instance.player[i];
        }
        Debug.Log(GamePlayerCtrl.Instance.CountPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
