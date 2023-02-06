using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.Control
{
    public interface IControllable
    {
        public void PushKeyboardInput(Vector3 input);
        public void PushMouseInput(Vector3 look, Vector2 scroll);
    }
}
