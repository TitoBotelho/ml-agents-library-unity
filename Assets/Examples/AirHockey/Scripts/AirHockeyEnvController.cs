using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class AirHockeyEnvController : MonoBehaviour
{
    [Header("References")]
    public AgentAirHockey agentBlue;
    public AgentAirHockey agentRed;
    public Rigidbody puckRb;

    [Header("Spawn Points (optional)")]
    public Transform blueSpawn;
    public Transform redSpawn;
    public Transform puckSpawn;

    [Header("Rewards")]
    public float goalReward = 1f;
    public float concedePenalty = -1f;

    [Header("Half-field clamp (XZ)")]
    public bool clampAgentsToHalf = true;
    public Vector2 blueMinXZ = new Vector2(-5f, -10f); // (minX, minZ)
    public Vector2 blueMaxXZ = new Vector2(5f, 0f);    // (maxX, maxZ)
    public Vector2 redMinXZ = new Vector2(-5f, 0f);
    public Vector2 redMaxXZ = new Vector2(5f, 10f);

    Vector3 _blueStart;
    Vector3 _redStart;
    Vector3 _puckStart;

    void Start()
    {
        // Determina posições iniciais caso os spawns não estejam definidos
        _blueStart = blueSpawn ? blueSpawn.position : (agentBlue ? agentBlue.transform.position : Vector3.zero);
        _redStart = redSpawn ? redSpawn.position : (agentRed ? agentRed.transform.position : Vector3.zero);
        _puckStart = puckSpawn ? puckSpawn.position : (puckRb ? puckRb.transform.position : Vector3.zero);

        ResetEnvironment();
    }

    void FixedUpdate()
    {
        if (!clampAgentsToHalf) return;
        if (agentBlue)
        {
            ClampAgentToBounds(agentBlue, blueMinXZ, blueMaxXZ);
        }
        if (agentRed)
        {
            ClampAgentToBounds(agentRed, redMinXZ, redMaxXZ);
        }
    }

    void ClampAgentToBounds(AgentAirHockey ag, Vector2 minXZ, Vector2 maxXZ)
    {
        var t = ag.transform;
        var rb = ag.GetComponent<Rigidbody>();
        var p = t.position;
        float clampedX = Mathf.Clamp(p.x, minXZ.x, maxXZ.x);
        float clampedZ = Mathf.Clamp(p.z, minXZ.y, maxXZ.y);

        // Se bateu no limite, anula a componente da velocidade que empurra para fora
        if (rb)
        {
            var v = rb.velocity;
            if (p.x <= minXZ.x && v.x < 0f) v.x = 0f;
            if (p.x >= maxXZ.x && v.x > 0f) v.x = 0f;
            if (p.z <= minXZ.y && v.z < 0f) v.z = 0f;
            if (p.z >= maxXZ.y && v.z > 0f) v.z = 0f;
            rb.velocity = v;
        }
        t.position = new Vector3(clampedX, p.y, clampedZ);
    }

    public void GoalScoredBlue()
    {
        // Blue marcou
        if (agentBlue) agentBlue.AddReward(goalReward);
        if (agentRed) agentRed.AddReward(concedePenalty);
        EndEpisodesAndReset();
    }

    public void GoalScoredRed()
    {
        // Red marcou
        if (agentRed) agentRed.AddReward(goalReward);
        if (agentBlue) agentBlue.AddReward(concedePenalty);
        EndEpisodesAndReset();
    }

    void EndEpisodesAndReset()
    {
        if (agentBlue) agentBlue.EndEpisode();
        if (agentRed) agentRed.EndEpisode();
        ResetEnvironment();
    }

    public void ResetEnvironment()
    {
        // Reseta puck
        if (puckRb)
        {
            puckRb.velocity = Vector3.zero;
            puckRb.angularVelocity = Vector3.zero;
            puckRb.transform.position = _puckStart;
        }
        // Reseta agentes
        if (agentBlue)
        {
            var rbB = agentBlue.GetComponent<Rigidbody>();
            if (rbB) { rbB.velocity = Vector3.zero; rbB.angularVelocity = Vector3.zero; }
            agentBlue.transform.position = _blueStart;
        }
        if (agentRed)
        {
            var rbP = agentRed.GetComponent<Rigidbody>();
            if (rbP) { rbP.velocity = Vector3.zero; rbP.angularVelocity = Vector3.zero; }
            agentRed.transform.position = _redStart;
        }
    }
}
