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

        private DataAction currentAction = DataAction.None;

        void Start()
        {
            EventManager.Listen(Events.Events.EscapeMenuToggle, EscapeMenuToggle);
            saveDataExplorer = fileExplorerPanel.GetComponent<SaveDataExplorer.SaveDataExplorer>();
            saveDataExplorer.selectionMade += HandleSelection;
            fileExplorerPanel?.SetActive(false);
            MainPanel?.SetActive(false);
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
                    currentAction = DataAction.Save;
                    ActivePanel = fileExplorerPanel;
                    break;
                case MenuButtonAction.LoadSigns:
                    currentAction = DataAction.Load;
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

        private void HandleSelection()
        {
            switch (currentAction)
            {
                case DataAction.Save:
                    EventManager.TriggerEvent(Events.Events.SaveSignData, null);
                    break;
                case DataAction.Load:
                    EventManager.TriggerEvent(Events.Events.LoadSignData, null);
                    break;
                default:
                    break;
            }
            ActivePanel = MainPanel;
            currentAction = DataAction.None;
        }

        private enum DataAction
        {
            None,
            Save,
            Load,
        }

    }

}