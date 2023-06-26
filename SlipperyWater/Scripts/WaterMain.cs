using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SlipperyWater.Scripts
{
    public static class WaterMain
    {
        public static bool Initalized { get; private set; }

        // Surfaces
        public static List<Collider> waterSurfaces = new List<Collider>();
        private static GameObject primaryWaterSurface;

        // Dictionaries
        private static Dictionary<Material, Texture2D> materialTextDict = new Dictionary<Material, Texture2D>();
        private static Dictionary<Material, Vector4> materialVectorDict = new Dictionary<Material, Vector4>();
        private static Dictionary<Material, Color> materialColourDict = new Dictionary<Material, Color>();

        // Audio
        private static GameObject audioDirectory;

        // Constants
        private const float waterScale = 0.25f;
        private const float waterAlpha = 0.75f;

        public static void Initalize()
        {
            primaryWaterSurface = GameObject.Find("Level/beach/Beach_Main_Geo/B_Terrain_prefab/B_WaterPlane/B_WaterPlane");
            audioDirectory = GameObject.Find("Level/beach/Beach_SoundObjects");
            Initalized = true;
        }

        public static void EnableWater()
        {
            var waterVolumes = GameObject.Find("Level").GetComponentsInChildren<WaterVolume>(true);
            foreach (var waterVolume in waterVolumes)
            {
                foreach (var volumeRenderer in waterVolume.GetComponentsInChildren<MeshRenderer>(true))
                {
                    var streamRendererMaterial = volumeRenderer.material;
                    streamRendererMaterial.mainTexture = materialTextDict[streamRendererMaterial];
                    streamRendererMaterial.color = materialColourDict[streamRendererMaterial];
                    if (volumeRenderer.TryGetComponent(out WaterSurfaceMaterialController streamRendererController))
                    {
                        streamRendererController.ScrollY = 0.5f;
                        streamRendererController.Scale = 1f;
                        var controllerRenderer = AccessTools.Field(streamRendererController.GetType(), "renderer").GetValue(streamRendererController);
                        if (controllerRenderer != null)
                        {
                            var method = streamRendererController.GetType().GetMethod("ApplyProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            method.Invoke(streamRendererController, null);
                        }
                    }
                    else
                        volumeRenderer.material.SetVector("_ScrollSpeedAndScale", materialVectorDict[volumeRenderer.material]);
                }

                foreach (var volumeMaterialController in waterVolume.GetComponentsInChildren<WaterSurfaceMaterialController>(true))
                    volumeMaterialController.enabled = true;
                waterVolume.enabled = true;

                var volumeObject = waterVolume.gameObject;
                volumeObject.layer = LayerMask.NameToLayer("Water");

                foreach (var volumeCollider in waterVolume.GetComponentsInChildren<BoxCollider>(true))
                {
                    volumeCollider.enabled = true;
                    volumeCollider.isTrigger = true;
                }
            }

            try
            {
                var primaryWaterStreams = GameObject.Find("Level").GetComponentsInChildren<WaterSurfaceMaterialController>(true).First(a => a.transform.parent.name == "B_WaterfallsStreams (1)" || a.name == "WaterfallsStreams");
                if (primaryWaterStreams != null && primaryWaterStreams.TryGetComponent(out MeshCollider waterStreamCollider) && primaryWaterStreams.TryGetComponent(out MeshFilter waterStreamFilter) && primaryWaterStreams.TryGetComponent(out MeshRenderer waterStreamRenderer))
                {
                    waterStreamCollider.sharedMesh = waterStreamFilter.sharedMesh;
                    waterStreamCollider.enabled = false;
                    waterStreamCollider.gameObject.GetOrAddComponent<GorillaSurfaceOverride>().overrideIndex = 59;

                    var streamRendererMaterial = waterStreamRenderer.material;
                    streamRendererMaterial.mainTexture = materialTextDict[streamRendererMaterial];
                    streamRendererMaterial.color = materialColourDict[streamRendererMaterial];
                    if (waterStreamRenderer.TryGetComponent(out WaterSurfaceMaterialController streamRendererController))
                    {
                        streamRendererController.ScrollY = 0.5f;
                        streamRendererController.Scale = 1f;
                        var controllerRenderer = AccessTools.Field(streamRendererController.GetType(), "renderer").GetValue(streamRendererController);
                        if (controllerRenderer != null)
                        {
                            var method = streamRendererController.GetType().GetMethod("ApplyProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            method.Invoke(streamRendererController, null);
                        }
                    }
                    else
                        waterStreamRenderer.material.SetVector("_ScrollSpeedAndScale", materialVectorDict[waterStreamRenderer.material]);
                }

                if (primaryWaterSurface != null && primaryWaterSurface.TryGetComponent(out MeshRenderer waterSurfaceRenderer))
                {
                    var streamRendererMaterial = waterSurfaceRenderer.material;
                    streamRendererMaterial.mainTexture = materialTextDict[streamRendererMaterial];
                    streamRendererMaterial.color = materialColourDict[streamRendererMaterial];
                    if (waterSurfaceRenderer.TryGetComponent(out WaterSurfaceMaterialController streamRendererController))
                    {
                        streamRendererController.ScrollY = 0.5f;
                        streamRendererController.Scale = 1f;
                        var controllerRenderer = AccessTools.Field(streamRendererController.GetType(), "renderer").GetValue(streamRendererController);
                        if (controllerRenderer != null)
                        {
                            var method = streamRendererController.GetType().GetMethod("ApplyProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            method.Invoke(streamRendererController, null);
                        }
                    }
                    else
                        waterSurfaceRenderer.material.SetVector("_ScrollSpeedAndScale", materialVectorDict[waterSurfaceRenderer.material]);
                }
            }
            catch { }

            waterSurfaces.ForEach(a => a.gameObject.layer = LayerMask.NameToLayer("Water"));
            materialTextDict = new Dictionary<Material, Texture2D>();
            materialColourDict = new Dictionary<Material, Color>();
            materialVectorDict = new Dictionary<Material, Vector4>();
            waterSurfaces = new List<Collider>();
            audioDirectory.GetComponentsInChildren<AudioSource>(true).ToList().ForEach(a => a.mute = false);
        }

        public static async void DisableWater()
        {
            var waterVolumes = GameObject.Find("Level").GetComponentsInChildren<WaterVolume>(true);
            foreach (var waterVolume in waterVolumes)
            {
                foreach (var volumeRenderer in waterVolume.GetComponentsInChildren<MeshRenderer>(true))
                {
                    var streamRendererMaterial = volumeRenderer.material;
                    materialTextDict.AddOrUpdate(streamRendererMaterial, streamRendererMaterial.mainTexture as Texture2D);
                    streamRendererMaterial.mainTexture = Plugin.IceTex;
                    materialColourDict.AddOrUpdate(streamRendererMaterial, streamRendererMaterial.color);
                    streamRendererMaterial.color = new Color(0.9f, 0.9f, 0.9f, waterAlpha);
                    if (volumeRenderer.TryGetComponent(out WaterSurfaceMaterialController streamRendererController))
                    {
                        streamRendererController.ScrollY = 0f;
                        streamRendererController.Scale = waterScale;
                        var controllerRenderer = AccessTools.Field(streamRendererController.GetType(), "renderer").GetValue(streamRendererController);
                        if (controllerRenderer != null)
                        {
                            var method = streamRendererController.GetType().GetMethod("ApplyProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            method.Invoke(streamRendererController, null);
                        }
                    }
                    else
                    {
                        materialVectorDict.Add(volumeRenderer.material, volumeRenderer.material.GetVector("_ScrollSpeedAndScale"));
                        volumeRenderer.material.SetVector("_ScrollSpeedAndScale", Vector4.zero.WithZ(waterScale));
                    }
                }
                foreach (var volumeMaterialController in waterVolume.GetComponentsInChildren<WaterSurfaceMaterialController>(true))
                    volumeMaterialController.enabled = false;
                waterVolume.enabled = false;

                var volumeObject = waterVolume.gameObject;
                volumeObject.layer = 0;

                foreach (var volumeCollider in waterVolume.GetComponentsInChildren<BoxCollider>(true))
                {
                    volumeCollider.enabled = false;
                    volumeCollider.isTrigger = false;
                    if (waterVolume.name == "OceanWater" || waterVolume.name == "CaveWaterVolume")
                    {
                        volumeCollider.enabled = true;
                        waterSurfaces.Add(volumeCollider);
                        volumeCollider.gameObject.GetOrAddComponent<GorillaSurfaceOverride>().overrideIndex = 59;
                    }
                }
            }

            try
            {
                var primaryWaterStreams = GameObject.Find("Level").GetComponentsInChildren<WaterSurfaceMaterialController>(true).First(a => a.transform.parent.name == "B_WaterfallsStreams (1)" || a.name == "WaterfallsStreams");
                if (primaryWaterStreams != null && primaryWaterStreams.TryGetComponent(out MeshCollider waterStreamCollider) && primaryWaterStreams.TryGetComponent(out MeshFilter waterStreamFilter) && primaryWaterStreams.TryGetComponent(out MeshRenderer waterStreamRenderer))
                {
                    waterStreamCollider.sharedMesh = waterStreamFilter.sharedMesh;
                    waterStreamCollider.enabled = true;
                    waterStreamCollider.gameObject.GetOrAddComponent<GorillaSurfaceOverride>().overrideIndex = 59;
                    waterSurfaces.Add(waterStreamCollider);

                    var streamRendererMaterial = waterStreamRenderer.material;
                    materialTextDict.AddOrUpdate(streamRendererMaterial, streamRendererMaterial.mainTexture as Texture2D);
                    streamRendererMaterial.mainTexture = Plugin.IceTex;
                    materialColourDict.AddOrUpdate(streamRendererMaterial, streamRendererMaterial.color);
                    streamRendererMaterial.color = new Color(0.9f, 0.9f, 0.9f, waterAlpha);
                    if (waterStreamRenderer.TryGetComponent(out WaterSurfaceMaterialController streamRendererController))
                    {
                        streamRendererController.ScrollY = 0f;
                        streamRendererController.Scale = waterScale;
                        var controllerRenderer = AccessTools.Field(streamRendererController.GetType(), "renderer").GetValue(streamRendererController);
                        if (controllerRenderer != null)
                        {
                            var method = streamRendererController.GetType().GetMethod("ApplyProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            method.Invoke(streamRendererController, null);
                        }
                    }
                    else
                    {
                        materialVectorDict.Add(waterStreamRenderer.material, waterStreamRenderer.material.GetVector("_ScrollSpeedAndScale"));
                        waterStreamRenderer.material.SetVector("_ScrollSpeedAndScale", Vector4.zero.WithZ(waterScale));
                    }
                }

                if (primaryWaterSurface != null && primaryWaterSurface.TryGetComponent(out MeshRenderer waterSurfaceRenderer))
                {
                    var streamRendererMaterial = waterSurfaceRenderer.material;
                    materialTextDict.AddOrUpdate(streamRendererMaterial, streamRendererMaterial.mainTexture as Texture2D);
                    streamRendererMaterial.mainTexture = Plugin.IceTex;
                    streamRendererMaterial.mainTextureScale *= 0.7f;
                    materialColourDict.AddOrUpdate(streamRendererMaterial, streamRendererMaterial.color);
                    streamRendererMaterial.color = new Color(0.9f, 0.9f, 0.9f, waterAlpha);
                    if (waterSurfaceRenderer.TryGetComponent(out WaterSurfaceMaterialController streamRendererController))
                    {
                        streamRendererController.ScrollY = 0f;
                        streamRendererController.Scale = waterScale;
                        var controllerRenderer = AccessTools.Field(streamRendererController.GetType(), "renderer").GetValue(streamRendererController);
                        if (controllerRenderer != null)
                        {
                            var method = streamRendererController.GetType().GetMethod("ApplyProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            method.Invoke(streamRendererController, null);
                        }
                    }
                    else
                    {
                        materialVectorDict.Add(waterSurfaceRenderer.material, waterSurfaceRenderer.material.GetVector("_ScrollSpeedAndScale"));
                        waterSurfaceRenderer.material.SetVector("_ScrollSpeedAndScale", Vector4.zero.WithZ(waterScale));
                    }
                }
            }
            catch { }

            PhysicMaterial physicMaterial = new PhysicMaterial("Slippery")
            {
                staticFriction = 0f,
                dynamicFriction = 0f,
                frictionCombine = PhysicMaterialCombine.Minimum,
                bounceCombine = PhysicMaterialCombine.Minimum
            };
            waterSurfaces.ForEach(a =>
            {
                a.gameObject.layer = 0;
                a.material = physicMaterial;
            });
            audioDirectory.GetComponentsInChildren<AudioSource>(true).ToList().ForEach(a => a.mute = true);

            await Task.Delay(400);
            var bodyColliderBounds = Player.Instance.bodyCollider.bounds;
            var boxColliderArray = waterSurfaces.Where(a => a.GetComponent<BoxCollider>() != null).ToArray();
            foreach(var genericCollider in boxColliderArray)
            {
                var boxCollider = genericCollider.GetComponent<BoxCollider>();
                if (bodyColliderBounds.Intersects(boxCollider.bounds))
                {
                    PlayerUtils.Teleport(WaterPatches.LastPosition);
                    break;
                }
            }
        }
    }
}
