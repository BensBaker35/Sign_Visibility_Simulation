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
using RIT.RochesterLOS.Services;
using Unity.Mathematics;

namespace RIT.RochesterLOS.Signage
{
    public class SignManager : MonoBehaviour, ISignService
    {
        private string signDataFileLocation = "Assets/Data/SignPlaces.csv";
        [SerializeField] List<SignTypeToObject> signTypeKVP;
        private Dictionary<SignType, GameObject> signTypeMap;
        private List<ArcGISLocationComponent> signDataLocations;
        private Dictionary<int, SignData> signDataTable;
        private Dictionary<SignData, GameObject> signDataGameObjectMap;
        //private SignData currentlyEditing;
        private SignType currentType = SignType.BASE;


        public event ISignService.NewSignSelection OnNewSignSelection;

        private void SerializeHandler(object _) => Serialization.SignSerializer.Serialize(signDataTable.Values.ToList());
        private async void LoadSignsHandler(object _) => await LoadSigns();
        private void SelectSignObjectHandler(object p) => currentType = (SignType)p;

        private void Awake()
        {
            
            EventManager.Listen(Events.Events.SaveSignData, SerializeHandler);
            EventManager.Listen(Events.Events.LoadSignData, LoadSignsHandler);
            EventManager.Listen(Events.Events.SelectSignObject, SelectSignObjectHandler);

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

            ServiceLocator.RegisterService<ISignService>(this);
            signDataTable = new();
            signDataGameObjectMap = new();
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(Events.Events.SaveSignData, SerializeHandler);
            EventManager.RemoveListener(Events.Events.LoadSignData, LoadSignsHandler);
            EventManager.RemoveListener(Events.Events.SelectSignObject, SelectSignObjectHandler);
        }

        private async void Start()
        {
            await LoadSigns();

            EventManager.TriggerEvent(Events.Events.SignsPlaced, null);

        }

        private async Task LoadSigns()
        {
            var signDataArray = await Serialization.SignSerializer.Deserialize();

            foreach (var sign in signDataArray)
            {
                var signPrefab = GetObjectForType(sign.Type);
                var signObject = Instantiate(signPrefab, transform);
                var signLocation = signObject.GetComponent<ArcGISLocationComponent>();

                signLocation.enabled = true;
                signLocation.Rotation = new Esri.ArcGISMapsSDK.Utils.GeoCoord.ArcGISRotation(0, 90, 0);
                signLocation.Position = new Esri.GameEngine.Geometry.ArcGISPoint(sign.Lon, sign.Lat, sign.Elev, ArcGISSpatialReference.WGS84());

                signObject.name = sign.GetHashCode().ToString();

                signDataTable.Add(sign.GetHashCode(), sign);
                signDataGameObjectMap.Add(sign, signObject);
            }
        }

        public GameObject GetObjectForType(SignType sType)
        {
            GameObject toReturn;
            if (signTypeMap.TryGetValue(sType, out toReturn))
            {
                return toReturn;
            }
            else
            {
                return signTypeMap[SignType.BASE];
            }
        }

        public GameObject[] GetSignPreFabs()
        {
            return signTypeMap.Values.ToArray(); 
        }

        public void AddNewSign(ArcGISPoint point, string name)
        {
            var dataEntry = new SignData()
            {
                Lat = point.Y,
                Lon = point.X,
                Elev = point.Z,
                Name = name,
                Type = currentType,
            };

            signDataTable.Add(dataEntry.GetHashCode(), dataEntry);

            var signGameObject = Instantiate(GetObjectForType(currentType), transform.position, Quaternion.identity, transform);
            var signLoc = signGameObject.GetComponent<ArcGISLocationComponent>();
            signLoc.enabled = true;
            signLoc.Position = point;

            //signGameObject.name = name;
            signGameObject.name = dataEntry.GetHashCode().ToString();
            Debug.Log($"Sign Manager: Creating Sign with ID {signGameObject.name}");
            signDataGameObjectMap.Add(dataEntry, signGameObject);

            OnNewSignSelection(dataEntry.GetHashCode());
        }

        public bool TrySelect(string tag)
        {
            int hashLookup;
            var success = int.TryParse(tag, out hashLookup);
            if( success && signDataTable.ContainsKey(hashLookup))
            {
                OnNewSignSelection(hashLookup);
                return true;
            }
            return false;
        }

        public double3 GetPositionData(int id)
        {
            SignData sd;
            if(signDataTable.TryGetValue(id, out sd))
            {
                return new double3(sd.Lon, sd.Lat, sd.Elev);
            }

            return default(double3);
        }

        public void UpdatePosition(int id, double3 data)
        {
            var signObject = signDataGameObjectMap[signDataTable[id]];

            var newPos = new ArcGISPoint(data.x, data.y, data.z);
            signObject.GetComponent<ArcGISLocationComponent>().Position = newPos;

            if(currentType != signDataTable[id].Type)
            {
                var tmp = signDataTable[id];
                tmp.Type = currentType;
                signDataTable[id] = tmp;
            }
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



}
