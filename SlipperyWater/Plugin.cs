using BepInEx;
using HarmonyLib;
using SlipperyWater.Scripts;
using System.IO;
using System.Reflection;
using UnityEngine;
using Utilla;

namespace SlipperyWater
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private bool InRoom;
        private Harmony WaterPatch;

        public static Texture2D IceTex { get; private set; }

        public async void Start()
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SlipperyWater.Resources.waterTexA.png");
            byte[] bytes = new byte[manifestResourceStream.Length];
            await manifestResourceStream.ReadAsync(bytes, 0, bytes.Length);

            IceTex = new Texture2D(512, 512, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Point,
                name = "iceTexA"
            };
            IceTex.LoadImage(bytes);
            IceTex.Apply();

            if (WaterPatch != null) return;
            WaterPatch = new Harmony(PluginInfo.GUID);
            WaterPatch.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void OnEnable()
        {
            if (InRoom && WaterMain.Initalized)
                WaterMain.DisableWater();
        }

        public void OnDisable()
        {
            if (WaterMain.Initalized)
                WaterMain.EnableWater();
        }

        [ModdedGamemodeJoin]
        public void OnJoin()
        {
            InRoom = true;
            if (enabled && WaterMain.Initalized)
                WaterMain.DisableWater();
        }

        [ModdedGamemodeLeave]
        public void OnLeave()
        {
            InRoom = false;
            if (WaterMain.Initalized)
                WaterMain.EnableWater();
        }
    }
}
