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
        }
    }

    public class DeadlyReentryGhost : MonoBehaviour
    {
        AerodynamicsFX afx;

        public static float Multiplier = 20000;
        public static float Exponent = 2;

        protected bool debugging = false;
        protected Rect windowPos = new Rect(100, 100, 0, 0);

        public void OnGUI()
        {
            if (debugging)
            {
                windowPos = GUILayout.Window("DeadlyReentry".GetHashCode(), windowPos, DrawWindow, "Deadly Reentry Setup");
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.D))
            {
                debugging = !debugging;
            }

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

                        if (!Physics.Raycast(ray, 10))
                        {
                            p.temperature += Mathf.Pow(afx.FxScalar * afx.state, Exponent) * Multiplier * TimeWarp.deltaTime;
                        }
                    }
                }
            }
        }

        public void DrawWindow(int windowID)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding = new RectOffset(5, 5, 3, 0);
            buttonStyle.margin = new RectOffset(1, 1, 1, 1);
            buttonStyle.stretchWidth = false;
            buttonStyle.stretchHeight = false;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.wordWrap = false;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", buttonStyle))
            {
                debugging = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Exponent:", labelStyle);
            string newExponent = GUILayout.TextField(Exponent.ToString(), GUILayout.MinWidth(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Multiplier:", labelStyle);
            string newMultiplier = GUILayout.TextField(Multiplier.ToString(), GUILayout.MinWidth(100));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow();

            if (GUI.changed)
            {
                float newValue;
                if (float.TryParse(newExponent, out newValue))
                {
                    Exponent = newValue;
                }

                if (float.TryParse(newMultiplier, out newValue))
                {
                    Multiplier = newValue;
                }
            }
        }
    }
}
