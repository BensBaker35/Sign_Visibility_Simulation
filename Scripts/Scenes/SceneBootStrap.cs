using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using RIT.RochesterLOS.UI;

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
            if (SceneManager.GetSceneByName(parentUIScene) == null) 
            {
                SceneManager.LoadScene(parentUIScene, LoadSceneMode.Additive);
            } 

            
            var s = SceneManager.GetSceneByName(parentUIScene);
            var ui = s.GetRootGameObjects().First(o => o.GetComponent<GeneralUIManager>() != null);
            ui?.GetComponent<GeneralUIManager>().Init();
            

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
