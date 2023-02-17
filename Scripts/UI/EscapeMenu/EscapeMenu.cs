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
            this.GetMenuRoot().SetActive(menuActive);
        }

        public void OnEscapeButtonClick()
        {
            EventManager.TriggerEvent(Events.Events.EscapeMenuToggle, null);
        }
        public override void OnButtonClick(int action)
        {
            var desiredAction = (MenuButtonAction)action;
            switch (desiredAction)
            {
                case MenuButtonAction.SaveSigns:
                    goto default;
                    //EventManager.TriggerEvent(Events.Events.Save, null);
                    break;
                case MenuButtonAction.LoadSigns:
                    goto default;
                    //EventManager.TriggerEvent(Events.Events.Load, null);
                    break;
                case MenuButtonAction.SwitchToLOSView:
                    EventManager.TriggerEvent(Events.Events.ChangeScene, "LOS_Explorable");
                    break;
                case MenuButtonAction.OpenMapConfig:
                default:
                    Debug.LogWarning($"Escape Menu action not supported {action}");
                    break;
            }
        }

    }

}