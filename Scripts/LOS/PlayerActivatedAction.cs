using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using UnityEngine;

namespace RIT.RochesterLOS.LOS
{
    [RequireComponent(typeof(ArcGISLocationComponent))]
    public abstract class PlayerActivatedAction : MonoBehaviour
    {
     
        public delegate void Activate(Collider collider);
        public Activate PlayerInActivation;
        //public abstract void React(Collider collider);
        //public abstract Collider Notify();
    
    }
}
