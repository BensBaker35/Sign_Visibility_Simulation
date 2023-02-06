using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.Signage.Placement
{
    // Location Pin from https://www.flaticon.com/free-icon/location-pin_64113
    public class MarkerSizing : MonoBehaviour
    {
        private GameObject cameraObject;
        // Start is called before the first frame update
        void Start()
        {
            cameraObject = Camera.main.gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            //From: https://answers.unity.com/questions/589412/place-2d-image-in-3d-space-not-gui-not-3d-object.html
            transform.up = cameraObject.transform.position - transform.position;
            transform.forward = -cameraObject.transform.up;

            //Adapted from https://forum.unity.com/threads/how-to-increase-or-decrease-the-scale-of-a-gameobject-according-to-distance-from-camera.821160/
            var minDistance = 0.5f;
            var maxDistance = 3f;
            var minScale = 0.5f;
            var maxScale = 4f;

            //TODO Fix to always align point on position
            var scale = Mathf.Lerp(minScale, maxScale, Mathf.InverseLerp(minDistance, maxDistance, Vector3.Distance(cameraObject.transform.position, transform.position)));
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
