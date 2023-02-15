using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using RIT.RochesterLOS.LOS;
using System.Linq;
using RIT.RochesterLOS.Events;

namespace RIT.RochesterLOS.Signage
{
    public class SignManager : MonoBehaviour
    {
        private string signDataFileLocation = "Assets/Data/SignPlaces.csv";
        [SerializeField] List<SignTypeToObject> signTypeKVP;
        private Dictionary<SignType, GameObject> signTypeMap;
        private List<SignData> signData;

        private void Awake()
        {
            //EventManager.Listen(Events.Events.Save, (_) => Serialization.SignSerializer.Serialize(signData));

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



        }

        private async void Start()
        {
            var signDataArray = await Serialization.SignSerializer.Deserialize();
            signData = signDataArray.ToList();
            foreach (var sign in signData)
            {
                var signPrefab = GetObjectForType(sign.Type);
                var signObject = Instantiate(signPrefab, transform);
                var signLocation = signObject.GetComponent<ArcGISLocationComponent>();

                signLocation.enabled = true;
                signLocation.Rotation = new Esri.ArcGISMapsSDK.Utils.GeoCoord.ArcGISRotation(0, 90, 0);
                signLocation.Position = new Esri.GameEngine.Geometry.ArcGISPoint(sign.Lon, sign.Lat, sign.Elev, ArcGISSpatialReference.WGS84());
                
                signObject.name = sign.Name;
            }

            EventManager.TriggerEvent(Events.Events.SignsPlaced, null);

        }

        internal GameObject GetObjectForType(SignType sType)
        {   
            GameObject toReturn;
            if(signTypeMap.TryGetValue(sType, out toReturn))
            {
                return toReturn;
            }
            else 
            {
                return signTypeMap[SignType.BASE];
            }
        }
        
        internal void AddNewSign(SignData data)
        {
            signData.Add(data);
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
