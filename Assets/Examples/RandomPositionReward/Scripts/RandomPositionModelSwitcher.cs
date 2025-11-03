using UnityEngine;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using System.Collections;
using UnityEngine.UI; // Adicionado para manipular botões

namespace Unity.MLAgentsExamples
{
    public class RandomPositionModelSwitcher : MonoBehaviour
    {
        [Header("Referências")]
        public Agent agent; // Arraste o Agent aqui (ou deixe em branco para auto-detectar no mesmo GameObject)

        [Header("Modelos ONNX")]
        public NNModel model200; // ~200 iterações (quase sem treino)
        public NNModel model60k;
        public NNModel model140k;

        BehaviorParameters _behavior;

        // Persistência da seleção do modelo entre reinícios de episódio/cena
        enum ModelOption { Model200 = 0, Model60k = 1, Model140k = 2, HeuristicOnly = 3 }
        const string PlayerPrefsKey = "ModelSwitcher.Selection";
        static ModelOption s_selected = ModelOption.Model200;

        // Evitar reiniciar episódio durante Awake/entrada no Play
        bool _initialized;

        // ==== NOVO: Referências de UI e cores ====
        [Header("UI Botões (opcional)")]
        public Button button200;
        public Button button60k;
        public Button button140k;
        [Header("Cores de Estado")]
        public Color normalColor = new Color(0.18f,0.18f,0.18f,1f);
        public Color selectedColor = new Color(0.10f,0.45f,0.85f,1f);
        public Color hoverColor = new Color(0.25f,0.55f,0.90f,1f);
        [Tooltip("Aplicar cores via Image.color (desabilite se usar estilos próprios)")] public bool autoApplyColors = true;
        [Tooltip("Adicionar indicação visual inicial no botão 200 (ex: pulso)")] public bool animateFirstSelection = true;
        Coroutine pulseRoutine;
        // ==========================================

        void Awake()
        {
            if (agent == null)
            {
                agent = GetComponent<Agent>();
            }
            _behavior = agent != null ? agent.GetComponent<BehaviorParameters>() : GetComponent<BehaviorParameters>();

            // Carrega última seleção salva (PlayerPrefs)
            if (PlayerPrefs.HasKey(PlayerPrefsKey))
            {
                s_selected = (ModelOption)PlayerPrefs.GetInt(PlayerPrefsKey);
            }

            // Aplica sem reiniciar (o reinício ocorrerá depois do Start)
            ApplySelection(applyRestart:false);
            UpdateButtonVisual();
        }

        void Start()
        {
            _initialized = true;
            if (animateFirstSelection && s_selected == ModelOption.Model200)
            {
                TryStartPulse(button200);
            }
        }

        // Botão 1: "Sem treinamento" -> usa o modelo com ~200 iterações
        public void UseHeuristic()
        {
            s_selected = ModelOption.Model200;
            SaveSelection();
            ApplySelection();
            UpdateButtonVisual();
        }

        // Opcional: caso queira realmente usar HeuristicOnly
        public void UseHeuristicOnly()
        {
            s_selected = ModelOption.HeuristicOnly;
            SaveSelection();
            ApplySelection();
            UpdateButtonVisual();
        }

        // Botão 2: Carregar modelo com ~60k iterações
        public void Use60k()
        {
            s_selected = ModelOption.Model60k;
            SaveSelection();
            ApplySelection();
            UpdateButtonVisual();
        }

        // Botão 3: Carregar modelo com ~140k iterações
        public void Use140k()
        {
            s_selected = ModelOption.Model140k;
            SaveSelection();
            ApplySelection();
            UpdateButtonVisual();
        }

        void ApplySelection(bool applyRestart = true)
        {
            switch (s_selected)
            {
                case ModelOption.Model200:
                    ApplyModel(model200, applyRestart);
                    break;
                case ModelOption.Model60k:
                    ApplyModel(model60k, applyRestart);
                    break;
                case ModelOption.Model140k:
                    ApplyModel(model140k, applyRestart);
                    break;
                case ModelOption.HeuristicOnly:
                    if (_behavior == null) return;
                    _behavior.Model = null;
                    _behavior.BehaviorType = BehaviorType.HeuristicOnly;
                    if (applyRestart) TryRestartEpisode();
                    break;
            }
        }

        void SaveSelection()
        {
            PlayerPrefs.SetInt(PlayerPrefsKey, (int)s_selected);
            PlayerPrefs.Save();
        }

        void ApplyModel(NNModel model, bool applyRestart = true)
        {
            if (_behavior == null) return;
            if (model == null)
            {
                Debug.LogWarning("ModelSwitcher: modelo não atribuído para a opção selecionada; mantendo configuração atual.");
                return;
            }
            _behavior.Model = model;
            _behavior.InferenceDevice = InferenceDevice.CPU; // Altere para GPU se desejar
            _behavior.BehaviorType = BehaviorType.InferenceOnly; // Garante uso do modelo local
            if (applyRestart) TryRestartEpisode();
        }

        void TryRestartEpisode()
        {
            // Evita pausar o Editor por reiniciar durante Awake/entrada no Play
            if (!_initialized || !Application.isPlaying) return;
            if (agent == null) return;
            StartCoroutine(RestartNextFrame());
        }

        System.Collections.IEnumerator RestartNextFrame()
        {
            yield return null; // espera 1 frame
            if (agent != null)
            {
                agent.EndEpisode();
            }
        }

        // ==== NOVO: Atualização visual dos botões ====
        void UpdateButtonVisual()
        {
            if (!autoApplyColors) return;
            ResetButton(button200);
            ResetButton(button60k);
            ResetButton(button140k);
            Button target = null;
            switch (s_selected)
            {
                case ModelOption.Model200: target = button200; break;
                case ModelOption.Model60k: target = button60k; break;
                case ModelOption.Model140k: target = button140k; break;
                case ModelOption.HeuristicOnly: target = button200; break; // opcional mapear HeuristicOnly
            }
            if (target != null)
            {
                var img = target.GetComponent<Image>();
                if (img) img.color = selectedColor;
                StopPulse();
            }
        }

        void ResetButton(Button b)
        {
            if (b == null) return;
            var img = b.GetComponent<Image>();
            if (img) img.color = normalColor;
        }

        void TryStartPulse(Button b)
        {
            if (b == null) return;
            StopPulse();
            pulseRoutine = StartCoroutine(PulseButton(b));
        }

        IEnumerator PulseButton(Button b)
        {
            var img = b.GetComponent<Image>();
            if (img == null) yield break;
            float t = 0f;
            Color baseColor = selectedColor;
            Color accent = hoverColor;
            while (true)
            {
                t += Time.unscaledDeltaTime;
                float s = (Mathf.Sin(t * 3f) + 1f) * 0.5f; // 0..1
                img.color = Color.Lerp(baseColor, accent, s * 0.5f);
                yield return null;
            }
        }

        void StopPulse()
        {
            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
                pulseRoutine = null;
            }
        }
        // ============================================
    }
}
