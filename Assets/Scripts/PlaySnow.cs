using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json.Linq; // ⇒ 事前に Newtonsoft JSON Package を追加

public class PlaySnow : MonoBehaviour
{
    [SerializeField] ParticleSystem snowEffect;

    void Start()
    {
        snowEffect.Play(); // スタート時に雪を発生させる
    }
}
