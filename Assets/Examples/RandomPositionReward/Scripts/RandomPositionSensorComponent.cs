using System;
using Unity.MLAgents.Sensors;
using UnityEngine;

// RandomPositionSensorComponent: gera 3 canais one-hot (Agente, Big, Small)
// Observação total = gridSizeX * gridSizeY * 3
public class RandomPositionSensorComponent : SensorComponent
{
    [Header("Grid")] public int gridSizeX = 8; public int gridSizeY = 8; public float cellSize = 1f; public Vector2 origin = Vector2.zero;
    [Header("Refs")] public Transform agentTransform; public Transform bigRewardTransform; public Transform smallRewardTransform;

    public override ISensor[] CreateSensors() => new ISensor[]{ new RandomPositionSensor(this) };

    int IndexOf(Transform t)
    {
        if (t == null) return -1;
        var local = new Vector2(t.position.x - origin.x, t.position.z - origin.y);
        int gx = Mathf.FloorToInt(local.x / Mathf.Max(0.0001f, cellSize));
        int gy = Mathf.FloorToInt(local.y / Mathf.Max(0.0001f, cellSize));
        if (gx < 0 || gy < 0 || gx >= gridSizeX || gy >= gridSizeY) return -1;
        return gy * gridSizeX + gx;
    }

    class RandomPositionSensor : ISensor
    {
        readonly RandomPositionSensorComponent c; readonly int cells; readonly int obs; readonly ObservationSpec spec;
        public RandomPositionSensor(RandomPositionSensorComponent comp){ c=comp; cells = Mathf.Max(1, comp.gridSizeX) * Mathf.Max(1, comp.gridSizeY); obs = cells * 3; spec = ObservationSpec.Vector(obs); }
        public ObservationSpec GetObservationSpec() => spec;
        public string GetName() => "RandomPosition3xOneHot";
        public int Write(ObservationWriter writer)
        {
            for(int i=0;i<obs;i++) writer[i]=0f;
            if (c == null) return obs;
            Mark(writer,0, c.IndexOf(c.agentTransform));
            Mark(writer,cells, c.IndexOf(c.bigRewardTransform));
            Mark(writer,cells*2, c.IndexOf(c.smallRewardTransform));
            return obs;
        }
        void Mark(ObservationWriter w,int baseOffset,int idx){ if ((uint)idx < (uint)cells){ int p=baseOffset+idx; if ((uint)p < (uint)obs) w[p]=1f; } }
        public byte[] GetCompressedObservation()=>null;
        public CompressionSpec GetCompressionSpec()=>CompressionSpec.Default();
        public void Update(){}
        public void Reset(){}
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        for (int y=0; y<gridSizeY; y++)
        for (int x=0; x<gridSizeX; x++)
        {
            var center = new Vector3(origin.x + x*cellSize, 0f, origin.y + y*cellSize);
            Gizmos.DrawWireCube(center, new Vector3(cellSize,0.01f,cellSize));
        }
    }
#endif
}
