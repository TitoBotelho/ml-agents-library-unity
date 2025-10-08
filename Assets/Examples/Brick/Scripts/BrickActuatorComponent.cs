using System;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Unity.MLAgentsExamples
{
    /// <summary>
    /// A simple example of a ActuatorComponent.
    /// This should be added to the same GameObject as the BrickController
    /// </summary>
    public class BrickActuatorComponent : ActuatorComponent
    {
        public BrickController brickController;
        ActionSpec m_ActionSpec = ActionSpec.MakeDiscrete(5); // 0=stay,1=left,2=right,3=forward,4=back

        /// <summary>
        /// Creates a BasicActuator.
        /// </summary>
        /// <returns>Corresponding actuators.</returns>
        public override IActuator[] CreateActuators()
        {
            return new IActuator[] { new BrickActuator(brickController) };
        }

        public override ActionSpec ActionSpec
        {
            get { return m_ActionSpec; }
        }
    }

    /// <summary>
    /// Simple actuator that converts the action into a {-1, 0, 1} direction
    /// </summary>
    public class BrickActuator : IActuator
    {
        public BrickController brickController;
        ActionSpec m_ActionSpec = ActionSpec.MakeDiscrete(5);

        public BrickActuator(BrickController controller)
        {
            brickController = controller;
        }

        public ActionSpec ActionSpec
        {
            get { return m_ActionSpec; }
        }

        /// <inheritdoc/>
        public String Name
        {
            get { return "Brick2D"; }
        }

        public void ResetData()
        {

        }

        public void OnActionReceived(ActionBuffers actionBuffers)
        {
            var a = actionBuffers.DiscreteActions[0];
            int dx = 0, dz = 0;
            switch (a)
            {
                case 1: dx = -1; break; // left
                case 2: dx = 1; break;  // right
                case 3: dz = 1; break;  // forward (+z)
                case 4: dz = -1; break; // back (-z)
            }
            brickController.Move2D(dx, dz);
        }

        public void Heuristic(in ActionBuffers actionBuffersOut)
        {
            var discrete = actionBuffersOut.DiscreteActions;
            discrete[0] = 0; // stay default
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) discrete[0] = 1;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) discrete[0] = 2;
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) discrete[0] = 3;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) discrete[0] = 4;
        }

        public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {

        }

    }
}
