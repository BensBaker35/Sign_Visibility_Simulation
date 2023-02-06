using Esri.ArcGISMapsSDK.Components;
using Esri.HPFramework;
using RIT.RochesterLOS.Events;
using UnityEngine;
using Unity.Mathematics;
using Esri.GameEngine.Geometry;
using System.Collections.Generic;

namespace RIT.RochesterLOS.Signage.Placement
{
    public class PlacementController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject signPlacementObject;

        private ArcGISMapComponent arcGISMapComponent;
        private List<Signage.SignData> data;
        
        
        private void Awake()
        {
            EventManager.Listen(Events.Events.WorldReady, (p) => 
            {
                arcGISMapComponent = (ArcGISMapComponent)p;
                
            });
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
                    var simPosition = math.inverse(arcGISMapComponent.WorldMatrix).HomogeneousTransformPoint(hit.point.ToDouble3());
                    var geoPosition = arcGISMapComponent.View.WorldToGeographic(simPosition);
                    geoPosition = Esri.ArcGISMapsSDK.Utils.GeoCoord.GeoUtils.ProjectToSpatialReference(geoPosition, ArcGISSpatialReference.WGS84());                
                    
                    var newSign = Instantiate(signPlacementObject, hit.point, Quaternion.identity, transform);
                    var newSignLocation = newSign.GetComponent<ArcGISLocationComponent>();
                    newSignLocation.enabled = true;
                    newSignLocation.Position = geoPosition;

                    data.Add(new SignData(){
                        Lat = geoPosition.Y,
                        Lon = geoPosition.X,
                        Elev = geoPosition.Z,
                        Name = "From Placement",
                        Type = SignType.BASE
                    });

                }
            }
        }

        
    }
}
