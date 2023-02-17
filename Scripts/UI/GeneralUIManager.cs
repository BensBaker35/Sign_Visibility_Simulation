using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Events;
using RIT.RochesterLOS.Serialization;
using UnityEngine;

namespace RIT.RochesterLOS.UI
{
    public class GeneralUIManager : MonoBehaviour
    {
        //TODO Get from some scene config descriptor
        //[SerializeField] private GameObject EscapeMenuUIPrefab;
        [SerializeField] private List<GameObject> menusInScene;

        void Awake()
        {
            Serializer.Instance.RegisterUnitySerializationTarget<UIInjector>("SceneAssets");
            EventManager.Listen(Events.Events.ChangeScene, SceneChanged);
        }      

        public void Init()
        {
            Debug.Log("Init UI");
            SceneChanged("Start");
        }  


        private void SceneChanged(object package)
        {
            if(package is string)
            {
                var str = (string)package;
                var uiInjection = Serializer.Instance.GetUnityObject<UIInjector>(str);
                if(uiInjection == null)
                {
                    Debug.LogError($"Failed to find UI object for {str}");
                }

                if(menusInScene != null && menusInScene.Count > 0)
                {
                    RemoveOldMenus();
                }
                SetUpSceneUI(uiInjection.MenuRoots);
                menusInScene[0].GetComponent<MenuLogic>()?.SetRootActive();
            }
        }

        private void SetUpSceneUI(List<GameObject> menus)
        {
            if(menusInScene == null)
            {
                menusInScene = new();
            }
            
            foreach(var menu in menus)
            {
                GameObject menuItem = Instantiate(menu, this.transform);
                menuItem.SetActive(false);
                menusInScene.Add(menuItem);
            }

            //menusInScene[0].SetActive(true);
        }

        private void RemoveOldMenus()
        {
            //Possibly make a call to unload the asset
            foreach(var menu in menusInScene)
            {
                Destroy(menu);
            }
            

        }
        
    }

}
