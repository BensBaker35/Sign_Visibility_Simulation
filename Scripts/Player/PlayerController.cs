using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using RIT.RochesterLOS.Events;
using Unity.Mathematics;
using UnityEngine;

namespace RIT.RochesterLOS.Player
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly ArcGISPoint PLAYER_SPAWN = new ArcGISPoint(-77.60638, 43.15719, 175, ArcGISSpatialReference.WGS84());
        [SerializeField] private float lookSpeed = 150f;
        [SerializeField] private float moveSpeed = 100f;
        [SerializeField] private float groundingDistance = 0.4f; //Distance from ground
        [SerializeField] private float initialLoadDistanceCheck = 3f;
        [SerializeField] private float mapLoadWaitTime = 15f;
        //[SerializeField] private GameObject cameraObject;
    
        private Camera childCamera; //Main Camera
        private ArcGISMapComponent arcGISMapComponent;
        private CharacterController characterController;
        private float xRotation = 0f;
        private float maxGroundCheckDistance = 20f;
        private bool mapReady = false;
        private bool isGrounded = false;
        private Vector3 velocity;
        private float gravity = -9.81f;
        
        // Start is called before the first frame update
        void Start()
        {
            arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();
            childCamera = GetComponentInChildren<Camera>();
            
            characterController = GetComponent<CharacterController>();
            

            Cursor.lockState = CursorLockMode.Locked;

            StartCoroutine(WaitforMapLoad());
        }

        // Update is called once per frame
        void Update()
        {
            
            if(!mapReady) return;

            float mousX = Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
            float mousY = Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;

            xRotation -= mousY;
            xRotation = Mathf.Clamp(xRotation, -75f, 75f);

            childCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mousX);

            isGrounded = Physics.CheckSphere(transform.position, groundingDistance);

            if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");

            var moveVec = (transform.right * x) + (transform.forward * z);
            

            characterController.Move(moveVec * moveSpeed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            characterController.Move(velocity * Time.deltaTime);

            
        }

//TODO Refactor out
        private IEnumerator WaitforMapLoad()
        {
            Debug.Log("Waiting For Proper Collider Initialization");
            yield return new WaitUntil(() => 
            {
                RaycastHit hit;
                var hitCollider = Physics.Raycast(transform.position, Vector3.down, out hit, initialLoadDistanceCheck);

                return hitCollider;
            });
            
            mapReady = true;
            EventManager.TriggerEvent(Events.Events.WorldReady, arcGISMapComponent.View);
            Debug.Log("Colliders Found!");
        }


        [ExecuteAlways]
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * initialLoadDistanceCheck));
        }

    }
}

