using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHockeyRewarder : MonoBehaviour
{
    [Tooltip("Recompensa por tocar no puck.")]
    public float touchReward = 0.05f;
    [Tooltip("Tempo mínimo entre recompensas para o mesmo agente.")]
    public float touchCooldown = 0.2f;
    [Tooltip("Escala opcional via EnvironmentParameters (puck_touch). Se 0 desativa.")]
    public bool useEnvParameterScale = true;

    // Registro de último toque por agente
    Dictionary<int, float> _lastTouchTime = new Dictionary<int, float>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision c)
    {
        // Verifica se quem tocou é um agente
        var agent = c.gameObject.GetComponent<AgentAirHockey>();
        if (agent == null) return;

        int id = agent.GetInstanceID();
        float now = Time.time;
        float last;
        if (_lastTouchTime.TryGetValue(id, out last))
        {
            if (now - last < touchCooldown) return; // cooldown ativo
        }

        _lastTouchTime[id] = now;

        float scale = 1f;
#if UNITY_MLAGENTS_PRESENT
        if (useEnvParameterScale)
        {
            // Se Academy estiver disponível, tenta usar parâmetro de ambiente opcional
            var envParams = Unity.MLAgents.Academy.Instance.EnvironmentParameters;
            scale = envParams.GetWithDefault("puck_touch", 1f);
        }
#endif
        agent.AddReward(touchReward * scale);
    }
}
