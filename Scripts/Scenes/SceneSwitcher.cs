using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Events;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace RIT.RochesterLOS.Scenes
{
    public class SceneSwitcher : MonoBehaviour
    {
        void Awake()
        {
            EventManager.Listen(Events.Events.ChangeScene, ChangeScene);
            DontDestroyOnLoad(this.gameObject);
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void ChangeScene(object package)
        {
            if(package is string)
            {
                var str = (string)package;
                var sceneLoadOp = SceneManager.LoadSceneAsync(str, LoadSceneMode.Single);
                
            }
            else 
            {
                Debug.LogError("Change Scene Event package is an unsupported type");
            }
        }
    }
}