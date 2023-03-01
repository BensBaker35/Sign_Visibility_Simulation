using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.LOS
{
    //TOODO Look at renaming to somethign better
    [RequireComponent(typeof(LineRenderer))]
    public class LineOfSightCheck : PlayerActivatedAction
    {

        [SerializeField] private Transform rayCaster;
        [SerializeField] private int raycastLayer = 3;
        private LineRenderer losRenderer;

        // Start is called before the first frame update
        void Start()
        {
            losRenderer = GetComponent<LineRenderer>();
            losRenderer.positionCount = 2; //Mus define position count before assignment
            PlayerInActivation = TakeAction;
        }

        // Update is called once per frame
        void Update()
        {
            // if(playerTarget == null) return;
            // //TODO Modify to preform LOS check only when player is close
            // var direction = playerTarget.position - transform.position;

            // RaycastHit hit;
            // if (Physics.Raycast(transform.position, direction, out hit))
            // {
            //     var hitPlayer = hit.transform.Equals(playerTarget);
            //     //Debug.Log(hitPlayer ? "Has LOS" : "Lost LOS");
            //     losRenderer.endColor = losRenderer.startColor = hitPlayer ? Color.green : Color.red;
                
            //     losRenderer.material.SetFloat("_Has_LOS", hitPlayer ? 1f : 0f);//_Has_LOS is defined by shader
            //     losRenderer.SetPositions(new []{this.transform.position, playerTarget.transform.position});
                
            // }
        }

        void TakeAction(Collider player)
        {
            if(rayCaster == null){
                Debug.LogWarning("No Raycaster Defined, using default");
                rayCaster = transform;
            }
            var direction = player.transform.position - rayCaster.position;

            RaycastHit hit;
            if (Physics.Raycast(rayCaster.position, direction, out hit))
            {
                var hitPlayer = hit.transform.Equals(player.transform);
                //Debug.Log(hitPlayer ? "Has LOS" : "Lost LOS");
                losRenderer.endColor = losRenderer.startColor = hitPlayer ? Color.green : Color.red;
                
                losRenderer.material.SetFloat("_Has_LOS", hitPlayer ? 1f : 0f);//_Has_LOS is defined by shader
                losRenderer.SetPositions(new []{rayCaster.position, player.transform.position});
                
            }
            else 
            {
                
                losRenderer.SetPositions(new Vector3[0]);
            }
        }
    }
}
