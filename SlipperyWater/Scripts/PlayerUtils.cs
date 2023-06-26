using HarmonyLib;
using GorillaLocomotion;
using UnityEngine;

namespace SlipperyWater.Scripts
{
    public static class PlayerUtils
    {
        public static void Teleport(Vector3 point)
        {
            var localPlayer = Player.Instance;
            var playerRigibody = localPlayer.GetComponent<Rigidbody>();

            Vector3 fixedPoint = point - localPlayer.bodyCollider.transform.position + localPlayer.transform.position;
            localPlayer.transform.position = fixedPoint;

            AccessTools.Field(localPlayer.GetType(), "lastPosition").SetValue(localPlayer, fixedPoint);
            AccessTools.Field(localPlayer.GetType(), "velocityHistory").SetValue(localPlayer, new Vector3[localPlayer.velocityHistorySize]);

            localPlayer.headCollider.transform.position = fixedPoint;
            AccessTools.Field(localPlayer.GetType(), "lastHeadPosition").SetValue(localPlayer, fixedPoint);

            localPlayer.leftControllerTransform.position = fixedPoint;
            AccessTools.Field(localPlayer.GetType(), "lastLeftHandPosition").SetValue(localPlayer, fixedPoint);
            localPlayer.rightControllerTransform.position = fixedPoint;
            AccessTools.Field(localPlayer.GetType(), "lastRightHandPosition").SetValue(localPlayer, fixedPoint);
            localPlayer.bodyCollider.attachedRigidbody.transform.position = fixedPoint;

            playerRigibody.velocity = Vector3.zero;
            localPlayer.currentVelocity = Vector3.zero;
            AccessTools.Field(localPlayer.GetType(), "denormalizedVelocityAverage").SetValue(localPlayer, Vector3.zero);
        }
    }
}
