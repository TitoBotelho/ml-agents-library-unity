using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using UnityEngine.Serialization;

public class RandomPositionController : MonoBehaviour
{
    public float timeBetweenDecisionsAtInference;
    float m_TimeSinceDecision;
    [HideInInspector] public int positionX;
    [HideInInspector] public int positionZ;

    [Header("Goal Positions (fixos)")]
    [SerializeField] int smallX = 0;
    [SerializeField] int smallZ = 0;
    [SerializeField] int largeX = 7;
    [SerializeField] int largeZ = 7;

    public GameObject largeGoal;
    public GameObject smallGoal;
    const int k_MinPosition = 0;
    const int k_MaxPosition = 7;
    public const int k_Extents = k_MaxPosition - k_MinPosition;

    Agent m_Agent;

    public void Awake() { }

    public void OnEnable()
    {
        m_Agent = GetComponent<Agent>();
        // Posição inicial aleatória no grid 8x8 (0..7)
        positionX = UnityEngine.Random.Range(k_MinPosition, k_MaxPosition + 1);
        positionZ = UnityEngine.Random.Range(k_MinPosition, k_MaxPosition + 1);
        transform.position = new Vector3(positionX, 0f, positionZ);

        // Randomizar metas dentro do limite, evitando sobreposição com o agente e entre si
        do
        {
            smallX = UnityEngine.Random.Range(k_MinPosition, k_MaxPosition + 1);
            smallZ = UnityEngine.Random.Range(k_MinPosition, k_MaxPosition + 1);
        } while (smallX == positionX && smallZ == positionZ);

        do
        {
            largeX = UnityEngine.Random.Range(k_MinPosition, k_MaxPosition + 1);
            largeZ = UnityEngine.Random.Range(k_MinPosition, k_MaxPosition + 1);
        } while ((largeX == smallX && largeZ == smallZ) || (largeX == positionX && largeZ == positionZ));

        smallGoal.transform.position = new Vector3(smallX, 0f, smallZ);
        largeGoal.transform.position = new Vector3(largeX, 0f, largeZ);
    }

    public void MoveDirection(int direction)
    {
        Move2D(direction, 0);
    }

    public void Move2D(int dx, int dz)
    {
        positionX += dx;
        positionZ += dz;
        if (positionX < k_MinPosition) positionX = k_MinPosition;
        if (positionX > k_MaxPosition) positionX = k_MaxPosition;
        if (positionZ < k_MinPosition) positionZ = k_MinPosition;
        if (positionZ > k_MaxPosition) positionZ = k_MaxPosition;
        transform.position = new Vector3(positionX, 0f, positionZ);
        m_Agent.AddReward(-0.01f);
        CheckGoals();
    }

    void CheckGoals()
    {
        bool onSmall = positionX == smallX && positionZ == smallZ;
        bool onLarge = positionX == largeX && positionZ == largeZ;
        if (onSmall)
        {
            m_Agent.AddReward(0.1f);
            m_Agent.EndEpisode();
            ResetAgent();
        }
        else if (onLarge)
        {
            m_Agent.AddReward(1f);
            m_Agent.EndEpisode();
            ResetAgent();
        }
    }

    public void ResetAgent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        m_Agent = null;
    }

    public void FixedUpdate()
    {
        WaitTimeInference();
    }

    void WaitTimeInference()
    {
        if (m_Agent == null)
        {
            return;
        }
        if (Academy.Instance.IsCommunicatorOn)
        {
            m_Agent?.RequestDecision();
        }
        else
        {
            if (m_TimeSinceDecision >= timeBetweenDecisionsAtInference)
            {
                m_TimeSinceDecision = 0f;
                m_Agent?.RequestDecision();
            }
            else
            {
                m_TimeSinceDecision += Time.fixedDeltaTime;
            }
        }
    }
}
