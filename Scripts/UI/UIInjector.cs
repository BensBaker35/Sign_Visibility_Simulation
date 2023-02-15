using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.UI
{
    [CreateAssetMenu(fileName = "Scene UI", menuName = "ScriptableObjects/SceneUI", order = 1)]
    public class UIInjector  : ScriptableObject
    {
        public List<GameObject> MenuRoots;
    }

}
