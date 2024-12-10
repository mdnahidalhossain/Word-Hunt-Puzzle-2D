using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;

public class ProgressBar : MonoBehaviour
{
    public GameObject progressBar;
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        PlayProgressBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void PlayProgressBar()
    {
        //LeanTween.scaleX(progressBar, 1.0f, time);
    }
}
