using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracingController : MonoBehaviour
{

    public ComputeShader _shader;

    private RenderTexture _target;

    public Texture2D _SkyboxTexture;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

    }

    private void SetShaderParameters()
    {
        _shader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        _shader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);

        _shader.SetTexture(0, "_SkyboxTexture", _SkyboxTexture);
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();

        Render(destination);

    }

    private void Render(RenderTexture destination)
    {
        InitRenderTexture();

        _shader.SetTexture(0, "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        _shader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(_target, destination);
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            if (_target != null)
                _target.Release();
            // Get a render target for Ray Tracing
            _target = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }


}
