using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DeadlyReentry
{
    public class DeadlyReentry : KSP.Testing.UnitTest
    {
        public DeadlyReentry()
        {
            GameObject ghost = new GameObject("DeadlyReentryGhost", typeof(DeadlyReentryGhost));
            GameObject.DontDestroyOnLoad(ghost);

            base.TestStartUp();
        }
    }

    public class DeadlyReentryGhost : MonoBehaviour
    {
        AerodynamicsFX afx;

        void Update()
        {
            if (FlightGlobals.ready && (FlightGlobals.ActiveVessel != null))
            {
                if (afx == null)
                {
                    GameObject fx = GameObject.Find("FXLogic");
                    if (fx != null)
                    {
                        afx = fx.GetComponent<AerodynamicsFX>();
                    }
                }

                if (afx != null)
                {
                    if (afx.FxScalar > 0)
                    {
                        //print("DeadlyReentryGhost - FxScalar = " + afx.FxScalar);
                        foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                        {
                            p.temperature += afx.FxScalar * 1000.0f * TimeWarp.deltaTime;
                        }
                    }
                }
            }
        }
    }
}
