using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Events;
using UnityEngine;

namespace RIT.RochesterLOS.UI
{
    public class GeneralUIManager : MonoBehaviour
    {
        //TODO Get from some scene config descriptor
        [SerializeField] private GameObject EscapeMenuUIPrefab;
        private bool escapeMenuActive = false;
        private GameObject activeMenu = null;

        // Start is called before the first frame update
        void Start()
        {
            EventManager.Listen(Events.Events.EscapeMenuToggle, EscapeMenuToggle);
            DontDestroyOnLoad(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void EscapeMenuToggle(object _)
        {
            escapeMenuActive = !escapeMenuActive;
            if (escapeMenuActive)
            {
                activeMenu = Instantiate(EscapeMenuUIPrefab, transform);
            }
            else
            {
                Destroy(activeMenu);
            }
        }

        public void OnEscapeButtonClick()
        {
            EventManager.TriggerEvent(Events.Events.EscapeMenuToggle, null);
        }
    }
}
