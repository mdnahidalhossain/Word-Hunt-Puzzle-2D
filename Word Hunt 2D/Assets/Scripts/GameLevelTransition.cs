using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevelTransition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovetoLevel2()
    {
        SceneManager.LoadScene(2);
    }

    public void MovetoLevel3()
    {
        SceneManager.LoadScene(3);
    }

    public void MovetoLevel4()
    {
        SceneManager.LoadScene(4);
    }

    public void MovetoLevel5()
    {
        SceneManager.LoadScene(5);
    }

    public void MovetoLevel6()
    {
        SceneManager.LoadScene(6);
    }

    public void MovetoLevel7()
    {
        SceneManager.LoadScene(7);
    }

    public void MovetoLevel8()
    {
        SceneManager.LoadScene(8);
    }

    public void MovetoLevel9()
    {
        SceneManager.LoadScene(9);
    }
}
