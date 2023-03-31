using Esri.ArcGISMapsSDK.Components;
using Esri.HPFramework;
using RIT.RochesterLOS.Events;
using UnityEngine;
using Unity.Mathematics;
using Esri.GameEngine.Geometry;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using RIT.RochesterLOS.Services;

namespace RIT.RochesterLOS.Signage.Placement
{
    [RequireComponent(typeof(SignManager))]
    public class PlacementController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private double baseOffset = 5d;

        private ArcGISMapComponent arcGISMapComponent;
        private ISignService signManager;
        //private List<Signage.SignData> data;

        private void WorldReadyHandler(object p) => arcGISMapComponent = (ArcGISMapComponent)p;

        private void Awake()
        {
            EventManager.Listen(Events.Events.WorldReady, WorldReadyHandler);
            
            //data = new();
            Debug.Log($"SignManager Awake {signManager != null}");
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(Events.Events.WorldReady, WorldReadyHandler);
        }
        // Start is called before the first frame update
        void Start()
        {
            signManager = (ISignService)ServiceLocator.GetService<ISignService>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var mousePosScreen = Input.mousePosition;
                var mouseRay = mainCamera.ScreenPointToRay(mousePosScreen);

                RaycastHit hit;
                if (Physics.Raycast(mouseRay, out hit))
                {
                    var geoPosition = Analysis.AnalysisUtil.SimPositionToGeo(hit.point, ArcGISSpatialReference.WGS84());

                    if (!signManager.TrySelect(hit.collider.name))
                    {
                        //TODO possibly refactor into sign manager so it doesn't have to expose so much
                        var signPlacementObject = signManager.GetObjectForType(SignType.BASE);

                        

                        geoPosition = new ArcGISPoint(
                            geoPosition.X,
                            geoPosition.Y,
                            geoPosition.Z + baseOffset,
                            geoPosition.SpatialReference
                        );

                        //Debug.Log($"SignManager: {signManager != null}, geoPosition: {geoPosition.Z != null}");
                        signManager.AddNewSign(geoPosition, "From Placement");
                    }




                }
            }
        }


    }
}
