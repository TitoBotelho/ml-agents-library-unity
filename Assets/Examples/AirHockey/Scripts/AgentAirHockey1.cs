using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentAirHockey1 : Agent
{
    [SerializeField] float speed = 5f;
    Rigidbody rb;
    DecisionRequester dr;

    [Header("Refs")]
    [SerializeField] Rigidbody puckRb; // arraste o Rigidbody do puck aqui

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        dr = GetComponent<DecisionRequester>();
    }

    void Update()
    {
        // Se não houver DecisionRequester, solicita decisões a cada frame
        if (dr == null || !dr.enabled)
        {
            RequestDecision();
        }
    }

    void ApplyInput(float inputX, float inputZ)
    {
        if (rb != null)
        {
            var v = rb.velocity;
            v.x = inputX * speed;
            v.z = inputZ * speed;
            rb.velocity = v;
        }
        else
        {
            var move = new Vector3(inputX, 0f, inputZ);
            transform.Translate(move * speed * Time.deltaTime, Space.World);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agente: posição XZ e velocidade XZ (4 floats)
        var posA = transform.position;
        var velA = rb ? rb.velocity : Vector3.zero;
        sensor.AddObservation(posA.x);
        sensor.AddObservation(posA.z);
        sensor.AddObservation(velA.x);
        sensor.AddObservation(velA.z);

        // Puck: posição XZ e velocidade XZ (4 floats)
        if (puckRb)
        {
            var posP = puckRb.position;
            var velP = puckRb.velocity;
            sensor.AddObservation(posP.x);
            sensor.AddObservation(posP.z);
            sensor.AddObservation(velP.x);
            sensor.AddObservation(velP.z);
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Branch 0: frente(1)/trás(2) -> eixo Z; Branch 1: direita(1)/esquerda(2) -> eixo X
        int aForwardBack = actions.DiscreteActions[0];
        int aRightLeft   = actions.DiscreteActions[1];

        float inputZ = 0f; // frente +, trás -
        if (aForwardBack == 1) inputZ = 1f;
        else if (aForwardBack == 2) inputZ = -1f;

        float inputX = 0f; // direita +, esquerda -
        if (aRightLeft == 1) inputX = 1f;
        else if (aRightLeft == 2) inputX = -1f;

        ApplyInput(inputX, inputZ);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var d = actionsOut.DiscreteActions;
        // Branch 0: frente/trás
        d[0] = 0; // neutro
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) d[0] = 1; // frente
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) d[0] = 2; // trás

        // Branch 1: direita/esquerda
        d[1] = 0; // neutro
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) d[1] = 1; // direita
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) d[1] = 2; // esquerda
    }
}
