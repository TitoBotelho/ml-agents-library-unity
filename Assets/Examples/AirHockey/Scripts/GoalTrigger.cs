using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoalTrigger : MonoBehaviour
{
    public AirHockeyEnvController env;
    public enum Side { Blue, Red }
    public Side side = Side.Red;

    [Tooltip("Se verdadeiro, apenas o puck dispara o gol (tag 'puck').")]
    public bool onlyPuck = true;

    [Tooltip("Tempo mínimo entre ativações para evitar múltiplos triggers.")]
    public float cooldown = 0.2f;

    float _lastTriggerTime = -999f;
    Collider _col;

    void Awake()
    {
        _col = GetComponent<Collider>();
        if (_col && !_col.isTrigger)
        {
            Debug.LogWarning($"[GoalTrigger] Collider em {name} não está como Trigger. Marcando isTrigger=true.");
            _col.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (onlyPuck && !other.CompareTag("puck")) return;
        if (Time.time - _lastTriggerTime < cooldown) return;
        _lastTriggerTime = Time.time;

        if (!env)
        {
            env = FindObjectOfType<AirHockeyEnvController>();
            if (!env)
            {
                Debug.LogWarning("[GoalTrigger] AirHockeyEnvController não encontrado na cena.");
                return;
            }
        }

        if (side == Side.Blue) env.GoalScoredBlue();
        else env.GoalScoredRed();
    }
}
