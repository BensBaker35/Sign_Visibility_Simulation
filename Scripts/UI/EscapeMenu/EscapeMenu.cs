using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace RIT.RochesterLOS.UI.EscapeMenu
{
    public class EscapeMenu : MonoBehaviour
    {
        [SerializeField] private GameObject mainButtonPanel;
        [SerializeField] private GameObject fileExplorerPanel;
        [SerializeField] private GameObject MapConfigPanel;

        private GameObject _activePanel;
        private GameObject ActivePanel
        {
            get{ return _activePanel;}
            set
            {
                _activePanel?.SetActive(false);
                _activePanel = value;
                _activePanel.SetActive(true);
            }
        }


        void Awake()
        {

            
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnButtonClick(int action)
        {
            var desiredAction = (EscapeMenuButtonAction)action;
            switch(desiredAction)
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