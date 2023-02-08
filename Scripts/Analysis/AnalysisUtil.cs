using System.Collections;
using System.Collections.Generic;
using Esri.GameEngine.Geometry;
using UnityEngine;
using System;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using RIT.RochesterLOS.Events;
using Unity.Mathematics;
using Esri.GameEngine.View;
using Esri.HPFramework;

namespace RIT.RochesterLOS.Analysis
{

    public class AnalysisUtil
    {
        public enum ConvertTo
        {
            NY_STATE_West,
            WGS84
        }

        public enum MoveDirection
        {
            EW,
            NS
        }

        private const string WGS84_WKT = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
        private const string NYW_WKT = "PROJCS[\"NAD83 / New York West\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",40],PARAMETER[\"central_meridian\",-78.58333333333333],PARAMETER[\"scale_factor\",0.9999375],PARAMETER[\"false_easting\",350000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";

        private CoordinateSystemFactory fac;
        private CoordinateTransformationFactory ctfac;
        private ICoordinateTransformation wgs84ToNYW;
        private double4x4 worldMatrix;
        private ArcGISView view;
        private static AnalysisUtil instance;

        private static AnalysisUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new();
                }
                return instance;
            }

        }

        private AnalysisUtil()
        {
            fac = new();
            var wgs84 = fac.CreateFromWkt(WGS84_WKT);
            var nyw = fac.CreateFromWkt(NYW_WKT);

            ctfac = new();
            wgs84ToNYW = ctfac.CreateFromCoordinateSystems(wgs84, nyw);


            EventManager.Listen(Events.Events.WorldReady, SaveMapTools);
        }

        private void SaveMapTools(object package)
        {
            var component = (Esri.ArcGISMapsSDK.Components.ArcGISMapComponent)package;
            worldMatrix = component.WorldMatrix;
            view = component.View;
        }
        private void _TestConvertCoordinates(ArcGISPoint p1)
        {
            var points = wgs84ToNYW.MathTransform.Transform(new[] { p1.X, p1.Y });
            Debug.Log($"X: {points[0]}, Y: {points[1]}");

            var inversePoints = wgs84ToNYW.MathTransform.Inverse().Transform(points);
            Debug.Log($"Long: {inversePoints[0]}, Lat: {inversePoints[1]}");
        }

        public static void ConvertCoordinates(ArcGISPoint p1)
        {
            Instance._TestConvertCoordinates(p1);
        }


        private double[] _convertCoordinateTo(double[] origin, ConvertTo coordTo)
        {
            switch (coordTo)
            {
                case ConvertTo.NY_STATE_West:
                    return wgs84ToNYW.MathTransform.Transform(origin);
                case ConvertTo.WGS84:
                    return wgs84ToNYW.MathTransform.Inverse().Transform(origin);
                default:
                    return null;
            }
        }
        //Need to look into other functions if possible, only ended up with a 7 meter diference
        private double[] _moveCoordinates(double[] origin, double amount, MoveDirection direction)
        {
            var inMeters = _convertCoordinateTo(origin, ConvertTo.NY_STATE_West);
            var inMetersTest = inMeters.Clone();
            if (direction == MoveDirection.EW)
            {
                inMeters[0] += amount;
            }
            else
            {
                inMeters[1] += amount;
            }

            return _convertCoordinateTo(inMeters, ConvertTo.WGS84);
        }

        private double _calcDistance(double[] origin, double[] measureTo)
        {
            _toRadians(ref origin);
            _toRadians(ref measureTo);

            var earthRad = 6356752.3142;

            return Math.Acos(Math.Sin(origin[1]) * Math.Sin(measureTo[1]) + Math.Cos(measureTo[1]) * Math.Cos(measureTo[1]) * Math.Cos(measureTo[0] - measureTo[0])) * earthRad;
        }

        private void _toRadians(ref double[] toConvert)
        {
            for (var i = 0; i < toConvert.Length; i++)
            {
                toConvert[i] *= Math.PI * 180;
            }
        }

        public static ArcGISPoint MoveCoordinate(ArcGISPoint point, float amount, MoveDirection dir)
        {
            var pointAsDouble = new Double[] { point.X, point.Y };
            var finalPoint = Instance._moveCoordinates(pointAsDouble, amount, dir);
            return new ArcGISPoint(finalPoint[0], finalPoint[1], point.Z, ArcGISSpatialReference.WGS84());
            // var builder = (ArcGISPolylineBuilder) ArcGISGeometryBuilder.Create(ArcGISGeometryType.Polyline, ArcGISSpatialReference.WGS84());
            // var nextP = Instance._moveCoordinates(new[] {point.X, point.Y}, 1, dir);
            // builder.AddPoint(point.X, point.Y, point.Z);
            // builder.AddPoint(nextP[0], nextP[1], point.Z);

            //var lp = (ArcGISPolyline) ArcGISGeometryEngine.Offset(builder.ToGeometry(), amount, ArcGISGeometryOffsetType.Bevelled, 0, 0);
            //return  lp.Parts.GetPart(0).EndPoint;
            //var amount_d = ((0.0000001d/0.111d) * (double)amount);
            //return (ArcGISPoint) ArcGISGeometryEngine.Offset(point, amount, ArcGISGeometryOffsetType.Bevelled, 0, 0);

        }

        public static double DistanceBetweenPoints(ArcGISPoint origin, ArcGISPoint measureTo)
        {

            return ArcGISGeometryEngine.Distance(origin, measureTo);

            //return Instance._calcDistance(new[] {origin.X, origin.Y}, new[] {measureTo.X, measureTo.Y});
        }

        public static ArcGISPoint SimPositionToGeo(Vector3 simPoint)
        {
            return SimPositionToGeo(simPoint, ArcGISSpatialReference.WGS84());
        }
        public static ArcGISPoint SimPositionToGeo(Vector3 simPoint, ArcGISSpatialReference projectSpatial)
        {
           
            var geoPosition = Instance._simPositionToGeo(simPoint.ToDouble3());
            return Esri.ArcGISMapsSDK.Utils.GeoCoord.GeoUtils.ProjectToSpatialReference(geoPosition, projectSpatial);
        }

        private ArcGISPoint _simPositionToGeo(double3 simPoint)
        {
            var simPosition = math.inverse(worldMatrix).HomogeneousTransformPoint(simPoint);
            var geoPosition = view.WorldToGeographic(simPosition);
            return geoPosition;
        }

    }


}
