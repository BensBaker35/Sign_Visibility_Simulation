using System.Collections;
using System.Collections.Generic;
using Esri.GameEngine.Geometry;
using RIT.RochesterLOS.Signage;
using Unity.Mathematics;
using UnityEngine;

namespace RIT.RochesterLOS.Services
{
    public interface ISignService : IService
    {
        public delegate void NewSignSelection(int id);
        public event NewSignSelection OnNewSignSelection;
        GameObject[] GetSignPreFabs();
        GameObject GetObjectForType(SignType type);
        void AddNewSign(ArcGISPoint point, string name);
        bool TrySelect(string tag);
        double3 GetPositionData(int id);
        void UpdatePosition(int id, double3 point);
        //void CreateOrSelect

    }

    public enum SignEditType
    {
        Update,
        Delete,
        New,
        Edit
    }
    
    public enum SignType
    {
        BASE,
        HIGH_VIS,
        OBSTACLE,

    }
}
