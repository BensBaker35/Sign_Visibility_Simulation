using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;

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


            var signData = await ReadSignsAsync();
            foreach (var sign in signData)
            {
                GameObject signPrefab;
                var success = signTypeMap.TryGetValue(sign.Type, out signPrefab);
                var signObject = Instantiate(success ? signPrefab : signTypeMap[SignType.BASE], transform);
                var signLocation = signObject.GetComponent<ArcGISLocationComponent>();

                signLocation.enabled = true;
                signLocation.Position = new Esri.GameEngine.Geometry.ArcGISPoint(sign.Long, sign.Lat, sign.Elev, ArcGISSpatialReference.WGS84());

                signObject.name = sign.Name;
            }
        }

        private void Start()
        {


        }

        private async Task<SignData[]> ReadSignsAsync()
        {
            var data = await File.ReadAllLinesAsync(signDataFileLocation, System.Text.Encoding.UTF8);

            var signs = new SignData[data.Length - 1]; //Don't care about first row headers
            for (var i = 1; i < data.Length; i++)
            {
                var line = data[i];
                var values = line.Split(',');
                Debug.Log($"{values[0]}, {values[1]}, {values[2]}, {values[3]}, {values[4]}");
                try
                {
                    signs[i - 1] = new SignData()
                    {
                        Lat = float.Parse(values[0]),
                        Long = float.Parse(values[1]),
                        Elev = float.Parse(values[2]),
                        Type = !string.IsNullOrEmpty(values[3]) ? (SignType)Enum.Parse(typeof(SignType), values[3], true) : SignType.BASE,
                        Name = !string.IsNullOrEmpty(values[4]) ? values[4] : "Sign " + values[3],

                    };
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to parse: \n{e}\n{line}, {i}");
                }
            }

            return signs;
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
        public float Lat { get; set; }
        public float Long { get; set; }
        public float Elev { get; set; }
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
