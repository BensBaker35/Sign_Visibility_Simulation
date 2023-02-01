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
        [SerializeField] private float lookSpeed = 150f;
        [SerializeField] private float moveSpeed = 100f;
        [SerializeField] private float groundingDistance = 0.4f; //Distance from ground
        [SerializeField] private float initialLoadDistanceCheck = 3f;
        [SerializeField] private float mapLoadWaitTime = 15f;
    
        private Camera childCamera; //Main Camera
        private HPTransform hpTransform;
        private ArcGISLocationComponent loc;
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
            
            hpTransform = GetComponent<HPTransform>();
            loc = GetComponent<ArcGISLocationComponent>();
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
            Debug.Log("Colliders Found!");
        }

        private ArcGISRotation CalcualteBearing(ArcGISPoint p1, ArcGISPoint p2)
        {
            
            double longDelta = p2.X - p1.X;
            var X = math.cos(p2.Y) * math.sin(longDelta);
            var latP1P2 = math.cos(p1.Y) * math.sin(p2.Y);
            var Y =  latP1P2 - latP1P2 *  math.cos(longDelta);
            Debug.Log($"Radians: {X}, {Y}");
            //var b = math.atan2(X, Y);
            X = math.degrees(X);
            Y = math.degrees(Y);

            Debug.Log($"Degrees: {X}, {Y}");

            return new ArcGISRotation(Y, 90, 0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * initialLoadDistanceCheck));
        }

    }
}

