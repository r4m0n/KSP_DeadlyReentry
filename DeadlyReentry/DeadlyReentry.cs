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

                if ((afx != null) && (afx.FxScalar > 0))
                {
                    Ray ray = new Ray();

                    foreach (Part p in FlightGlobals.ActiveVessel.Parts)
                    {
                        ray.direction = (p.Rigidbody.GetPointVelocity(p.transform.position) + Krakensbane.GetFrameVelocityV3f() - Krakensbane.GetLastCorrection() * TimeWarp.fixedDeltaTime).normalized;
                        ray.origin = p.transform.position;
                    }
                }
            }
        }
    }
}
