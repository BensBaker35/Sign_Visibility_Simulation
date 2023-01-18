using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Extent;
using Esri.GameEngine.Map;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.Layers;
using Esri.Unity;
using UnityEngine;
using Esri.GameEngine;

namespace RIT.RochesterLOS.Mapping
{
    /// <summary>
    /// Script to handle communication between ESRI API, ArcGIS SDK for Unity, and Unity itself
    /// 
    /// </summary>
    [ExecuteAlways]
    public class MapControl : MonoBehaviour
    {

        public static readonly string MAIN_API_KEY = "AAPKd7b681581d6a439cb98d19ac0aefb8c0ZNzHfjFulf5_xRityexgJc_CHdbmCH7YFYyqSheV7vwp_isYxmEPO_4MumWoV8rE";
        
        private static  readonly ArcGISPoint ROCHESTER_COORDINATES = new(-77.6066334, 43.1566377, 176, ArcGISSpatialReference.WGS84()); // X, Y, Z, Spatial Reference
        private ArcGISMapComponent arcGISMapComponent;
        private ArcGISCameraComponent cameraComponent;

    

        // Start is called before the first frame update
        void Start()
        {
            CreateArcGISMapComponent();
            //CreateArcGISCamera();
            CreateArcGISMap();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Creates the actual map from specified layers and data
        /// </summary>
        private void CreateArcGISMap()
        {
            Debug.Log("Creating Map");

            var arcGISMap = new Esri.GameEngine.Map.ArcGISMap(arcGISMapComponent.MapType);
            
            arcGISMap.Basemap = new ArcGISBasemap(ArcGISBasemapStyle.ArcGISImageryStandard, MAIN_API_KEY);

            arcGISMap.Elevation = new Esri.GameEngine.Map.ArcGISMapElevation(new Esri.GameEngine.Elevation.ArcGISImageElevationSource(
                "https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer", "Elevation", MAIN_API_KEY));

            var buildingLayer = new ArcGIS3DObjectSceneLayer(
                "https://tiles.arcgis.com/tiles/RQcpPaCpMAXzUI5g/arcgis/rest/services/InnerLoopParcel3D/SceneServer", MAIN_API_KEY);
            arcGISMap.Layers.Add(buildingLayer);
            

            arcGISMapComponent.View.Map = arcGISMap;
        }

        /// <summary>
        /// Creates the Map Component, a speicalized gameobject that controls rendering and interaction with the map
        /// </summary>
        private void CreateArcGISMapComponent()
        {
            Debug.Log("Creating Map Component");
            arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();

            if (!arcGISMapComponent)
            {
                var arcGISMapGameObject = new GameObject("ArcGISMap");
                arcGISMapComponent = arcGISMapGameObject.AddComponent<ArcGISMapComponent>();
            }
            arcGISMapComponent.OriginPosition = ROCHESTER_COORDINATES;
            arcGISMapComponent.MapType = Esri.GameEngine.Map.ArcGISMapType.Local;
            arcGISMapComponent.MapTypeChanged += new ArcGISMapComponent.MapTypeChangedEventHandler(CreateArcGISMap);
        }

        /// <summary>
        /// Sets up the camera for use with ArcGIS SDK for Unity
        /// 
        /// </summary>
        private void CreateArcGISCamera()
        {
            //TODO, move to Station Camera
            Debug.Log("Creating Camera");
            cameraComponent = Camera.main.gameObject.GetComponent<ArcGISCameraComponent>();

            if (!cameraComponent)
            {
                Debug.Log("Can't Find Camera Component");
                var cameraGameObject = Camera.main.gameObject;

                cameraGameObject.transform.SetParent(arcGISMapComponent.transform, false);
                cameraComponent = cameraGameObject.AddComponent<ArcGISCameraComponent>();
                //cameraGameObject.AddComponent<ArcGISCameraControllerComponent>();
                cameraGameObject.AddComponent<ArcGISRebaseComponent>();
            }

            var cameraLocationComponent = cameraComponent.GetComponent<ArcGISLocationComponent>();

            if (!cameraLocationComponent)
            {
                cameraLocationComponent = cameraComponent.gameObject.AddComponent<ArcGISLocationComponent>();
                cameraLocationComponent.Position = ROCHESTER_COORDINATES;
                cameraLocationComponent.Rotation = new ArcGISRotation(0, 90, 0);
            }
        }
    }

}

