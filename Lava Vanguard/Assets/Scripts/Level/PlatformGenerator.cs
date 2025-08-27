using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PlatformGenerator : MonoBehaviour
{
    public static PlatformGenerator Instance { get; private set; }
    public Transform platformContainer;
    public GameObject platformPrefab;
    public GameObject longPlatformPrefab;
    public List<PlatformView[]> platforms = new List<PlatformView[]>();
    public Transform shopBackground;
    public PlatformView longPlatformRef = null;
    [HideInInspector]
    public bool canGenerate = true;

    private int layerIndex = 0;
    private static readonly int COL = 5;
    private static readonly float InitialY = -3;
    private static readonly float InitialX = (COL / 2) * (-5);
    private static readonly float IntervalY = 2;
    private static readonly float IntervalX = 5;

    private static bool boss02Platform = true;
    private void Awake()
    {
        Instance = this;
    }
    public void GenerateOneLayer(bool[] preset)
    {
        var layer = new PlatformView[COL];
        for (int i = 0; i < preset.Length; i++)
        {
            if (preset[i])
            {
                int from = 0;
                if (layerIndex != 0)
                {
                    List<int> pre = new List<int>();
                    if (platforms[^1][i] != null) pre.Add(0);
                    if (i > 0 && platforms[^1][i - 1] != null) pre.Add(1);
                    if (i < COL - 1 && platforms[^1][i + 1] != null) pre.Add(2);
                    if (pre[0] == 0) from = 0;
                    else from = pre[Random.Range(0, pre.Count - 1)]; 
                }

                layer[i] = CreatePlatform(i, from);
            }
        }
        //Special Case
        if (layer[2] != null) layer[2].SetBottomSize(new Vector2(3, 3.5f));
        if (platforms.Count == 1) layer[1].SetRightSize(new Vector2(3, 0.5f));
        platforms.Add(layer);
        layerIndex++;
    }
    public void GenerateOneLayer()
    {
        var lastLayer = platforms[^1];
        var currentLayer = new PlatformView[COL];
        int count = Random.Range(COL / 2 - COL / 4, COL / 2 + COL / 4 + 1);
        bool[] reach = new bool[COL];
        for (int i = 0; i < lastLayer.Length; i++)
        {
            if (lastLayer[i] != null)
            {
                reach[Mathf.Clamp(i - 1, 0, COL - 1)] = true;
                reach[i] = true;
                reach[Mathf.Clamp(i + 1, 0, COL - 1)] = true;
            }
        }

        if (platforms.Count >= 2)
            for (int i = 0; i < reach.Length; i++) 
            {
                if (platforms[^1][i] != null && platforms[^2][i] != null)
                    reach[i] = false;
            }



        List<int> trueIndices = new List<int>();

        for (int i = 0; i < reach.Length; i++)
        {
            if (reach[i])
            {
                trueIndices.Add(i);
            }
        }

        count = Mathf.Min(count, trueIndices.Count);
        trueIndices = trueIndices.OrderBy(x => Random.value).ToList();

        trueIndices = trueIndices.Take(count).ToList();

        for (int i = 0; i < trueIndices.Count; i++)
        {
            int from = 0;
            if (layerIndex != 0)
            {
                List<int> pre = new List<int>();
                var j = trueIndices[i];
                if (platforms[^1][j] != null) pre.Add(0);
                if (j > 0 && platforms[^1][j - 1] != null) pre.Add(1);
                if (j < COL - 1 && platforms[^1][j + 1] != null) pre.Add(2);
                if (pre[0] == 0) from = 0;
                else from = pre[Random.Range(0, pre.Count - 1)];
            }
            currentLayer[trueIndices[i]] = CreatePlatform(trueIndices[i], from);
        }
        //Special Case
        if (platforms.Count == 1)
        {
            if (currentLayer[1] != null)
                currentLayer[1].SetRightSize(new Vector2(3, 0.5f));
            if (currentLayer[3] != null)
                currentLayer[3].SetLeftSize(new Vector2(3, 0.5f));
        }
        platforms.Add(currentLayer);
        layerIndex++;
    }
    public void GenerateLongLayer()
    {
        var longPlatform = CreateLongPlatform();
        var layer = new PlatformView[COL];
        for (int i = 1; i < COL - 1; i++) 
        {
            layer[i] = longPlatform;
        }
        shopBackground.SetParent(longPlatform.transform, true);
        shopBackground.localPosition = new Vector2(0, -13.25f);
        shopBackground.gameObject.SetActive(true);
        platforms.Add(layer);
        layerIndex++;
    }
    private void RemoveOneLayer()
    {
        if (platforms.Count >= 8)
        {
            var layer0 = platforms[0];
            platforms.RemoveAt(0);
            for (int i = 0; i < layer0.Length; i++)
            {
                if (layer0[i] != null) 
                {
                    if (layer0[i] != null && layer0[i].gameObject != null)
                    {
                        Destroy(layer0[i].gameObject);
                    }
                }
            }
        }
    }

    public PlatformView CreatePlatform(int column, int from = 0)
    {
        // Small random offset of -0.5, 0, or 0.5 for variation
        float offsetX = new float[] { -0f, 0f, 0f }[Random.Range(0, 3)];
        float offsetY = new float[] { -0f, 0f, 0f }[Random.Range(0, 3)];
        Vector2 position = new Vector2(InitialX + column * IntervalX + offsetX, InitialY + layerIndex * IntervalY + offsetY);

        // Instantiate the platform and initialize it
        PlatformView platform = Instantiate(platformPrefab, platformContainer).GetComponent<PlatformView>();
        platform.Init(new Vector2(3, 0.5f), position, from);
        return platform;
    }

    public PlatformView CreateLongPlatform()
    {
        PlatformView platform = Instantiate(longPlatformPrefab, platformContainer).GetComponent<PlatformView>();
        platform.Init(new Vector2(13, 0.5f), new Vector2(0, InitialY + layerIndex * IntervalY), -1);
        platform.tag = "LongPlatform";
        longPlatformRef = platform;
        CameraController.Instance.UpdateDistance(platform.transform);
        return platform;
    }

    private float nextGenerateY; // The Y position where the next platform should be generated
    private Camera mainCamera;

    public void Init()
    {
        GenerateOneLayer(new bool[] { false, false, true, false, false });
    }
    private void Start()
    {
        mainCamera = Camera.main;
        //Init();
        nextGenerateY = mainCamera.transform.position.y + IntervalY;
    }
    public void StartGenerating()
    {
        int cnt = Tutorial.Instance.tutorial ? 1 : 5;
        for (int i = 0; i < cnt; i++)
        {
            GenerateOneLayer();
        }
    }
    private void Update()
    {
        if (Time.timeScale == 1 && mainCamera.transform.position.y >= nextGenerateY && canGenerate)  
        {
            if (LevelManager.Instance.genLongPlatform)
            {
                GenerateLongLayer();
                LevelManager.Instance.genLongPlatform = false;
                canGenerate = false;
                RemoveOneLayer();
            }
            else
            {
                if (LevelManager.Instance.wave == 19)
                {
                    if(boss02Platform == true)
                        GenerateOneLayer(new bool[] { true, false, true, false, true });
                    else
                        GenerateOneLayer(new bool[] { false, true, false, true, false });
                    boss02Platform = !boss02Platform;
                    RemoveOneLayer();
                }
                else
                {
                    GenerateOneLayer();
                    RemoveOneLayer();
                }
            }
            nextGenerateY += IntervalY; // Update the threshold for the next layer
        }
    }
    public Vector3 GetRevivePosition()
    {
        GenerateLongLayer();
        return platforms[^1][2].transform.position;
    }
}
