using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSnapExtension : Cinemachine.CinemachineExtension
{
    [SerializeField] private float pixelSize = 8f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam, 
        CinemachineCore.Stage stage, 
        ref CameraState state, 
        float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {
            Vector3 pos = state.FinalPosition;
            float orthographicSize = state.Lens.OrthographicSize;
            float snapUnit = orthographicSize * 2 / Screen.height * pixelSize;
            pos.x = Mathf.Round(pos.x / snapUnit) * snapUnit;
            pos.y = Mathf.Round(pos.y / snapUnit) * snapUnit;
            state.PositionCorrection += pos - state.FinalPosition;

            //Debug.Log($"snapUnit: {snapUnit}, orthoSize: {orthographicSize}, screenHeight: {Screen.height}");
        }
    }
}
