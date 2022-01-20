using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject Cam1;
    public GameObject Cam2;

    public GameObject Trigger;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Cam1.SetActive(false);
            Cam2.SetActive(true);
        }

    }
}
