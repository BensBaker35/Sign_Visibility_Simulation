using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RIT.RochesterLOS.Events;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace RIT.RochesterLOS.Scenes
{
    public class SceneSwitcher : MonoBehaviour
    {
        private string activeScene = "Start";
        void Awake()
        {
            EventManager.Listen(Events.Events.ChangeScene, ChangeScene);
            //DontDestroyOnLoad(this.gameObject);
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

                if (str.Equals(activeScene)) 
                {
                    return;
                }
                
                var sceneTask = SceneManager.LoadSceneAsync(str, LoadSceneMode.Additive);
                sceneTask.completed += (op) => {
                    SceneManager.UnloadSceneAsync(activeScene);
                    activeScene = str;
                };
            }
            else 
            {
                Debug.LogError("Change Scene Event package is an unsupported type");
            }
        }

        private GameObject GetUIInjection()
        {
            return SceneManager.GetActiveScene().GetRootGameObjects().First();
        }
    }
}                                                                                                                                                                                                                                                                           