using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using RIT.RochesterLOS.UI;
using System;
using RIT.RochesterLOS.Services;
using RIT.RochesterLOS.Serialization;

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

        void Awake()
        {
            //Should also do work to set up other components
            ServiceLocator.RegisterService<IByteSerialization>(new SerializationService());
            ServiceLocator.RegisterService<ITextSerialization>(new FileSerializationService());
            ServiceLocator.RegisterService<IUnityObjectSerializer>(new UnitySerializationService());
            ServiceLocator.RegisterService<IConfigurationService>(new Configuraion.ConfigurationService());
        }
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
        }

        void SetupUI()
        {
            Debug.Log("Setting up UI");
            ((IUIService)ServiceLocator.GetService<IUIService>()).Init();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
