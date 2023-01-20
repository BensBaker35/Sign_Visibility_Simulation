using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.LOS
{
    public class LineOfSightCheck : MonoBehaviour
    {

        [SerializeField] private Transform playerTarget;
        private LineRenderer losRenderer;

        // Start is called before the first frame update
        void Start()
        {
            losRenderer = GetComponent<LineRenderer>();
            losRenderer.positionCount = 2; //Mus define position count before assignment

        }

        // Update is called once per frame
        void Update()
        {
            if(playerTarget == null) return;
            //TODO Modify to preform LOS check only when player is close
            var direction = playerTarget.position - transform.position;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                var hitPlayer = hit.transform.Equals(playerTarget);
                Debug.Log(hitPlayer ? "Has LOS" : "Lost LOS");
                losRenderer.endColor = losRenderer.startColor = hitPlayer ? Color.green : Color.red;
                
                losRenderer.material.SetFloat("_Has_LOS", hitPlayer ? 1f : 0f);//_Has_LOS is defined by shader
                losRenderer.SetPositions(new []{this.transform.position, playerTarget.transform.position});
                
            }
        }
    }
}
