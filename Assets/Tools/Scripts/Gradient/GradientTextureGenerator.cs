using UnityEngine;
using UnityEngine.UI;

namespace alexnown
{
    [ExecuteInEditMode]
    public class GradientTextureGenerator : MonoBehaviour
    {
        private const string DefaultShaderName = "Unlit/BlendWithRotatable";

        public Gradient Gradient;
        [Range(0, 360)]
        public int Angle;
        [Range(64, 2048)]
        public int TextureWidth = 256;
        public TextureWrapMode WrapMode = TextureWrapMode.Clamp;
        [Header("Material settings")]
        public string TextureFieldName = "_SecondTex";
        public string AngleFieldName = "_AngleDeg";
        [Header("Target graphic")]
        public GameObject GraphicGo = null;
        public bool CreateMaterialByShader = true;
        public Shader TargetShader = null;

        private Material _material;


        public Texture2D GenerateTexture(int width)
        {
            var texture = new Texture2D(width, 1, TextureFormat.ARGB32, false);
            texture.wrapMode = WrapMode;
            for (int i = 0; i < width; i++)
            {
                var color = Gradient.Evaluate((float)i / width);
                texture.SetPixel(i, 0, color);
            }
            texture.Apply(false);
            return texture;
        }

        private void Start()
        {
            InitializeTargetMaterial();
            UpdateMaterial();
        }

        private void InitializeTargetMaterial()
        {
            if (CreateMaterialByShader)
            {
                _material = new Material(TargetShader);
                SetMaterialToTargetGo(GraphicGo, _material);
            }
            else
            {
                _material = GetMaterialFromGraphicGo(GraphicGo);
                _material = Instantiate(_material);
                SetMaterialToTargetGo(GraphicGo, _material);
            }
        }

        private void UpdateMaterial()
        {
            if (_material == null) return;
            _material.SetFloat(AngleFieldName, Angle);
            _material.SetTexture(TextureFieldName, GenerateTexture(TextureWidth));
        }

        private Material GetMaterialFromGraphicGo(GameObject graphicGo)
        {
            if (graphicGo == null) return null;
            var graphic = graphicGo.GetComponent<Graphic>();
            if (graphic != null) return graphic.defaultMaterial;
            var rend = graphicGo.GetComponent<Renderer>();
            if (rend != null) return rend.sharedMaterial;
            return null;
        }

        private void SetMaterialToTargetGo(GameObject graphicGo, Material mat)
        {
            if (graphicGo == null) return;
            var graphic = graphicGo.GetComponent<Graphic>();
            if (graphic != null) graphic.material = mat;
            var rend = graphicGo.GetComponent<Renderer>();
            if (rend != null) rend.material = mat;
        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            if (!Application.isPlaying && CreateMaterialByShader && _material != null)
            {
                SetMaterialToTargetGo(GraphicGo, null);
                DestroyImmediate(_material);
            }
        }

        protected void OnValidate()
        {
            if (GraphicGo == null)
            {
                if (gameObject.GetComponent<Graphic>() != null || gameObject.GetComponent<Renderer>() != null)
                    GraphicGo = gameObject;
            }
            if (CreateMaterialByShader && TargetShader == null)
                TargetShader = Shader.Find(DefaultShaderName);
            if (Gradient == null)
            {
                Gradient = new Gradient();
                Gradient.SetKeys(new[]
                {
                    new GradientColorKey(new Color32(36,145,214,255), 0),
                    new GradientColorKey(new Color32(36, 211,214,255), 1)
                },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
            }
            InitializeTargetMaterial();
            UpdateMaterial();
        }
#endif
    }
}