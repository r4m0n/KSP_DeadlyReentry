#define DEBUG

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
        DebugWindow window;
        AerodynamicsFX afx;

        public float Multiplier { get; set; }
        public float Exponent { get; set; }

        public void Start()
        {
            window = new DebugWindow(this);
            Multiplier = 20000.0f;
            Exponent = 2f;
        }

        public void Update()
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

                        if (!Physics.Raycast(ray, 10))
                        {
                            p.temperature += Mathf.Pow(afx.FxScalar, Exponent) * Multiplier * TimeWarp.deltaTime;
                        }
                    }
                }
            }
        }

        public void ToggleDebugWindow()
        {
            window.SetVisible(!window.IsVisible());
        }
    }

    class DebugWindow : Window
    {
        private DeadlyReentryGhost ghost;

        public DebugWindow(DeadlyReentryGhost ghost)
            : base("Deadly Reentry", null)
        {
            this.ghost = ghost;
        }

        protected override void Draw(int windowID)
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
                SetVisible(false);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Exponent:", labelStyle);
            string newExponent = GUILayout.TextField(ghost.Exponent.ToString(), GUILayout.MinWidth(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Multiplier:", labelStyle);
            string newMultiplier = GUILayout.TextField(ghost.Multiplier.ToString(), GUILayout.MinWidth(100));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow();

            if (GUI.changed)
            {
                float newValue;
                if (float.TryParse(newExponent, out newValue))
                {
                    ghost.Exponent = newValue;
                }

                if (float.TryParse(newMultiplier, out newValue))
                {
                    ghost.Multiplier = newValue;
                }
            }
        }
    }
}

public class DeadlyReentryModule : PartModule
{
    private DeadlyReentry.DeadlyReentryGhost drg;

    [KSPEvent(guiActive = true, guiName = "Toggle Deadly Reentry Debug Window", active = false)]
    public void ToggleDebugWindow()
    {
        drg.ToggleDebugWindow();
    }

    public override void OnStart(StartState state)
    {
#if DEBUG
        if (state != StartState.Editor)
        {
            drg = GameObject.Find("DeadlyReentryGhost").GetComponent<DeadlyReentry.DeadlyReentryGhost>();
            Events["ToggleDebugWindow"].active = true;
        }
#endif
    }
}
