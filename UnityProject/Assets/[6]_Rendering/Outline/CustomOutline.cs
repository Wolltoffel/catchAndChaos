using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum OutlineMaterial
{
  fill,mask
}

public enum Effect {
  Emission,OutlineWidth

}

[RequireComponent(typeof(MeshFilter))]
[DisallowMultipleComponent]
public class CustomOutline : MonoBehaviour
{

#region Outline Properties

  [HideInInspector]public bool _preRenderOutline;
  public bool preRenderOutline
  {
    get{return _preRenderOutline;}
    
    set
    {
      if (value != _preRenderOutline)
      {
            _preRenderOutline = value;
            UpdateOutline();
        }
    } 
  }


  [HideInInspector]public bool _activeOutline;
  public bool activeOutline 
  {
    get{return _activeOutline;}
    
    set
    {
      if (value != _activeOutline)
      {
            _activeOutline = value;
            UpdateOutline();
        }
    } 
  }

  [Header("Outline Properties")]

  Coroutine pulsating;
  [HideInInspector]float _emissionStrength=0;
  public float emissionStrength
  {
     get{return _emissionStrength;}
    
    set
    {
      if (value != _emissionStrength)
      {
            _emissionStrength = value;
            SetEmissionStrength(value);
        }
    } 
  }

  [HideInInspector]public Color _outlineColor;
  public Color outlineColor
  {
     get{return _outlineColor;}
    
    set
    {
      if (value != _outlineColor)
      {
            _outlineColor = value;
            SetOutlineColor(value);
        }
    } 
  }

  [HideInInspector]public bool _activeCutoff;
  public bool activeCutoff
  {
     get{return _activeCutoff;}
    
    set
    {
      if (value != _activeCutoff)
      {
            _activeCutoff = value;
            SetCutOff(value);
        }
    } 
  }

  [HideInInspector]public Texture2D _cutoffTex;
  public Texture2D cutoffTex
  {
     get{return _cutoffTex;}
    
    set
    {
      if (value != _cutoffTex)
      {
            _cutoffTex = value;
            SetCutOffTex(value);
        }
    } 
  }


  [HideInInspector] public float _outlineWidth;
  public float outlineWidth
  {
     get{return  _outlineWidth;}
    
    set
    {
      if (value != _outlineWidth)
      {
            _outlineWidth = value;
            SetOutlineWidth(value);
        }
    } 
  }

  [HideInInspector] public bool _seeThroughWalls;
  public bool seeThroughWalls
  {
     get{return  _seeThroughWalls;}
    
    set
    {
      if (value != _seeThroughWalls)
      {
            _seeThroughWalls = value;
            SetSeeThroughWalls(value);
        }
    } 
  }
  # endregion

#region  Normals

  [System.Serializable]
  public class ComputedNormals{
    public List<Vector3> smoothNormals;
  }
  [HideInInspector]public ComputedNormals computedNormals = new ComputedNormals();
#endregion

#region Materials
  Material outLineFill;
  Material outLineMask;

  Renderer _meshRenderer;
  Renderer meshRenderer
  {
    get 
    {
      if(_meshRenderer==null)
        _meshRenderer = GetComponent<Renderer>();
      if (_meshRenderer==null)
         _meshRenderer = GetComponentInChildren<Renderer>();
      return _meshRenderer;
    }
    set{_meshRenderer = value;}
  }
  Material[] _activeMaterials;
  Material[] activeMaterials
  {
    get
    {
        _activeMaterials = meshRenderer.sharedMaterials;
        return _activeMaterials;
    }

    set{_activeMaterials=value;}
  } 
  #endregion

  void Start()
  {
      UpdateOutline();
  }

  void OnValidate()
  {
      if (preRenderOutline){
        InjectNormals();
      }

      UpdateOutline();
  }

  public void UpdateOutline()
  {

      if (activeOutline&&Application.isEditor)
        preRenderOutline = true;

      List<Material> materials = new List<Material>(activeMaterials);
      RemoveOutlineMaterials(OutlineMaterial.fill);
      RemoveOutlineMaterials(OutlineMaterial.mask);
      materials = new List<Material>(activeMaterials);

      if (outLineFill==null)
        ImportOutlineMaterial(OutlineMaterial.fill);
      if (outLineMask==null)
        ImportOutlineMaterial(OutlineMaterial.mask);

      SetOutlineColor(outlineColor);
      SetOutlineWidth(outlineWidth);
      SetSeeThroughWalls(seeThroughWalls);
      SetEmissionStrength(emissionStrength);
      
      if (activeOutline &&!CheckOutlineMaterials(materials))
      {
        InjectNormals();
        materials.Add(outLineFill);
        materials.Add (outLineMask);
        meshRenderer.sharedMaterials = materials.ToArray();
        meshRenderer.materials = materials.ToArray();
      }
  }

#region  Set Material Properties
  void RemoveOutlineMaterials(OutlineMaterial outlineMaterial)
  {
      List<Material> outlineMaterials = new List<Material>(GetOutlineMaterials(outlineMaterial));
      List<Material> activeMaterialsList = new List<Material>(activeMaterials);
      for (int i =0;i<outlineMaterials.Count;i++)
      {
          activeMaterialsList.Remove(outlineMaterials[i]);
      }

        meshRenderer.sharedMaterials = activeMaterialsList.ToArray();
        meshRenderer.materials = activeMaterialsList.ToArray();
  }

  bool CheckOutlineMaterials(List<Material> materials)
  {
      List<Material> fillMats = new List<Material>(GetOutlineMaterials(OutlineMaterial.fill));
      List<Material> maskMats = new List<Material>(GetOutlineMaterials(OutlineMaterial.mask));
     
      if (fillMats.Count>=1 && maskMats.Count>=1)
        return true;

      return false;
  }


  Material[] GetOutlineMaterials(OutlineMaterial materialSelect)
  { 
    List<Material> outlineMaterials = new List<Material>();
    GetOutlineMaterialData(materialSelect,out string name);
    List<Material> materials = new List<Material>(activeMaterials);
    
    for (int i =0;i<materials.Count;i++)
    {
        if (materials[i].ToString().Contains(name))
        {
          outlineMaterials.Add(materials[i]);
        }
    }
    return outlineMaterials.ToArray();
  }

  void ImportOutlineMaterial(OutlineMaterial outline)
  {     
    string materialName;
    Material material;
    
    material = GetOutlineMaterialData(outline,out materialName);


    Material outlineTemplate = Resources.Load<Material>(@"Materials/"+materialName);

    if (outlineTemplate!=null)
    {
      material = Instantiate(outlineTemplate);
      material.name = materialName+"(temp)";

      if (outline==OutlineMaterial.fill)
        outLineFill = material;
      else
        outLineMask = material;

    }
      
    else
      Debug.LogError("Could not not find Outline Material. Put Materials in RessourcesFolder!");
  }
  Material GetOutlineMaterialData(OutlineMaterial outlineMaterial, out string name)
  {

    if (outlineMaterial == OutlineMaterial.mask)
    {
        name =  "OutlineMask";
        return outLineMask;
    }
    else
    {
         name =  "OutlineFill";
         return  outLineFill;
    }
  }

  void SetOutlineWidth(float value)
  {
    if (outLineFill!=null)
        outLineFill.SetFloat("_Outline",value);
      
  }

  void SetEmissionStrength(float intensity)
  {
        if (outLineFill!=null && pulsating==null)
        {
          outLineFill.SetFloat("_EmissionIntensity",intensity);
        }          
  }

  public void StartPulsating(float start, float max,Effect effect = Effect.Emission,float speed = 1)
  {
    if (pulsating==null)
      pulsating = StartCoroutine(PulsateEffect(start,max,speed,effect));
  }

  public void StopPulsatingEmission()
  {
    StopCoroutine(pulsating);
    pulsating = null;
  }
  System.Collections.IEnumerator PulsateEffect(float start, float max,float speed, Effect effect)
  {
      float lerpValue = 0;
      float currentValue = emissionStrength;
      float startTime = Time.time;
      while (true)
      {;
        lerpValue = Mathf.Abs(Mathf.Sin((Time.time-startTime)*speed));
        
        currentValue = Mathf.Lerp(start,max,lerpValue);
        
        if (outLineFill!=null)
        {
          if (effect == Effect.Emission)
            outLineFill.SetFloat("_EmissionIntensity",currentValue);
          if (effect == Effect.OutlineWidth)
            outLineFill.SetFloat("_Outline",currentValue);
        }


        yield return null;
      }
  }

    void SetOutlineColor(Color color)
  {
        if (outLineFill!=null)
        {
          outLineFill.SetColor("_OutlineColor",color);
        }          
  }

  void SetCutOff(bool active)
  {
    if (outLineFill!=null)
    {
        if (active)
          outLineFill.SetFloat("_activeAlphaMap",1);
        else
          outLineFill.SetFloat("_activeAlphaMap",0);
    }    
  }

  void SetCutOffTex(bool active)
  {
    if (outLineFill!=null)
        outLineFill.SetTexture("_cutoffTex",cutoffTex);  
  }

  void SetSeeThroughWalls(bool active)
  {
    if (outLineFill!=null)
    {
      if (seeThroughWalls)
        outLineFill.SetFloat("_zTest",(float)UnityEngine.Rendering.CompareFunction.Always);
      else
        outLineFill.SetFloat("_zTest",(float)UnityEngine.Rendering.CompareFunction.LessEqual);
    }
        
  }

#endregion

#region SmoothNormals
  public void RecalculateNormals()
  {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        computedNormals.smoothNormals = SmoothNormals(mesh);
  }

  void InjectNormals()
  {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;

         if (!mesh.isReadable)
         {
            Debug.LogWarning($"Not allowed to access vertices on mesh {mesh.name} (isReadable is false; Read/Write must be enabled in import settings");
            return;
         }

        if (computedNormals.smoothNormals==null)
          RecalculateNormals();

       meshFilter.sharedMesh.SetUVs(2,computedNormals.smoothNormals);

       //Combine Submeshes
       CombineSubmeshes(mesh);

  }

  void CombineSubmeshes(Mesh mesh)
  {
    if (mesh.subMeshCount==1 || mesh.subMeshCount>activeMaterials.Length)
      return;

    mesh.subMeshCount++;
    mesh.SetTriangles(mesh.triangles,mesh.subMeshCount-1);
    
  }

  List<Vector3> SmoothNormals(Mesh mesh)
  {
      IEnumerable<IGrouping<Vector3,KeyValuePair<Vector3,int>>> groups = mesh.vertices.Select((vertex,index)=>new KeyValuePair<Vector3,int>(vertex,index)).GroupBy(pair=>pair.Key);

      List<Vector3> smoothNormals = new List<Vector3>(mesh.normals);

      for (int i=0;i<groups.Count();i++)
      {
        List<KeyValuePair<Vector3,int>> group = groups.ElementAt(i).ToList();

        Vector3 smoothNormal = Vector3.zero;

        if (group.Count<=1)
          continue;

        for (int j=0;j<group.Count;j++)
        {
            int value = group[j].Value;
            smoothNormal += smoothNormals[group[j].Value];
        }

        smoothNormal.Normalize();

        for (int k=0;k<group.Count;k++)
        {
            smoothNormals[group[k].Value] = smoothNormal;
        }
      }

      return smoothNormals;
  }
  #endregion

}