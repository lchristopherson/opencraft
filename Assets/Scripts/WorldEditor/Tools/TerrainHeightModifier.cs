using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.TerrainAPI;

    /**
    * TODO: Ensure coordinates are in bounds 
    * TODO: Configure brush sizes and weights
    */
    public class TerrainHeightModifier : MonoBehaviour, IOnTerrainMouseAction, IEnableable
    {
        public float brushSize;
        public float brushStrength;
        public Texture2D brushTexture;

        private bool enabled = false;

        public void Enable() 
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
        }

        public void OnPress(Terrain terrain, IMousePressCollision collision) 
        {
            //
        }

        public void OnDrag(Terrain terrain, IMousePressCollision collision) 
        {
            if (!enabled) return;

            PaintHeight(terrain, collision.hit.textureCoord);
        }

        public void OnRelease(Terrain terrain, IMousePressCollision collision) 
        {
            // Flush delayed height painting actions
            PaintContext.ApplyDelayedActions();
        }

        private void PaintHeight(Terrain terrain, Vector2 uv)
        {
            Material mat = TerrainPaintUtility.GetBuiltinPaintMaterial();

            float rotationDegrees = 0.0f;
            BrushTransform brushXform = TerrainPaintUtility.CalculateBrushTransform(terrain, uv, brushSize, rotationDegrees);
            PaintContext paintContext = TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds());

            // apply brush
            Vector4 brushParams = new Vector4(brushStrength * 0.01f, 0.0f, 0.0f, 0.0f);
            mat.SetTexture("_BrushTex", brushTexture);
            mat.SetVector("_BrushParams", brushParams);
            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);

            Graphics.Blit(paintContext.sourceRenderTexture, paintContext.destinationRenderTexture, mat, (int)TerrainPaintUtility.BuiltinPaintMaterialPasses.RaiseLowerHeight);

            TerrainPaintUtility.EndPaintHeightmap(paintContext, "Terrain Paint - MyPaintHeightTool");
        }
    }
