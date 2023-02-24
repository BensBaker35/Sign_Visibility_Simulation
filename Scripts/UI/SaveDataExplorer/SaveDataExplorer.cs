using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RIT.RochesterLOS.Services;
using static TMPro.TMP_Dropdown;


namespace RIT.RochesterLOS.UI.SaveDataExplorer
{
    public class SaveDataExplorer : MenuLogic
    {
        [SerializeField] private TMPro.TMP_Dropdown fileSelect;
        private IConfigurationService config;
        private ITextSerialization serializer;
        private string selectedValue;
        
        public delegate void SelectionMade();
        public event SelectionMade selectionMade;

        // Start is called before the first frame update
        void Start()
        {
            config = (IConfigurationService)ServiceLocator.GetService<IConfigurationService>();
            serializer = (ITextSerialization)ServiceLocator.GetService<ITextSerialization>();

            var currentSaveDirectory = config.GetConfigValue("data_directory");
            var options = new List<OptionData>();
            foreach(var optionStr in serializer.GetDirectoryListings((string)currentSaveDirectory))
            {
                options.Add(new(optionStr));
            }

            fileSelect.AddOptions(options);
            fileSelect.onValueChanged.AddListener(OnFileSelectionChanged);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public override void OnButtonClick(int action)
        {
            var desiredAction = (MenuButtonAction)action;
            switch(desiredAction)
            {
                case MenuButtonAction.Select:
                //Need to notify that a new file has been selected
                    if(!string.IsNullOrEmpty(selectedValue))
                    {
                        config.UpdateValue("active_sign_data", selectedValue);
                    }
                    
                break;
                case MenuButtonAction.New:
                //Create a new file with a name
                //Repopulate drop down
                break;
                default:
                Debug.LogWarning($"Save Data Menu action not supported {action}");
                break;
            }
        }

        private void OnFileSelectionChanged(int selection)
        {
            selectedValue = fileSelect.options[selection].text;
        }
    }
}
