using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] 
    private AudioSource m_backGround = null;
    [SerializeField] 
    private AudioSource m_effect = null;

    [Header("AudioClip_BGM")]
    [SerializeField]
    public List<AudioClip> BGMClip = new List<AudioClip>();

    [Header("AudioClip_Effect")]
    [SerializeField]
    public List<AudioClip> EffectClip = new List<AudioClip>();

    private Dictionary<eBGMSound, AudioClip> BGMDic = new Dictionary<eBGMSound, AudioClip>();
    private Dictionary<eEffectSound, AudioClip> EffectDic = new Dictionary<eEffectSound, AudioClip>();

    public void Init()
    {
        for (int i = 0; i < BGMClip.Count; i++)
        {
            Enum.TryParse(BGMClip[i].name, out eBGMSound eBGM);
            BGMDic.Add(eBGM, BGMClip[i]);
        }

        for (int i = 0; i < EffectClip.Count; i++)
        {
            Enum.TryParse(EffectClip[i].name, out eEffectSound eEffect);
            EffectDic.Add(eEffect, EffectClip[i]);
        }
    }

    public void SetBGM(eBGMSound _BGM)
    {
        if (BGMDic.ContainsKey(_BGM))
        {
            m_backGround.clip = BGMDic[_BGM];
            m_backGround.Play();
        }
    }

    public void SetEffect(eEffectSound _Effect)
    {
        if (EffectDic.ContainsKey(_Effect))
            m_effect.PlayOneShot(EffectDic[_Effect]);
    }
}