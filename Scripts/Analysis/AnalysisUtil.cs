using System.Collections;
using System.Collections.Generic;
using Esri.GameEngine.Geometry;
using UnityEngine;
using System;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;

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
        private static AnalysisUtil instance;

        public static AnalysisUtil Instance 
        {
            get 
            {
                if(instance == null)
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
        }


        private void _TestConvertCoordinates(ArcGISPoint p1)
        {
            var points = wgs84ToNYW.MathTransform.Transform(new[] {p1.X, p1.Y});
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
            switch(coordTo)
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
            if(direction == MoveDirection.EW)
            {
                inMeters[0] += amount;
            }
            else 
            {
                inMeters[1] += amount;
            }
            return _convertCoordinateTo(inMeters, ConvertTo.WGS84);
        }

        public static ArcGISPoint MoveCoordinate(ArcGISPoint point, float amount, MoveDirection dir)
        {
            var pointAsDouble = new Double[]{point.X, point.Y};
            var finalPoint = Instance._moveCoordinates(pointAsDouble, amount, dir);
            return new ArcGISPoint(finalPoint[0], finalPoint[1], point.Z, ArcGISSpatialReference.WGS84());

        }


    }

    
}
