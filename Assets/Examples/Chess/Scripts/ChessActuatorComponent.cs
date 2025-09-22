using System;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Unity.MLAgentsExamples
{
    /// <summary>
    /// A simple example of a ActuatorComponent.
    /// This should be added to the same GameObject as the ChessController
    /// </summary>
    public class ChessActuatorComponent : ActuatorComponent
    {
        public ChessController chessController;
        ActionSpec m_ActionSpec = ActionSpec.MakeDiscrete(3);

        /// <summary>
        /// Creates a BasicActuator.
        /// </summary>
        /// <returns>Corresponding actuators.</returns>
        public override IActuator[] CreateActuators()
        {
            return new IActuator[] { new ChessActuator(chessController) };
        }

        public override ActionSpec ActionSpec
        {
            get { return m_ActionSpec; }
        }
    }

    /// <summary>
    /// Simple actuator that converts the action into a {-1, 0, 1} direction
    /// </summary>
    public class ChessActuator : IActuator
    {
        public ChessController chessController;
        ActionSpec m_ActionSpec;

        public ChessActuator(ChessController controller)
        {
            chessController = controller;
            m_ActionSpec = ActionSpec.MakeDiscrete(3);
        }

        public ActionSpec ActionSpec
        {
            get { return m_ActionSpec; }
        }

        /// <inheritdoc/>
        public String Name
        {
            get { return "Chess"; }
        }

        public void ResetData()
        {

        }

        public void OnActionReceived(ActionBuffers actionBuffers)
        {
            var movement = actionBuffers.DiscreteActions[0];

            var direction = 0;

            switch (movement)
            {
                case 1:
                    direction = -1;
                    break;
                case 2:
                    direction = 1;
                    break;
            }

            chessController.MoveDirection(direction);
        }

        public void Heuristic(in ActionBuffers actionBuffersOut)
        {
            var direction = Input.GetAxis("Horizontal");
            var discreteActions = actionBuffersOut.DiscreteActions;
            if (Mathf.Approximately(direction, 0.0f))
            {
                discreteActions[0] = 0;
                return;
            }
            var sign = Math.Sign(direction);
            discreteActions[0] = sign < 0 ? 1 : 2;
        }

        public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {

        }

    }
}
