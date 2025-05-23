using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Valve.VR.InteractionSystem.Sample
{
    public class sliceButton : MonoBehaviour
    {

        public bool buttonDown = false;

        public void OnButtonDown(Hand fromHand)

        {
            buttonDown = true;

        }

        public void OnButtonUp(Hand fromHand)

        {
            buttonDown = false;

        }
    }
}

