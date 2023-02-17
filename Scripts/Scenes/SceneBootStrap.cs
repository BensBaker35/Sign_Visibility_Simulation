using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using RIT.RochesterLOS.UI;
using System;

namespace RIT.RochesterLOS.Scenes
{
    /*
    Only used in the start screen to initialize 
    */
    [DisallowMultipleComponent]
    public class SceneBootStrap : MonoBehaviour
    {
        [SerializeField] private string parentUIScene = "GeneralUI";
        // Start is called before the first frame update
        void Start()
        {
            try
            {
                SetupUI();
            }
            catch (Exception)
            {
                Debug.Log("UI not loaded, loading in");
                var sceneTask = SceneManager.LoadSceneAsync(parentUIScene, LoadSceneMode.Additive);
                sceneTask.completed += (_) => SetupUI();
            }
            // finally
            // {
            //     SetupUI();
            // }
            // if (SceneManager.GetSceneByName(parentUIScene) == null) 
            // {
            //     SceneManager.LoadScene(parentUIScene, LoadSceneMode.Additive);
            // } 





        }

        void SetupUI()
        {
            Debug.Log("Setting up UI");
            var s = SceneManager.GetSceneByName(parentUIScene);
            var ui = s.GetRootGameObjects();
            foreach(var o in ui)
            {
//                Debug.Log(o.name);
                var genUI = o.GetComponent<GeneralUIManager>();
                if( genUI != null)
                {
                    genUI.Init();
                }
            }
            //.FirstOrDefault(o => o.GetComponent<GeneralUIManager>() != null);
            if(ui == null)
            {
                Debug.LogWarning("Can't find General UI Manager");
            }
            //ui?.GetComponent<GeneralUIManager>().Init();
        }

        // Update is called once per frame
        void Update()
        {

        }


        private void PopulateSceneSerializers()
        {

        }
    }
}
