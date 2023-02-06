using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.MapView;
using RIT.RochesterLOS.Control;
using RIT.RochesterLOS.Player;
using UnityEngine;

namespace RIT.RochesterLOS.CustomCamera
{
    public class PlacementViewCamera : ArcGISCameraComponent, IControllable
    {
        [SerializeField] private float zoomMultiplier = 1.5f;
        [SerializeField] private float zoomDuration = 1f;

        void Awake()
        {
            base.Initialize();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        protected override void Update()
        {
            //Input.Scr
            base.Update();
        }

        public void PushKeyboardInput(Vector3 input)
        {
            transform.Translate(input.x, 0, -input.y, Space.World);
        }

        public void PushMouseInput(Vector3 look, Vector2 scroll)
        {
            if(Input.GetMouseButton(1)) //Right Click
            {
                transform.Rotate(Vector3.forward * -look.x);
            }

            var angle = Mathf.Abs((this.verticalFov / scroll.y) - this.verticalFov);
            verticalFov = Mathf.MoveTowards(this.verticalFov, scroll.y * this.verticalFov, angle / zoomDuration * Time.deltaTime);
        }
    }
}
