using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using f3;
using g3;
using UnityEngine;

namespace UnityEngine
{
    public enum BlendMode
    {
        Zero,
        One,
        DstColor,
        SrcColor,
        OneMinusDstColor,
        SrcAlpha,
        OneMinusSrcColor,
        DstAlpha,
        OneMinusDstAlpha,
        SrcAlphaSaturate,
        OneMinusSrcAlpha
    }

    public class GameObject
    {
        public Transform transform;
    }

    public class Texture
    {
    }

    public class Transform
    {
        public Vector3f position;
        public Vector3f localPosition;
        public Quaternionf rotation;
        public Quaternionf localRotation;
    }

    public class Material
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public Colorf color;
        public int renderQueue;

        public virtual void SetInt(string identifier, int value)
        {
            values[identifier] = value;
        }
        public virtual int GetInt(string identifier)
        {
            return (int)values[identifier];
        }

        public virtual void SetFloat(string identifier, float value)
        {
            values[identifier] = value;
        }
        public virtual float GetFloat(string identifier)
        {
            return (float)values[identifier];
        }

        public virtual void SetVector(string identifier, Vector4f value)
        {
            values[identifier] = value;
        }
        public virtual Vector4f GetVector(string identifier)
        {
            return (Vector4f)values[identifier];
        }
    }
}

namespace f3
{
    /// <summary>
    /// fMaterial wraps a Material for frame3Sharp. The idea is that eventually we
    ///  will be able to "replace" Material with something else, ie non-Unity stuff.
    ///
    /// implicit cast operators allow transparent conversion from fMaterial to Material    
    /// </summary>
    public class fMaterial
    {
        protected UnityEngine.Material unityMat;

        public fMaterial(UnityEngine.Material m)
        {
            unityMat = m;
        }

        public virtual string name { get; set; }

        public virtual Colorf color { get; set; }

        public virtual Texture mainTexture { get; set; }

        public virtual int renderQueue { get; set; }


        public virtual void SetInt(string identifier, int value)
        {
            unityMat.SetInt(identifier, value);
        }
        public virtual int GetInt(string identifier)
        {
            return unityMat.GetInt(identifier);
        }

        public virtual void SetFloat(string identifier, float value)
        {
            unityMat.SetFloat(identifier, value);
        }
        public virtual float GetFloat(string identifier)
        {
            return unityMat.GetFloat(identifier);
        }

        public virtual void SetVector(string identifier, Vector4f value)
        {
            unityMat.SetVector(identifier, value);
        }
        public virtual Vector4f GetVector(string identifier)
        {
            return unityMat.GetVector(identifier);
        }
    }




    /// <summary>
    /// This is a specialization of fMaterial that (should) automatically switch
    /// between opaque and transparent rendering modes depending on alpha value.
    /// In Unity this is non-trivial because you can't just change renderQueue,
    /// also requires various shader flags.
    /// (Possibly it is best to initialize this w/ StandardShader)
    /// </summary>
    public class fDynamicTransparencyMaterial : fMaterial
    {
        public int OpaqueRenderQueue = 1000;
        public int TransparentRenderQueue = 3000;

        public fDynamicTransparencyMaterial(UnityEngine.Material m) : base(m)
        {
        }

        public override Colorf color
        {
            get { return unityMat.color; }
            set
            {
                bool alpha_change = (value.a == 1.0f && unityMat.color.a != 1.0f) ||
                                    (value.a != 1.0f && unityMat.color.a == 1.0f);
                unityMat.color = value;
                if (alpha_change)
                {
                    DebugUtil.Log(2, "changing alpha to {0}", value.a);
                    if (value.a == 1)
                    {
                        MaterialUtil.SetupMaterialWithBlendMode(unityMat, MaterialUtil.BlendMode.Opaque);
                        unityMat.renderQueue = OpaqueRenderQueue;
                    }
                    else
                    {
                        MaterialUtil.SetupMaterialWithBlendMode(unityMat, MaterialUtil.BlendMode.Transparent);
                        unityMat.renderQueue = TransparentRenderQueue;
                    }
                }
            }
        }

    }

    public class UnityUtil
    {
        public static Frame3f GetGameObjectFrame(GameObject go, CoordSpace eSpace)
        {
            if (eSpace == CoordSpace.WorldCoords)
                return new Frame3f(go.transform.position, go.transform.rotation);
            else if (eSpace == CoordSpace.ObjectCoords)
                return new Frame3f(go.transform.localPosition, go.transform.localRotation);
            else
                throw new ArgumentException("not possible without refernce to scene!");
        }
    }

    public class MaterialUtil
    {
        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,        // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
        }

        static public void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        {
        }
    }
}
