using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using RIT.RochesterLOS.UI;

namespace RIT.RochesterLOS.Scenes
{
    [DisallowMultipleComponent]
    public class SceneBootStrap : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuPrefab;
        [SerializeField] private string parentUIScene = "GeneralUI";
        // Start is called before the first frame update
        void Start()
        {
            if(SceneManager.GetSceneByName(parentUIScene) != null) return;
            
            SceneManager.LoadScene(parentUIScene, LoadSceneMode.Additive);
            var s = SceneManager.GetSceneByName(parentUIScene);
            var ui = s.GetRootGameObjects().First(o => o.GetType() == typeof(GeneralUIManager));

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
