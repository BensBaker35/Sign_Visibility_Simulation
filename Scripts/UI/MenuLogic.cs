using System.Collections;
using System.Collections.Generic;
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
    }
}
