using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Events;
using RIT.RochesterLOS.Serialization;
using RIT.RochesterLOS.Services;
using UnityEngine;

namespace RIT.RochesterLOS.UI
{
    public class GeneralUIManager : MonoBehaviour, IUIService
    {
        //TODO Get from some scene config descriptor
        //[SerializeField] private GameObject EscapeMenuUIPrefab;
        [SerializeField] private List<GameObject> menusInScene;
        private string SceneAssetsLoc = "SceneAssets/";
        private IUnityObjectSerializer serializer;

        void Awake()
        {

            ServiceLocator.RegisterService<IUIService>(this);
            EventManager.Listen(Events.Events.SceneActive, SceneChanged);
            EventManager.Listen(Events.Events.LoadScene, (_) => SceneChanged("Loading"));

        }

        public void Init()
        {
            Debug.Log("Init UI");
            serializer = (IUnityObjectSerializer)ServiceLocator.GetService<IUnityObjectSerializer>();

            SceneChanged("Start");

        }


        private void SceneChanged(object package)
        {
            if (package is string)
            {
                var str = (string)package;
#if SIGN_SIM_DEMO
                str += "_Demo";
#endif
                var uiInjection = serializer.GetUnityObject<UIInjector>(SceneAssetsLoc + str);
                if (uiInjection == null)
                {
#if SIGN_SIM_DEMO
                    var old_str = str.Replace("_Demo", "");
                    uiInjection = serializer.GetUnityObject<UIInjector>(SceneAssetsLoc + old_str);
#else
                    Debug.LogError($"Failed to find UI object for {str}");
                    return;
#endif

                }

                if (menusInScene != null && menusInScene.Count > 0)
                {
                    RemoveOldMenus();
                }
                SetUpSceneUI(uiInjection.MenuRoots);
                menusInScene[0].GetComponent<MenuLogic>()?.SetRootActive();
            }
        }

        private void SetUpSceneUI(List<GameObject> menus)
        {
            if (menusInScene == null)
            {
                menusInScene = new();
            }
            else
            {
                menusInScene.Clear();
            }

            foreach (var menu in menus)
            {
                GameObject menuItem = Instantiate(menu, this.transform);
                //menuItem.SetActive(false);
                menusInScene.Add(menuItem);
            }

            //menusInScene[0].SetActive(true);
        }

        private void RemoveOldMenus()
        {
            //Possibly make a call to unload the asset
            foreach (var menu in menusInScene)
            {
                Destroy(menu);
            }


        }

        public void TriggerEscapeMenu()
        {
            EventManager.TriggerEvent(Events.Events.EscapeMenuToggle, null);
        }

    }

}
