using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WorldShaderController : MonoBehaviour
{
    public Vector2 range = new Vector2(-10, 5000f);
    public AnimationCurve curve;
    
    
    [OnValueChanged("UpdateShader"), Range(0f, 1f)]
    public float value;
    
    [OnValueChanged("UpdateShader")]
    public float edge;
    [OnValueChanged("UpdateShader")]
    public float edgeEmission;
    [OnValueChanged("UpdateShader"), ColorUsage(true,true)]
    public Color edgeColor;
    
    
    [OnValueChanged("UpdateShader")]
    public Color color;
    [OnValueChanged("UpdateShader")]
    public float smoothness;

    public Light light;
    public Material skybox;
    
    public EnvState[] states;
    
    private void UpdateShader()
    {
        float distance = Mathf.Lerp(range.x, range.y, curve.Evaluate(value));

        if (value < 0.5f)
        {
            float f = value * 2f;
            UpdateSky(states[0].skybox, states[1].skybox, f);
            light.color = Color.Lerp(states[0].mainLight, states[1].mainLight, f);
            RenderSettings.fogColor = Color.Lerp(states[0].fog, states[1].fog, f);
        }
        else
        {
            float f = value * 2f - 1f;
            UpdateSky(states[1].skybox, states[2].skybox, f);
            light.color = Color.Lerp(states[1].mainLight, states[2].mainLight, f);
            RenderSettings.fogColor = Color.Lerp(states[1].fog, states[2].fog, f);
        }
        
        Shader.SetGlobalColor("_World_Color", color);
        Shader.SetGlobalFloat("_World_Smothness",  Mathf.Lerp(smoothness,0f,value) );
        Shader.SetGlobalFloat("_World_Distance",  distance);
        Shader.SetGlobalColor("_World_Emission", edgeColor);
        Shader.SetGlobalFloat("_World_Edge", edge);
        Shader.SetGlobalFloat("_World_Edge_Emission", edgeEmission);
        Shader.SetGlobalVector("_World_Epicentrum", transform.position);
    }

    private void UpdateSky(Material start, Material end, float t)
    {
        float f1 = Mathf.Lerp( start.GetFloat("_SunSize"), end.GetFloat("_SunSize"), t);
        float f2 = Mathf.Lerp( start.GetFloat("_SunSizeConvergence"), end.GetFloat("_SunSizeConvergence"), t);
        float f3 = Mathf.Lerp( start.GetFloat("_AtmosphereThickness"), end.GetFloat("_AtmosphereThickness"), t);
        Color c1 = Color.Lerp( start.GetColor("_SkyTint"), end.GetColor("_SkyTint"), t);
        Color c2 = Color.Lerp( start.GetColor("_GroundColor"), end.GetColor("_GroundColor"), t);
        float f4 = Mathf.Lerp( start.GetFloat("_Exposure"), end.GetFloat("_Exposure"), t);

        skybox.SetFloat("_SunSize",f1);
        skybox.SetFloat("_SunSizeConvergence",f2);
        skybox.SetFloat("_AtmosphereThickness",f3);
        skybox.SetColor("_SkyTint",c1);
        skybox.SetColor("_GroundColor",c2);
        skybox.SetFloat("_Exposure",f4);
    }
    
    [System.Serializable]
    public class EnvState
    {
        public Material skybox;
        public Color fog;
        public Color mainLight;
    }
}
