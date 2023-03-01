using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RIT.RochesterLOS.Signage;
using RIT.RochesterLOS.Services;
using RIT.RochesterLOS.Events;
using Unity.Mathematics;
using UnityEngine.UI;

namespace RIT.RochesterLOS.UI.DataEditor
{
    public class DataEditor : MenuLogic
    {
        private int currentlyEditing;
        private ISignService signService;

        [SerializeField] private TMPro.TMP_InputField altInput;
        [SerializeField] private TMPro.TMP_InputField lonInput;
        [SerializeField] private TMPro.TMP_InputField latInput;
        [SerializeField] private TMPro.TMP_Dropdown objectSelect;
        [SerializeField] private Button acceptButton;
        bool isDirty = false;
        

        void Awake()
        {
            objectSelect.onValueChanged.AddListener(UpdateTypeSelction);
            altInput.onValueChanged.AddListener(MarkDirty);
            lonInput.onValueChanged.AddListener(MarkDirty);
            latInput.onValueChanged.AddListener(MarkDirty);
            
        }

        // Start is called before the first frame update
        void Start()
        {
            signService = (ISignService)ServiceLocator.GetService<ISignService>();
            signService.OnNewSignSelection += OnNewSignSelection;

            List<TMPro.TMP_Dropdown.OptionData> options = new();
            foreach(var t in System.Enum.GetValues(typeof(SignType)))
            {
                options.Add(new TMPro.TMP_Dropdown.OptionData(t.ToString()));
            }
            objectSelect.ClearOptions();
            objectSelect.AddOptions(options);
        }

        // Update is called once per frame
        void Update()
        {
            acceptButton.interactable = isDirty;
        }

        public override void OnButtonClick(int action)
        {
            var dataAction = (MenuButtonAction)action;
            switch(dataAction)
            {
                case MenuButtonAction.Select:
                    UpdateSignData();
                break;
                default:
                Debug.LogWarning($"{dataAction} is not supported by DataEditor");
                break;
            }
        }


        private void OnNewSignSelection(int id)
        {
            currentlyEditing = id;
            var pos = signService.GetPositionData(id);
            
            altInput.text = pos.z.ToString();
            lonInput.text = pos.x.ToString();
            latInput.text = pos.y.ToString();

            isDirty = false;
        }

        private void UpdateSignData()
        {
            var lon = double.Parse(lonInput.text);
            var lat = double.Parse(latInput.text);
            var elv = double.Parse(altInput.text);

            signService.UpdatePosition(currentlyEditing, new double3(lon, lat, elv));
            isDirty = false;
        }

        private void UpdateTypeSelction(int option)
        {
            var selected = objectSelect.options[option];

            var signType = System.Enum.Parse<SignType>(selected.text);
            EventManager.TriggerEvent(Events.Events.SelectSignObject, signType);

            isDirty = true;
        }

        private void MarkDirty(string _)
        {
            isDirty = true;
        }
    }
}