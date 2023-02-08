using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using RIT.RochesterLOS.Control;
using RIT.RochesterLOS.Events;
using Unity.Mathematics;
using UnityEngine;

namespace RIT.RochesterLOS.Player
{
    public class PlayerController : MonoBehaviour, IControllable
    {
        private static readonly ArcGISPoint PLAYER_SPAWN = new ArcGISPoint(-77.60638, 43.15719, 175, ArcGISSpatialReference.WGS84());
       
        [SerializeField] private float groundingDistance = 0.4f; //Distance from ground

        //[SerializeField] private GameObject cameraObject;
    
        private Camera childCamera; //Main Camera
        private CharacterController characterController;
        private float xRotation = 0f;
        private bool isGrounded = false;
        private Vector3 velocity;
        private float gravity = -9.81f;
        private bool mapReady = false;

        void Awake()
        {
            EventManager.Listen(Events.Events.WorldReady, (_) => mapReady = true);
        }

        // Start is called before the first frame update
        void Start()
        {
            
            childCamera = GetComponentInChildren<Camera>();
            
            characterController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            if(!mapReady) return;
            
            isGrounded = Physics.CheckSphere(transform.position, groundingDistance);

            if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            
            velocity.y += gravity * Time.deltaTime;

            characterController.Move(velocity * Time.deltaTime);

            
        }

        public void PushKeyboardInput(Vector3 input)
        {
            characterController.Move(input);
        }

        public void PushMouseInput(Vector3 look, Vector2 scroll)
        {
            var mousY = look.y;
            var mousX = look.x;

            xRotation -= mousY;
            xRotation = Mathf.Clamp(xRotation, -75f, 75f);

            childCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mousX);

        }
    }
}

