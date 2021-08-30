using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Candle : MonoBehaviour
{
    ParticleSystem pSystem;
    AudioSource aSource;

    // Start is called before the first frame update
    void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
        aSource = GetComponent<AudioSource>();

        pSystem.Stop();
	}

	public void LightEnd(SteamVR_Input_Sources source, bool value)
	{
		pSystem.Stop();
	}

	public void LightStart (SteamVR_Input_Sources source, bool value)
	{
		pSystem.Play();
		aSource.Play();
	}
}
