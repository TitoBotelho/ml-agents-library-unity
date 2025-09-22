using System;
using Unity.MLAgents.Sensors;

namespace Unity.MLAgentsExamples
{
    /// <summary>
    /// A simple example of a SensorComponent.
    /// This should be added to the same GameObject as the BasicController
    /// </summary>
    public class ChessSensorComponent : SensorComponent
    {
        public ChessController chessController;

        /// <summary>
        /// Creates a BasicSensor.
        /// </summary>
        /// <returns>Corresponding sensors.</returns>
        public override ISensor[] CreateSensors()
        {
            return new ISensor[] { new ChessSensor(chessController) };
        }
    }

    /// <summary>
    /// Simple Sensor implementation that uses a one-hot encoding of the Agent's
    /// position as the observation.
    /// </summary>
    public class ChessSensor : SensorBase
    {
        public ChessController chessController;

        public ChessSensor(ChessController controller)
        {
            chessController = controller;
        }

        /// <summary>
        /// Generate the observations for the sensor.
        /// In this case, the observations are all 0 except for a 1 at the position of the agent.
        /// </summary>
        /// <param name="output"></param>
        public override void WriteObservation(float[] output)
        {
            // One-hot encoding of the position
            Array.Clear(output, 0, output.Length);
            output[chessController.position] = 1;
        }

        /// <inheritdoc/>
        public override ObservationSpec GetObservationSpec()
        {
            return ObservationSpec.Vector(ChessController.k_Extents);
        }

        /// <inheritdoc/>
        public override string GetName()
        {
            return "Chess";
        }

    }
}
