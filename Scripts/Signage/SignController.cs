using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RIT.RochesterLOS.LOS;
using Esri.ArcGISMapsSDK.Components;
using RIT.RochesterLOS.Events;

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
            Debug.Log("Sign Awake");
            EventManager.Listen(Events.Events.WorldReady, SetOnGround);
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Sign Start");
            EventManager.Listen(Events.Events.WorldReady, SetOnGround);
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

        public void SetOnGround(object _)
        {
            Debug.Log("Set On Ground");
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, maxDistance))
            {
                Debug.Log(hit.collider.gameObject.name);
                var groundPosition = Analysis.AnalysisUtil.SimPositionToGeo(hit.point);
                locationComponent.Position = groundPosition;
            }
        }

        [ExecuteAlways]
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * maxDistance));
        }
    }
}
