using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] int sceneToLoad = 1;

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                print("Portal triggered");
                SceneManager.LoadScene(sceneToLoad);
            }
            
        }
    }
}

