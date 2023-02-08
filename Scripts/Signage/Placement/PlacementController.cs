using Esri.ArcGISMapsSDK.Components;
using Esri.HPFramework;
using RIT.RochesterLOS.Events;
using UnityEngine;
using Unity.Mathematics;
using Esri.GameEngine.Geometry;
using System.Collections.Generic;

namespace RIT.RochesterLOS.Signage.Placement
{
    [RequireComponent(typeof(SignManager))]
    public class PlacementController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private double baseOffset = 5d;

        private ArcGISMapComponent arcGISMapComponent;
        private SignManager signManager;
        //private List<Signage.SignData> data;


        private void Awake()
        {
            EventManager.Listen(Events.Events.WorldReady, (p) =>
            {
                arcGISMapComponent = (ArcGISMapComponent)p;

            });
            signManager = GetComponent<SignManager>();
            //data = new();
        }
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePosScreen = Input.mousePosition;
                var mouseRay = mainCamera.ScreenPointToRay(mousePosScreen);

                RaycastHit hit;
                if (Physics.Raycast(mouseRay, out hit))
                {
                    var geoPosition = Analysis.AnalysisUtil.SimPositionToGeo(hit.point, ArcGISSpatialReference.WGS84());


                    var signPlacementObject = signManager.GetObjectForType(SignType.BASE);

                    var newSign = Instantiate(signPlacementObject, hit.point, Quaternion.identity, transform);
                    var newSignLocation = newSign.GetComponent<ArcGISLocationComponent>();
                    newSignLocation.enabled = true;
                    newSignLocation.Position = new ArcGISPoint(
                        geoPosition.X, 
                        geoPosition.Y, 
                        geoPosition.Z + baseOffset, 
                        geoPosition.SpatialReference
                    );

                    signManager.AddNewSign(new SignData()
                    {
                        Lat = geoPosition.Y,
                        Lon = geoPosition.X,
                        Elev = geoPosition.Z,
                        Name = "From Placement",
                        Type = SignType.BASE,
                    });



                }
            }
        }


    }
}
