using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupController : MonoBehaviour
{
    private static StartupController instance;
    // Start is called before the first frame update
    void Start()
    {
        if(instance != null)
            return;
        instance = this;

        Cursor.lockState = CursorLockMode.Locked;

        /*TESTING*/
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
