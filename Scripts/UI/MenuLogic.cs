using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Events;
using UnityEngine;

namespace RIT.RochesterLOS.UI
{
    public abstract class MenuLogic : MonoBehaviour
    {

        [SerializeField] private GameObject menuRoot;

        public virtual GameObject GetMenuRoot()
        {
            return menuRoot;
        }

        public void SetRootActive()
        {
            GetMenuRoot().SetActive(true);
        }

        public abstract void OnButtonClick(int action);

        [System.Serializable]
        protected enum MenuButtonAction
        {
            SwitchToLOSView = 0,
            OpenMapConfig = 1,
            LoadSigns = 2,
            SaveSigns = 3,
            SwitchToPlacementView = 4,
        }
    }
}
