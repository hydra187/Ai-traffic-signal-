using System;
using System.Collections.Generic;
using UnityEngine;

// The whole demonstration uses Unity primitives so it is easy to inspect and change.
public class TrafficGame : MonoBehaviour
{
    Material road, grass, white, yellow, red, green, dark, blue, orange;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void StartGame()
    {
        if (FindFirstObjectByType<TrafficGame>() == null)
            new GameObject("Traffic Controller").AddComponent<TrafficGame>();
    }

    void Awake()
    {
        road = Mat(new Color(.15f, .18f, .2f));
        grass = Mat(new Color(.28f, .42f, .29f));
        white = Mat(new Color(.88f, .88f, .79f));
        yellow = Mat(new Color(.95f, .72f, .2f));
        red = Mat(new Color(.82f, .19f, .14f));
        green = Mat(new Color(.18f, .78f, .35f));
        dark = Mat(new Color(.09f, .1f, .12f));
        blue = Mat(new Color(.22f, .5f, .79f));
        orange = Mat(new Color(.94f, .49f, .22f));
        BuildScene();
    }

    Material Mat(Color color)
    {
        Material m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.color = color;
        return m;
    }

    GameObject Box(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = name;
        g.transform.position = position;
        g.transform.localScale = scale;
        g.GetComponent<Renderer>().material = material;
        return g;
    }

    void BuildScene()
    {
        Box("Ground", new Vector3(0, -.17f, 0), new Vector3(85, .2f, 85), grass);
        Box("North south road", Vector3.zero, new Vector3(12, .12f, 80), road);
        Box("East west road", new Vector3(0, .015f, 0), new Vector3(80, .12f, 12), road);
        for (int i = -36; i <= 36; i += 6)
        {
            if (Mathf.Abs(i) < 8) continue;
            Box("Lane marking", new Vector3(0, .085f, i), new Vector3(.18f, .02f, 2.8f), yellow);
            Box("Lane marking", new Vector3(i, .09f, 0), new Vector3(2.8f, .02f, .18f), yellow);
        }
        for (int side = -1; side <= 1; side += 2)
        {
            for (int j = -4; j <= 4; j += 2)
            {
                Box("Crosswalk", new Vector3(j, .1f, side * 8), new Vector3(1.1f, .02f, .6f), white);
                Box("Crosswalk", new Vector3(side * 8, .1f, j), new Vector3(.6f, .02f, 1.1f), white);
            }
        }
        for (int x = -1; x <= 1; x += 2)
            for (int z = -1; z <= 1; z += 2)
            {
                Box("Sidewalk block", new Vector3(x * 19, -.02f, z * 19), new Vector3(23, .25f, 23), white);
                for (int k = 0; k < 3; k++)
                    Box("Building", new Vector3(x * (13 + k * 5), 1.3f + k, z * (15 + k * 3)),
                        new Vector3(3, 2.6f + k * 2, 3), k % 2 == 0 ? blue : orange);
            }
        Camera cam = Camera.main;
        if (cam == null) cam = new GameObject("Main Camera").AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.transform.position = new Vector3(0, 38, -34);
        cam.transform.rotation = Quaternion.Euler(48, 0, 0);
        cam.fieldOfView = 55;
        cam.backgroundColor = new Color(.65f, .79f, .87f);
        Light light = FindFirstObjectByType<Light>();
        if (light == null) light = new GameObject("Sun").AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
        light.intensity = 1.4f;
    }
}
