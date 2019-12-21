using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum AiStatus {Attack,Defends,MoveAway}
public class GameCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmos()
    {
        
    }
}
