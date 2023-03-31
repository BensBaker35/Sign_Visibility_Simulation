using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Events;
using UnityEngine;
using TMPro;

namespace RIT.RochesterLOS.UI.StartMenu
{
    public class StartMenu : MenuLogic
    {
        [SerializeField] private TMP_Dropdown configSelect;
        
        
        void Start()
        {
            this.isMain = true;
            
            if (configSelect != null)
            {
                configSelect.onValueChanged.AddListener(OnConfigSelectChange);
                
                configSelect.AddOptions(new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData("Default") });
                configSelect.SetValueWithoutNotify(0);
            }
            else
            {
                Debug.LogWarning("Config Select is not set");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnButtonClick(int action)
        {
            var desiredAction = (MenuButtonAction)action;
            switch (desiredAction)
            {
                case MenuButtonAction.SwitchToLOSView:
                    EventManager.TriggerEvent(RIT.RochesterLOS.Events.Events.ChangeScene, "LOS_Explorable");
                    break;
                case MenuButtonAction.OpenMapConfig:
                    goto default;
                case MenuButtonAction.SwitchToPlacementView:
                    EventManager.TriggerEvent(Events.Events.ChangeScene, "SignPlacement");
                    break;
                case MenuButtonAction.Quit:
                    Application.Quit(0);
                    break;
                default:
                    Debug.LogWarning($"Menu action not supported by Start Menu: {action}");
                    break;
            }
        }

        public void OnConfigSelectChange(int selection)
        {
            Debug.LogWarning("Loading and Saving of Different area configs is not supported yet");
        }
    }

}
