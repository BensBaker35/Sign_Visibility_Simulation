using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using RIT.RochesterLOS.Events;
using UnityEngine;

namespace RIT.RochesterLOS.Control
{
    public sealed class ControllableService : MonoBehaviour
    {
        [SerializeField] private IControllable controlling;
        [SerializeField] private float lookSpeed = 150f;
        [SerializeField] private float moveSpeed = 25f;
        [SerializeField] private float initialLoadDistanceCheck = 3f;
        [SerializeField] private CursorLockMode lockMode = CursorLockMode.Confined;

        private bool mapReady = false;
        private ArcGISMapComponent arcGISMapComponent;

        void Awake()
        {
            EventManager.Listen(Events.Events.WorldReady, (p) => mapReady = true);
        }
    // Start is called before the first frame update
        void Start()
        {
            if(controlling == null)
            {
                controlling = GetComponent<IControllable>();
                if(controlling == null) {
                    Debug.LogWarning("CONTROLLABLE SERVICE: Can't Find Controllable object");
                }
            }
            arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();
            
            Cursor.lockState = lockMode;
            StartCoroutine(WaitforMapLoad());
        }

        // Update is called once per frame
        void Update()
        {
            if(!mapReady) return;
            
            var mouseX = Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
            var mouseScroll = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
            
            var mouseVec = new Vector3(mouseX, mouseY, 0);
            //var scrollVec = Input.mouseScrollDelta * Time.deltaTime;
            controlling.PushMouseInput(mouseVec, new Vector2(0, mouseScroll));

            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");

            var moveVec = (transform.right * x) + (transform.forward * z);

            moveVec *= moveSpeed * Time.deltaTime;

            controlling.PushKeyboardInput(moveVec);
            
  
        }

        private IEnumerator WaitforMapLoad()
        {
            Debug.Log("CONTROLLABLE SERVICE: Waiting For Proper Collider Initialization");
            yield return new WaitUntil(CheckGround);

            Debug.Log($"CONTROLLABLE SERVICE: Colliders Found!");
            EventManager.TriggerEvent(Events.Events.WorldReady, arcGISMapComponent);
            
        }

        private bool CheckGround()
        {
            RaycastHit hit;
            var hitCollider = Physics.Raycast(transform.position, Vector3.down, out hit, initialLoadDistanceCheck);
            if (hitCollider)
            {
                Debug.Log($"CONTROLLABLES SERVICE: hit collider - {hit.collider.name}", hit.collider.gameObject);
            }

            return hitCollider;
        }

        [ExecuteAlways]
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * initialLoadDistanceCheck));
        }

    }
}
