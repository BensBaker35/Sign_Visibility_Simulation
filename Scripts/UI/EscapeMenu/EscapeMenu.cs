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
        //[SerializeField] private GameObject MapConfigPanel;
        [SerializeField] private GameObject MainPanel;
        private bool menuActive = false;
        private SaveDataExplorer.SaveDataExplorer saveDataExplorer;

        private GameObject _activePanel;
        private GameObject ActivePanel
        {
            get { return _activePanel; }
            set
            {
                if (_activePanel != null)
                    _activePanel.SetActive(false);

                _activePanel = value;

                if (_activePanel != null)
                    _activePanel.SetActive(true);
            }
        }

        void Start()
        {
            EventManager.Listen(Events.Events.EscapeMenuToggle, EscapeMenuToggle);
            saveDataExplorer = fileExplorerPanel.GetComponent<SaveDataExplorer.SaveDataExplorer>();
            saveDataExplorer.selectionMade += () => ActivePanel = MainPanel;
            fileExplorerPanel?.SetActive(false);
            MainPanel?.SetActive(false);
            //MapConfigPanel?.SetActive(false);
            ActivePanel = null;
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
            menuActive = !menuActive;
            //this.GetMenuRoot().SetActive(menuActive);
            ActivePanel = menuActive ? MainPanel : null;
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
                    ActivePanel = fileExplorerPanel;
                    break;
                case MenuButtonAction.LoadSigns:
                    ActivePanel = fileExplorerPanel;
                    break;
                case MenuButtonAction.SwitchToLOSView:
                    EventManager.TriggerEvent(Events.Events.ChangeScene, "LOS_Explorable");
                    break;
                case MenuButtonAction.SwitchToEditView:
                    EventManager.TriggerEvent(Events.Events.ChangeScene, "SignPlacement");
                    break;
                case MenuButtonAction.Quit:
                    Application.Quit(0);
                    break;
                case MenuButtonAction.ExitToMenu:
                    EventManager.TriggerEvent(Events.Events.ChangeScene, "Start");
                    break;
                case MenuButtonAction.OpenMapConfig:
                default:
                    Debug.LogWarning($"Escape Menu action not supported {action}");
                    break;
            }
        }

    }

}