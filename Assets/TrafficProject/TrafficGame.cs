using System;
using System.Collections.Generic;
using UnityEngine;

// The whole demonstration uses Unity primitives so it is easy to inspect and change.
public class TrafficGame : MonoBehaviour
{
    class Car
    {
        public GameObject body;
        public int direction;
        public float distance;
        public float wait;
    }

    readonly List<Car> cars = new List<Car>();
    readonly float[,,] q = new float[3, 3, 2];
    readonly System.Random random = new System.Random(42);
    Material road, grass, white, yellow, red, green, dark, blue, orange;
    GameObject[] lamps = new GameObject[4];
    int phase, passed, switches;
    float timer, spawnTimer, decisionTimer, totalWait;
    const float DecisionSeconds = 7f;

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
        Train();
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
        Vector3[] poles = { new Vector3(-7, 1.6f, -7), new Vector3(7, 1.6f, 7),
                            new Vector3(-7, 1.6f, 7), new Vector3(7, 1.6f, -7) };
        for (int i = 0; i < 4; i++)
        {
            Box("Signal post", new Vector3(poles[i].x, .8f, poles[i].z), new Vector3(.25f, 1.6f, .25f), dark);
            Box("Signal housing", poles[i], new Vector3(.65f, 1.2f, .65f), dark);
            lamps[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lamps[i].name = "Signal light";
            lamps[i].transform.position = poles[i] + Vector3.up * .28f;
            lamps[i].transform.localScale = Vector3.one * .48f;
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
        UpdateLamps();
    }

    void Train()
    {
        // Small tabular Q-learning exercise: state = low/medium/high queue for each road.
        // Action 0 gives north/south green; action 1 gives east/west green.
        const float alpha = .18f, gamma = .85f;
        for (int episode = 0; episode < 4000; episode++)
        {
            int ns = random.Next(3), ew = random.Next(3), current = random.Next(2);
            for (int step = 0; step < 24; step++)
            {
                int action = random.NextDouble() < .18 ? random.Next(2) : Best(ns, ew);
                int nextNs = Mathf.Clamp(ns + random.Next(2) - (action == 0 ? 1 : 0), 0, 2);
                int nextEw = Mathf.Clamp(ew + random.Next(2) - (action == 1 ? 1 : 0), 0, 2);
                float reward = -(nextNs + nextEw) - (action == current ? 0 : .3f);
                q[ns, ew, action] += alpha * (reward + gamma * Mathf.Max(q[nextNs, nextEw, 0], q[nextNs, nextEw, 1]) - q[ns, ew, action]);
                ns = nextNs; ew = nextEw; current = action;
            }
        }
    }

    int Best(int ns, int ew) { return q[ns, ew, 0] >= q[ns, ew, 1] ? 0 : 1; }
    int Bucket(int count) { return count < 2 ? 0 : count < 5 ? 1 : 2; }

    void Update()
    {
        float dt = Time.deltaTime;
        timer += dt; spawnTimer += dt; decisionTimer += dt;
        if (spawnTimer >= 1.7f)
        {
            spawnTimer = 0;
            Spawn(random.Next(4));
        }
        int ns = 0, ew = 0;
        foreach (Car c in cars) if (c.distance > 7 && c.distance < 22) { if (c.direction < 2) ns++; else ew++; }
        if (decisionTimer >= DecisionSeconds)
        {
            decisionTimer = 0;
            int next = Best(Bucket(ns), Bucket(ew));
            if (next != phase) { phase = next; switches++; UpdateLamps(); }
        }
        for (int i = cars.Count - 1; i >= 0; i--)
        {
            Car c = cars[i];
            bool greenNow = (c.direction < 2 ? 0 : 1) == phase;
            bool stop = !greenNow && c.distance > 7.5f && c.distance < 11.5f;
            float speed = stop ? 0 : 5.8f;
            // Keep a little spacing between cars in the same lane.
            foreach (Car other in cars)
                if (other != c && other.direction == c.direction && other.distance < c.distance && c.distance - other.distance < 3.3f)
                    speed = Mathf.Min(speed, Mathf.Max(0, (c.distance - other.distance - 2.5f) * 2));
            c.distance -= speed * dt;
            if (speed < .2f) { c.wait += dt; totalWait += dt; }
            PositionCar(c);
            if (c.distance < -38) { Destroy(c.body); cars.RemoveAt(i); passed++; }
        }
    }

    void Spawn(int direction)
    {
        Car c = new Car { direction = direction, distance = 37 };
        c.body = Box("Car " + (direction < 2 ? "NS" : "EW"), Vector3.zero,
            direction < 2 ? new Vector3(1.6f, .8f, 2.5f) : new Vector3(2.5f, .8f, 1.6f),
            direction % 2 == 0 ? blue : orange);
        PositionCar(c);
        cars.Add(c);
    }

    void PositionCar(Car c)
    {
        Vector3 p = c.direction == 0 ? new Vector3(-3, .55f, c.distance) :
                    c.direction == 1 ? new Vector3(3, .55f, -c.distance) :
                    c.direction == 2 ? new Vector3(c.distance, .55f, 3) : new Vector3(-c.distance, .55f, -3);
        c.body.transform.position = p;
    }

    void UpdateLamps()
    {
        for (int i = 0; i < lamps.Length; i++)
            lamps[i].GetComponent<Renderer>().material = (i < 2 ? 0 : 1) == phase ? green : red;
    }
}
