using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    [Header("Grid — match RoadNetwork")]
    public int   gridWidth  = 5;
    public int   gridHeight = 5;
    public float blockSize  = 20f;

    [Header("Challenges")]
    public float checkpointTimeLimit = 60f;
    public int   checkpointCount     = 6;
    public int   coinCount           = 10;
    public float coinTimeLimit       = 45f;

    enum Challenge { None, Checkpoints, Coins, Survival }
    Challenge current = Challenge.None;

    List<Checkpoint> active = new List<Checkpoint>();
    GameObject player;

    int  nextCheckpoint;
    int  coinsCollected;
    float timer;
    float survivalTimer;
    bool challengeRunning;

    string message    = "";
    string subMessage = "";
    string countdown  = "";
    float  countdownScale = 1f;

    void Start()
    {
        player = GameObject.Find("Car");
        StartCoroutine(RunChallenges());
    }

    IEnumerator RunChallenges()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            yield return RunCheckpointChallenge();
            yield return new WaitForSeconds(3f);
            yield return RunCoinChallenge();
            yield return new WaitForSeconds(3f);
            yield return RunSurvivalChallenge();
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator Countdown(string challengeName)
    {
        message    = challengeName;
        subMessage = "";

        foreach (string n in new[] { "3", "2", "1", "GO!" })
        {
            countdown      = n;
            countdownScale = 1.5f;
            float t = 0f;
            while (t < 0.9f)
            {
                t += Time.deltaTime;
                countdownScale = Mathf.Lerp(1.5f, 0.8f, t / 0.9f);
                yield return null;
            }
        }
        countdown = "";
    }

    IEnumerator TimeUp()
    {
        countdown      = "TIME'S UP!";
        countdownScale = 1.8f;
        float t = 0f;
        while (t < 2f)
        {
            t += Time.deltaTime;
            countdownScale = Mathf.Lerp(1.8f, 1.0f, t / 2f);
            yield return null;
        }
        countdown = "";
    }

    // ── Checkpoint Race ──────────────────────────────────────────────────────

    IEnumerator RunCheckpointChallenge()
    {
        yield return Countdown("CHECKPOINT RACE");

        current = Challenge.Checkpoints;
        nextCheckpoint = 0;
        challengeRunning = true;
        timer = checkpointTimeLimit;

        SpawnCheckpoints();
        HighlightNextCheckpoint();

        message    = "CHECKPOINT RACE";
        subMessage = $"Drive through all {checkpointCount} checkpoints!";

        while (challengeRunning)
        {
            timer -= Time.deltaTime;
            subMessage = $"Checkpoint {nextCheckpoint + 1}/{checkpointCount}  —  {timer:0}s";
            if (timer <= 0f) { yield return TimeUp(); Fail(""); break; }
            yield return null;
        }

        ClearActive();
    }

    void SpawnCheckpoints()
    {
        List<Vector3> intersections = AllIntersections();
        Shuffle(intersections);

        for (int i = 0; i < Mathf.Min(checkpointCount, intersections.Count); i++)
        {
            Vector3 pos = intersections[i] + Vector3.up * 1.5f;
            var cp = BuildRing(pos, i, false);
            active.Add(cp);
        }
    }

    void HighlightNextCheckpoint()
    {
        for (int i = 0; i < active.Count; i++)
            active[i].SetColor(i == nextCheckpoint ? Color.yellow : new Color(1f, 1f, 1f, 0.3f));
    }

    // ── Coin Collection ──────────────────────────────────────────────────────

    IEnumerator RunCoinChallenge()
    {
        yield return Countdown("COIN HUNT");

        current = Challenge.Coins;
        coinsCollected = 0;
        challengeRunning = true;
        timer = coinTimeLimit;

        SpawnCoins();

        message    = "COIN HUNT";
        subMessage = $"Collect all {coinCount} coins!";

        while (challengeRunning)
        {
            timer -= Time.deltaTime;
            subMessage = $"Coins: {coinsCollected}/{coinCount}  —  {timer:0}s";
            if (timer <= 0f) { yield return TimeUp(); Fail(""); break; }
            yield return null;
        }

        ClearActive();
    }

    void SpawnCoins()
    {
        List<Vector3> intersections = AllIntersections();
        Shuffle(intersections);

        for (int i = 0; i < Mathf.Min(coinCount, intersections.Count); i++)
        {
            Vector3 pos = intersections[i] + Vector3.up * 1.2f;
            active.Add(BuildCoin(pos, i));
        }
    }

    // ── Survival ─────────────────────────────────────────────────────────────

    IEnumerator RunSurvivalChallenge()
    {
        yield return Countdown("SURVIVAL");
        current = Challenge.Survival;
        survivalTimer = 0f;
        challengeRunning = true;

        message    = "SURVIVAL";
        subMessage = "Don't hit the traffic cars! Survive as long as you can.";

        // attach collision listener to player
        var listener = player.GetComponent<CollisionListener>();
        if (listener == null) listener = player.AddComponent<CollisionListener>();
        listener.onHitTraffic = () => { if (challengeRunning) EndSurvival(); };

        while (challengeRunning)
        {
            survivalTimer += Time.deltaTime;
            subMessage = $"Survived: {survivalTimer:0}s — avoid the traffic!";
            yield return null;
        }

        Destroy(player.GetComponent<CollisionListener>());
    }

    void EndSurvival()
    {
        challengeRunning = false;
        message    = $"SURVIVAL ENDED";
        subMessage = $"You lasted {survivalTimer:0} seconds!";
    }

    // ── Shared helpers ────────────────────────────────────────────────────────

    public void OnCheckpointHit(Checkpoint cp, GameObject car)
    {
        if (car != player) return;

        if (current == Challenge.Checkpoints && cp.index == nextCheckpoint)
        {
            nextCheckpoint++;
            if (nextCheckpoint >= checkpointCount)
            {
                Win($"Race complete in {checkpointTimeLimit - timer:0.0}s!");
            }
            else
            {
                HighlightNextCheckpoint();
            }
        }
        else if (current == Challenge.Coins && cp.isCoin)
        {
            coinsCollected++;
            Destroy(cp.gameObject);
            active.Remove(cp);
            if (coinsCollected >= coinCount)
                Win($"All coins collected in {coinTimeLimit - timer:0.0}s!");
        }
    }

    void Win(string msg)
    {
        challengeRunning = false;
        message    = "✓ COMPLETE!";
        subMessage = msg;
    }

    void Fail(string msg)
    {
        challengeRunning = false;
        message    = "✗ FAILED";
        subMessage = msg;
    }

    void ClearActive()
    {
        foreach (var cp in active)
            if (cp != null) Destroy(cp.gameObject);
        active.Clear();
    }

    List<Vector3> AllIntersections()
    {
        float totalW = gridWidth  * blockSize;
        float totalH = gridHeight * blockSize;
        var list = new List<Vector3>();
        for (int row = 0; row <= gridHeight; row++)
            for (int col = 0; col <= gridWidth; col++)
                list.Add(new Vector3(
                    col * blockSize - totalW / 2f, 0f,
                    row * blockSize - totalH / 2f));
        return list;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // ── Object builders ───────────────────────────────────────────────────────

    Checkpoint BuildRing(Vector3 pos, int index, bool isCoin)
    {
        var root = new GameObject($"Checkpoint_{index}");
        root.transform.position = pos;

        // outer ring
        for (int i = 0; i < 12; i++)
        {
            float angle = i / 12f * Mathf.PI * 2f;
            var seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seg.transform.SetParent(root.transform);
            seg.transform.localPosition = new Vector3(Mathf.Cos(angle) * 2f, Mathf.Sin(angle) * 2f, 0f);
            seg.transform.localScale    = new Vector3(0.3f, 0.3f, 0.3f);
            Destroy(seg.GetComponent<Collider>());
        }

        var trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius    = 2.5f;

        var cp = root.AddComponent<Checkpoint>();
        cp.index  = index;
        cp.isCoin = isCoin;
        cp.SetColor(Color.yellow);

        return cp;
    }

    Checkpoint BuildCoin(Vector3 pos, int index)
    {
        var root = new GameObject($"Coin_{index}");
        root.transform.position = pos;

        var body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        body.transform.SetParent(root.transform);
        body.transform.localScale    = new Vector3(0.8f, 0.1f, 0.8f);
        body.transform.localPosition = Vector3.zero;
        body.GetComponent<Renderer>().material.color = new Color(1f, 0.8f, 0f);
        Destroy(body.GetComponent<Collider>());

        var trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius    = 1.2f;

        var cp = root.AddComponent<Checkpoint>();
        cp.index  = index;
        cp.isCoin = true;
        cp.SetColor(new Color(1f, 0.8f, 0f));

        return cp;
    }

    // ── HUD ───────────────────────────────────────────────────────────────────

    void OnGUI()
    {
        float w = 500f;
        float cx = Screen.width / 2f - w / 2f;

        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter };

        // challenge name + progress
        style.fontSize  = 22;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(cx, 16f, w, 36f), message, style);

        style.fontSize  = 16;
        style.fontStyle = FontStyle.Normal;
        GUI.Label(new Rect(cx, 52f, w, 28f), subMessage, style);

        // big centred countdown / TIME'S UP
        if (countdown != "")
        {
            bool isTimeUp = countdown == "TIME'S UP!";
            int fontSize  = Mathf.RoundToInt((isTimeUp ? 64f : 100f) * countdownScale);
            style.fontSize  = fontSize;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = isTimeUp ? Color.red : Color.yellow;

            float lh = fontSize + 10f;
            GUI.Label(new Rect(cx, Screen.height / 2f - lh / 2f, w, lh), countdown, style);
        }
    }
}
