using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace RIT.RochesterLOS.UI.EscapeMenu
{
    public class EscapeMenu : MenuLogic
    {
        [SerializeField] private GameObject fileExplorerPanel;
        [SerializeField] private GameObject MapConfigPanel;
        private bool menuActive = false;

        private GameObject _activePanel;
        private GameObject ActivePanel
        {
            get { return _activePanel; }
            set
            {
                _activePanel?.SetActive(false);
                _activePanel = value;
                _activePanel.SetActive(true);
            }
        }

        void Start()
        {
            EventManager.Listen(Events.Events.EscapeMenuToggle, EscapeMenuToggle);
            //DontDestroyOnLoad(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                EventManager.TriggerEvent(Events.Events.EscapeMenuToggle, null);
            }
        }

        private void EscapeMenuToggle(object _)
        {
            // escapeMenuActive = !escapeMenuActive;
            // if (escapeMenuActive)
            // {
            //     activeMenu = Instantiate(EscapeMenuUIPrefab, transform);
            // }
            // else
            // {
            //     Destroy(activeMenu);
            // }
            menuActive = !menuActive;
            this.gameObject.SetActive(menuActive);
        }

        public void OnEscapeButtonClick()
        {
            EventManager.TriggerEvent(Events.Events.EscapeMenuToggle, null);
        }
        public void OnButtonClick(int action)
        {
            var desiredAction = (EscapeMenuButtonAction)action;
            switch (desiredAction)
            {
                case EscapeMenuButtonAction.SaveSigns:
                    //EventManager.TriggerEvent(Events.Events.Save, null);
                    break;
                case EscapeMenuButtonAction.LoadSigns:
                    //EventManager.TriggerEvent(Events.Events.Load, null);
                    break;
                case EscapeMenuButtonAction.SwitchToLOSView:
                    EventManager.TriggerEvent(Events.Events.ChangeScene, "LOS_Explorable");
                    break;
                case EscapeMenuButtonAction.OpenMapConfig:
                default:
                    Debug.LogWarning($"Escape Menu action not supported {action}");
                    break;
            }
        }

        [System.Serializable]
        public enum EscapeMenuButtonAction
        {
            SwitchToLOSView = 0,
            OpenMapConfig = 1,
            LoadSigns = 2,
            SaveSigns = 3,
        }

    }

}