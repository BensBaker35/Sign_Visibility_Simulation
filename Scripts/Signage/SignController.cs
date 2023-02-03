using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RIT.RochesterLOS.LOS;
using Esri.ArcGISMapsSDK.Components;

namespace RIT.RochesterLOS.Signage
{
    [RequireComponent(typeof(ArcGISLocationComponent))]
    [RequireComponent(typeof(SphereCollider))]
    public class SignController : MonoBehaviour
    {
        private SphereCollider triggerCollider;
        private ArcGISLocationComponent locationComponent;
        private PlayerActivatedAction[] actions;
        private float maxDistance = 15f; //TODO Provide some real world value
        void Awkae()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            triggerCollider = GetComponent<SphereCollider>();
            locationComponent = GetComponent<ArcGISLocationComponent>();
            actions = GetComponents<PlayerActivatedAction>();
            
            triggerCollider.radius = maxDistance;
        }   

        // Update is called once per frame
        void Update()
        {

        }


        void OnTriggerStay(Collider other)
        {
            foreach(var a in actions)
            {
                a.PlayerInActivation(other);
            }
        }
    }
}
