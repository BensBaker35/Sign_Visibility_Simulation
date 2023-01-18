using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using Unity.Mathematics;
using UnityEngine;

namespace RIT.RochesterLOS.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float lookSpeed = 150f;
        [SerializeField] private float moveSpeed = 100f;
        [SerializeField] private float maxGroundDistance = 5f; //Distance from ground
        [SerializeField] private float minGroundDistance = 3f;
        [SerializeField] private float mapLoadWaitTime = 15f;
    
        private Camera childCamera; //Main Camera
        private HPTransform hpTransform;
        private ArcGISLocationComponent loc;
        private ArcGISMapComponent arcGISMapComponent;
        private CharacterController characterController;
        private float xRotation = 0f;
        private float maxGroundCheckDistance = 20f;
        private bool mapReady = false;
        
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



            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");

            var moveVec = (transform.right * x) + (transform.forward * z);
            

            characterController.Move(moveVec * moveSpeed * Time.deltaTime);

            
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, maxGroundCheckDistance))
            {
                if(hit.distance > maxGroundDistance)
                {
                    
                    
                    transform.position = new Vector3(
                        transform.position.x, 
                        transform.position.y - (hit.distance - maxGroundDistance), 
                        transform.position.z
                    );
                }
                else if(hit.distance < minGroundDistance)
                {
                    transform.position = new Vector3(
                        transform.position.x, 
                        transform.position.y + minGroundDistance, 
                        transform.position.z
                    );
                }
            }//Todo change to recognize if an object is above, move player up
            // else if(Physics.Raycast(transform.position, Vector3.down, out hit, maxGroundCheckDistance))
            // {
            //     transform.position = new Vector3(transform.position.x, hit.point.y + 2, transform.position.z);
            // }


            //characterController.Move(Vector3.up * yVelocity * gravityApprox * Time.deltaTime)

            //arcGISMapComponent.View.WorldToGeographic()
            // var worldPosition = math.inverse(arcGISMapComponent.WorldMatrix).HomogeneousTransformPoint(Input.mousePosition.ToDouble3());
            // var geoPosition = arcGISMapComponent.View.WorldToGeographic(worldPosition);
            //var correctedPos =  GeoUtils.ProjectToSpatialReference(geoPosition, arcGISMapComponent.View.SpatialReference);
            //Debug.Log($"Positon? {correctedPos.X}, {correctedPos.Y}, {correctedPos.Z}");
            // var b = CalcualteBearing(loc.Position, geoPosition);
            // loc.Rotation = b;
        }

        private IEnumerator WaitforMapLoad()
        {
            Debug.Log("Start Wait");
            yield return new WaitForSecondsRealtime(mapLoadWaitTime);
            mapReady = true;
            Debug.Log("End Wait");
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

    }
}

