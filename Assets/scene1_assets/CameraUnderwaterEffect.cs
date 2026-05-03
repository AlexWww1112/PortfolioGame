using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraUnderwaterEffect : MonoBehaviour
{
    public LayerMask waterLayers;
    public Shader shader;

    [Header("Depth Effect")]
    public Color depthColor = new Color(0, 0.42f, 0.87f);
    public float depthStart = -12, depthEnd = 98;
    public LayerMask depthLayers = ~0; // All layers selected by default

    private Camera cam, depthCam;
    private RenderTexture depthTexture, colourTexture;
    private Material material;
    private bool inWater;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (shader)
        {
            material = new Material(shader);
        }

        depthTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16, RenderTextureFormat.Depth);
        colourTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.Default);

        GameObject go = new GameObject("Depth Cam");
        depthCam = go.AddComponent<Camera>();
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        depthCam.CopyFrom(cam);
        depthCam.cullingMask = depthLayers;
        depthCam.depthTextureMode = DepthTextureMode.Depth;
        depthCam.SetTargetBuffers(colourTexture.colorBuffer, depthTexture.depthBuffer);
        depthCam.enabled = false;
    }

    private void OnApplicationQuit()
    {
        if (depthTexture) depthTexture.Release();
        if (colourTexture) colourTexture.Release();
    }

    private void FixedUpdate()
    {
        Vector3[] corners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, cam.stereoActiveEye, corners);

        RaycastHit hit;
        Vector3 start = transform.position + transform.TransformVector(corners[1]);
        Vector3 end = transform.position + transform.TransformVector(corners[0]);

        Collider[] c = Physics.OverlapSphere(end, 0.01f, waterLayers);
        if (c.Length > 0)
        {
            inWater = true;

            c = Physics.OverlapSphere(start, 0.01f, waterLayers);
            if (c.Length > 0)
            {
                material.SetVector("_WaterLevel", new Vector2(0, 1));
            }
            else if (Physics.Linecast(start, end, out hit, waterLayers))
            {
                float delta = hit.distance / (end - start).magnitude;
                material.SetVector("_WaterLevel", new Vector2(0, 1 - delta));
            }
        }
        else
        {
            inWater = false;
        }
    }

    private void Reset()
    {
        Shader[] shaders = Resources.FindObjectsOfTypeAll<Shader>();
        foreach (Shader s in shaders)
        {
            if (s.name.Contains(this.GetType().Name))
            {
                shader = s;
                return;
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material && inWater)
        {
            depthCam.Render();

            // Update shader properties every frame
            material.SetColor("_DepthColor", depthColor);
            material.SetFloat("_DepthStart", depthStart);
            material.SetFloat("_DepthEnd", depthEnd);
            material.SetTexture("_DepthMap", depthTexture);

            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
