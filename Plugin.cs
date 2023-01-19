using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Nomad
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource logger;

        private void Awake()
        {
            // Plugin startup logic
            logger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded");
            Logger.LogInfo($"Patching...");
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Patched");
        }

        [HarmonyPatch(typeof(ItemStats), "Awake")]
        [HarmonyPostfix]
        static void Awake_Postfix(ItemStats __instance)
        {
            // Debug drawing
            /*
            if (__instance.GetComponent<AttachSystemRaftBase>() != null)
            {
                foreach (var ag in __instance.GetComponent<AttachSystemRaftBase>().attachGroups)
                {
                    foreach (var acp in ag.attachCheckPoints)
                    {
                        LineRenderer lr = acp.gameObject.AddComponent<LineRenderer>();
                        lr.useWorldSpace = false;
                        lr.startWidth = .2f;
                        lr.endWidth = .05f;
                        lr.SetPositions(new Vector3[] { new Vector3(0, 0, 0), acp.transform.up.normalized * .1f });

                        var acpgo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(acpgo.GetComponent<SphereCollider>());
                        acpgo.transform.SetParent(acp.transform);
                        //acpgo.transform.localScale = new Vector3(.1f, .1f, .1f);
                    }
                }
            }*/

            if ((__instance.AttachToTerrain && __instance.GetComponent<AttachSystemRaftBase>() == null && __instance.Tag != "SunClock_Simple") || __instance.Tag == "Fish_Trap")
            {
                logger.Log(LogLevel.Debug, $"Enabling attach for {__instance.Tag}");

                CraftableObject co = __instance.GetComponent<CraftableObject>();
                if (co == null)
                {
                    __instance.Draggable = true;
                }
                else
                {
                    AccessTools.Field(typeof(CraftableObject), "origDraggable").SetValue(co, true);
                }

                GameObject ap = new GameObject();
                ap.transform.SetParent(__instance.gameObject.transform);
                ap.transform.localPosition = new Vector3(0, 0, 0);
                ap.transform.localRotation = Quaternion.identity;
                ap.transform.up *= -1;

                AttachSystemRaftBase asrb = __instance.gameObject.AddComponent<AttachSystemRaftBase>();
                asrb.attachGroups = new AttachSystemRaftBase.attachGroup_t[] {
                    new AttachSystemRaftBase.attachGroup_t() {
                        attachCheckPoints = new Transform[] {
                            ap.transform, ap.transform, ap.transform, ap.transform
                        }
                    }
                };
                asrb.attachingObject = ObjectManager.Instance.GetObject("Rope_Strong");

                // Debug drawing
                /*
                LineRenderer lr = ap.AddComponent<LineRenderer>();
                lr.useWorldSpace = false;
                lr.startWidth = .2f;
                lr.endWidth = .05f;
                lr.SetPositions(new Vector3[] { new Vector3(0, 0, 0), ap.transform.up.normalized * .1f });
                
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(g.GetComponent<SphereCollider>());
                g.transform.SetParent(ap.transform);
                g.transform.localPosition = new Vector3(0, 0, 0);
                g.transform.localScale = new Vector3(.1f, .1f, .1f);
                */
            }
        }
    }
}
