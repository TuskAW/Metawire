using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.AssetImporters;

namespace Metawire {

[ScriptedImporter(1, "metawire")]
public sealed class MetawireImporter : ScriptedImporter
{
    #region ScriptedImporter implementation

    [SerializeField] Shape _shape = Shape.Line;
    [SerializeField] Line _line = new Line();
    [SerializeField] Quad _quad = new Quad();
    [SerializeField] Circle _circle = new Circle();
    [SerializeField] bool _readWrite = false;

    public override void OnImportAsset(AssetImportContext context)
    {
        var gameObject = new GameObject();
        var mesh = ImportAsMesh(context.assetPath);

        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var pipelineAsset = GraphicsSettings.currentRenderPipeline;
        var baseMaterial = pipelineAsset ? pipelineAsset.defaultLineMaterial :
          AssetDatabase.GetBuiltinExtraResource<Material>("Default-Line.mat");
        
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = baseMaterial;

        context.AddObjectToAsset("prefab", gameObject);
        if (mesh != null) context.AddObjectToAsset("mesh", mesh);

        context.SetMainObject(gameObject);
    }

    #endregion

    #region Reader implementation

    Mesh ImportAsMesh(string path)
    {
        var mesh = new Mesh();
        mesh.name = "Mesh";

        switch (_shape)
        {
            case Shape.Line: _line.Generate(mesh); break;
            case Shape.Quad: _quad.Generate(mesh); break;
            case Shape.Circle: _circle.Generate(mesh); break;
        }

        mesh.RecalculateBounds();
        mesh.UploadMeshData(!_readWrite);

        return mesh;
    }

    #endregion
}

} // namespace Metawire