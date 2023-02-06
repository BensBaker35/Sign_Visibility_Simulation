using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using RIT.RochesterLOS.LOS;

namespace RIT.RochesterLOS.Signage
{
    public class SignManager : MonoBehaviour
    {
        private string signDataFileLocation = "Assets/Data/SignPlaces.csv";
        [SerializeField] List<SignTypeToObject> signTypeKVP;
        private Dictionary<SignType, GameObject> signTypeMap;

        private async void Awake()
        {
            if (signTypeKVP.Count == 0)
            {
                Debug.LogWarning("No Sign Mappings Available, must add prefabs for signs to spawn");
                return;
            }

            signTypeMap = new();
            foreach (var mapping in signTypeKVP)
            {
                signTypeMap.Add(mapping.TypeOfSign, mapping.SignPrefab);
            }


            var signData = await Serialization.SignSerializer.Deserialize();
            foreach (var sign in signData)
            {
                GameObject signPrefab;
                var success = signTypeMap.TryGetValue(sign.Type, out signPrefab);
                var signObject = Instantiate(success ? signPrefab : signTypeMap[SignType.BASE], transform);
                var signLocation = signObject.GetComponent<ArcGISLocationComponent>();

                signLocation.enabled = true;
                signLocation.Position = new Esri.GameEngine.Geometry.ArcGISPoint(sign.Lon, sign.Lat, sign.Elev, ArcGISSpatialReference.WGS84());
                
                signObject.name = sign.Name;
            }
        }

        private void Start()
        {


        }

        

        /// <summary>
        /// Helper Class to programatically create mapping of sign objects to type
        /// </summary>
        [System.Serializable]
        private class SignTypeToObject
        {
            public SignType TypeOfSign;
            public GameObject SignPrefab;
        }

    }

#nullable enable
    internal struct SignData
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Elev { get; set; }
        public string? Name { get; set; }
        public SignType Type { get; set; }
    }
#nullable disable

    internal enum SignType
    {
        BASE,
        HIGH_VIS,

    }

}
