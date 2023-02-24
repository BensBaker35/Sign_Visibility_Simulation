using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using UnityEngine;

namespace RIT.RochesterLOS.LOS
{
    [RequireComponent(typeof(SphereCollider))]
    public class ViewCheck : PlayerActivatedAction
    {
        private float distance;
        private SphereCollider sphereCollider;
        private ArcGISLocationComponent arcGISLocationComponent;
        // Start is called before the first frame update

        void Awake()
        {
            arcGISLocationComponent = GetComponent<ArcGISLocationComponent>();
            sphereCollider = GetComponent<SphereCollider>();
            RIT.RochesterLOS.Events.EventManager.Listen(Events.Events.WorldReady, SetupSphere);
        }

        void Start()
        {

        


        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerInActivation(other);
            Debug.Log("Entered Trigger");
        }

        private void SetupSphere(object package)
        {
            var distanceCheckPostion = Analysis.AnalysisUtil.MoveCoordinate(arcGISLocationComponent.Position, 50, Analysis.AnalysisUtil.MoveDirection.NS);
            Debug.Log($"Offset Test: {distanceCheckPostion.X}, {distanceCheckPostion.Y}, {distanceCheckPostion.Z}");
            Debug.Log(Analysis.AnalysisUtil.DistanceBetweenPoints(arcGISLocationComponent.Position, distanceCheckPostion));
            var mapView = (Esri.GameEngine.View.ArcGISView)package;
            var testPoint1 = mapView.GeographicToWorld(distanceCheckPostion);
            var testPoint2 = mapView.GeographicToWorld(arcGISLocationComponent.Position);

            double newRadius = Unity.Mathematics.math.distance(testPoint1, testPoint2);

            sphereCollider.radius = (float)newRadius;

            var temp = Instantiate(new GameObject(), transform.parent);
            testPoint1 = GetComponent<HPTransform>().UniversePosition;
            temp.transform.position = new Vector3((float)testPoint1.x, (float)testPoint1.y, (float)testPoint1.z);

        }

    }
}
